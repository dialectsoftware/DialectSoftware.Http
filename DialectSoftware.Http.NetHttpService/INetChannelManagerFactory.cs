using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WADL;

namespace DialectSoftware.Http.Services
{
    public interface INetChannelManagerFactory
    {
        NetHttpChannelManager CreateChannelManager(WADLUtility.Url baseUrl, NetHttpChannelManager parent);
    }
}
