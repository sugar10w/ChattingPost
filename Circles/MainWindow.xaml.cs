using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Circles
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private int currentColorId = 0, tartgetColorId = 0, speedOfColor = 1;
        private DispatcherTimer timer;
        private static Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            rect0.Fill = new SolidColorBrush(MyColor.HSI(currentColorId));         
            rect1.Fill = new SolidColorBrush(MyColor.HSI(currentColorId+72));
            rect2.Fill = new SolidColorBrush(MyColor.HSI(currentColorId+144));
            rect3.Fill = new SolidColorBrush(MyColor.HSI(currentColorId-144));
            rect4.Fill = new SolidColorBrush(MyColor.HSI(currentColorId-72));
        }

        private void Grid_Loaded_1(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50);
            timer.Tick += timer1_Tick;
            timer.Start();

            Keyboard.Focus(textBoxUserName);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
          int d = currentColorId - tartgetColorId;
            if (d < -180) d += 360;
            else if (d > 180) d -= 360;

            if (d <= speedOfColor && d >= -speedOfColor) currentColorId = tartgetColorId;
            else if (d < 0) currentColorId = (currentColorId + speedOfColor) % 360;
            else currentColorId = (currentColorId + 360 - speedOfColor) % 360;

            rect0.Fill = new SolidColorBrush(MyColor.HSI(currentColorId));         
            rect1.Fill = new SolidColorBrush(MyColor.HSI(currentColorId+72));
            rect2.Fill = new SolidColorBrush(MyColor.HSI(currentColorId+144));
            rect3.Fill = new SolidColorBrush(MyColor.HSI(currentColorId-144));
            rect4.Fill = new SolidColorBrush(MyColor.HSI(currentColorId-72));

            if (User.WrongPassword)
            {
                MessageBox.Show("密码错误！");
            }

            if (User.LoginFlag)
            {
                NextStage();
            }

        }

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            string s = textBoxUserName.Text;
            tartgetColorId = MyColor.NameColorId(s);
        }

        private void textBoxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string userName = textBoxUserName.Text.Trim();
                string password = textBoxPassword.Password.TrimEnd();
                int t = ClientSocket.ConnectServer(userName, password);
                Console.WriteLine("Tring to connect the server");
                if (t == 1) this.Close();
            }
        }

        private void NextStage()
        {
            new Window1().Show();
            timer.Stop();
            this.Close();
        }
    }
}
