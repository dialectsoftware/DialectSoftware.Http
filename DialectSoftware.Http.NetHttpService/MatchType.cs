using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialectSoftware.Http.Services
{
    [Flags]
    public enum MatchType
    {
        None = 1,
        Query = 2,
        Static = 4,
        Template = 8
    }

}
