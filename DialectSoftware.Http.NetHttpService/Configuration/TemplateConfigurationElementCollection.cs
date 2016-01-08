using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;


/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

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
