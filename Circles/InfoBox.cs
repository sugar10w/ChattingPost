using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Circles
{
    class InfoBox
    {
        static Thread threadClearing = null;

        static string info = "";
        static bool refreshed = false;

        static public bool Refreshed { get { return refreshed; } }

        static public void AddInfo(string s)
        {
            info = s + "\r\n" + info;
            if (info.Length > 300) info = info.Substring(0, 300)+" ... ";
            refreshed = true;

            if (threadClearing == null) StartThread();
        }

        static public string Info
        {
            get
            {
                refreshed = false;
                return info;
            }
        }

        static public void StartThread()
        {
            threadClearing = new Thread(clearInfo);
            threadClearing.IsBackground = true;
            threadClearing.Start();
        }
        static public void clearInfo()
        {
            while (true)
            {
                if (!Refreshed) info = "";
                Thread.Sleep(3000);
            }
        }
    }
}
