using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Xml.Schema;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace WADL
{
    
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [XmlRoot(ElementName = "resource")]
    public class WADLMethod:WADLAttribute  
    {
        [Flags]
        internal enum State
        {
            None = 1,
            EmptyWADLMethod = 2,
            EmptyWADLMethodCurrent = 4,
            EmptyWADLMethodCurrentWithWADLPathVariable = 8,
        }

        private HttpMethods method;
        [XmlAttribute(AttributeName = "name", Namespace = "http://research.sun.com/wadl")]
        public HttpMethods HttpMethod
        {
            get { return method; }
            set { method = value; }
        }

        private string name;
        [XmlAttribute(AttributeName = "id", Namespace = "http://research.sun.com/wadl")]
        public string MethodName
        {
            get { return name; }
            set { name = value; }
        }

        private string queryType;
        [XmlAttribute(AttributeName = "queryType", Namespace = "http://research.sun.com/wadl")]
        public string QueryType
        {
            get { return queryType; }
            set { queryType = value; }
        }

        public WADLMethod()
            : this("",HttpMethods.GET)
        {
           
        }

        public WADLMethod(String methodName)
            : this(methodName, HttpMethods.GET)
        {
        }


        public WADLMethod(String methodName, HttpMethods method)
            : base()
        {
            this.name = methodName;
            this.method = method;

            this.queryType = QueryTypes.urlEncoded;
            this.parameters = new List<WADLParam>();
            this.headers = new List<WADLParam>();
            this.representations = new List<WADLRepresentation>(); 
        }


        private WADLDoc doc;
        public WADLDoc Doc
        {
            get { return doc; }
            set { doc = value; }
        }

        private MethodInfo methodInfo;
        public MethodInfo MethodInfo
        {
            get { return methodInfo; }
            set { methodInfo = value; }
        }

        private List<WADLParam> parameters;
        public List<WADLParam> Parameters
        {
            get { return parameters; }
        }

        private List<WADLParam> headers;
        public List<WADLParam> Headers
        {
            get { return headers; }
        }

        private List<WADLRepresentation> representations;
        public List<WADLRepresentation> Representations
        {
            get { return representations; }
            set { representations = value; }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            
            XmlRootAttribute root = (XmlRootAttribute)this.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true).First<object>();
            PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            PropertyInfo name = properties.First<PropertyInfo>(delegate(PropertyInfo prop)
            {
                return prop.Name == "MethodName";
            });

            var Fs = (from parameter in Parameters
                      select parameter).Where<WADLParam>(delegate(WADLParam attribute)
                      {
                          return attribute.Style == ParamStyles.Form;
                      });

            var Qs = (from parameter in Parameters
                      select parameter).Where<WADLParam>(delegate(WADLParam attribute)
                      {
                          return attribute.Style == ParamStyles.Query ;
                      });
            
            var Qh = (from parameter in Parameters
                      select parameter).Where<WADLParam>(delegate(WADLParam attribute)
                      {
                          return attribute.Style == ParamStyles.Header;
                      });

            var Pp = (from parameter in Parameters
                      select parameter).Where<WADLParam>(delegate(WADLParam attribute)
                      {
                          return attribute.Style == ParamStyles.Template;
                      });

            string path = String.IsNullOrEmpty(this.MethodName) ? "/" : this.MethodName == "/"?"/":String.Concat("/", this.MethodName);
            foreach (WADLParam pathAttribute in Pp)
            {
                path = String.Concat(path,path.EndsWith("/")?"{":"/{", pathAttribute.Name, "}");           
            }

            string query = String.Empty;
            foreach (WADLParam pathAttribute in Qs)
            {
                query = String.Concat(query, String.IsNullOrEmpty(query) ? "?" : "&", pathAttribute.Name, "={", pathAttribute.Name, "}");
            }

            path = String.Concat(path,query);

            if (name.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Length == 1 && !String.IsNullOrEmpty(path))
            {
                XmlAttributeAttribute attribute = (XmlAttributeAttribute)name.GetCustomAttributes(typeof(XmlAttributeAttribute), true).First<object>();
                writer.WriteAttributeString("path", path);
            }

            foreach (WADLAttribute pathAttribute in Pp)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLParam), root.Namespace);
                    rez.Serialize(mem, pathAttribute);
                    mem.Flush();
                    mem.Position = 0;
                    using (StreamReader sr = new StreamReader(mem))
                    {
                        sr.ReadLine();
                        writer.WriteRaw(sr.ReadToEnd());
                        sr.Close();
                    }
                    mem.Close();
                }
            }

            writer.WriteStartElement("method");
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Length == 1 && property.Name != "MethodName" )
                {
                    XmlAttributeAttribute attribute = (XmlAttributeAttribute)property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).First<object>();
                    writer.WriteAttributeString(attribute.AttributeName,property.GetValue(this, null).ToString());
                }
            }

            if (Doc != null)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLDoc), root.Namespace);
                    rez.Serialize(mem, doc);
                    mem.Flush();
                    mem.Position = 0;
                    using (StreamReader sr = new StreamReader(mem))
                    {
                        sr.ReadLine();
                        writer.WriteRaw(sr.ReadToEnd());
                        sr.Close();
                    }
                    mem.Close();
                }

            }

            writer.WriteStartElement("request");
            foreach (WADLParam parameter in Fs)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLParam), root.Namespace);
                    rez.Serialize(mem, parameter);
                    mem.Flush();
                    mem.Position = 0;
                    using (StreamReader sr = new StreamReader(mem))
                    {
                        sr.ReadLine();
                        writer.WriteRaw(sr.ReadToEnd());
                        sr.Close();
                    }
                    mem.Close();
                }
            }
            foreach (WADLParam parameter in Qs)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLParam), root.Namespace);
                    rez.Serialize(mem, parameter);
                    mem.Flush();
                    mem.Position = 0;
                    using (StreamReader sr = new StreamReader(mem))
                    {
                        sr.ReadLine();
                        writer.WriteRaw(sr.ReadToEnd());
                        sr.Close();
                    }
                    mem.Close();
                }
            }
            writer.WriteEndElement();
            writer.WriteStartElement("response");

            foreach (WADLParam parameter in headers)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLParam), root.Namespace);
                    rez.Serialize(mem, parameter);
                    mem.Flush();
                    mem.Position = 0;
                    using (StreamReader sr = new StreamReader(mem))
                    {
                        sr.ReadLine();
                        writer.WriteRaw(sr.ReadToEnd());
                        sr.Close();
                    }
                    mem.Close();
                }
            }

            #region comment
            //foreach (WADLFault fault in faults)
            //{
            //    using (MemoryStream mem = new MemoryStream())
            //    {
            //        XmlSerializer rez = new XmlSerializer(typeof(WADLFault), root.Namespace);
            //        rez.Serialize(mem, fault);
            //        mem.Flush();
            //        mem.Position = 0;
            //        using (StreamReader sr = new StreamReader(mem))
            //        {
            //            sr.ReadLine();
            //            writer.WriteRaw(sr.ReadToEnd());
            //            sr.Close();
            //        }
            //        mem.Close();
            //    }
            //}
            #endregion

            foreach (WADLRepresentation representation in representations)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLRepresentation), root.Namespace);
                    rez.Serialize(mem, representation);
                    mem.Flush();
                    mem.Position = 0;
                    using (StreamReader sr = new StreamReader(mem))
                    {
                        sr.ReadLine();
                        writer.WriteRaw(sr.ReadToEnd());
                        sr.Close();
                    }
                    mem.Close();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all WADLMethod attributes on a method
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static WADLMethod GetWADLMethod(MethodInfo method)
        {
            WADLMethod[] attributes = (WADLMethod[])method.GetCustomAttributes(typeof(WADLMethod), true);
            if(attributes.Length == 0)
            {
                return null;
            }
            else
            {
                //make sure there is only one
                WADLMethod wadlMethod = attributes.Single<WADLMethod>();
                wadlMethod.methodInfo = method;
                return wadlMethod;
            }

            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static WADLMethod[] GetWADLMethods(Type type, WADLParameterAddedHandler onWADLParameterAdded, WADLRepresentationAddedHandler onWADLRepresentationAdded)
        {
          State EmptyWADLMethodFlag = State.None;
          var headers = WADLParam.GetWADLParams(type);   
          return (from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    select method).Where<MethodInfo>(delegate(MethodInfo member)
             {
                 return WADLMethod.GetWADLMethod(member) != null;

             }).Aggregate<MethodInfo, List<WADLMethod>>(new List<WADLMethod>(), delegate(List<WADLMethod> list, MethodInfo member)
            {

                WADLMethod method = WADLMethod.GetWADLMethod(member);
                method.doc = WADLDoc.GetWADLDoc(member);
                
                if (String.IsNullOrEmpty(method.MethodName))
                {
                    if ((EmptyWADLMethodFlag & State.EmptyWADLMethod) == State.EmptyWADLMethod)
                    {
                        //throw new NotSupportedException("only one WADLMethod can be defined without a MethodName");
                    }
                    else
                    {
                        EmptyWADLMethodFlag = State.EmptyWADLMethod | State.EmptyWADLMethodCurrent;
                    }
                }

                WADLParam.GetWADLParams(method.MethodInfo, delegate(WADLParam wadlParam, XmlSchemaSet schemas)
                {
                    if ((EmptyWADLMethodFlag & State.EmptyWADLMethodCurrent) == State.EmptyWADLMethodCurrent && wadlParam.Style == ParamStyles.Template)
                    {
                        EmptyWADLMethodFlag |= State.EmptyWADLMethodCurrentWithWADLPathVariable;
                    }
                   
                    if (method.Parameters.Exists(delegate(WADLParam param)
                    {
                        return param.Name.ToLower() == wadlParam.Name.ToLower() && param.Style == wadlParam.Style;
                    }))
                    {
                        throw new NotSupportedException(String.Format("Duplicate WADLParam {0}", wadlParam.Name));
                    }
                    else
                    {
                        method.Parameters.Add(wadlParam);
                        if (onWADLParameterAdded != null)
                            onWADLParameterAdded(wadlParam, schemas);
                    }
                });

                 WADLParam[] methodheaders = WADLParam.GetWADLParams(member);
                 headers.Union<object>(methodheaders.Except<object>(headers, ((IEqualityComparer<object>)new WADLParam()))
                     ).Aggregate<object,List<WADLParam>>(method.Headers,delegate(List<WADLParam> aggregator, object header){
                         aggregator.Add((WADLParam)header);
                    return aggregator;
                 }); 

                if ((EmptyWADLMethodFlag & State.EmptyWADLMethodCurrent) == State.EmptyWADLMethodCurrent)
                {
                    if ((EmptyWADLMethodFlag & State.EmptyWADLMethodCurrentWithWADLPathVariable) == State.EmptyWADLMethodCurrentWithWADLPathVariable)
                    {
                        EmptyWADLMethodFlag = EmptyWADLMethodFlag ^ State.EmptyWADLMethodCurrent;
                    }
                    //else //obsolete?
                    //throw new InvalidOperationException("WADLMethods without a MethodName must define at least one WADLParam Template");

                }

                WADLRepresentation.GetWADLRepresentations(member.ReturnParameter, delegate(WADLRepresentation rep, XmlSchemaSet schemas)
                {
                    method.Representations.Add(rep);
                    onWADLRepresentationAdded(rep, schemas);
                });

                WADLRepresentation.GetWADLRepresentations(member.DeclaringType, delegate(WADLRepresentation rep, XmlSchemaSet schemas)
                {
                    if (!method.Representations.Contains(rep, (IEqualityComparer<WADLRepresentation>)new WADLRepresentation()))
                    {
                        method.Representations.Add(rep);
                        onWADLRepresentationAdded(rep, schemas);
                    }
                
                });
               


                list.Add(method);
                return list;


            }).ToArray<WADLMethod>();
        }

    }
}
