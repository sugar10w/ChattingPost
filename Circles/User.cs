using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Newtonsoft.Json.Linq;

namespace Circles
{
    class User
    {
        string nickName = "", password="";
        public string NickName { get { return nickName; } }
        public string Password { get { return password; } }

        int colorId;
        public int ColorId { get { return colorId; } }

        Color color;        
        public Color Color { get { return color; } }
        public Color LightColor { get { return MyColor.HSI(colorId, 0.1); } }

        public User(string nickname, string password)
        {
            this.nickName = nickname;
            this.password = password;
            colorId = MyColor.NameColorId(nickName);
            color = MyColor.HSI(colorId);
        }

        //登陆控制
        static private bool loginFlag = false, wrongPassword = false;
        static public bool LoginFlag { get { return loginFlag; } }
        //检查是否收到了密码错误的提示
        static public bool WrongPassword
        {
            get
            {
                if (!wrongPassword) return false;
                else
                {
                    wrongPassword = false;
                    return true;
                }
            }
        }

        //检查用户是否成功登陆
        static public void Login(JObject j)
        {
            if (j["user"].ToString().Equals("wrong"))
            {
                loginFlag = false;
                wrongPassword = true;
                MessagesKeeper.Reset();
                ClientSocket.Stop();
            }
            else if (j["user"].ToString().Equals("accept"))
            {
                loginFlag = true;
            }
            else if (j["user"].ToString().Equals("new"))
            {
                loginFlag = true;
                InfoBox.AddInfo("新用户"+j["nickname"].ToString()+"已创建。");
            }
        }
    }
}
