using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg_Http_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FfmpegHttpServer ffmpegServer = new FfmpegHttpServer(new string[] { "http://+:32280/" });
            ffmpegServer.Run();

            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
