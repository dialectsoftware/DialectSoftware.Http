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
    public class DefaultEncoder:INetHttpEncoder
    {
        public object Decode(WADLParam param, object value)
        {
            throw new NotImplementedException();
        }

        public void Encode(Stream stream, WADLRepresentation param, object value)
        {
            switch (System.Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.SByte:
                case TypeCode.Byte:
                    stream.WriteByte((byte)value);
                break;
                case TypeCode.Object:
                if (value is Byte[])
                    {
                        byte[] bytes = (byte[])value;
                        stream.Write(bytes, 0, bytes.Length);
                    }
                    else
                    {
                        StreamWriter objWriter = new StreamWriter(stream);
                        objWriter.AutoFlush = true;
                        objWriter.Write(value.ToString());
                    }
                    break;
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    break;
                case TypeCode.Char:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Boolean:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.DateTime:
                case TypeCode.String:
                default:
                    StreamWriter defaultWriter = new StreamWriter(stream);
                    defaultWriter.AutoFlush = true;
                    defaultWriter.Write(value.ToString());
                    
                    break;
            }

           

        }
   
    }
}
