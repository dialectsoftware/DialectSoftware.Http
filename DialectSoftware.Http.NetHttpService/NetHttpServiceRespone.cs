using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.IO;
using System.Net;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public class NetHttpServiceRespone:IDisposable
    {
        Stream stream;
        StreamWriter writer;
        OutgoingWebResponseContext response;

        public NetHttpServiceRespone(OutgoingWebResponseContext response, out Stream stream)
        {
            stream = new MemoryStream();
            this.stream = stream;
            this.writer = new StreamWriter(stream); 
            this.response = response;
        }

        // Summary:
        //     Gets and sets the content length header from the outgoing Web response.
        //
        // Returns:
        //     The content length header of the outgoing Web response.
        public long ContentLength { get { return response.ContentLength; } set { response.ContentLength = value; } }
        //
        // Summary:
        //     Gets and sets the content type header from the outgoing Web response.
        //
        // Returns: 
        //     The content type header of the outgoing Web response.
        public string ContentType { get { return response.ContentType; }  set { response.ContentType = value; } }
        //
        // Summary:
        //     Gets and sets the etag header from the outgoing Web response.
        //
        // Returns:
        //     The etag header of the outgoing Web response.
        public string ETag { get { return response.ETag; } set { response.ETag = value; } }
        //
        // Summary:
        //     Gets the headers from the outgoing Web response.
        //
        // Returns:
        //     A System.Net.WebHeaderCollection instance that contains the headers from
        //     the outgoing Web response.
        public WebHeaderCollection Headers { get { return response.Headers; } }
        //
        // Summary:
        //     Gets and sets the last modified header of the outgoing Web response.
        //
        // Returns:
        //     A System.DateTime instance that contains the time the requested resource
        //     was last modified.
        public DateTime LastModified { get { return response.LastModified; } set { response.LastModified = value; } }
        //
        // Summary:
        //     Gets and sets the location header from the outgoing Web response.
        //
        // Returns:
        //     The location header from the outgoing Web response.
        public string Location { get { return response.Location; } set { response.Location = value; } }
        //
        // Summary:
        //     Gets and sets the status code of the outgoing Web response.
        //
        // Returns:
        //     An System.Net.HttpStatusCode instance that contains the status code of the
        //     outgoing Web response.
        public HttpStatusCode StatusCode { get { return response.StatusCode; } set { response.StatusCode = value; } }
        //
        // Summary:
        //     Gets and sets the status description of the outgoing Web response.
        //
        // Returns:
        //     The status description of the outgoing Web response.
        public string StatusDescription { get { return response.StatusDescription; } set { response.StatusDescription = value; } }
        //
        // Summary:
        //     Gets and sets a value that indicates whether Windows Communication Foundation
        //     (WCF) omits data that is normally written to the entity body of the response
        //     and forces an empty response to be returned.
        //
        // Returns:
        //     If true, WCF omits any data that is normally written to the entity body of
        //     the response and forces an empty response to be returned. The default value
        //     is false.
        public bool SuppressEntityBody { get { return response.SuppressEntityBody; } set { response.SuppressEntityBody = value; } }

        // Summary:
        //     Sets the HTTP status code of the outgoing Web response to System.Net.HttpStatusCode.Created
        //     and sets the Location header to the provided URI.
        //
        // Parameters:
        //   locationUri:
        //     The System.Uri instance to the requested resource.
        public void SetStatusAsCreated(Uri locationUri)
        {
            response.SetStatusAsCreated(locationUri);
        }
        //
        // Summary:
        //     Sets the HTTP status code of the outgoing Web response to System.Net.HttpStatusCode.NotFound.
        public void SetStatusAsNotFound()
        {
            response.SetStatusAsNotFound();
        }
        //
        // Summary:
        //     Sets the HTTP status code of the outgoing Web response to System.Net.HttpStatusCode.NotFound
        //     with the specified description.
        //
        // Parameters:
        //   description:
        //     The description of status.
        public void SetStatusAsNotFound(string description)
        {
            response.SetStatusAsNotFound(description);
        }

        public void Write(object value)
        {
            if (value is byte[])
            {
                byte[] bytes = (byte[])value;
                this.writer.BaseStream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                this.writer.Write(value);
            }
        }

        public Stream Buffer
        {
            get { return stream;  }
        }

        #region IDisposable Members

        public void Dispose()
        {
            writer.Flush();
            stream.Flush();
            stream.Position = 0;
        }

        #endregion
    }
}
