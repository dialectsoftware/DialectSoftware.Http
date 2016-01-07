using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace WADL
{
    delegate void iff<T>(T condition, Exception exception);
    delegate bool Condition();

    public abstract class WADLAttribute:Attribute,IXmlSerializable 
    {
        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public abstract void WriteXml(System.Xml.XmlWriter writer);

        public abstract void Validate();
 
        #endregion
    }
}
