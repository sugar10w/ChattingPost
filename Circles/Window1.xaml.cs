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
        Random random = new Random();
        int randomMove = 20;

        public Window1()
        {
            InitializeComponent();
        }

        private ScrollViewer makeIndexTextBlock(Message msg)
        {
            ScrollViewer sv = null;
            sv = MainGrid.FindName("scrollViewerIndex"+msg.Id) as ScrollViewer;
            if (sv != null) return sv;

            sv = new ScrollViewer();
            sv.Height = msg.Size(0);
            sv.Width = msg.Size(0);
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            sv.Margin = new Thickness(10);

            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.Inlines.Add(new Bold(new Run(msg.SenderName+": ")));
            tb.Inlines.Add(new Run(msg.Content));
            tb.Background = new SolidColorBrush(msg.Color);
            tb.FontSize = msg.FontSize() ;
            tb.MouseUp += tb_MouseUp;
            tb.Tag = msg.Id;

            sv.Content=tb;

            return sv;
        }
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
                    ScrollViewer sv = makeIndexTextBlock(m1);
                    if (!stackPanel.Children.Contains(sv))
                    {
                        stackPanel.Children.Add(sv);
                        MainGrid.RegisterName("scrollViewerIndex" + m1.Id, sv);
                    }
                }
            }
        }

        private ScrollViewer makeTextBlock(Message msg, int level = 0)
        {
            ScrollViewer sv = null;
            TextBlock tb = null;
            sv = MainGrid.FindName("scrollViewer" + msg.Id) as ScrollViewer;
            if (sv == null)
            {
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
                if (sv.IsVisible) return sv;
                tb = sv.Content as TextBlock;
            }

            sv.Visibility = Visibility.Visible;

            sv.Height = msg.Size(level);
            sv.Width = msg.Size(level);
            tb.FontSize = msg.FontSize(level);

            if (level >= 2 || level < 0)
            {
                tb.Background = new SolidColorBrush(msg.LightColor);
                tb.Foreground = new SolidColorBrush(Message.LightFontColor);
            }
            else
            {
                tb.Background = new SolidColorBrush(msg.Color);
                tb.Foreground = Brushes.Black;
            }

            if (level == 0)
            {
                sv.Margin = new Thickness(0);
            }
            else if (level >= -1)
            {
                ScrollViewer sv0 = MainGrid.FindName("scrollViewer" + msg.Father) as ScrollViewer;
                if (sv0 != null)
                {
                    double distance = sv0.Width + sv.Width - randomMove / 2 + random.Next(randomMove);
                    sv.Margin = Message.Position(msg.Place, distance, sv0.Margin);
                }
            }
            else if (level <= -2)
            {
                ScrollViewer sv0 = MainGrid.FindName("scrollViewer" + currentCenterId) as ScrollViewer;
                Message m0=MessagesKeeper.Get(currentCenterId);
                if (sv0 != null && m0!=null)
                {
                    double distance = sv0.Width + sv.Width - randomMove / 2 + random.Next(randomMove);
                    sv.Margin = Message.Position(m0.Place, distance, sv0.Margin, -1);
                }
            }

            return sv;
        }
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

            ScrollViewer sv0 = makeTextBlock(m0, 0);
            if (!gridMessages.Children.Contains(sv0))
            {
                gridMessages.Children.Add(sv0);
                gridMessages.RegisterName("scrollViewer" + m0.Id, sv0);
            }

            if (m0.Father != 0)
            {
                Message m_2 = MessagesKeeper.Get(m0.Father);
                ScrollViewer sv_2 = makeTextBlock(m_2, -2);
                if (!gridMessages.Children.Contains(sv_2))
                {
                    gridMessages.Children.Add(sv_2);
                    gridMessages.RegisterName("scrollViewer" + m0.Father, sv_2);
                }

                foreach (int sonId in m_2.SonsId)
                {
                    Message m_1 = MessagesKeeper.Get(sonId);
                    if (m_1 == null) continue;
                    ScrollViewer sv_1 = makeTextBlock(m_1, -1);
                    if (!gridMessages.Children.Contains(sv_1))
                    {
                        gridMessages.Children.Add(sv_1);
                        gridMessages.RegisterName("scrollViewer" + m_1.Id, sv_1);
                    }
                }

            }



            foreach (int sonId in m0.SonsId)
            {
                Message m1 = MessagesKeeper.Get(sonId);
                if (m1 == null) continue;
                ScrollViewer sv1 = makeTextBlock(m1, 1);
                if (!gridMessages.Children.Contains(sv1))
                {
                    gridMessages.Children.Add(sv1);
                    gridMessages.RegisterName("scrollViewer" + m1.Id, sv1);
                }

                foreach (int sonsonId in m1.SonsId)
                {
                    Message m2 = MessagesKeeper.Get(sonsonId);
                    if (m2 == null) continue;
                    ScrollViewer sv2 = makeTextBlock(m2, 2);
                    if (!gridMessages.Children.Contains(sv2))
                    {
                        gridMessages.Children.Add(sv2);
                        gridMessages.RegisterName("scrollViewer" + m2.Id, sv2);
                    }
                }

            }

        }

        private void ShowMe()
        {
            Message m0, m1, m2;

            m0 = MessagesKeeper.Get(currentCenterId,3);
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
        private void GoTo(int id)
        {
            ClearMessages();
            currentCenterId = id;
            if (id == 0) ButtonBack.Visibility = Visibility.Collapsed; else ButtonBack.Visibility = Visibility.Visible;
            ShowMe();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            this.user = ClientSocket.user;

            rectangleBottom.Fill = new SolidColorBrush(user.Color);

            ButtonBack.Background = new SolidColorBrush(user.Color);
            ButtonBack.PreviewMouseUp += ButtonBack_PreviewMouseUp;

            labelUserName.Content = user.NickName;

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(200);
            timer.Tick += timer1_Tick;
            timer.Start();

            labelUserName.MouseUp += labelUserName_MouseUp;
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

        void tb_MouseUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = sender as TextBlock;
            int id = (int)tb.Tag;

            GoTo(id);
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowMe();
        }

        private void textBoxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string str = textBoxInput.Text.Trim();
                textBoxInput.Text = "";
                if (!str.Equals("")) ClientSocket.Submit(str, currentCenterId);
            }
        }

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
