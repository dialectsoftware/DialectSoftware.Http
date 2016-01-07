using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using WADL;

namespace DialectSoftware.Http.Services.Configuration
{
    public class TemplateConfigurationElement : ConfigurationElement, INetChannelManagerFactory
    {
        public TemplateConfigurationElement()
            : base()
        {

        }

        public TemplateConfigurationElement(String name)
            : this()
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

        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public TemplateConfigurationElementCollection Templates
        {
            get
            {
                return (TemplateConfigurationElementCollection)this[""];
            }
            set
            {
                this[""] = value;
            }
        }


        #region INetChannelManagerFactory Members

        public NetHttpChannelManager CreateChannelManager(WADLUtility.Url baseUrl, NetHttpChannelManager parent)
        {
            NetHttpChannelManager channel = (NetHttpChannelManager)Activator.CreateInstance(System.Type.GetType(this.Type), new object[] { baseUrl });
            channel.Parent = parent;

            foreach (TemplateConfigurationElement template in Templates)
            {
                channel.InnerChannels.Add(template.CreateChannelManager(channel.ChannelUrl, channel));
            }
            return channel;
        }

        #endregion
    }
}






