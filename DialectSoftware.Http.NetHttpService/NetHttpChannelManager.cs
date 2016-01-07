using WADL;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Net;
using System.Web.UI;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Threading;

//http://www.mnot.net/cache_docs/#CACHE-CONTROL
namespace DialectSoftware.Http.Services
{
    [WADLRepresentation(Status = HttpStatusCode.Ambiguous)]
    [WADLRepresentation(Status = HttpStatusCode.NotFound)]
    [WADLRepresentation(Status = HttpStatusCode.NotImplemented)]
    [WADLRepresentation(Status = HttpStatusCode.InternalServerError)]
    [WADLRepresentation(Status = HttpStatusCode.BadRequest)]
    [WADLRepresentation(Status = HttpStatusCode.NotImplemented)]
    [WADLRepresentation(Status = HttpStatusCode.NotModified)]
    [WADLRepresentation(Status = HttpStatusCode.RequestTimeout)]
    [WADLRepresentation(Status = HttpStatusCode.Unauthorized)]
    [NetHttpInstancingMode(InstancingMode.ReentrantSingleton)]
    public abstract class NetHttpChannelManager
    {
        String rss;
        String wadl;

        protected internal WADLUtility.Url BaseUrl
        {
            get;
            private set;
        }

        protected internal WADLUtility.Url ChannelUrl
        {
            get;
            private set;
        }

        protected internal NetHttpChannelManager Parent
        {
            get;
            internal set;
        }

        protected internal List<NetHttpChannelManager> InnerChannels
        {
            get;
            private set;
        }

        public NetHttpChannelManager(WADLUtility.Url baseUrl)
        {
            BaseUrl = baseUrl;
            rss = String.Empty;
            wadl = WADLUtility.GenerateWadl(this.GetType(), BaseUrl);
            WADLApplication application = WADLApplication.GetWADLApplication(this.GetType());
            ChannelUrl = WADLUtility.Url.CreateUrl(baseUrl, application.Name);
            InnerChannels = new List<NetHttpChannelManager>();
        }

        [WADLMethod("?wadl", HttpMethods.GET)]
        [return: WADLRepresentation(ContentType = WADL.ContentTypes.xml, Path = "application", Href="http://www.w3.org/Submission/wadl/wadl.xsd")]
        public virtual String Wadl()
        {
            return wadl;
            #region comment
            //string text = String.Empty;
            //WADL.WADLFormatter wadler = new WADL.WADLFormatter(this.GetType(), BaseUrl);
            //using (Stream s = wadler.Serialize(Encoding.UTF8))
            //{
            //    s.Flush();
            //    s.Position = 0;
            //    using (StreamReader reader = new StreamReader(s))
            //    {
            //        text = reader.ReadToEnd();
            //        reader.Close();
            //    }
            //    s.Close();
            //}
            //return text;
            #endregion
        }

        [WADLMethod("/", HttpMethods.GET)]
        [return: WADLRepresentation(ContentType = ContentTypes.rss, Path = SchemaTypes.anyType)]
        public virtual String List()
        {
            if (String.IsNullOrEmpty(rss))
            {
                if (Monitor.TryEnter(this))
                {
                    this.rss = Syndicate();
                    Monitor.Exit(this);
                }
                else
                {
                    return Syndicate();
                }
            }
            return this.rss;
        }

        protected virtual String Syndicate()
        {
            SyndicationFeed feed = new SyndicationFeed(this.GetType().AssemblyQualifiedName, "Channel Resources", BaseUrl);
            feed.Authors.Add(new SyndicationPerson(this.GetType().ToString()));
            feed.Categories.Add(new SyndicationCategory("Channels"));
            feed.Description = new TextSyndicationContent("Channel Resource List");

            List<NetHttpChannelDispatcher> templates = GetTemplates();
            templates = (from template in GetTemplates()
                         orderby template.Template.Url.ToString()
                         select template).Where(delegate(NetHttpChannelDispatcher candidate)
                         {
                             return (candidate.Template.PathSegmentVariableNames.Count == 0 &&
                                     candidate.Template.QueryValueVariableNames.Count == 0 && candidate.Method.HttpMethod == HttpMethods.GET);
                         }).ToList();

            List<SyndicationItem> items = new List<SyndicationItem>();
            templates.ForEach(delegate(NetHttpChannelDispatcher dispatcher)
            {
                if (dispatcher.Template.PathSegmentVariableNames.Count == 0 &&
                    dispatcher.Template.QueryValueVariableNames.Count == 0)
                {
                    if (ChannelUrl.PathAndQuery.TrimEnd('/') != dispatcher.Template.Url.PathAndQuery.TrimEnd('/'))
                    {
                        SyndicationItem item = new SyndicationItem(
                            dispatcher.Instance.GetType().AssemblyQualifiedName,
                            dispatcher.Template.Url.PathAndQuery,
                            dispatcher.Template.Url,
                            items.Count.ToString(),
                            DateTime.Now
                            );

                        items.Add(item);
                    }
                }
            });
            feed.Items = items;

            string rss = String.Empty;
            using (TextWriter text = new StringWriter())
            {
                Rss20FeedFormatter formatter = new Rss20FeedFormatter(feed);
                using (XmlTextWriter writer = new XmlTextWriter(text))
                {
                    formatter.WriteTo(writer);
                    writer.Flush();
                    rss = text.ToString();
                    writer.Close();
                    text.Close();
                }
            }
            return rss;
        }

        protected string GetHeader(string name)
        {
            //return context.Request.Headers[name];
            return WebOperationContext.Current.IncomingRequest.Headers[name];
        }

        protected string GetHeader(HttpRequestHeader header)
        {
            return GetHeader(header);
        }

        protected void AddHeader(string name, string value)
        {
            //context.Response.Headers.Add(name, value);
            WebOperationContext.Current.OutgoingResponse.Headers.Add(name, value);
        }

        protected void AddHeader(HttpResponseHeader header, string value)
        {
            AddHeader(header, value);

        }

        protected WADLMethod[] GetWADLMethods()
        {
            WADL.WADLFormatter wadler = new WADL.WADLFormatter(this.GetType(), BaseUrl);
            return wadler.IntroSpect().Resources.Single<WADLResource>().Methods.ToArray();
        }

        protected internal List<NetHttpChannelDispatcher> GetTemplates()
        {
            List<NetHttpChannelDispatcher> templates = new List<NetHttpChannelDispatcher>();
            GetTemplates(templates);
            return templates;
        }

        private List<NetHttpChannelDispatcher> GetTemplates(List<NetHttpChannelDispatcher> templates)
        {
            InnerChannels.ForEach(delegate(NetHttpChannelManager manager)
            {
                manager.GetTemplates(templates);
            });

            WADLMethod[] methods = GetWADLMethods();
            XDocument wadl = XDocument.Parse(Wadl());
            var resources = wadl.Descendants(XName.Get("resource",CONSTANTS.Namespace)).ToArray<XElement>();
            int index = 0;
            Array.ForEach<WADLMethod>(methods, delegate(WADLMethod method)
            {
               WADLUtility.Url url = WADLUtility.Url.CreateUrl(resources[index].Parent.Attribute("base").Value, resources[index].Attribute("path").Value);
               NetHttpChannelDispatcher dispatcher = new NetHttpChannelDispatcher(this, method, new NetHttpUriTemplate(url, true));
               templates.Add(dispatcher);
               index++;       
            });
       
           return templates;
        }
  
    }
}

