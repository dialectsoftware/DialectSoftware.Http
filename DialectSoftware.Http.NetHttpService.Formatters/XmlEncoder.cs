using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using WADL;
using System.Xml.Serialization;

namespace DialectSoftware.Http.Services.Encoders
{
    public class XmlEncoder : INetHttpEncoder
    {
        public object Decode(WADLParam param, object value)
        {
            XmlSerializer sr = new XmlSerializer(param.ParameterInfo.ParameterType);
            using (StringReader reader = new StringReader((string)value))
            {
                value = sr.Deserialize(System.Xml.XmlReader.Create(reader));
                reader.Close();
            }
            return value;
        }

        public void Encode(Stream stream, WADLRepresentation param, object value)
        {
            if (param.ParameterInfo.ParameterType == typeof(String) && param.Path != SchemaTypes.@string)
            {
                StreamWriter objWriter = new StreamWriter(stream);
                objWriter.AutoFlush = true;
                objWriter.Write(value.ToString());
            }
            else
            {
                XmlSerializer sr = new XmlSerializer(value.GetType());
                sr.Serialize(stream, value);
                stream.Flush();
                stream.Position = 0;
            }

        }

    }
}
