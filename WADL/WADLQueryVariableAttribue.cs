using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;

namespace WADL
{
    
    [XmlRoot(ElementName = "query_variable")]
    public class WADLQueryVariable:WADLPathVariable 
    {
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


        public WADLQueryVariable()
            : base()
        {
            this.required = false;
            this.repeating = false;
        }

        public WADLQueryVariable(String name)
            : base(name)
        {
            this.required = false;
            this.repeating = false;
        }


        public WADLQueryVariable(String name, String type)
            : base(name, type)
        {
            this.required = false;
            this.repeating = false;   
        }
    }
}
