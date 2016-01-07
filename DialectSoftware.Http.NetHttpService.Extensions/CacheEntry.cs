using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialectSoftware.Http.Services
{
    [Serializable]
    public class CacheEntry
    {
        public String ContentType
        {
            get;
            set;
        }

        public byte[] Response
        {
            get;
            set;
        }

    }
}
