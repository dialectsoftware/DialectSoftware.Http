using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Reflection;
using System.IO;
using System.Net;

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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = true)]
    [XmlRoot(ElementName = "fault")]
    public class WADLFault : WADLAttribute, IEqualityComparer<object>
    {

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
        
        private ParameterInfo parameterInfo;

        public ParameterInfo ParameterInfo
        {
            get { return parameterInfo; }
            set { parameterInfo = value; }
        }

        public WADLFault()
        {
            this.status = HttpStatusCode.BadRequest;
            this.contentType = ContentTypes.octetStream;
        }

        public WADLFault(string contentType)
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
            throw new NotImplementedException();
        }

        #region IEqualityComparer<object> Members

        public new bool Equals(object x, object y)
        {
            return ((WADLFault)x).status == ((WADLFault)y).status && ((WADLFault)x).ContentType.ToLower() == ((WADLFault)y).ContentType.ToLower();
        }

        public int GetHashCode(object obj)
        {
            return this.status.GetHashCode();
        }

        #endregion

        public static WADLFault[] GetAttributes(System.Type type)
        {
            return (WADLFault[])type.GetCustomAttributes(typeof(WADLFault), true);
        }
    }
}
