using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;
using System.Configuration;
using DialectSoftware.Http.Services.Configuration;
using System.Reflection;
using System.Net;
using System.Web;
using System.Xml.Linq;
using System.ServiceModel.Activation;
using System.Collections.Specialized;
using WADL;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

//TODO: cache
//TODO: add use of header variables in request.
//TODO: chained methods where result from one item in the url is then passed to the next item in the url as a parameter.
//TODO: logging
//TODO: security
//client:"WcfTestClient.exe"
namespace DialectSoftware.Http.Services
{
    
    /// <summary>
    /// Refactor this as a router
    /// </summary>
    [WADLApplication("", "channels=http://www.dialectsoftware.com/channels/")]
    [WADLResource("/")]
    [WADLDoc("This is the NetHttpService channel manager")]
    [AspNetCompatibilityRequirements(RequirementsMode=AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] 
    public class NetHttpService : NetHttpChannelManager,INetHttpService
    {
        public struct Status
        {
            public ServiceStatus CurrentStatus;
            public String Message;
        }

        #region INetHttpService Members

        List<NetHttpChannelDispatcher> Templates
        {
            get;
            set;
        }

        Status State
        {
            get;
            set;
        }

        public NetHttpService()
            : base(WADL.WADLUtility.Url.CreateUrl(TemplateConfigurationHandler.GetConfiguration().BaseUri))
        {
            try
            {
                TemplateConfigurationHandler.GetConfiguration().CreateChannels(this);
                Templates = GetTemplates();
                State = new Status { CurrentStatus = ServiceStatus.OK, Message = "" };
            }
            catch (Exception e)
            {
                State = new Status { CurrentStatus = ServiceStatus.FAULT, Message = e.ToString() };
            }
        }

        public NetHttpService(WADLUtility.Url baseUrl)
            : base(baseUrl)
            //: base(WADL.WADLUtility.Url.CreateUrl(baseUrl))
        {
            
        }

        public virtual Stream Get(Message request)
        {
            return ProcessRequest();
        }

        public virtual Stream Post(Message request)
        {
            return ProcessRequest();
        }

        public virtual Stream Put(Message request)
        {
            return ProcessRequest(); //throw new HttpException((int)HttpStatusCode.MethodNotAllowed, "method not allowed");
        }

        public virtual Stream Delete(Message request)
        {
            return ProcessRequest();
            //throw new HttpException((int)HttpStatusCode.MethodNotAllowed, "method not allowed");
        }

        #endregion

        private Stream ProcessRequest()
        {
            if (State.CurrentStatus == ServiceStatus.FAULT)
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Server-Status", State.CurrentStatus.ToString());
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Server-Status-Message", System.Web.HttpUtility.UrlEncode(State.Message));
                return null;
            }
            else
            {
                WebOperationContext.Current.OutgoingResponse.Headers.Add("Server-Status", State.CurrentStatus.ToString());
            }

            Stream output;
            using (NetHttpContext context = new NetHttpContext(WebOperationContext.Current, out output))
            {
                NetHttpChannelDispatcher dispatcher = null;
                try
                {
                    var matches = (from keys in Templates
                                   select keys).Where<NetHttpChannelDispatcher>(delegate(NetHttpChannelDispatcher item)
                                   {
                                       //return item.IsMatch(new Uri(Configuration.BaseUri), context.Request.Url);
                                       return item.IsMatch(this.BaseUrl, context.Request.Url);

                                   }).ToArray<NetHttpChannelDispatcher>();

                    switch (matches.Length)
                    {
                        case 0:
                            throw new HttpException((int)HttpStatusCode.NotFound, "not found");
                        case 1:
                            dispatcher = matches[0];
                            break;
                        default:
                            try
                            {
                                if (context.Request.Params.Count == 0)
                                {
                                    var nonQuery = (from match in matches
                                                    select match).Where(delegate(NetHttpChannelDispatcher item)
                                                    {
                                                        return item.MatchType == MatchType.Static;
                                                    });

                                    if (nonQuery.Count() == 0)
                                    {
                                        nonQuery = (from match in matches
                                                    select match).Where(delegate(NetHttpChannelDispatcher item)
                                                    {
                                                        return item.MatchType == MatchType.Template;
                                                    });
                                    }

                                    dispatcher = nonQuery.Single();
                                }
                                else
                                {
                                    var query = (from match in matches
                                                 select match).Where(delegate(NetHttpChannelDispatcher item)
                                                 {
                                                     return (item.MatchType & MatchType.Static) == MatchType.Static && (item.MatchType & MatchType.Query) == MatchType.Query;
                                                 });

                                    if (query.Count() == 0)
                                    {
                                        query = (from match in matches
                                                 select match).Where(delegate(NetHttpChannelDispatcher item)
                                                 {
                                                     return (item.MatchType & MatchType.Query) == MatchType.Query; //&& item.Method.Parameters.Count() == context.Request.Params.Count;
                                                 });
                                    }
                                    
                                    if (query.Count() > 1)
                                    {
                                        query = (from match in query
                                                 select match).Where(delegate(NetHttpChannelDispatcher item)
                                                 {
                                                     return (item.MatchType & MatchType.Query) == MatchType.Query && item.Method.Parameters.Count() == context.Request.Params.Count;
                                                 });
                                    }

                                    dispatcher = query.Single();
                                }
                            }
                            catch (InvalidOperationException)
                            {
                                int max = matches.Max<NetHttpChannelDispatcher>(m => ((NetHttpUriTemplate)m.Template).Url.GetMatchingSegments(context.Request.Url).Length);
                                dispatcher = (from match in matches
                                              select match).Where(delegate(NetHttpChannelDispatcher candidate)
                                              {
                                                  bool use = ((NetHttpUriTemplate)candidate.Template).Url.GetMatchingSegments(context.Request.Url).Length == max;
                                                  if (context.Request.Params.Count == 0)
                                                  {
                                                      use = use && (((candidate.MatchType & MatchType.Static) == MatchType.Static) || ((candidate.MatchType & MatchType.Template) == MatchType.Template));
                                                  }
                                                  else
                                                  {
                                                      use = use && ((candidate.MatchType & MatchType.Query) == MatchType.Query);
                                                  }
                                                  return use;
                                              }).Single();
                            }

                            break;

                    }

                    dispatcher.Invoke(context);

                }
                //TODO: Find best approach
                catch (HttpException httpException)
                {
                    //context.Response.ContentType = "text/plain; charset=UTF-8";
                    //context.Response.StatusCode = (HttpStatusCode)httpException.GetHttpCode();
                    //context.Response.StatusDescription = httpException.Message;
                    //context.Response.SetStatusAsNotFound(httpException.Message);
                    switch (httpException.ErrorCode)
                    {
                        case 404:
                            context.Response.SetStatusAsNotFound(httpException.Message);
                            context.Response.SuppressEntityBody = true;
                            break;
                        default:
                            throw httpException;
                    }
                }
                catch (System.Exception exception)
                {
                    var httpException = exception.InnerException as HttpException;
                    if (httpException == null)
                    {
                        context.Response.ContentType = "text/plain; charset=UTF-8";
                        context.Response.StatusCode = HttpStatusCode.BadRequest;
                        context.Response.StatusDescription = exception.Message;
                        
                    }
                    else
                    {
                        switch (httpException.ErrorCode)
                        { 
                            case 404:
                                context.Response.SetStatusAsNotFound(httpException.Message);
                                context.Response.SuppressEntityBody = true;
                                break;
                            default:
                                throw exception.InnerException;
                        }
                        
                    }
                }

                return output;
            }
        }
    }


 


    


}
