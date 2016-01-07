using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Xml.Linq;
using System.ServiceModel.Web;
using System.IO;
using System.Text.RegularExpressions;
using DialectSoftware.Http.Services.Configuration;

namespace DialectSoftware.Http.Services
{
    public static class NetHttpUtility
    {
        public static bool TryParsePostBody(out NetHttpPostBody postBody)
        {
            System.ServiceModel.Channels.Message message = OperationContext.Current.RequestContext.RequestMessage;
            //string body =  message.Version.Envelope == System.ServiceModel.EnvelopeVersion.None  ? message.ToString() : message.GetBody<String>(); 
            //string body = XDocument.Parse(message.GetReaderAtBodyContents().ReadOuterXml()).Root.Value;
            var body = message.GetBody<String>(); 
            switch (WebOperationContext.Current.IncomingRequest.ContentType)
            {
                case "application/x-www-form-urlencoded":
                    body = System.Text.ASCIIEncoding.ASCII.GetString(Convert.FromBase64String(body));
                    var query = System.Web.HttpUtility.ParseQueryString(body);
                    postBody = new NetHttpPostBody(query) { ContentType = WebOperationContext.Current.IncomingRequest.ContentType };
                break;
                case "multipart/form-data":
                    using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(body)))
                    {
                        postBody = Parse(stream, Encoding.UTF8);
                        stream.Close();
                    }
                break;
                default:
                    postBody = new NetHttpPostBody();
                    postBody.Parameters.Add("formBody", body);
                break;
            }

            if (postBody.FileContents != null || postBody.Parameters.Count != 0)
                return true;
            else
                return false;
        
        }

        //https://bitbucket.org/lorenzopolidori/http-form-parser/pull-requests
        private static NetHttpPostBody Parse(Stream stream, Encoding encoding, String FilePartName = null)
        {
            NetHttpPostBody postBody = new NetHttpPostBody() { ContentType = WebOperationContext.Current.IncomingRequest.ContentType };
            //postBody.Success = false;

            // Read the stream into a byte array
            byte[] data = stream.ToByteArray();

            // Copy to a string for header parsing
            string content = encoding.GetString(data);

            // The first line should contain the delimiter
            int delimiterEndIndex = content.IndexOf("\r\n");

            if (delimiterEndIndex > -1)
            {
                string delimiter = content.Substring(0, content.IndexOf("\r\n"));

                string[] sections = content.Split(new string[] { delimiter }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string s in sections)
                {
                    if (s.Contains("Content-Disposition"))
                    {
                        // If we find "Content-Disposition", this is a valid multi-part section
                        // Now, look for the "name" parameter
                        Match nameMatch = new Regex(@"(?<=name\=\"")(.*?)(?=\"")").Match(s);
                        string name = nameMatch.Value.Trim().ToLower();

                        if (name == FilePartName)
                        {
                            // Look for Content-Type
                            Regex re = new Regex(@"(?<=Content\-Type:)(.*?)(?=\r\n\r\n)");
                            Match contentTypeMatch = re.Match(content);

                            // Look for filename
                            re = new Regex(@"(?<=filename\=\"")(.*?)(?=\"")");
                            Match filenameMatch = re.Match(content);

                            // Did we find the required values?
                            if (contentTypeMatch.Success && filenameMatch.Success)
                            {
                                // Set properties
                                postBody.ContentType = contentTypeMatch.Value.Trim();
                                postBody.Filename = filenameMatch.Value.Trim();

                                // Get the start & end indexes of the file contents
                                int startIndex = contentTypeMatch.Index + contentTypeMatch.Length + "\r\n\r\n".Length;

                                byte[] delimiterBytes = encoding.GetBytes("\r\n" + delimiter);
                                int endIndex = data.IndexOfArray(delimiterBytes, startIndex);//Misc.IndexOf(data, delimiterBytes, startIndex);

                                int contentLength = endIndex - startIndex;

                                // Extract the file contents from the byte array
                                byte[] fileData = new byte[contentLength];

                                Buffer.BlockCopy(data, startIndex, fileData, 0, contentLength);

                                postBody.FileContents = fileData;
                            }
                        }
                        else if (!string.IsNullOrEmpty(name.Trim()))
                        {
                            // Get the start & end indexes of the file contents
                            int startIndex = nameMatch.Index + nameMatch.Length + "\r\n\r\n".Length;
                            postBody.Parameters.Add(name, s.Substring(startIndex).TrimEnd(new char[] { '\r', '\n' }).Trim());
                        }
                    }
                }

                // If some data has been successfully received, set success to true
                //if (postBody.FileContents != null || postBody.Parameters.Count != 0)
                //    postBody.Success = true;
            }
            return postBody;
        }

    }
}
