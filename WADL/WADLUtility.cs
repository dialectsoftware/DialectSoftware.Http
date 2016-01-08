using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Schema;
using System.Threading;
using System.Web;
using System.Collections.Specialized;

/// ******************************************************************************************************************
/// * Copyright (c) 2011 Dialect Software LLC                                                                        *
/// * This software is distributed under the terms of the Apache License http://www.apache.org/licenses/LICENSE-2.0  *
/// *                                                                                                                *
/// ******************************************************************************************************************

namespace WADL
{
    //http://mikehadlow.blogspot.com/2007/01/writing-your-own-xsdexe.html
    public static class WADLUtility
    {
        public class Url
        {
            Uri uri;

            public Url(Uri uri)
            {
                this.uri = uri;
                //Params = HttpUtility.ParseQueryString(uri.Query);
            }

            public Url(String uri):this(new Uri(uri))
            {
                
            }

            // Summary:
            //     Gets the absolute path of the URI.
            //
            // Returns:
            //     A System.String containing the absolute path to the resource.
            public string AbsolutePath { get { return uri.AbsolutePath;} }
            //
            // Summary:
            //     Gets the absolute URI.
            //
            // Returns:
            //     A System.String containing the entire URI.
            public string AbsoluteUri { get { return uri.AbsoluteUri;} }
            //
            // Summary:
            //     Gets the Domain Name System (DNS) host name or IP address and the port number
            //     for a server.
            //
            // Returns:
            //     A System.String containing the authority component of the URI represented
            //     by this instance.
            public string Authority { get { return uri.Authority;} }
            //
            // Summary:
            //     Gets an unescaped host name that is safe to use for DNS resolution.
            //
            // Returns:
            //     A System.String that contains the unescaped host part of the URI that is
            //     suitable for DNS resolution; or the original unescaped host string, if it
            //     is already suitable for resolution.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     This instance represents a relative URI, and this property is valid only
            //     for absolute URIs.
            public string DnsSafeHost { get { return uri.DnsSafeHost ;} }
            //
            // Summary:
            //     Gets the escaped URI fragment.
            //
            // Returns:
            //     A System.String that contains any URI fragment information.
            public string Fragment { get { return uri.Fragment;} }
            //
            // Summary:
            //     Gets the host component of this instance.
            //
            // Returns:
            //     A System.String that contains the host name. This is usually the DNS host
            //     name or IP address of the server.
            public string Host { get { return uri.Host;} }
            //
            // Summary:
            //     Gets the type of the host name specified in the URI.
            //
            // Returns:
            //     A member of the System.UriHostNameType enumeration.
            public UriHostNameType HostNameType { get { return uri.HostNameType; } }
            //
            // Summary:
            //     Gets whether the System.Uri instance is absolute.
            //
            // Returns:
            //     A System.Boolean value that is true if the System.Uri instance is absolute;
            //     otherwise, false.
            public bool IsAbsoluteUri { get { return uri.IsAbsoluteUri;} }
            //
            // Summary:
            //     Gets whether the port value of the URI is the default for this scheme.
            //
            // Returns:
            //     A System.Boolean value that is true if the value in the System.Uri.Port property
            //     is the default port for this scheme; otherwise, false.
            public bool IsDefaultPort { get{return uri.IsDefaultPort;} }
            //
            // Summary:
            //     Gets a value indicating whether the specified System.Uri is a file URI.
            //
            // Returns:
            //     A System.Boolean value that is true if the System.Uri is a file URI; otherwise,
            //     false.
            public bool IsFile { get{return uri.IsFile;} }
            //
            // Summary:
            //     Gets whether the specified System.Uri references the local host.
            //
            // Returns:
            //     A System.Boolean value that is true if this System.Uri references the local
            //     host; otherwise, false.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     This instance represents a relative URI, and this property is valid only
            //     for absolute URIs.
            public bool IsLoopback { get{return uri.IsLoopback;}}
            //
            // Summary:
            //     Gets whether the specified System.Uri is a universal naming convention (UNC)
            //     path.
            //
            // Returns:
            //     A System.Boolean value that is true if the System.Uri is a UNC path; otherwise,
            //     false.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     This instance represents a relative URI, and this property is valid only
            //     for absolute URIs.
            public bool IsUnc { get { return uri.IsUnc; } }
            //
            // Summary:
            //     Gets a local operating-system representation of a file name.
            //
            // Returns:
            //     A System.String that contains the local operating-system representation of
            //     a file name.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     This instance represents a relative URI, and this property is valid only
            //     for absolute URIs.
            public string LocalPath { get { return uri.LocalPath; } }
            //
            // Summary:
            //     Gets the original URI string that was passed to the System.Uri constructor.
            //
            // Returns:
            //     A System.String containing the exact URI specified when this instance was
            //     constructed; otherwise, System.String.Empty.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     This instance represents a relative URI, and this property is valid only
            //     for absolute URIs.
            public string OriginalString { get { return uri.OriginalString; } }
            //
            // Summary:
            //     Gets the System.Uri.AbsolutePath and System.Uri.Query properties separated
            //     by a question mark (?).
            //
            // Returns:
            //     A System.String that contains the System.Uri.AbsolutePath and System.Uri.Query
            //     properties separated by a question mark (?).
            public string PathAndQuery { get{return HttpUtility.UrlDecode(uri.PathAndQuery);} }
            //
            // Summary:
            //     Gets the port number of this URI.
            //
            // Returns:
            //     An System.Int32 value that contains the port number for this URI.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     This instance represents a relative URI, and this property is valid only
            //     for absolute URIs.
            public int Port { get { return uri.Port; } }
            /// <summary>
            /// Summary:
            ///     Gets any query information included in the specified URI.
            /// 
            /// Returns:
            ///    A NameValueCollection that contains any query information included in the specified
            ///    URI.
            /// </summary>
            public NameValueCollection Query
            {
                get { return HttpUtility.ParseQueryString(uri.Query); }
            }
            //
            // Summary:
            //     Gets the scheme name for this URI.
            //
            // Returns:
            //     A System.String that contains the scheme for this URI, converted to lowercase.
            //
            // Exceptions:
            //   System.InvalidOperationException:
            //     This instance represents a relative URI, and this property is valid only
            //     for absolute URIs.
            public string Scheme { get { return uri.Scheme; } }
            //
            // Summary:
            //     Gets an array containing the path segments that make up the specified URI.
            //
            // Returns:
            //     A System.String array that contains the path segments that make up the specified
            //     URI.
            public string[] Segments { get { return uri.Segments; } }
            //
            // Summary:
            //     Indicates that the URI string was completely escaped before the System.Uri
            //     instance was created.
            //
            // Returns:
            //     A System.Boolean value that is true if the dontEscape parameter was set to
            //     true when the System.Uri instance was created; otherwise, false.
            public bool UserEscaped { get { return uri.UserEscaped; } }
            //
            // Summary:
            //     Gets the user name, password, or other user-specific information associated
            //     with the specified URI.
            //
            // Returns:
            //     A System.String that contains the user information associated with the URI.
            //     The returned value does not include the '@' character reserved for delimiting
            //     the user information part of the URI.
            public string UserInfo { get { return uri.UserInfo; } }
          
            public override string ToString()
            {
                return uri.OriginalString;
            }

            public String MakeRelativeTo(Uri path)
            {
                String[] segments1 = path.OriginalString.Split('/');
                String[] segments2 = this.uri.ToString().Split('/');
                String[] relativeSegments = segments2.Except<String>(segments1).ToArray<String>();
                String relativePath = String.Empty;
                Array.ForEach<String>(relativeSegments, delegate(String segment)
                {
                    relativePath = String.Join("/", new String[] {relativePath, segment});
                });
                return relativePath;
            }

            public String[] GetMatchingSegments(Uri path)
            {
                int i = 0;
                String[] matches = Segments.TakeWhile(delegate(String segment) {
                    bool ok = i < path.Segments.Length && path.Segments[i].Equals(segment, StringComparison.InvariantCultureIgnoreCase);
                    i++;
                    return ok;
                }).ToArray();

                return matches;
            }

            public static implicit operator String(Url url)
            {
                return url.ToString();
            }

            public static implicit operator Uri(Url url)
            {
                return url.uri;
            }

            public static Url CreateUrl(params string[] segments)
            {
                string url = String.Empty;
                Array.ForEach<string>(segments, delegate(string segment)
                {
                    if (url.Length == 0)
                        url = segment;
                    else
                    {
                        if (url.EndsWith("/"))
                        {
                            if (segment.StartsWith("/"))
                            {
                                url = url.TrimEnd('/');
                            }
                            url = String.Concat(url, segment);
                        }
                        else
                        {
                            if (!segment.StartsWith("/"))
                            {
                                url = String.Concat(url, "/");
                            }
                            url = String.Concat(url, segment);
                        }
                    }
                });
                return new Url(url);
            }
        }

        public static XmlSchemaSet Infer<TClass>() where TClass:new()
        {
            TClass t = new TClass();
            return Infer(t.GetType());
        }

        public static XmlSchemaSet Infer(Type type)
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            XmlReflectionImporter importer = new XmlReflectionImporter();
            XmlSchemas schemas = new XmlSchemas();
            XmlSchemaExporter exporter = new XmlSchemaExporter(schemas);
            XmlTypeMapping xmlTypeMapping = importer.ImportTypeMapping(type);
            exporter.ExportTypeMapping(xmlTypeMapping);
            schemas.Compile(new ValidationEventHandler(delegate(object sender, ValidationEventArgs args)
            {
                throw args.Exception;
            }), false);

            for (int i = 0; i < schemas.Count; i++)
            {
                XmlSchema schema = schemas[i];
                schema.Namespaces.Add("xsd", "http://www.w3.org/2001/XMLSchema"); 

                try
                {
                    schemaSet.Add(schema);
                }
                catch (Exception exception2)
                {
                    if (((exception2 is ThreadAbortException) || (exception2 is StackOverflowException)) || (exception2 is OutOfMemoryException))
                    {
                        throw;
                    }
                    throw;
                }
            }
            return schemaSet;
        }

        public static void Serialize<TClass>(TClass obj, out Stream stream)
        {
            stream = new MemoryStream();
            XmlSerializer sr = new XmlSerializer(typeof(TClass));
            sr.Serialize(stream, obj);
            stream.Flush();
            stream.Position = 0;
        
        }

        public static void Serialize(object obj, Type type, out Stream stream)
        {
            stream = new MemoryStream();
            XmlSerializer sr = new XmlSerializer(type);
            sr.Serialize(stream, obj);
            stream.Flush();
            stream.Position = 0;
        
        }

        public static String GetSchemaName(XmlSchemaSet schemas, Type type)
        {
            foreach (XmlSchema schema in schemas.Schemas())
            {
                if (schema.SchemaTypes.Names.Count == 0)
                {
                    if (schemas.Count == 1)
                    {
                        return GetSchemaName(schema);
                    }
                }
                else
                {
                    foreach (XmlQualifiedName name in schema.SchemaTypes.Names)
                    {
                        if (type.Name.ToLower() == name.Name.ToLower() || schemas.Count == 1)
                            return name.Name;
                    }
                }
            }
            return null;
        }

        public static String GetSchemaName(XmlSchema schema)
        {
            string name = null;
            using (MemoryStream mem = new MemoryStream())
            {
                schema.Write(mem);
                mem.Flush();
                mem.Position = 0;
                XmlReader reader = XmlReader.Create(mem);
                reader.MoveToContent();
                while (reader.Read())
                {
                    if (reader.IsStartElement() && reader.LocalName == "simpleType")
                    {
                        name = reader.GetAttribute("name");
                        break;
                    }

                    if (reader.IsStartElement() && reader.LocalName == "element")
                    {
                        name = reader.GetAttribute("type");
                        break;
                    }
                }
                reader.Close();
                mem.Close();
            }
            return name;
        }

        public static String GenerateWadl(Type type, Uri uri)
        {
            string text = String.Empty;
            WADL.WADLFormatter wadler = new WADL.WADLFormatter(type, uri);
            using (Stream s = wadler.Serialize(Encoding.UTF8))
            {
                s.Flush();
                s.Position = 0;
                using (StreamReader reader = new StreamReader(s))
                {
                    text = reader.ReadToEnd();
                    reader.Close();
                }
                s.Close();
            }

            return text;
        }

    }
}
