using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Win32;

namespace Circles
{
    class ClientSocket
    {
        static Thread threadListening;

        static int port = 9999;
        static string host = "127.0.0.1";        
        static Socket soc;

        static public User user = null;

        public static int ConnectServer(string name,string password="")
        {
            user = new User(name, password);

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                soc.Connect(ipe);
            }
            catch (SocketException e)
            {
                System.Windows.MessageBox.Show("服务器连接错误！请检查服务器地址：" + host + ":" + port.ToString());
                return 1;
            }

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();            
            writer.WritePropertyName("nickname"); writer.WriteValue(user.NickName);
            writer.WritePropertyName("password"); writer.WriteValue(user.Password);
            writer.WritePropertyName("colorId"); writer.WriteValue(user.ColorId);
            writer.WriteEndObject();
            writer.Flush();
            Send(sw.GetStringBuilder().ToString());           
           
            threadListening = new Thread(ListenServer);
            threadListening.IsBackground = true;
            threadListening.Start();

            MessagesKeeper.Get(0, 3);

            return 0;
        }

        private static void ListenServer()
        {
            while (true)
            {
                byte[] result = new byte[1024];
                int receiveLength = 0;

                try
                {
                    receiveLength = soc.Receive(result);
                }
                catch (SocketException e)
                {
                    System.Windows.MessageBox.Show("服务器连接错误！您已经离线。");
                    return;
                }

                string s = Encoding.UTF8.GetString(result, 0, receiveLength);

                //      Console.WriteLine("RECEIVE: "+s);

                while (s.Contains("}"))
                {
                    try
                    {
                        int t = 0, cnt = 0;
                        while (s[t] != '{' && t < s.Length) ++t;
                        ++cnt;
                        while (cnt > 0 && t < s.Length)
                        {
                            ++t;
                            if (s[t] == '{') ++cnt;
                            if (s[t] == '}') --cnt;
                        }

                        if (t >= s.Length) break;

                        string s0 = s.Substring(0, t + 1);
                        s = s.Substring(t + 1);
                        JObject j;

                        j = (JObject)JsonConvert.DeserializeObject(s0);

                        if (j["action"].ToString().Equals("send"))
                        {
                            System.Windows.MessageBox.Show(j["content"].ToString());
                        }
                        else
                            MessagesKeeper.RefreshMessage(j);
                    }
                    catch
                    {
                        break;
                    }
                }
            }
        }

        public static int Submit(String s,int fatherId=0)
        {
            s = s.Trim();

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            writer.WritePropertyName("action"); writer.WriteValue("send");
            writer.WritePropertyName("senderName"); writer.WriteValue(user.NickName); 
            writer.WritePropertyName("colorId"); writer.WriteValue(user.ColorId);
            writer.WritePropertyName("content"); writer.WriteValue(s);
            writer.WritePropertyName("father"); writer.WriteValue(fatherId);
            writer.WriteEndObject();
            writer.Flush();
            return Send(sw.GetStringBuilder().ToString());

        }
        public static int Get(int id = 0)
        {
            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            writer.WritePropertyName("action"); writer.WriteValue("get");
            writer.WritePropertyName("id"); writer.WriteValue(id);
            writer.WriteEndObject();
            writer.Flush();
            return Send(sw.GetStringBuilder().ToString());
        }

        private static int Send(String s)
        {

            byte[] bs = Encoding.UTF8.GetBytes(s);
       //     Console.WriteLine("SEND: "+s);

            try
            {
                soc.Send(bs, bs.Length, 0);
            }
            catch (SocketException e)
            {
                System.Windows.MessageBox.Show("服务器连接错误！您已经离线。");
                return 1;
            }

            return 0;
        }

    }
}
