using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

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

    }
}
