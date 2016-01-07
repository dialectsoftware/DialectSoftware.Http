using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WADL;

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
