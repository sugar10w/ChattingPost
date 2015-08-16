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
        static Thread threadAsking=null;

        static private SortedList<int, Message> messages = new SortedList<int, Message>();

        static List<int> gettingIdList = new List<int>(), gettingDeepList=new List<int>();
        static List<bool> gettingSendedList = new List<bool>();

        static private void StartAsking()
        {
            threadAsking = new Thread(Asking);
            threadAsking.IsBackground = true;
            threadAsking.Start();
        }
        static private void Asking()
        {
            while (true)
            {
                if (gettingIdList.Count != 0)
                {
                    if (gettingDeepList.Count != gettingIdList.Count || gettingIdList.Count != gettingSendedList.Count || gettingSendedList.Count != gettingDeepList.Count)
                    {
                        gettingDeepList = new List<int>(); gettingIdList = new List<int>(); gettingSendedList = new List<bool>();
                        continue;
                    }

                    int id = gettingIdList[0], deep = gettingDeepList[0];

                    if (!gettingSendedList[0])
                    {
                        if (ClientSocket.Get(id) != 0)
                        {
                            Reset();
                        }
                    }
                    if (gettingIdList.Count == 0 || gettingDeepList.Count == 0 || gettingSendedList.Count == 0) continue;
                    gettingIdList.RemoveAt(0); gettingDeepList.RemoveAt(0); gettingSendedList.RemoveAt(0);
                    gettingIdList.Add(id); gettingDeepList.Add(deep); gettingSendedList.Add(true);


                }
                Thread.Sleep(10);
            }
        }
        static private void AddGet(int id, int deep = 0)
        {
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
           
            if (threadAsking == null) StartAsking();
        }
       
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

            if (gettingIdList.Contains(m.Id))
            {
                int t = gettingIdList.IndexOf(m.Id);
                int d = gettingDeepList[t];
                if (d>0) foreach (int sonsId in m.SonsId) AddGet(sonsId, d - 1);
                gettingIdList.RemoveAt(t); gettingDeepList.RemoveAt(t); gettingSendedList.RemoveAt(t);
            }
        }

        static public Message Get(int id, int deep = 0)
        {

            if (messages.ContainsKey(id))
            {
                AddGet(id, deep);
                if (deep > 0)
                    foreach (int sonsId in messages[id].SonsId) AddGet(sonsId, deep - 1);
                return messages[id];
            }
            else
            {
                AddGet(id, deep);
                return null;
            }
        }

        static public void Reset()
        {
            Console.WriteLine("RESET");

            gettingDeepList.Clear();
            gettingIdList.Clear();
            gettingSendedList.Clear();

            
        }
    }
}
