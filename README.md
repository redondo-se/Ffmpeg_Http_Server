# Ffmpeg_Http_Server
A C# .Net HTTP server specifically for using FFmpeg to transcode video on the fly.


# More Info

This server was created to address specific issues with using [Nvidia Shield](https://www.nvidia.com/en-us/shield/) devices as clients to [Channels DVR Server](https://getchannels.com/dvr-server/):

 - Some OTA TV stations use a specific interlaced encoder that causes macro blocking when hardware decoding is used on Nvidia Shield and Maxwell/Pascal (GTX 10xx) Nvidia GPUs.
 - Nvidia Shield cannot perform AI upscaling on interlaced content

This server is being run on a windows machine with an Nvidia GTX 2060 which is capable of properly decoding the problematic OTA streams in hardware. Here is an example URL passed to this server:

http://127.0.0.1:32280/ffmpeg?ffparams=-hwaccel%20cuvid%20-c%3Av%20mpeg2_cuvid%20-deint%20adaptive%20-drop_second_field%200%20-i%20%22http%3A%2F%2F127.0.0.1%3A8089%2Fdevices%2FANY%2Fchannels%2F8.1%2Fstream.mpg%22%20-c%3Av%20hevc_nvenc%20-rc%20vbr%20-cq%2024%20-qmin%2024%20-qmax%2024%20-b%3Av%200K%20-c%3Aa%20copy%20-r%2059.94%20-f%20mpegts

This url will transcode the OTA mpeg2 by using Nvidia cuvid to decode the stream in hardware and then encode the stream to HEVC in hardware.

## Requirements

 - A windows machine with .Net 4.5.1 installed
 - A capable GPU to handle video transcoding. FFmpeg parameters can be adjusted in the URL to match the capabilities of your specific GPU.
 - The path to the FFmpeg executable is currently hardcoded to:
 c:\ffmpeg\ffmpeg.exe

## Installation

 - Ensure that you have a working version of ffmpeg.exe in c:\ffmpeg
 - Place the Ffmpeg_Http_Server.exe in a folder of your choosing
 - Run the executable to start the HTTP server
 - The first time you run the executable you will be given a command that you must run as administrator to allow the .Net framework to run the HTTP server on the specified port:
 
	 netsh http add urlacl url=http://+:32280/ user=USERNAME
	 where USERNAME will be the windows user name
 - The server can be run with windows task scheduler or as a service with something like [NSSM](https://nssm.cc/).
