using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.Collections.ObjectModel;
using System.Net.Cache;
using System.Net;
using WADL;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Collections.Specialized;
using System.Web;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public class NetHttpServiceRequest : IDisposable
    {
        IncomingWebRequestContext request;

        public NetHttpServiceRequest(IncomingWebRequestContext request)
        {
            this.request = request;
            MessageProperties messageProperties = OperationContext.Current.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpointProperty =  messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            Headers.Add("REMOTE_HOST", endpointProperty.Address);
            Params = HttpUtility.ParseQueryString(request.UriTemplateMatch.RequestUri.Query);
            if (Method.Equals("post", StringComparison.CurrentCultureIgnoreCase) || Method.Equals("put", StringComparison.CurrentCultureIgnoreCase))
            {
                NetHttpPostBody postBody;
                if (NetHttpUtility.TryParsePostBody(out postBody))
                {
                    postBody.Parameters.Keys.Cast<String>().ToList().ForEach((k) =>
                    {
                        Params.Add(k, postBody.Parameters[k]);
                    });
                }
            }
        }

        // Summary:
        //     Gets the Accept header value from the incoming Web request.
        //
        // Returns:
        //     The Accept header from the incoming Web request.
        public string Accept { get { return request.Accept; } }
        //
        // Summary:
        //     Gets the ContentLength header value of the incoming Web request.
        //
        // Returns:
        //     The ContentLength header of the incoming Web request.
        public long ContentLength { get { return request.ContentLength; } }
        //
        // Summary:
        //     Gets the ContentType header value from the incoming Web request.
        //
        // Returns:
        //     The ContentType header from the incoming Web request.
        public string ContentType { get { return request.ContentType; } }
        //
        // Summary:
        //     Gets the headers for the incoming Web request.
        //
        // Returns:
        //     A System.Net.WebHeaderCollection instance that contains the headers of the
        //     incoming Web request.
        public WebHeaderCollection Headers { get { return request.Headers; } }
        /// <summary>
        /// 
        /// </summary>
        public NameValueCollection Params
        {
            get;
            private set;
        }
        //
        // Summary:
        //     Gets the HTTP method of the incoming Web request.
        //
        // Returns:
        //     The HTTP method of the incoming Web request.
        public string Method { get { return request.Method; } }
        //
        // Summary:
        //     Gets and sets the System.UriTemplateMatch instance created during the dispatch
        //     of the incoming Web request.
        //
        // Returns:
        //     A System.UriTemplateMatch instance.
        public UriTemplateMatch UriTemplateMatch { get { return request.UriTemplateMatch; } }
        //
        // Summary:
        //     Gets the UserAgent header value from the incoming Web request.
        //
        // Returns:
        //     The UserAgent header from the incoming Web request.
        public string UserAgent { get { return request.UserAgent; } }

        public WADLUtility.Url Url
        {
            get { return new WADLUtility.Url(request.UriTemplateMatch.RequestUri); }
        }

        public HttpRequestCachePolicy CachePolicy
        {
            get { return new HttpRequestCachePolicy(); }
        }


        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
