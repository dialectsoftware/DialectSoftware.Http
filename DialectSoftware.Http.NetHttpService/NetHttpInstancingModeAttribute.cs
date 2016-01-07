using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialectSoftware.Http.Services
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NetHttpInstancingModeAttribute:Attribute 
    {
        public InstancingMode Mode
        {
            get;
            set;
        }

        public NetHttpInstancingModeAttribute()
        { 
        
        }

        public NetHttpInstancingModeAttribute(InstancingMode mode)
        {
        }
    }
}
