using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WADL;
using System.IO;
using System.Xml.Serialization;

namespace DialectSoftware.Http.Services
{
    public interface INetHttpEncoder
    {
        object Decode(WADLParam param, object value);
        void Encode(Stream stream, WADLRepresentation param, object value);
       
    }
}
