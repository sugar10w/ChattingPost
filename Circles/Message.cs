using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;
using System.Windows;

namespace Circles
{
    class Message
    {
        int id = 0, father = 0, colorId = 0, hot = 0, place= 0;
        string content = "", senderName = "";
        Color color;
        Brush brush;
        List<int> sonsId = new List<int>();

        public string SenderName { get { return senderName; } }
        public string Content { get { return content; } }
        public Color Color { get { return color; } }
        public Color LightColor { get { return MyColor.HSI(colorId, 0.1); } }
        public Brush Brush { get { return brush;} }
        public int Id { get { return id; } }
        public int Father { get { return father; } }
        public List<int> SonsId { get { return sonsId; } }
        public int Hot { get { return hot; } }
        public int Place { get { return place; } }

        //构造函数
        public Message(JObject j)
        {
            this.id = int.Parse(j["id"].ToString());
            this.content = j["content"].ToString();
            this.father = int.Parse(j["father"].ToString());
            this.senderName = j["senderName"].ToString();
            this.colorId = int.Parse(j["colorId"].ToString());
            this.hot = int.Parse(j["hot"].ToString());
            this.place = int.Parse(j["place"].ToString());

            this.color = MyColor.HSI(colorId);
            this.brush = new SolidColorBrush(this.color);
            this.RefreshSons((JArray)j["sons"]);
        }
        
        //与服务器进行同步更新时使用
        public void RefreshMessage(JObject j)
        {
            //    this.id = int.Parse(j["id"].ToString());
            this.content = j["content"].ToString();
            //    this.father = int.Parse(j["father"].ToString());
            //    this.senderName = j["senderName"].ToString();
            //    this.colorId = int.Parse(j["colorId"].ToString());
            this.hot = int.Parse(j["hot"].ToString());

            //    this.color = MyColor.HSI(colorId);
            this.RefreshSons((JArray)j["sons"]);
        }
        public void RefreshSons(JArray ja)
        {
            foreach (var j in ja)
            {
                int i = int.Parse(j.ToString());
                if (!SonsId.Contains(i)) SonsId.Add(i);
            }
        }

        //返回实例的UI尺寸
        public double Size(int level = 0)
        {
            return HotSize(this.hot, level);
        }
        public double FontSize(int level = 0)
        {
            return FontSize(this.hot, level);
        }

        //UI的位置和尺寸的通用算法
        //level: -2 父亲；-1 兄弟；0 自己；1 儿子；2 孙子；
        static public Thickness Position(int pos, double distance, Thickness father, double K = 1)
        {
            double l = father.Left,
                t = father.Top,
                r = father.Right,
                b = father.Bottom;

         //   distance -= 50;
            distance = distance * ((pos / 12) * 2 + 3) / 3;
            pos %= 12;

            //0  1  2  3
            //11       4
            //10       5
            //9  8  7  6

            switch (pos)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    b += distance;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    t += distance;
                    break;
                case 4:
                case 11:
                    b += distance / 3;
                    break;
                case 5:
                case 10:
                    t += distance / 3;
                    break;
            }

            switch (pos)
            {
                case 0:
                case 11:
                case 10:
                case 9:
                    r += distance;
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                    l += distance;
                    break;
                case 1:
                case 8:
                    r += distance / 3;
                    break;
                case 2:
                case 7:
                    l += distance / 3;
                    break;
            }

            return new Thickness(l * K, t * K, r * K, b * K);
        }
        static public double HotSize(int hot,int level)
        {
            double size=0;

            switch (level)
            {
                case -2: size = 400; break;
                case -1: size = 240; break;
                case 0: size=240; break;
                case 1: size=140; break;
                case 2: size=100; break;
            }
            
            if (hot > 20) hot = 20;
            size = size * (1 + 0.02 * hot);

            return size;
        }
        static public double FontSize(int hot,int level)
        {
            double size=0;

            switch (level)
            {
                case -2: size = 60; break;
                case -1: size = 36; break;
                case 0: size = 36; break;
                case 1: size = 20; break;
                case 2: size = 16; break;
            }

            if (hot > 100) hot = 100;
            size = size * (1 + 0.005 * hot);

            return size;
        }
        
        static public Color LightFontColor = Color.FromArgb(100, 0, 0, 0);
    }
}
