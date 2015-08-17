using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Circles
{
    //作为 ClientSocket中的Listen进程 和 UI进程 的中转站使用
    class InfoBox
    {
        static string info = "";
        static bool refreshed = false;
        
        static public bool Refreshed { get { return refreshed; } }
        //读取信息。在操作上，应只允许UI控件读取此信息（此处有refreshed的调整）。
        static public string Info
        {
            get
            {
                refreshed = false;
                return info;
            }
        }

        //追加信息。ClientSocket中的Listen进程将服务器系统信息传送至此。
        static public void AddInfo(string s)
        {
            info = s + "\r\n" + info;
            if (info.Length > 300) info = info.Substring(0, 300) + " ... ";
            refreshed = true;

            if (threadClearing == null) StartThread();
        }

        //在长时间没有更新的情况下，定时清空信息。
        static Thread threadClearing = null;
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
