using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WADL;
using System.IO;
using System.Xml.Serialization;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public interface INetHttpEncoder
    {
        object Decode(WADLParam param, object value);
        void Encode(Stream stream, WADLRepresentation param, object value);
       
    }
}
