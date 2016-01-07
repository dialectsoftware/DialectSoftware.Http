using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace WADL
{
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    [XmlRoot(ElementName = "path_variable")]
    public class WADLPathVariable:WADLAttribute 
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


        public WADLPathVariable()
            : base()
        {
            this.type = SchemaTypes.none; 
        }

        public WADLPathVariable(String name)
            : this()
        {
            this.name = name;
        }


        public WADLPathVariable(String name, String type)
            : this()
        {
            this.name = name;
            this.type = type;
        }


        public override void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlRootAttribute root = (XmlRootAttribute)this.GetType().GetCustomAttributes(typeof(XmlRootAttribute), true).First<object>();
            PropertyInfo[] properties = this.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Length == 1)
                {
                    XmlAttributeAttribute attribute = (XmlAttributeAttribute)property.GetCustomAttributes(typeof(XmlAttributeAttribute), true).First<object>();
                    writer.WriteAttributeString(attribute.AttributeName, property.GetValue(this, null).ToString());
                }
            }
        }
    }
}
