using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

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
