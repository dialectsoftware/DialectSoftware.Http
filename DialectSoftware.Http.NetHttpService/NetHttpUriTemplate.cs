using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WADL;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public class NetHttpUriTemplate:UriTemplate 
    {
        WADLUtility.Url url;

        public WADLUtility.Url Url
        {
            get{return url;}
        }

        public NetHttpUriTemplate(WADLUtility.Url url)
            : this(url, true)
        { 
          
        }

        public NetHttpUriTemplate(WADLUtility.Url url, bool ignoreTrailingSlash)
            : base(url.PathAndQuery, ignoreTrailingSlash)
        { 
            this.url = url;
        }
    }
}
