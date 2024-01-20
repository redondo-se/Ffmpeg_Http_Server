using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace Ffmpeg_Http_Server
{
    public class Utilities
    {
        public static void KillProcessAndChildren(int pid)
        {
            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher
                  ("Select * From Win32_Process Where ParentProcessID=" + pid);
                ManagementObjectCollection moc = searcher.Get();
                foreach (ManagementObject mo in moc)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }

                Process proc = Process.GetProcessById(pid);
                proc.Kill();
                //if (!proc.CloseMainWindow())
                //{
                //    proc.Kill();
                //}
            }
            catch (Exception ex) { }
        }
    }
}
