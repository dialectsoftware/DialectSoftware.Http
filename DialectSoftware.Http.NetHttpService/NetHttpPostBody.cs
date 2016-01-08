using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public class NetHttpPostBody
    {
        internal NetHttpPostBody()
        {
            Parameters = new NameValueCollection();
        }

        internal NetHttpPostBody(NameValueCollection parameters)
        {
            Parameters = parameters;
        }

        public NameValueCollection Parameters
        {
            get;
            private set;
        }

        public string ContentType
        {
            get;
            internal set;
        }

        public string Filename
        {
            get;
            internal set;
        }

        public byte[] FileContents
        {
            get;
            internal set;
        }
    }
}
