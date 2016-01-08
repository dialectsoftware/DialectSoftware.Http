using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
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
    
    [AttributeUsage(AttributeTargets.Class,AllowMultiple=false,Inherited=true)] 
    [XmlRoot(ElementName="application",Namespace=CONSTANTS.Namespace)]
    public class WADLApplication : WADLAttribute
    {
        
        Dictionary<string,string> ns;

        [XmlIgnore()]
        private Dictionary<string, string> Namespaces
        {
            get { return ns; }

        }

        private string name;

        [XmlAttribute(AttributeName = "name", Namespace = "urn:wadl:extension")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public WADLApplication()
            : base()
        {
            this.resources = new List<WADLResource>();
            this.ns = new Dictionary<string,string>();
            this.ns.Add("xsd", "http://www.w3.org/2001/XMLSchema");
            this.ns.Add("xsi","http://www.w3.org/2001/XMLSchema-instance");
            this.hrefs = new List<string>();
            this.grammars = new Dictionary<String, XmlSchema>();
        }

        public WADLApplication(String name)
            : this()
        {
            this.name = name;
        }


        public WADLApplication(String name, params String[] namspaces)
            : this(name)
        {
            foreach(var namspace in namspaces)
            {
                this.ns.Add(namspace.Split('=')[0], namspace.Split('=')[1]);
            }
        }

        private List<WADLResource> resources;
        public List<WADLResource> Resources
        {
            get { return resources; }
        }

        private Dictionary<String, XmlSchema> grammars;
        internal Dictionary<String, XmlSchema> Grammars
        {
            get { return grammars; }
        }

        private List<String> hrefs;
        internal List<String> Includes
        {
            get { return hrefs; }
        }

        private WADLDoc doc;
        public WADLDoc Doc
        {
            get { return doc; }
            set { doc = value; }
        }

        public override void WriteXml(XmlWriter writer)
        {
            XmlRootAttribute root = (XmlRootAttribute)this.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true).First<object>();
            PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach(PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Length  == 1)
                {
                    XmlAttributeAttribute attribute = (XmlAttributeAttribute)property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).First<object>();
                    writer.WriteAttributeString("ext",attribute.AttributeName, attribute.Namespace, property.GetValue(this, null).ToString());
                }
            }
            foreach (string @namespace in ns.Keys)
            {
                writer.WriteAttributeString("xmlns", @namespace,null, ns[@namespace]);
            }
            writer.WriteAttributeString("xsi", "schemaLocation", null, CONSTANTS.Namespace);

            if (Doc != null)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLDoc));
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

            writer.WriteStartElement("grammars");
            foreach (String href in hrefs)
            {
                writer.WriteStartElement("include");
                writer.WriteAttributeString("href", href);
                writer.WriteEndElement();
            }

            foreach (String key in grammars.Keys)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSchema schema = grammars[key];
                    schema.Namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema");
                    schema.Write(mem);
                    mem.Flush();
                    mem.Position = 0;
                    XmlReader reader = XmlReader.Create(mem);
                    reader.MoveToContent();
                    writer.WriteRaw(reader.ReadInnerXml());
                    mem.Close();
                }

            }
            
            writer.WriteEndElement();
            
            
            foreach (WADLResource resource in Resources)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    XmlSerializer rez = new XmlSerializer(typeof(WADLResource));
                    rez.Serialize(mem, resource);
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

        public static WADLApplication GetWADLApplication(System.Type type)
        {
            WADLApplication application = (WADLApplication)type.GetCustomAttributes(typeof(WADLApplication), true).Single<object>();
            application.Doc = WADLDoc.GetWADLDoc(type);
            return application;
        }
    }
}
