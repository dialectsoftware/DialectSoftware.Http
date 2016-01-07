using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using DialectSoftware.Http.Services;
using System.ServiceModel.Web;
using System.IO;
using WADL;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml;
using System.Web;


//http://weblogs.java.net/blog/mhadley/archive/2009/02/draft_wadl_upda.html
namespace UnitTest
{
    public class Program
    {
        static void Main(string[] args)
        {
            
#region
            //DialectSoftware.Services.HospitalServices.HospitalTable t = new DialectSoftware.Services.HospitalServices.HospitalTable();
            //t.InitializationCompleted +=new EventHandler((s,o)=>{

            //    Console.WriteLine("Done...");
            
            //});
            //t.Initialize();
            //Console.ReadLine();
            //Console.WriteLine("Moving on...");
//            DialectSoftware.Services.SentinelServices.BSSIDTable t = new DialectSoftware.Services.SentinelServices.BSSIDTable();
//            t.SetValue(new String[] { @"<?xml version=""1.0"" encoding=""utf-8""?>
//                        <feed xmlns=""http://www.w3.org/2005/Atom"">
//
//                          <title>Example Feed</title>
//                          <subtitle>A subtitle.</subtitle>
//                          <link href=""http://example.org/feed/"" rel=""self"" />
//                          <link href=""http://example.org/"" />
//                          <id>urn:uuid:" + String.Format(t, "{0}", new long[]{ 0L, 28L, 10L, 246L, 4L, 37L}) + @"</id>
//                          <updated>2003-12-13T18:30:02Z</updated>
//
//
//                          <entry>
//                            <title>Atom-Powered Robots Run Amok</title>
//                            <link href=""http://example.org/2003/12/13/atom03"" />
//                            <link rel=""alternate"" type=""text/html"" href=""http://example.org/2003/12/13/atom03.html""/>
//                            <link rel=""edit"" href=""http://example.org/2003/12/13/atom03/edit""/>
//                            <id>urn:uuid:1225c695-cfb8-4ebb-aaaa-80da344efa6a</id>
//                            <updated>2003-12-13T18:30:02Z</updated>
//                            <summary>Some text.</summary>
//                            <author>
//                              <name>John Doe</name>
//                              <email>johndoe@example.com</email>
//                            </author>
//                          </entry>
//                        </feed>"
//            }, 0, 28, 10, 246, 4, 37);
//            int y = 0;
//            long tt = 0;
//            System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
//            for (; y < 10; y++)
//            {
//                stopWatch.Start();
//                int i = 0;
//                for (; i < 1000000; i++)
//                {
//                    var x = t.GetValue(0, 28, 10, 246, 4, 37);
//                    //var y = t.GetHashCode(255, 255, 255, 255, 255, 255);
//                    //x = ((DialectSoftware.Collections.Generics.Matrix<IEnumerable<string>>)t)[y].Single();
//                    //var z = t.GetCoordinatesAt(281474976710657);
//                }
//                stopWatch.Stop();
//                tt += stopWatch.ElapsedMilliseconds;
//                Console.WriteLine("Total {0} ns ", (1000* stopWatch.ElapsedMilliseconds)/i);
//                stopWatch.Reset();
//            }
//            Console.WriteLine("==========================");
//            Console.WriteLine("Average {0:d} ms ", tt/y);
//            t.InitializationCompleted += delegate(object sender, System.EventArgs asy)
//            {
//                Console.WriteLine("loaded...");
//            };
//            t.Initialize();
#endregion
            ServiceHost svc = new ServiceHost(typeof(NetHttpService));
            //svc.Description.Endpoints[0].Behaviors.Add(new CacheExtension());
            svc.Open();
            Console.WriteLine("running...");
            Console.ReadLine();
            svc.Close();
        }
    }
}