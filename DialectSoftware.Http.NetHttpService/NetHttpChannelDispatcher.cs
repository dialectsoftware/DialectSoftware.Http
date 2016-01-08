using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using WADL;
using System.IO;
using DialectSoftware.Http.Services.Configuration;
using System.Runtime.Serialization;
using System.Collections;
using System.ServiceModel.Web;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    //encoding
    //header expose to ChannelManager
    /// <summary>
    /// The Dispatcher selects the appropriate method to invoke based on the Url
    /// </summary>
    public class NetHttpChannelDispatcher
    {
        private UriTemplateMatch match;

        public WADLMethod Method
        {
            get;
            private set;
        }

        public MatchType MatchType
        {
            get;
            private set;

        }

        public NetHttpUriTemplate Template
        {
            get;
            private set;
        }

        public NetHttpChannelManager Instance
        {
            get;
            private set;
        }

        protected internal NetHttpChannelDispatcher(NetHttpChannelManager instance, WADLMethod method, NetHttpUriTemplate template)
        {
            Method = method;
            Instance = instance;
            Template = template;
            MatchType = MatchType.None;
        }

        public bool IsMatch(Uri baseUri, Uri candidate)
        {
            if (Method.HttpMethod.ToString().Equals(WebOperationContext.Current.IncomingRequest.Method, StringComparison.CurrentCultureIgnoreCase))
            //&& Method.QueryType.Equals(WebOperationContext.Current.IncomingRequest.ContentType, StringComparison.CurrentCultureIgnoreCase))
            {
                match = Template.Match(baseUri, candidate);
                if (!(match == null))
                {
                    if (Template.QueryValueVariableNames.Count == 0)
                    {
                        if (Template.PathSegmentVariableNames.Count == 0)
                        {
                            if (Template.Url.Query.Count == 0)
                            {
                                MatchType = MatchType.Static;
                            }
                            else
                            {
                                MatchType = MatchType.Static | MatchType.Query;
                            }
                        }
                        else
                        {
                            MatchType = MatchType.Template;
                        }
                    }
                    else
                    {
                        MatchType = MatchType.Query;
                    }
                    return true;
                }
            }
            return false;
        }

        //TODO: Method needs to be evaluated
        //deserialize parameters
        public void Invoke(NetHttpContext context)
        {
            List<object> parameters =  new List<object>();
            ParameterInfo[] @params = Method.MethodInfo.GetParameters();
            Array.ForEach(@params, delegate(ParameterInfo param) {
                WADLParam[] attributes = WADLParam.GetWADLParams(param);
                if (param.ParameterType.IsArray)
                {
                    Type type = Type.GetType(param.ParameterType.AssemblyQualifiedName.Replace("[]", ""));
                    Array array = Array.CreateInstance(type,attributes.Length);
                    IList list = Activator.CreateInstance(typeof(List<>).MakeGenericType(type)) as IList;
                    Resolve(context, list, attributes);
                    list.CopyTo(array, 0);
                    parameters.Add(array);
                }
                else
                {
                    if (attributes.Length == 0)
                    {
                        WADLParam wadlParam = WADLParam.CreateWADLParam(param);
                        parameters.Add(Resolve(context, wadlParam));
                    }
                    else
                    {
                        parameters.Add(Resolve(context, attributes.Single()));
                    }
                }
             });

            NetHttpChannelManager instanceToInvoke = Instance;
            if (Instance.GetInstancingModeAttribute() == InstancingMode.PerCall)
            {
                instanceToInvoke = Activator.CreateInstance(Instance.GetType()) as NetHttpChannelManager;
            }
            var result = Method.MethodInfo.Invoke(instanceToInvoke, parameters.ToArray());
            if (result == null)
            {
                WebOperationContext.Current.OutgoingResponse.SetStatusAsNotFound();
                WebOperationContext.Current.OutgoingResponse.SuppressEntityBody = true;
            }
            else 
            {
                var outputs = (from representation in Method.Representations
                               select representation).Where<WADLRepresentation>(delegate(WADLRepresentation rep)
                {
                    return rep.Status == System.Net.HttpStatusCode.OK;
                });

                WADLRepresentation output;

                if (outputs.Count() == 1)
                {
                    output = outputs.Single();
                    context.Response.ContentType = output.ContentType;
                }
                else
                {
                    output = outputs.DefaultIfEmpty(null).SingleOrDefault(o => o.ContentType == context.Response.ContentType);
                    if (output == null)
                    {
                        output = new WADLRepresentation(ContentTypes.unknown);
                        context.Response.ContentType = output.ContentType;
                    }
                }

                Encode(context.Response.Buffer, output, result);
            }

        }

        //static IFormatter GetEncoder(WADLParam param)
        //{
        //    Type type = Type.GetType(TemplateConfigurationHandler.GetConfiguration().Encoders[param.Encoding].Type);
        //    return (IFormatter)Activator.CreateInstance(type);
        //}

        static void Encode(Stream stream, WADLRepresentation param, object value)
        {
            TemplateConfigurationHandler handler = TemplateConfigurationHandler.GetConfiguration();
            var encoderCnfg = handler.Encoders[param.ContentType];
            if (!String.IsNullOrEmpty(encoderCnfg.Type))
            {
                Type encoder = Type.GetType(encoderCnfg.Type);
                INetHttpEncoder formatter = Activator.CreateInstance(encoder) as INetHttpEncoder;
                formatter.Encode(stream, param, value);
            }
        }

        static object Decode(WADLParam param, string value)
        {
            TemplateConfigurationHandler handler = TemplateConfigurationHandler.GetConfiguration();
            var encoderCnfg = handler.Encoders[param.Encoding??Encodings.plainText];
            Type encoder = Type.GetType(encoderCnfg.Type);
            INetHttpEncoder formatter = Activator.CreateInstance(encoder) as INetHttpEncoder;
            return formatter.Decode(param, value);
        }

        //Type type = param.ParameterInfo.ParameterType;
            //if (String.IsNullOrEmpty(param.Encoding) || param.Encoding == ContentTypes.txt)
            //{
            //    if (type.IsArray)
            //    {
            //        type = Type.GetType(type.AssemblyQualifiedName.Replace("[]", ""));
            //    }
            //    return Convert(type, value);
            //}
            //else
            //{
            //    object obj = null;
            //    TemplateConfigurationHandler handler = TemplateConfigurationHandler.GetConfiguration();
            //    Type encoder = Type.GetType(handler.Encoders[param.Encoding].Type);
            //    IFormatter formatter = Activator.CreateInstance(encoder, new object[]{type}) as IFormatter;
            //    using(Stream stream = new System.IO.MemoryStream())
            //    {
            //        using (StreamWriter writer = new StreamWriter(stream))
            //        {
            //            value = System.Web.HttpUtility.UrlDecode(value);
            //            writer.Write(value);
            //            obj = formatter.Deserialize(stream);
            //            writer.Close();
            //        }
            //        stream.Close();
            //    }
            //    return obj;
            //}
            
        //static object Convert(Type type, string value)
        //{
        //    value = System.Web.HttpUtility.UrlDecode(value);
        //    switch (System.Type.GetTypeCode(type))
        //    {
        //        case TypeCode.Char:
        //            return System.Convert.ToChar(value);
        //        case TypeCode.SByte:
        //            return System.Convert.ToSByte(value);
        //        case TypeCode.UInt16:
        //            return System.Convert.ToUInt16(value);
        //        case TypeCode.UInt32:
        //            return System.Convert.ToUInt32(value);
        //        case TypeCode.UInt64:
        //            return System.Convert.ToUInt64(value); ;
        //        case TypeCode.Boolean:
        //            return System.Convert.ToBoolean(value);
        //        case TypeCode.Byte:
        //            return System.Convert.ToByte(value); //?
        //        case TypeCode.Int16:
        //            return System.Convert.ToInt16(value);
        //        case TypeCode.Int32:
        //            return System.Convert.ToInt32(value);
        //        case TypeCode.Int64:
        //            return System.Convert.ToInt64(value);
        //        case TypeCode.Single:
        //            return System.Convert.ToSingle(value);
        //        case TypeCode.Double:
        //            return System.Convert.ToDouble(value);
        //        case TypeCode.Decimal:
        //            return System.Convert.ToDecimal(value);
        //        case TypeCode.DateTime:
        //            return System.Convert.ToDateTime(value);
        //        case TypeCode.String:
        //            return System.Convert.ToString(value); //can be any type in the event of a mismatch the app must catch it 
        //        case TypeCode.Object://check this
        //        case TypeCode.Empty:
        //        case TypeCode.DBNull:
        //        default:
        //            throw new NotSupportedException(String.Format("datatype {0}", type.Name));
        //    }
        //}

        private object Resolve(NetHttpContext context, WADLParam parameter)
        {
            string value = null;
            if (String.IsNullOrEmpty(parameter.Fixed))
            {
                switch (parameter.Style)
                {
                    case ParamStyles.Query:
                        value = match.QueryParameters[parameter.Name];
                        if (String.IsNullOrEmpty(value))
                        {
                            if (String.IsNullOrEmpty(parameter.Default))
                            {
                                if (parameter.IsRequired)
                                    throw new ArgumentNullException(parameter.Name);
                                else
                                    return null;
                            }
                            else
                            {
                                value = parameter.Default;
                                return Decode(parameter, value);
                            }
                        }
                        else
                            return Decode(parameter, value);
                    case ParamStyles.Form:
                        value = context.Request.Params[parameter.Name] ?? context.Request.Params["formBody"];
                        if (String.IsNullOrEmpty(value))
                        {
                            if (String.IsNullOrEmpty(parameter.Default))
                            {
                                if (parameter.IsRequired)
                                    throw new ArgumentNullException(parameter.Name);
                                else
                                    return null;
                            }
                            else
                            {
                                value = parameter.Default;
                                return Decode(parameter, value);
                            }
                        }
                        else
                            return Decode(parameter, value);
                    case ParamStyles.Template:
                        value = match.BoundVariables[parameter.Name];
                        if (String.IsNullOrEmpty(value))
                        {
                            if (String.IsNullOrEmpty(parameter.Default))
                            {
                                if (parameter.IsRequired)
                                    throw new ArgumentNullException(parameter.Name);
                                else
                                    return null;
                            }
                            else
                            {
                                value = parameter.Default;
                                return Decode(parameter, value);
                            }
                        }
                        else
                            return Decode(parameter, value);
                    case ParamStyles.Header:
                        value = context.Request.Headers[parameter.Name];
                        if (String.IsNullOrEmpty(value))
                        {
                            if (String.IsNullOrEmpty(parameter.Default))
                            {
                                if (parameter.IsRequired)
                                    throw new ArgumentNullException(parameter.Name);
                                else
                                    return null;
                            }
                            else
                            {
                                value = parameter.Default;
                                return Decode(parameter, value);
                            }
                        }
                        else
                            return Decode(parameter, value);
                    default:
                        throw new ArgumentException(parameter.Style.ToString());
                }
            }
            else
            {
                value = parameter.Fixed;
                return Decode(parameter, value);
            }

        }

        private void Resolve(NetHttpContext context, IList list, WADLParam[] parameters)
        {
            Array.ForEach(parameters, delegate(WADLParam parameter)
            {
                list.Add(Resolve(context, parameter));
            });
        
        }
    
    }
}

