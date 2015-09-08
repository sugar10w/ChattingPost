using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Circles
{
    class MessagesKeeper
    {
        //自动维护一个有序键值对
        static private SortedList<int, Message> messages = new SortedList<int, Message>();

        //定时向ClientSocket发送信息请求
        static Thread threadAsking = null;
        static private void startAsking()
        {
            threadAsking = new Thread(asking);
            threadAsking.IsBackground = true;
            threadAsking.Start();
        }
        static private void asking()
        {
            while (true)
            {
                if (gettingIdList.Count != 0 && !listOprating)
                {
                    if (gettingDeepList.Count != gettingIdList.Count || gettingIdList.Count != gettingSendedList.Count || gettingSendedList.Count != gettingDeepList.Count)
                        { Reset(); continue; }

                    int id = gettingIdList[0], deep = gettingDeepList[0];

                    if (!gettingSendedList[0])
                        if (ClientSocket.Get(id) != 0) Reset();

                    if (gettingIdList.Count == 0 || gettingDeepList.Count == 0 || gettingSendedList.Count == 0) continue;
                    gettingIdList.RemoveAt(0); gettingDeepList.RemoveAt(0); gettingSendedList.RemoveAt(0);
                    gettingIdList.Add(id); gettingDeepList.Add(deep); gettingSendedList.Add(true);
                }
                Thread.Sleep(50);
            }
        }

        //三个松散的循环队列 
        //TODO
        static List<int> gettingIdList = new List<int>(), gettingDeepList = new List<int>();
        static List<bool> gettingSendedList = new List<bool>();
        static bool listOprating = false;
        static private void addToList(int id, int deep = 0)
        {
            listOprating = true;
            if (gettingIdList.Contains(id))
            {
                int t = gettingIdList.IndexOf(id);
                if (t < 0 || t > gettingDeepList.Count) return;
                if (gettingDeepList[t] < deep)
                {
                    gettingDeepList[t] = deep;
                    gettingSendedList[t] = false;
                }
            }
            else
            {
                gettingIdList.Add(id);
                gettingDeepList.Add(deep);
                gettingSendedList.Add(false);
            }
            listOprating = false;

            if (threadAsking == null) startAsking();
        }
        
        //从ClientSocket接收Message信息
        static public void RefreshMessage(JObject j)
        {
            int jId = int.Parse(j["id"].ToString());

            Message m;
            if (messages.ContainsKey(jId))
            {
                m = messages[jId];
                m.RefreshMessage(j);
            }
            else
            {
                m = new Message(j);
                messages.Add(m.Id, m);
            }

            listOprating = true;
            if (gettingIdList.Contains(m.Id))
            {
                int t = gettingIdList.IndexOf(m.Id);
                int d = gettingDeepList[t];
                if (d>0) foreach (int sonsId in m.SonsId) addToList(sonsId, d - 1);
                gettingIdList.RemoveAt(t); gettingDeepList.RemoveAt(t); gettingSendedList.RemoveAt(t);
            }
            listOprating = false;
        }

        //从UI主页接收Message请求
        static public Message Get(int id, int deep = 0)
        {

            if (messages.ContainsKey(id))
            {
                addToList(id, deep);
                if (deep > 0)
                    foreach (int sonsId in messages[id].SonsId) addToList(sonsId, deep - 1);
                return messages[id];
            }
            else
            {
                addToList(id, deep);
                return null;
            }
        }

        //服务器连接错误时，MessagesKeeper需要重启
        static public void Reset()
        {
            Console.WriteLine("RESET MsgKeeper");

            gettingDeepList.Clear();
            gettingIdList.Clear();
            gettingSendedList.Clear();
        }
    }
}
