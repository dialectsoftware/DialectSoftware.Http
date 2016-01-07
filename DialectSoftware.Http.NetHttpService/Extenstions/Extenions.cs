using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using DialectSoftware.Http.Services;

namespace DialectSoftware.Http
{
    internal static class Extensions
    {
        internal static int IndexOfArray(this byte[] searchWithin, byte[] serachFor, int startIndex)
        {
            int index = 0;
            int startPos = Array.IndexOf(searchWithin, serachFor[0], startIndex);

            if (startPos != -1)
            {
                while ((startPos + index) < searchWithin.Length)
                {
                    if (searchWithin[startPos + index] == serachFor[index])
                    {
                        index++;
                        if (index == serachFor.Length)
                        {
                            return startPos;
                        }
                    }
                    else
                    {
                        startPos = Array.IndexOf<byte>(searchWithin, serachFor[0], startPos + index);
                        if (startPos == -1)
                        {
                            return -1;
                        }
                        index = 0;
                    }
                }
            }

            return -1;
        }

        internal static byte[] ToByteArray(this Stream stream)
        {
            byte[] buffer = new byte[32768];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write(buffer, 0, read);
                }
            }
        }

        internal static InstancingMode GetInstancingModeAttribute(this NetHttpChannelManager manager)
        {
            return manager.GetType().GetCustomAttributes(typeof(NetHttpInstancingModeAttribute), true)
                .Cast<NetHttpInstancingModeAttribute>()
                .Select(attribute => attribute.Mode).DefaultIfEmpty(InstancingMode.PerCall).SingleOrDefault();  
        
        }

        //internal static string ToQueryString(NameValueCollection nvc)
        //{
        //    return "?" + string.Join("&", Array.ConvertAll(nvc.AllKeys, key => string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(nvc[key]))));
        //}
    }
}
