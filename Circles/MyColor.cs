using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Circles
{
    class MyColor
    {
        static public Color HSI(double H, double alpha = 0.3)
        {
            double R = 0, G = 0, B = 0;
            double I = 150, S = 1, r60 = Math.PI / 3;

            while (H < 0) H += 360;
            while (H > 360) H -= 360;

            if (H >= 0 && H < 120)
            {
                H = H / 180 * Math.PI;
                R = I * (1 + S * Math.Cos(H) / Math.Cos(r60 - H));
                B = I * (1 - S);
                G = 3 * I - R - B;
            }
            else if (H >= 120 && H < 240)
            {
                H = H / 180 * Math.PI;
                G = I * (1 + S * Math.Cos(H - 2 * r60) / Math.Cos(r60 * 3 - H));
                R = I * (1 - S);
                B = 3 * I - R - G;
            }
            else if (H >= 240 && H < 360)
            {
                H = H / 180 * Math.PI;
                B = I * (1 + S * Math.Cos(H - 4 * r60) / Math.Cos(r60 * 5 - H));
                G = I * (1 - S);
                R = 3 * I - G - B;
            }

            if (R > 255) R = 255;
            if (G > 255) G = 255;
            if (B > 255) B = 255;

            return Color.FromArgb((byte)(alpha * 255), (byte)R, (byte)G, (byte)B);
        }

        static public int NameColorId(string s)
        {
            int h = 0;
            for (int i = 0; i < s.Length; ++i) h += s[i];
            return h % 360;
        }
    }
}
