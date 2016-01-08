using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Net;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace WADL
{
    /// <summary>
    /// A representation element describes a representation of a resource’s state and can either be declared
    /// globally as a child of the application element, embedded locally as a child of a request or response
    /// element, or referenced externally. A representation element has one of the following two combinations
    /// of attributes:
    /// In addition to the attributes listed , a representation element can have zero or more child representation
    /// variable elements, see section 2.6.1.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [XmlRoot(ElementName = "doc")]
    public class WADLDoc : WADLAttribute
    {
        private string title;
        [XmlAttribute(AttributeName = "title", Namespace = "http://research.sun.com/wadl")]
        public String Title
        {
            get { return title; }
            set { title = value; }
        }

        private string language;
        [XmlAttribute(AttributeName = "xml:lang",Namespace = "http://research.sun.com/wadl")]
        public String Language
        {
            get { return language; }
            set { language = value; }
        }

        private string documentation;
        /// <summary>
        /// Specifies the media type of the representation.
        /// </summary>
        public String Documentation
        {
            get { return documentation; }
            set { documentation = value; }
        }

        private WADLDoc()
        { 
        
        }

        public WADLDoc(string documentation)
        {
            this.language = "en-us";
            this.documentation = documentation;
        }

        public WADLDoc(string title, string documentation):this(documentation)
        {
            this.title = title;
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
            writer.WriteString(documentation);
        }

        public override void Validate()
        {
            throw new NotImplementedException();
        }

        public static WADLDoc GetWADLDoc(System.Type type)
        {
            WADLDoc[] doc = (WADLDoc[])type.GetCustomAttributes(typeof(WADLDoc), true);
            return doc.Length == 0 ? null : (WADLDoc)doc.Single<object>();
        }

        public static WADLDoc GetWADLDoc(MethodInfo method)
        {
            WADLDoc[] doc = (WADLDoc[])method.GetCustomAttributes(typeof(WADLDoc), true);
            return doc.Length == 0 ? null : (WADLDoc)doc.Single<object>();
        }
    }
}
