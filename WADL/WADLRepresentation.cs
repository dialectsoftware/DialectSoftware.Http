using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Net;
using System.Xml.Schema;
using System.Xml;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace WADL
{
    public delegate void WADLRepresentationAddedHandler(WADLRepresentation param, XmlSchemaSet schemas);
    /// <summary>
    /// A representation element describes a representation of a resource’s state and can either be declared
    /// globally as a child of the application element, embedded locally as a child of a request or response
    /// element, or referenced externally. A representation element has one of the following two combinations
    /// of attributes:
    /// In addition to the attributes listed , a representation element can have zero or more child representation
    /// variable elements, see section 2.6.1.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = true)]
    [XmlRoot(ElementName = "representation")]
    public class WADLRepresentation : WADLAttribute, IEqualityComparer<WADLRepresentation>
    {
        private string href;
        /// <summary>
        /// Specifies the media type of the representation.
        /// </summary>
        [XmlAttribute(AttributeName = "href", Namespace = "http://research.sun.com/wadl")]
        public String Href
        {
            get { return href; }
            set { href = value; }
        }


        private string contentType;
        /// <summary>
        /// Specifies the media type of the representation.
        /// </summary>
        [XmlAttribute(AttributeName = "mediaType", Namespace = "http://research.sun.com/wadl")]
        public String ContentType
        {
            get { return contentType; }
            set { contentType = value; }
        }


        private string element;
        /// <summary>
        /// For XML-based representations, specifies the qualified name of the root element as described
        /// within the grammars section – see section 2.2.
        /// </summary>
        [XmlAttribute(AttributeName = "element", Namespace = "http://research.sun.com/wadl")]
        public String Path
        {
            get { return element; }
            set { element = value; }
        }


        private HttpStatusCode status;
        /// <summary>
        /// Optionally present on response representations, provides a list of HTTP status codes associated
        /// with a particular representation. Note that multiple sibling representation elements may share one
        /// or more HTTP status codes: such elements may provide equivalent information in different formats.
        /// </summary>
        [XmlAttribute(AttributeName = "status", Namespace = "http://research.sun.com/wadl")]
        public HttpStatusCode Status
        {
            get { return status; }
            set { status = value; }
        }

        private Type type;
        public Type Type
        {
            get { return type; }
            set { type = value; }
        }

        private ParameterInfo parameterInfo;
        public ParameterInfo ParameterInfo
        {
            get { return parameterInfo; }
            set { parameterInfo = value; }
        }

        public WADLRepresentation()
        {
            this.status = HttpStatusCode.OK;
            this.element = SchemaTypes.none;
            this.contentType = ContentTypes.octetStream;
        }

        public WADLRepresentation(string contentType)
            : this()
        {
            this.contentType = contentType;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlRootAttribute root = (XmlRootAttribute)this.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true).First<object>();
            PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Length == 1)
                {
                    string value = null;
                    if (property.Name == "Status")
                    {
                        value = ((int)this.Status).ToString();
                    }
                    else
                    {
                        value = property.GetValue(this, null) as String;
                    }

                    if (!String.IsNullOrEmpty(value))
                    {
                        XmlAttributeAttribute attribute = (XmlAttributeAttribute)property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).First<object>();
                        writer.WriteAttributeString(attribute.AttributeName, value);
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

            Condition IsSupportedType = (delegate()
            {
                if (ParameterInfo != null)
                {
                    switch (Type.GetTypeCode(ParameterInfo.ParameterType))
                    {
                        case TypeCode.String:
                            break;
                        case TypeCode.Object:
                            if (!type.IsArray || !type.IsInstanceOfType(new byte[] { }))
                            {
                                return false; //throw new NotSupportedException(String.Format("WADLRepresentation attribute Type not supported {0}", type.Name));
                            }
                            break;
                        case TypeCode.Empty:
                        case TypeCode.DBNull:
                        default:
                            return false;    //throw new NotSupportedException(String.Format("WADLRepresentation attribute Type not supported {0}", type.Name));
                    }
                }
                return true;
            });

            
            Condition IsValidPathHref = (delegate()
            {
                if (Path != SchemaTypes.none || !String.IsNullOrEmpty(Href))
                {
                    eval(ParameterInfo == null, new NotSupportedException("WADLRepresentation attribute with Path and Href only supported on return types"));
                    eval(!IsSupportedType(), new NotSupportedException(String.Format("WADLRepresentation attribute Type not supported {0}", ParameterInfo.ParameterType.Name)));
                    eval(Type != null, new NotSupportedException("WADLRepresentation attributes that define a Path and Href can't define a Type"));
                    eval(((Path != SchemaTypes.none && Path != SchemaTypes.anyType) && String.IsNullOrEmpty(Href)), new NotSupportedException("WADLRepresentation that define a Path must also define an Href"));
                    eval((!String.IsNullOrEmpty(Href) && (Path == SchemaTypes.none || Path == SchemaTypes.anyType)), new NotSupportedException("WADLRepresentation that define an Href must also define a Path"));
                    return true;
                }
                else
                {
                    return false;
                }
                
             });

            Condition IsValidType = (delegate()
            {
                if (Type != null)
                {
                    eval(!IsSupportedType(), new NotSupportedException(String.Format("WADLRepresentation attribute Type not supported {0}", ParameterInfo.ParameterType.Name)));
                    eval((!String.IsNullOrEmpty(Path) && !String.IsNullOrEmpty(Href)), new NotSupportedException("WADLRepresentation that define a Type must not define an Href or Path"));
                    return true;
                }
                else
                {
                    return false;
                }
                
            });


            eval(String.IsNullOrEmpty(ContentType), new NotSupportedException("WADLRepresentation attributes must define a ContentType"));

            if (!IsValidPathHref())
            {
                IsValidType();
            }


        }

        #region IEqualityComparer<WADLRepresentation> Members

        public bool Equals(WADLRepresentation x, WADLRepresentation y)
        {
            Condition IsDuplicate = delegate()
            {
                bool criteria = x.ContentType.ToLower() == y.ContentType.ToLower();
                criteria = criteria && x.Status == y.Status;
                return criteria;
            };
            return IsDuplicate();
        }

        public int GetHashCode(WADLRepresentation obj)
        {
            return obj.Status.GetHashCode();
        }

        #endregion

        public static WADLRepresentation CreateWADLRepresentation(ParameterInfo parameter, out XmlSchemaSet schemas)
        {
            WADLRepresentation representation = CreateWADLRepresentation(parameter.ParameterType, out schemas);
            representation.ParameterInfo = parameter;
            representation.Type = parameter.ParameterType;
            return representation;
            #region delete
            //WADLRepresentation representation = null;
            //Type type = parameter.ParameterType;
            //if (type.GetInterface("IXmlSerializable") == null)//not IXmlSerializable
            //{
            //    //must infer
            //    if (type.IsSerializable)
            //    {
            //        schemas = WADLUtility.Infer(type);
            //    }
            //    else
            //    {
            //        throw new NotSupportedException(String.Format("{0} must be marked as serializable or implement the IXmlSerializable interface", type.Name));
            //    }
            //}
            //else
            //{
            //    var instance = Activator.CreateInstance(type);
            //    schemas = new XmlSchemaSet();
            //    XmlSchema schema = ((IXmlSerializable)instance).GetSchema();
            //    if (schema == null)
            //    {
            //        throw new NotSupportedException(String.Format("{0} must be marked as serializable or implement the IXmlSerializable interface", type.Name));
            //    }
            //    else
            //    {
            //        schemas.Add(schema);
            //    }
            //}

            //representation = new WADLRepresentation(ContentTypes.xml);
            //representation.Path = WADLUtility.GetSchemaName(schemas, type);
            //representation.ParameterInfo = parameter;
            //if (String.IsNullOrEmpty(representation.Path))
            //{
            //    throw new ArgumentNullException(String.Format("Could not locate Schema for {0}", type.Name));
            //}
            //return representation;
            #endregion
        }

        public static WADLRepresentation CreateWADLRepresentation(Type type, out XmlSchemaSet schemas)
        {
            WADLRepresentation representation = null;
            if (type.GetInterface("IXmlSerializable") == null)//not IXmlSerializable
            {
                //must infer
                if (type.IsSerializable)
                {
                    schemas = WADLUtility.Infer(type);
                }
                else
                {
                    throw new NotSupportedException(String.Format("{0} must be marked as serializable or implement the IXmlSerializable interface", type.Name));
                }
            }
            else
            {
                var instance = Activator.CreateInstance(type);
                schemas = new XmlSchemaSet();
                XmlSchema schema = ((IXmlSerializable)instance).GetSchema();
                if (schema == null)
                {
                    throw new NotSupportedException(String.Format("{0} must be marked as serializable or implement the IXmlSerializable interface", type.Name));
                }
                else
                {
                    schemas.Add(schema);
                }
            }

            representation = new WADLRepresentation(ContentTypes.xml);
            representation.Path = WADLUtility.GetSchemaName(schemas, type);
            
            if (String.IsNullOrEmpty(representation.Path))
            {
                throw new ArgumentNullException(String.Format("Could not locate Schema for {0}", type.Name));
            }
            return representation;
        }

        public static XmlSchemaSet Parse(WADLRepresentation representation)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            if (representation.Path != SchemaTypes.anyType)
            {
                if (representation.Type != null)
                {
                    WADLRepresentation rep = CreateWADLRepresentation(representation.Type, out schemas);
                    representation.Path = rep.Path;
                }
                else
                {
                    if (representation.ParameterInfo != null)
                    {
                        if (String.IsNullOrEmpty(representation.Href))
                        {
                            WADLRepresentation rep = CreateWADLRepresentation(representation.ParameterInfo, out schemas);
                            representation.Type = rep.Type;
                            representation.Path = rep.Path;
                        }
                    }
                }
            }    
            return schemas;
           
        }

        /// <summary>
        /// Get WADLRepresentation from a class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="onRepresentationAdded"></param>
        /// <returns></returns>
        public static WADLRepresentation[] GetWADLRepresentations(System.Type type, WADLRepresentationAddedHandler onRepresentationAdded)
        {
            iff<bool> eval = (delegate(bool candidate, Exception exception)
            {
                if (candidate) throw exception;

            });

            object[] attributes = type.GetCustomAttributes(typeof(WADLRepresentation), true);
            WADLRepresentation[] representations = null;
            if(attributes.Length == 0)
            {
                representations = new  WADLRepresentation[]{};
            }
            else
            {
                representations = (WADLRepresentation[])attributes;
            }
    
            return (from representation in representations
                    select representation).Aggregate<WADLRepresentation, List<WADLRepresentation>>(new List<WADLRepresentation>(), delegate(List<WADLRepresentation> list, WADLRepresentation rep)
                    {
                        Condition IsValidClassRepresentation = delegate()
                        {
                            //only status codes other than 200 allowed at class level
                            eval(rep.Status == HttpStatusCode.OK, new NotSupportedException("only WADLReprentations with a status other than 200 allowed in class definition"));
                            if (rep.Type != null)
                            {
                                //only type that inherit from System.Exception allowed in class
                                eval(rep.Type.IsSubclassOf(typeof(System.Exception)), new NotSupportedException("only classes that inherit from System.Exception allowed as Type attribute of WADLRepresentations in class definition"));
                            }
                            return true;
                        };
                       
                        if (IsValidClassRepresentation())
                            rep.Validate();
                        
                        XmlSchemaSet schemas = Parse(rep);
                        onRepresentationAdded(rep, schemas);
                        list.Add(rep);
                        return list;
                    }).ToArray<WADLRepresentation>();
        }

        /// <summary>
        /// Get WADLRepresentation from a return parameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="onRepresentationAdded"></param>
        /// <returns></returns>
        public static WADLRepresentation[] GetWADLRepresentations(ParameterInfo parameter, WADLRepresentationAddedHandler onRepresentationAdded)
        {
            iff<bool> eval = (delegate(bool candidate, Exception exception)
            {
                if (candidate) throw exception;

            });

            object[] attributes = parameter.GetCustomAttributes(typeof(WADLRepresentation), true);
            WADLRepresentation[] representations = null;
            if (attributes.Length == 0)
            { 
                XmlSchemaSet schemas;
                WADLRepresentation representation = CreateWADLRepresentation(parameter,out schemas);
                onRepresentationAdded(representation, schemas);
                return new WADLRepresentation[] { representation };
            }
            else
            {
                representations = (WADLRepresentation[])attributes;
            }
            bool OKSpecified = false;
            return (from representation in representations
                    select representation).Aggregate<WADLRepresentation, List<WADLRepresentation>>(new List<WADLRepresentation>(), delegate(List<WADLRepresentation> list, WADLRepresentation wadlRepresentation)
                    {
                        wadlRepresentation.ParameterInfo = parameter;
                        wadlRepresentation.Validate();
                        XmlSchemaSet schemas = Parse(wadlRepresentation);
                        onRepresentationAdded(wadlRepresentation, schemas);
                        if (wadlRepresentation.Status == HttpStatusCode.OK)
                        {
                            OKSpecified = true;
                        }
                        list.Add(wadlRepresentation);

                        //atleast 1 must be Status 200
                        eval (list.Count == representations.Length && !OKSpecified, new NotSupportedException("No Staus 200 WADLRepresentation defined"));
                        
                        return list;
                    }).ToArray<WADLRepresentation>();
        }

    
    }
}
