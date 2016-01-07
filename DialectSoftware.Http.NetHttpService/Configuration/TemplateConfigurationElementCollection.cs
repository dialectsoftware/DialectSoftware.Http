using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace DialectSoftware.Http.Services.Configuration
{
    [ConfigurationCollection(typeof(TemplateConfigurationElement), AddItemName = "template")]
    public class TemplateConfigurationElementCollection : ConfigurationElementCollection
    {

        public TemplateConfigurationElementCollection()
        {
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new TemplateConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((TemplateConfigurationElement)element).Name;
        }

        public void Add(TemplateConfigurationElement element)
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

        public new TemplateConfigurationElement this[String name]
        {
            get
            {
                TemplateConfigurationElement element = (TemplateConfigurationElement)base.BaseGet(name);
                if (element == null)
                    return new TemplateConfigurationElement(name);
                else
                    return (TemplateConfigurationElement)base.BaseGet(name);
            }
        }
    }
}
