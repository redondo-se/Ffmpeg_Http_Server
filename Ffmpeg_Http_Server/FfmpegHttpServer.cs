using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Ffmpeg_Http_Server
{
    public class FfmpegHttpServer : SimpleWebServer
    {
        private static readonly string FFMPEG_PATH = @"c:\ffmpeg\ffmpeg.exe";

        public FfmpegHttpServer(string[] prefixes) : base(prefixes)
        {

        }

        protected override void ProcessRequest(HttpListenerContext ctx)
        {
            // parent class SimpleWebServer will always close the response stream so don't need to do that here
            if (ctx.Request.RawUrl.ToLower().StartsWith("/ffmpeg"))
            {
                var qParams = HttpUtility.ParseQueryString(ctx.Request.Url.Query);
                string encodedffparams = qParams.Get("ffparams");
                string ffparams = HttpUtility.UrlDecode(encodedffparams);

                Process ffmpegProcess = new Process();
                ffmpegProcess.StartInfo.FileName = FFMPEG_PATH;
                ffmpegProcess.StartInfo.Arguments = ffparams + " -"; // output to standard output
                ffmpegProcess.StartInfo.UseShellExecute = false; //required to redirect standart input/output
                ffmpegProcess.StartInfo.RedirectStandardInput = true;
                ffmpegProcess.StartInfo.RedirectStandardOutput = true;
                ffmpegProcess.Start();

                var buffer = new byte[40960];

                ctx.Response.ContentType = "video/mpeg";

                try
                {
                    while (true)
                    {
                        int bytesRead = ffmpegProcess.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
                        ctx.Response.OutputStream.Write(buffer, 0, bytesRead);
                    }
                }
                catch (Exception ex)
                {
                    ctx.Response.OutputStream.Close();
                    Utilities.KillProcessAndChildren(ffmpegProcess.Id);
                    Console.WriteLine(ex.ToString());
                }
            }
        }
    }
}
