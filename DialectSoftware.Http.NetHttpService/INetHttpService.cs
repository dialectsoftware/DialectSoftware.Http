using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel.Activation;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    [ServiceContract]
    public interface INetHttpService
    {
        [OperationContract]
        [WebGet(UriTemplate="*")]
        Stream Get(Message request);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "*", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Post(Message request);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "*", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Put(Message request);


        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "*", BodyStyle = WebMessageBodyStyle.Bare)]
        Stream Delete(Message request);
    }

}
