using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Enterprise.Caching; 
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.ServiceModel.Web;
using System.IO;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace DialectSoftware.Http.Services
{
    public class CacheExtension : BehaviorExtensionElement, IEndpointBehavior,IOperationInvoker, IOperationBehavior 
    {
        private ICacheProvider provider;
        private IOperationInvoker innerInvoker;

        public CacheExtension()
        {
    
        }

        internal CacheExtension(IOperationInvoker invoker)
        {
            innerInvoker = invoker;
            provider = CacheProviderFactory.CreateProvider("RESTCacheProvider");
        }

        private string GenerateCachekey()
        {
            //Uri uri = OperationContext.Current.RequestContext.RequestMessage.Properties["Via"] as Uri;
            return WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.ToString();

        }

        private string GetCacheControlHeader()
        {
            return WebOperationContext.Current.OutgoingResponse.Headers[System.Net.HttpResponseHeader.CacheControl]; 
        }

        private string GetExpiresHeader()
        {
            return WebOperationContext.Current.OutgoingResponse.Headers[System.Net.HttpResponseHeader.Expires];
        }

        private byte[] GetBytes(Stream input)
        {
            long position = input.Position;
            byte[] buffer = new byte[16*1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                buffer = ms.ToArray();
                ms.Close();
            }
            input.Position = 0;
            return buffer;
        }

        #region IOperationInvoker Members

        public object[] AllocateInputs()
        {
            return innerInvoker.AllocateInputs();
        }

        public object Invoke(object instance, object[] inputs, out object[] outputs)
        {
            object o = null;
            string key = GenerateCachekey();
            if(provider.Contains(key))
            {
                outputs = new object[] { };
                WebOperationContext.Current.OutgoingResponse.Headers.Add("X-Cache-Lookup", "NetHttpService Cache Extension v1.0 ");
                CacheEntry entry = provider.GetData<CacheEntry>(key);
                WebOperationContext.Current.OutgoingResponse.ContentType = entry.ContentType;
                o = new MemoryStream(entry.Response);
            }
            else
            {
                o = innerInvoker.Invoke(instance, inputs, out outputs);
                byte[] bytes = GetBytes((Stream)o);
                WebOperationContext.Current.OutgoingResponse.Headers.Add("X-Cache", "MISS from NetHttpService Cache Extension v1.0 ");
                CacheEntry entry = new CacheEntry(){ContentType = WebOperationContext.Current.OutgoingResponse.ContentType, Response = bytes};
                provider.Add(key, entry);
            }
            return o;
        }

        public IAsyncResult InvokeBegin(object instance, object[] inputs, AsyncCallback callback, object state)
        {
            return innerInvoker.InvokeBegin(instance, inputs, callback, state);
        }

        public object InvokeEnd(object instance, out object[] outputs, IAsyncResult result)
        {
            return innerInvoker.InvokeEnd(instance, out outputs, result);
        }

        public bool IsSynchronous
        {
            get { return innerInvoker.IsSynchronous; }
        }

        #endregion

        #region IEndpointBehavior Members

        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {

           
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpoint.Contract.Operations.ToList().ForEach(o=>o.Behaviors.Add(new CacheExtension())); 
            //(from o in endpointDispatcher.DispatchRuntime.Operations select o).ToList().ForEach(o => o. new CacheExtension(o.Invoker));
       
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            
        }

        #endregion

        #region BehaviorExtensionElement

        public override Type BehaviorType
        {
            get { return typeof(CacheExtension); }
        }

        protected override object CreateBehavior()
        {
            return new CacheExtension() as IEndpointBehavior;
        }
        
        #endregion

        #region IOperationBehavior Members

        public void AddBindingParameters(OperationDescription operationDescription, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
            
        }

        public void ApplyClientBehavior(OperationDescription operationDescription, ClientOperation clientOperation)
        {
        }

        public void ApplyDispatchBehavior(OperationDescription operationDescription, DispatchOperation dispatchOperation)
        {
            dispatchOperation.Invoker = new CacheExtension(dispatchOperation.Invoker);
        }

        public void Validate(OperationDescription operationDescription)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

}
