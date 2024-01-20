using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Ffmpeg_Http_Server
{
    public abstract class SimpleWebServer
    {
        abstract protected void ProcessRequest(HttpListenerContext ctx);

        private readonly HttpListener _listener = new HttpListener();

        protected SimpleWebServer(string[] prefixes)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException(
                    "Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");
            
            foreach (string s in prefixes)
                _listener.Prefixes.Add(s);

            try
            {
                _listener.Start();
            }
            catch (HttpListenerException ex)
            {
                if (ex.Message.Contains("Access is denied"))
                {
                    Console.WriteLine(Environment.NewLine + Environment.NewLine + "!!! YOU MUST GRANT PERMISSION TO RUN HTTP LISTENER(s), run these commands:" + Environment.NewLine + Environment.NewLine);
                    foreach (string prefix in prefixes)
                    {
                        Console.WriteLine(string.Format(@"netsh http add urlacl url={0} user={1}", prefix, System.Security.Principal.WindowsIdentity.GetCurrent().Name));
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem((c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                ProcessRequest(ctx);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                try
                                {
                                    ctx.Response.OutputStream.Close();
                                }
                                catch (Exception ex) { }
                            }
                        }, _listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
