using System;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using WADL;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services.Configuration
{
    public class TemplateConfigurationHandler : ConfigurationSection
    {
        public TemplateConfigurationHandler()
        {
        }

        [ConfigurationProperty("baseUri", IsRequired = true)]
        public String BaseUri
        {
            get
            {
                return (String)this["baseUri"];
            }
            set
            {
                this["baseUri"] = value;
            }
        }

        [ConfigurationProperty("encoders", IsRequired = true, IsDefaultCollection = false)]
        public EncodingConfigurationElementCollection Encoders
        {
            get
            {
                return (EncodingConfigurationElementCollection)this["encoders"];
            }
            set
            {
                this["encoders"] = value;
            }
        }

        [ConfigurationProperty("templates", IsRequired = true)]
        public TemplateConfigurationElementCollection Templates
        {
            get
            { 
                return (TemplateConfigurationElementCollection)this["templates"]; 
            }
            set
            { 
                this["templates"] = value; 
            }
        }


        public void CreateChannels(NetHttpChannelManager root)
        {
            foreach (TemplateConfigurationElement template in Templates)
            {
                root.InnerChannels.Add(template.CreateChannelManager(new WADLUtility.Url(BaseUri), root));
            };
        }



        public static TemplateConfigurationHandler GetConfiguration()
        {
            return ConfigurationManager.GetSection("netHttpService") as TemplateConfigurationHandler;
        }

    }
}
