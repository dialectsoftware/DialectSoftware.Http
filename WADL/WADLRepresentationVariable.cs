using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace WADL
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    [XmlRoot(ElementName = "representation_variable")]
    public class WADLRepresentationVariable:WADLAttribute
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

        private string path;
        [XmlAttribute(AttributeName = "path", Namespace = "http://research.sun.com/wadl")]
        public String Path
        {
            get { return path; }
            set { path = value; }
        }

        private bool required;
        [XmlAttribute(AttributeName = "required", Namespace = "http://research.sun.com/wadl")]
        public bool IsRequired
        {
            get { return required; }
            set { required = value; }
        }

        private bool repeating;
        [XmlAttribute(AttributeName = "repeating", Namespace = "http://research.sun.com/wadl")]
        public bool IsRepeating
        {
            get { return repeating; }
            set { repeating = value; }
        }

        private object @fixed;
        [XmlAttribute(AttributeName = "@fixed", Namespace = "http://research.sun.com/wadl")]
        public object Fixed
        {
            get { return @fixed; }
            set { @fixed = value; }
        }

        public WADLRepresentationVariable()
        {
            this.required = false;
            this.repeating = false;
        }

        public WADLRepresentationVariable(string name, string type):this()
        {
            this.name = name;
            this.type = type;
        }

        public WADLRepresentationVariable(string name, string type, string path):this(name, type)
        {
            this.path = path;
        }

        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlRootAttribute root = (XmlRootAttribute)this.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true).First<object>();
            PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Length == 1)
                {
                    var value = property.GetValue(this, null);
                    if (value!= null)
                    {
                        XmlAttributeAttribute attribute = (XmlAttributeAttribute)property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).First<object>();
                        writer.WriteAttributeString(attribute.AttributeName, value.ToString());
                    }
                }
            }
        }
    }
}
