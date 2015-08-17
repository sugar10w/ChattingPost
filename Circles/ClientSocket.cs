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
        static int port = 9999;
        static string host = "127.0.0.1";        
        static Socket soc;

        static private User user = null;
        static public User User { get { return user; } }

        //连接服务器
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

            sendUserInfo();
            startListening();
            MessagesKeeper.Get(0, 3);

            return 0;
        }

        //持续监听服务器。在进程中启动防止监听卡死。
        static Thread threadListening;
        static private void startListening()
        {
            threadListening = new Thread(listenServer);
            threadListening.IsBackground = true;
            threadListening.Start();
        }
        static private void listenServer()
        {
            while (true)
            {
                byte[] result = new byte[1024];
                int receiveLength = 0;

                try
                {
                    if (soc != null) receiveLength = soc.Receive(result);
                    else throw new SocketException();
                }
                catch (SocketException e)
                {
                    InfoBox.AddInfo("服务器连接错误！您已离线。[Listening failed]");
                    return;
                }

                string s = Encoding.UTF8.GetString(result, 0, receiveLength);

                while (s.Contains("}"))
                {
                    try
                    {
                        int t = 0, cnt = 0;
                        while (s[t] != '{' && t < s.Length) ++t;
                        ++cnt;
                        while (cnt > 0 && t < s.Length - 1)
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
                            InfoBox.AddInfo(j["content"].ToString());
                        else if (j["action"].ToString().Equals("get"))
                            MessagesKeeper.RefreshMessage(j);
                        else if (j["action"].ToString().Equals("user"))
                            User.Login(j);
                    }
                    catch
                    {
                        Console.WriteLine("数据解析失败。");
                        break;
                    }
                }
            }
        }
        
        //密码错误时，ClientSocket暂停
        static public void Stop()
        {
            soc.Close();
            soc = null;
            threadListening.Abort();
        }

        //向服务器发送"user"信息
        private static int sendUserInfo()
        {
            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            writer.WritePropertyName("action"); writer.WriteValue("user");
            writer.WritePropertyName("nickname"); writer.WriteValue(User.NickName);
            writer.WritePropertyName("password"); writer.WriteValue(User.Password);
            writer.WritePropertyName("colorId"); writer.WriteValue(User.ColorId);
            writer.WriteEndObject();
            writer.Flush();
            return Send(sw.GetStringBuilder().ToString());
        }

        //接收的UI的发帖请求，向服务器发送"send"新帖子
        public static int Submit(String s,int fatherId=0)
        {
            s = s.Trim();

            StringWriter sw = new StringWriter();
            JsonWriter writer = new JsonTextWriter(sw);
            writer.WriteStartObject();
            writer.WritePropertyName("action"); writer.WriteValue("send");
            writer.WritePropertyName("senderName"); writer.WriteValue(User.NickName); 
            writer.WritePropertyName("colorId"); writer.WriteValue(User.ColorId);
            writer.WritePropertyName("content"); writer.WriteValue(s);
            writer.WritePropertyName("father"); writer.WriteValue(fatherId);
            writer.WriteEndObject();
            writer.Flush();
            return Send(sw.GetStringBuilder().ToString());

        }
        
        //接收MessagesKeeper.asking的请求，对服务器发送"get"请求
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

        //发送信息
        private static int Send(String s)
        {
            byte[] bs = Encoding.UTF8.GetBytes(s);

            try
            {
                soc.Send(bs, bs.Length, 0);
            }
            catch (SocketException e)
            {
                InfoBox.AddInfo("服务器连接错误！您已离线。[Sending failed]");
                return 1;
            }

            return 0;
        }

    }
}
