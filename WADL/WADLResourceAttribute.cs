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
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    [XmlRoot(ElementName = "resources")]
    public class WADLResource:WADLAttribute 
    {
        
        Uri baseUri;
        [XmlAttribute(AttributeName = "base", Namespace = "http://research.sun.com/wadl")]
        public Uri BaseUri
        {
            get { return baseUri; }
            set { baseUri = value; }
        }

        public WADLResource()
        {
            methods = new List<WADLMethod>();
            schemas = new XmlSchemaSet();
        }

        public WADLResource(String relativeUri):this()
        {
            this.relativeUri = relativeUri;
        }

        private string relativeUri;
        public string RelativeUri
        {
            get { return relativeUri; }
        }

        private List<WADLMethod> methods;
        public List<WADLMethod> Methods
        {
            get{return methods; }
        }

        private XmlSchemaSet schemas;
        public XmlSchemaSet Schemas
        {
            get { return schemas; }
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlRootAttribute root = (XmlRootAttribute)this.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true).First<object>();
            PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            PropertyInfo property = properties.First<PropertyInfo>(delegate(PropertyInfo prop){
               return prop.Name == "BaseUri" ;
            });
    
            XmlAttributeAttribute attribute = (XmlAttributeAttribute)property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Single<object>();
            writer.WriteAttributeString(attribute.AttributeName, WADLUtility.Url.CreateUrl(this.BaseUri.OriginalString, relativeUri));
    
            foreach (WADLMethod method in Methods)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLMethod),root.Namespace);
                    rez.Serialize(mem, method);
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

        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }

        public static WADLResource GetWADLResource(System.Type type, Uri baseUri)
        {
            WADLResource resource = (WADLResource)type.GetCustomAttributes(typeof(WADLResource), true).Single<object>();
            resource.BaseUri =baseUri;
            return resource;
        }
    }
}
