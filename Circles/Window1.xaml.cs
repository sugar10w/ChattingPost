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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Circles
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        User user;
        int currentCenterId = 0;
        DispatcherTimer timer;
      //  Random random = new Random();
      //  int randomMove = 20;

        int cntDownSysInfoMax = 10, cntDownSysInfo = 0;

        //页面初始布置
        public Window1()
        {
            InitializeComponent();

            this.user = ClientSocket.User;
            labelUserName.Content = user.NickName;

            rectangleBottom.Fill = new SolidColorBrush(user.Color);
            ButtonBack.Background = new SolidColorBrush(user.Color);
            TextSysInfo.Background = new SolidColorBrush(user.LightColor);

            ButtonBack.PreviewMouseUp += ButtonBack_PreviewMouseUp;
            labelUserName.MouseUp += labelUserName_MouseUp;
        }
        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += timer1_Tick;
            timer.Start();

            cntDownSysInfoMax = 5000 / (int)timer.Interval.TotalMilliseconds;
        }
        void ButtonBack_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (currentCenterId == 0) return;
            Message msg = MessagesKeeper.Get(currentCenterId);
            if (msg == null) return;
            GoTo(msg.Father);

        }
        void labelUserName_MouseUp(object sender, MouseButtonEventArgs e)
        {
            GoTo(0);

        }

        //总UI控制
        private void GoTo(int id)
        {
            ClearMessages();
            currentCenterId = id;
            if (id == 0) ButtonBack.Visibility = Visibility.Collapsed; else ButtonBack.Visibility = Visibility.Visible;
            ShowMe();
        }
        private void ShowMe()
        {
            Message m0;

            m0 = MessagesKeeper.Get(currentCenterId, 3);
            if (m0 == null) return;

            if (currentCenterId == 0)
            {
                ShowIndex();
            }
            else
            {
                ShowMessage();
            }
        }
        void tb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            int id = (int)tb.Tag;

            GoTo(id);
        }

        //Index页面下的UI控制
        private void ShowIndex()
        {
            scrollIndex.Visibility = Visibility.Visible;
            scrollMessages.Visibility = Visibility.Collapsed;

            Message m0 = MessagesKeeper.Get(0);
            if (m0 == null)
            {
                MessageBox.Show("数据加载中，请稍后……");
                return;
            }

            foreach (int sonId in m0.SonsId)
            {
                Message m1 = MessagesKeeper.Get(sonId);
                if (m1 != null)
                {
                    ScrollViewer sv = getIndexTextBlock(m1);
                    if (!stackPanel.Children.Contains(sv))
                    {
                        stackPanel.Children.Add(sv);
                        MainGrid.RegisterName("scrollViewerIndex" + m1.Id, sv);
                    }
                }
            }
        }
        private ScrollViewer getIndexTextBlock(Message msg)
        {
            ScrollViewer sv = null;
            sv = MainGrid.FindName("scrollViewerIndex" + msg.Id) as ScrollViewer;
            if (sv != null) return sv;

            sv = new ScrollViewer();
            sv.Height = msg.Size(0);
            sv.Width = msg.Size(0);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.Margin = new Thickness(10);

            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Inlines.Add(new Bold(new Run(msg.SenderName + ": ")));
            tb.Inlines.Add(new Run(msg.Content));
            tb.Background = new SolidColorBrush(msg.Color);
            tb.FontSize = msg.FontSize();
            tb.MouseUp += tb_MouseUp;
            tb.Tag = msg.Id;

            sv.Content = tb;

            return sv;
        }

        //Message页面下的UI控制
        private void ShowMessage()
        {
            scrollIndex.Visibility = Visibility.Collapsed;
            scrollMessages.Visibility = Visibility.Visible;

            Message m0 = MessagesKeeper.Get(currentCenterId);
            if (m0 == null)
            {
                MessageBox.Show("数据加载中，请稍后……");
                return;
            }

            //自己
            ScrollViewer sv0 = getTextBlock(m0, 0);
            if (!gridMessages.Children.Contains(sv0))
            {
                gridMessages.Children.Add(sv0);
                gridMessages.RegisterName("scrollViewer" + m0.Id, sv0);
            }

            //父亲
            if (m0.Father != 0)
            {
                Message m_2 = MessagesKeeper.Get(m0.Father);
                ScrollViewer sv_2 = getTextBlock(m_2, -2);
                if (!gridMessages.Children.Contains(sv_2))
                {
                    gridMessages.Children.Add(sv_2);
                    gridMessages.RegisterName("scrollViewer" + m0.Father, sv_2);
                }

                foreach (int sonId in m_2.SonsId)
                {
                    //兄弟
                    Message m_1 = MessagesKeeper.Get(sonId);
                    if (m_1 == null || m_1.Id == currentCenterId) continue;
                    ScrollViewer sv_1 = getTextBlock(m_1, -1);
                    if (!gridMessages.Children.Contains(sv_1))
                    {
                        gridMessages.Children.Add(sv_1);
                        gridMessages.RegisterName("scrollViewer" + m_1.Id, sv_1);
                    }
                }
            }

            //儿子
            foreach (int sonId in m0.SonsId)
            {
                Message m1 = MessagesKeeper.Get(sonId);
                if (m1 == null) continue;
                ScrollViewer sv1 = getTextBlock(m1, 1);
                if (!gridMessages.Children.Contains(sv1))
                {
                    gridMessages.Children.Add(sv1);
                    gridMessages.RegisterName("scrollViewer" + m1.Id, sv1);
                }

                //孙子
                foreach (int sonsonId in m1.SonsId)
                {
                    Message m2 = MessagesKeeper.Get(sonsonId);
                    if (m2 == null) continue;
                    ScrollViewer sv2 = getTextBlock(m2, 2);
                    if (!gridMessages.Children.Contains(sv2))
                    {
                        gridMessages.Children.Add(sv2);
                        gridMessages.RegisterName("scrollViewer" + m2.Id, sv2);
                    }
                }
            }
        }
        private ScrollViewer getTextBlock(Message msg, int level = 0)
        {
            ScrollViewer sv = null;
            TextBlock tb = null;

            //查找目标UI是否已存在于容器
            sv = MainGrid.FindName("scrollViewer" + msg.Id) as ScrollViewer;
            if (sv == null)
            {
                //没有找到，新建UI
                sv = new ScrollViewer();
                tb = new TextBlock();

                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Inlines.Add(new Bold(new Run(msg.SenderName + ": ")));
                tb.Inlines.Add(new Run(msg.Content));
                tb.Background = new SolidColorBrush(msg.Color);
                tb.MouseUp += tb_MouseUp;
                tb.Tag = msg.Id;
                sv.Content = tb;
            }
            else
            {
                ////已经正常显示则不修改
                //if (sv.IsVisible) return sv;
                //否则进行微调
                tb = sv.Content as TextBlock;
            }

            sv.Height = msg.Size(level);
            sv.Width = msg.Size(level);
            tb.FontSize = msg.FontSize(level);

            if (level == 0 || level == 1)
            {
                //正常显示
                tb.Background = new SolidColorBrush(msg.Color);
                tb.Foreground = Brushes.Black;
            }
            else
            {
                //弱显示
                tb.Background = new SolidColorBrush(msg.LightColor);
                tb.Foreground = new SolidColorBrush(Message.LightFontColor);
            }

            if (!sv.IsVisible)
            {
                if (level == 0)
                {
                    //正中间
                    sv.Margin = new Thickness(0);
                }
                else if (level >= -1)
                {
                    //儿子位置正关联
                    ScrollViewer sv0 = MainGrid.FindName("scrollViewer" + msg.Father) as ScrollViewer;
                    if (sv0 != null)
                    {
                        double distance = sv0.Width + sv.Width;// -randomMove / 2 + random.Next(randomMove);
                        sv.Margin = Message.Position(msg.Place, distance, sv0.Margin);
                    }
                }
                else if (level == -2)
                {
                    //父亲位置负关联
                    ScrollViewer sv0 = MainGrid.FindName("scrollViewer" + currentCenterId) as ScrollViewer;
                    Message m0 = MessagesKeeper.Get(currentCenterId);
                    if (sv0 != null && m0 != null)
                    {
                        double distance = sv0.Width + sv.Width;// -randomMove / 2 + random.Next(randomMove);
                        sv.Margin = Message.Position(m0.Place, distance, sv0.Margin, -1);
                    }
                }
            }

            sv.Visibility = Visibility.Visible;

            return sv;
        }

        //将Message页面清空
        private void ClearMessages()
        {
            foreach (Object obj in gridMessages.Children)
            {
                if (obj is ScrollViewer)
                {
                    ScrollViewer sv = obj as ScrollViewer;
                    sv.Visibility = Visibility.Collapsed;
                }
            }
        }

        //定时检查
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (cntDownSysInfo > 0) --cntDownSysInfo;
            if (cntDownSysInfo > 0) TextSysInfo.Visibility = Visibility.Visible;
            else TextSysInfo.Visibility = Visibility.Collapsed;
            if (InfoBox.Refreshed)
            {
                TextSysInfo.Text = InfoBox.Info;
                cntDownSysInfo = cntDownSysInfoMax;
            }

            ShowMe();
        }

        //回车发帖
        private void textBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string str = textBoxInput.Text.Trim();
                textBoxInput.Text = "";
                if (!str.Equals(""))
                    if (ClientSocket.Submit(str, currentCenterId) == 1) InfoBox.AddInfo("信息\""+str+"\"发送失败。");
            }
        }

        //Index页面下的滚轮控制
        private void Window_PreviewMouseWheel_1(object sender, MouseWheelEventArgs e)
        {
            if (!scrollIndex.IsVisible) return;

            int d = e.Delta / 120;
            if (d > 0)
            {
                for (int i = 0; i < d; ++i) scrollIndex.LineLeft();
            }
            else if (d < 0)
            {
                for (int i = 0; i > d; --i) scrollIndex.LineRight();
            }
        }

    }
}
