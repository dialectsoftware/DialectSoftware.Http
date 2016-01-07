using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DialectSoftware.Http.Services.Configuration
{
    public class EncodingConfigurationElement : ConfigurationElement
    {
        public EncodingConfigurationElement():base()
        { 
        
        
        }

        public EncodingConfigurationElement(string name)
            : base()
        {
            this["name"] = name;
        }

        [ConfigurationProperty("name", DefaultValue = "", IsRequired = true)]
        public String Name
        {
            get
            {
                return (String)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("type", DefaultValue = "", IsRequired = true)]
        public String Type
        {
            get
            {
                return (String)this["type"];
            }
            set
            {
                this["type"] = value;
            }
        }
    }
}
