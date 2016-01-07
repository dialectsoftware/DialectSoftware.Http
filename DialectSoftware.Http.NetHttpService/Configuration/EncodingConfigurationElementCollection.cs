using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DialectSoftware.Http.Services.Configuration
{
    [ConfigurationCollection(typeof(EncodingConfigurationElement), AddItemName = "encoder")]
    public class EncodingConfigurationElementCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("defaultEncoder", DefaultValue = "", IsRequired = true)]
        public String DefaultEncoder
        {
            get
            {
                return (String)base["defaultEncoder"];
            }
            set
            {
                base["defaultEncoder"] = value;
            }
        }

        public EncodingConfigurationElementCollection()
        {
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new EncodingConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((EncodingConfigurationElement)element).Name;
        }

        public void Add(EncodingConfigurationElement element)
        {
            this.BaseAdd(element);
        }

        public void Remove(string key)
        {
            this.BaseRemove(key);
        }

        public void Clear()
        {
            this.BaseClear();
        }

        public new EncodingConfigurationElement this[String name]
        {
            get
            {
                EncodingConfigurationElement element = (EncodingConfigurationElement)base.BaseGet(name);
                if (element == null)
                    return new EncodingConfigurationElement(name) { Type = DefaultEncoder };
                else
                    return (EncodingConfigurationElement)base.BaseGet(name);
            }
        }
    }
}
