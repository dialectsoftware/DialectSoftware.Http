﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.IO;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public class NetHttpContext : IDisposable
    {
        WebOperationContext context;
        NetHttpServiceRequest request;

        public NetHttpServiceRequest Request
        {
            get { return request; }
            set { request = value; }
        }

        NetHttpServiceRespone response;

        public NetHttpServiceRespone Response
        {
            get { return response; }
            set { response = value; }
        }

        public NetHttpContext(WebOperationContext context, out Stream stream)
        {
            this.context = context;
            this.request = new NetHttpServiceRequest(context.IncomingRequest);
            this.response = new NetHttpServiceRespone(context.OutgoingResponse, out stream);
            
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.Request.Dispose();
            this.Response.Dispose();
        }

        #endregion
    }
}
