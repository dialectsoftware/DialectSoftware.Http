using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.Xml.Schema;
using System.Collections;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections.Specialized;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace WADL
{
    public delegate void WADLParameterAddedHandler(WADLParam param, XmlSchemaSet schemas);

    [AttributeUsage(AttributeTargets.Parameter|AttributeTargets.ReturnValue|AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [XmlRoot(ElementName = "param")]
    public class WADLParam : WADLAttribute, IEqualityComparer<object>
    {
        private string name;
        [XmlAttribute(AttributeName = "name", Namespace = "http://research.sun.com/wadl")]
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        private string type;
        [XmlAttribute(AttributeName = "type", Namespace = "http://research.sun.com/wadl")]
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private ParamStyles style;
        [XmlAttribute(AttributeName = "style", Namespace = "http://research.sun.com/wadl")]
        public ParamStyles Style
        {
            get { return style; }
            set { style = value; }
        }

        string encoding;
        [XmlAttribute(AttributeName = "ext:content-transfer-encoding", Namespace = "urn:wadl:extension")]
        public string Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        private string @default;
        [XmlAttribute(AttributeName = "default", Namespace = "http://research.sun.com/wadl")]
        public string Default
        {
            get { return @default; }
            set { @default = value; }
        }

        private bool required;
        [XmlAttribute(AttributeName = "required", Namespace = "http://research.sun.com/wadl")]
        public bool IsRequired
        {
            get { return required; }
            set { required = value; }
        }

        private string @fixed;
        [XmlAttribute(AttributeName = "fixed", Namespace = "http://research.sun.com/wadl")]
        public string Fixed
        {
            get { return @fixed; }
            set { @fixed = value; }
        }

        public WADLParam()
            : base()
        {
            this.required = true;
            this.encoding = Encodings.plainText;
            this.type = SchemaTypes.none;
            this.style = ParamStyles.Query;
        }

        public WADLParam(String name)
            : this()
        {
            this.name = name;
        }


        public WADLParam(String name, String type)
            : this(name)
        {
            this.type = type;
        }

        private ParameterInfo parameterInfo;

        public ParameterInfo ParameterInfo
        {
            get { return parameterInfo; }
            set { parameterInfo = value; }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlRootAttribute root = (XmlRootAttribute)this.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true).First<object>();
            PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Length == 1)
                {
                    var propValue = property.GetValue(this, null);
                    if (propValue != null)
                    {
                        XmlAttributeAttribute attribute = (XmlAttributeAttribute)property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).First<object>();
                        writer.WriteAttributeString(attribute.AttributeName, propValue.ToString());
                    }
                }
            }
        }

        public override void Validate()
        {
            iff<bool> eval = (delegate(bool candidate, Exception exception)
            {
                if (candidate) throw exception;
            });

            //must have a name
            eval(String.IsNullOrEmpty(Name), new NotSupportedException("empty WADLParam name property"));

            if (this.Style == ParamStyles.Query || this.Style == ParamStyles.Form || this.Style == ParamStyles.Template)
            {

                //out params not supported
                eval(ParameterInfo.IsOut, new NotSupportedException(String.Format("[out] parameters {0}", this.ParameterInfo.Member.ToString())));
                
                //void not supported
                //eval(ParameterInfo.Name == null && ParameterInfo.ParameterType.Name.ToLower() == "void", new NotSupportedException(String.Format("[void] {0}", this.ParameterInfo.Member.ToString())));

                //optional params must have a default value (since possibility of multi attribute test against [this].
                eval(!this.IsRequired && String.IsNullOrEmpty(this.Default), new NotSupportedException("optional WADLParam must supply a default value"));

                //optional params are not supported on Template may bot need the one above?
                eval(!this.IsRequired && this.Style == ParamStyles.Template, new NotSupportedException("optional WADLParam only supported on ParamStyles.Query"));

                //type must match parameter except in the case of multiple params
                Condition IsValidType = (delegate()
                {
                    switch (System.Type.GetTypeCode(parameterInfo.ParameterType))
                    {
                        case TypeCode.Char:
                            return Type == WADL.SchemaTypes.@ushort;
                        case TypeCode.SByte:
                            return Type == WADL.SchemaTypes.@byte;
                        case TypeCode.UInt16:
                            return Type == WADL.SchemaTypes.@ushort;
                        case TypeCode.UInt32:
                            return Type == WADL.SchemaTypes.@uint;
                        case TypeCode.UInt64:
                            return Type == WADL.SchemaTypes.@ulong;
                        case TypeCode.Boolean:
                            return Type == WADL.SchemaTypes.boolean;
                        case TypeCode.Byte:
                            return Type == WADL.SchemaTypes.@ubyte;
                        case TypeCode.Int16:
                            return Type == WADL.SchemaTypes.@short;
                        case TypeCode.Int32:
                            return Type == WADL.SchemaTypes.@int;
                        case TypeCode.Int64:
                            return Type == WADL.SchemaTypes.@long;
                        case TypeCode.Single:
                            return Type == WADL.SchemaTypes.@float;
                        case TypeCode.Double:
                            return Type == WADL.SchemaTypes.@double;
                        case TypeCode.Decimal:
                            return Type == WADL.SchemaTypes.@decimal;
                        case TypeCode.DateTime:
                            return Type == WADL.SchemaTypes.dateTime;
                        case TypeCode.String:
                            return true; //can be any type in the event of a mismatch the app must catch it 
                        case TypeCode.Object://check this
                            WADLParam[] parameters = WADLParam.GetWADLParams(parameterInfo);
                            Condition IsValidMutipleParams = (delegate()
                            {
                                MethodInfo methodInfo = ((MethodInfo)parameterInfo .Member);
                                return (parameters.Length >= 1 && (methodInfo.GetParameters().Length == 1 && (parameterInfo.ParameterType.IsInstanceOfType(new object[] { }) || parameterInfo.ParameterType.IsInstanceOfType(new string[] { }) || parameterInfo.ParameterType.IsInstanceOfType(new NameValueCollection()))));
                            });
                            if (parameters.Length > 1)
                            {
                                eval(!IsValidMutipleParams(), new NotSupportedException(String.Format("multiple WADLParam attributes {0} in {1} only supported on methods with a single value of object[], string[], or NameValueCollection", parameterInfo.Member.ToString(), parameterInfo.Member.DeclaringType.Name)));
                                return true;
                            }
                            else
                            {
                                return Type == SchemaTypes.none;
                            }
                        case TypeCode.Empty:
                        case TypeCode.DBNull:
                        default:
                            throw new NotSupportedException(String.Format("datatype {0}", parameterInfo.ParameterType.Name));
                    }
                });

                eval(!IsValidType(), new InvalidCastException(String.Format("[WADLParam({0})]{1} in {2}", this.Name, this.ParameterInfo.Member.ToString(), ParameterInfo.Member.DeclaringType.Name)));

            }
            else
            {
                //header params
                eval(this.type == SchemaTypes.none,new NotSupportedException(String.Format("invalid WADLParam SchemaType {0}", Name)));
                eval(this.encoding != Encodings.plainText, new NotSupportedException(String.Format("invalid WADLParam Encoding {0}", Name)));
            }

        }

        public static WADLParam CreateWADLParam(ParameterInfo parameter)
        {
            // we do not generate representations for primitive types.
            WADLParam param;
            System.Type type = parameter.ParameterType;
            switch (System.Type.GetTypeCode(type))
            {
                case TypeCode.Char:
                    param = new WADLParam(parameter.Name, SchemaTypes.@ushort) { ParameterInfo = parameter };
                    break;

                case TypeCode.SByte:
                    param = new WADLParam(parameter.Name, SchemaTypes.@byte) { ParameterInfo = parameter };
                    break;

                case TypeCode.UInt16:
                    param = new WADLParam(parameter.Name, SchemaTypes.@ushort) { ParameterInfo = parameter };
                    break;

                case TypeCode.UInt32:
                    param = new WADLParam(parameter.Name, SchemaTypes.@uint) { ParameterInfo = parameter };
                    break;

                case TypeCode.UInt64:
                    param = new WADLParam(parameter.Name, SchemaTypes.@ulong) { ParameterInfo = parameter };
                    break;

                case TypeCode.Boolean:
                    param = new WADLParam(parameter.Name, SchemaTypes.boolean) { ParameterInfo = parameter };
                    break;

                case TypeCode.Byte:
                    param = new WADLParam(parameter.Name, SchemaTypes.@ubyte) { ParameterInfo = parameter };
                    break;

                case TypeCode.Int16:
                    param = new WADLParam(parameter.Name, SchemaTypes.@short) { ParameterInfo = parameter };
                    break;

                case TypeCode.Int32:
                    param = new WADLParam(parameter.Name, SchemaTypes.@int) { ParameterInfo = parameter };
                    break;

                case TypeCode.Int64:
                    param = new WADLParam(parameter.Name, SchemaTypes.@long) { ParameterInfo = parameter };
                    break;

                case TypeCode.Single:
                    param = new WADLParam(parameter.Name, SchemaTypes.@float) { ParameterInfo = parameter };
                    break;

                case TypeCode.Double:
                    param = new WADLParam(parameter.Name, SchemaTypes.@double) { ParameterInfo = parameter };
                    break;

                case TypeCode.Decimal:
                    param = new WADLParam(parameter.Name, SchemaTypes.@decimal) { ParameterInfo = parameter };
                    break;

                case TypeCode.DateTime:
                    param = new WADLParam(parameter.Name, SchemaTypes.dateTime) { ParameterInfo = parameter };
                    break;

                case TypeCode.String:
                    param = new WADLParam(parameter.Name, SchemaTypes.@string) { ParameterInfo = parameter };
                    break;

                case TypeCode.Object://we generate for complex types
                    param = new WADLParam(parameter.Name) { ParameterInfo = parameter };
                    
                    break;

                case TypeCode.Empty:
                case TypeCode.DBNull:
                default:
                    throw new NotSupportedException("unsupported data type");
            }

            OptionalAttribute[] optional = (OptionalAttribute[])parameter.GetCustomAttributes(typeof(OptionalAttribute),true);

            DefaultValueAttribute[] defaults = (DefaultValueAttribute[])parameter.GetCustomAttributes(typeof(DefaultValueAttribute),true);
            
            return param;
        }

        public static WADLParam CreateWADLParam(ParameterInfo parameter, ref XmlSchemaSet schemas, string encoding = Encodings.json)
        {
            WADLParam param;
            System.Type type = parameter.ParameterType;
            switch (System.Type.GetTypeCode(type))
            {
                case TypeCode.Object://we generate representations for complex types
                    WADLRepresentation representation = WADLRepresentation.CreateWADLRepresentation(parameter, out schemas);
                    param = CreateWADLParam(parameter);
                    param.Type = representation.Path;
                    param.Encoding = encoding;
                    param.ParameterInfo = parameter;
                    break;
                //but we do not generate representations for primitive types.
                default:
                    param = CreateWADLParam(parameter);
                    break;
            }
            
            return param;
        }

        public static XmlSchemaSet Parse(WADLParam attribute)
        {
            //used to infer the schematype 
            XmlSchemaSet schemas = new XmlSchemaSet();
            WADLParam parameter = CreateWADLParam(attribute.ParameterInfo, ref schemas, attribute.Encoding);
            attribute.Type = parameter.Type;
            attribute.Encoding = parameter.Encoding; 
            return schemas;
        }

        #region IEqualityComparer<WADLParam> Members

        public new bool Equals(object x, object y)
        {
            return ((WADLParam)x).Name.ToLower() == ((WADLParam)y).Name.ToLower();
        }

        public int GetHashCode(object obj)
        {
           return ((WADLParam)obj).Name.ToLower().GetHashCode();
        }

        #endregion

        public static WADLParam[] GetWADLParams(MethodInfo method, WADLParameterAddedHandler onParameterAdded)
        {
               return (from paramater in method.GetParameters()
                    select paramater)
                    .Aggregate<ParameterInfo, List<WADLParam>>(new List<WADLParam>(), delegate(List<WADLParam> list, ParameterInfo parameter)
                    {
                        XmlSchemaSet schemas = null;
                        WADLParam[] parameters = WADLParam.GetWADLParams(parameter);
                        if (parameters.Length == 0)
                        {
                            WADLParam param = CreateWADLParam(parameter, ref schemas);
                            list.Add(param);
                            onParameterAdded(param, schemas);
                            
                        }
                        else
                        {
                           
                            Array.ForEach<WADLParam>(parameters, delegate(WADLParam param) { 
                                param.ParameterInfo = parameter;
                                param.Validate();
                                if(param.Type == SchemaTypes.none) 
                                    schemas = Parse(param);
                                list.Add(param);
                                onParameterAdded(param, schemas);
                            }); 
                        };
                        return list;
                    }).ToArray<WADLParam>();
        }

        public static WADLParam[] GetWADLParams(System.Type type)
        {
            WADLParam[] headers = (WADLParam[])type.GetCustomAttributes(typeof(WADLParam), true);
            return (from header in headers
                    select header).Aggregate<WADLParam, List<WADLParam>>(new List<WADLParam>(), delegate(List<WADLParam> list, WADLParam parameter)
                    {
                        if (((WADLParam)parameter).Style == ParamStyles.Header)
                        {
                            if (((WADLParam)parameter).Type == SchemaTypes.none)
                            {
                                throw new NotSupportedException(String.Format("WADLParam {0} must specify a valid SchemaType", ((WADLParam)parameter).Name));
                            }
                            else
                            {
                                list.Add((WADLParam)parameter);
                            }
                        }
                        else
                            throw new NotSupportedException(String.Format("WADLParam {0} values must have a Style of ParamStyles.Header", ((WADLParam)parameter).Name));
                        return list;
                    }).ToArray<WADLParam>();
        }

        public static WADLParam[] GetWADLParams(MethodInfo method)
        {
            var headers = method.ReturnParameter.GetCustomAttributes(typeof(WADLParam), true);
            return (from header in headers
                    select header).Aggregate<object, List<WADLParam>>(new List<WADLParam>(), delegate(List<WADLParam> list, object parameter)
                    {
                        WADLParam param = ((WADLParam)parameter);
                        if (param.Style == ParamStyles.Header)
                        {
                            param.Validate();
                            list.Add(param);
                        }
                        else
                            throw new NotSupportedException(String.Format("invalid WADLParam Style {0}", param.Name));
                        return list;
                    }).ToArray<WADLParam>();
        }

        public static WADLParam[] GetWADLParams(ParameterInfo parameter)
        {
            WADLParam[] parameters = (WADLParam[])parameter.GetCustomAttributes(typeof(WADLParam), true);
            Array.ForEach(parameters, delegate(WADLParam param){
                param.parameterInfo = parameter;             
            });
            return parameters;
        }
      
    }
}

      