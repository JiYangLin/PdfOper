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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pdfOper
{
    /// <summary>
    /// OptWaitWnd.xaml 的交互逻辑
    /// </summary>
    public partial class OptWaitWnd
    {
        public OptWaitWnd(string title, string str,bool showBtn = false)
        {
            InitializeComponent();
            this.Title = title;
            inf.Content = str;

            this.Loaded += OptPromtWnd_Loaded;
            if (!showBtn) btnSp.Visibility = Visibility.Hidden;
        }

        private void OptPromtWnd_Loaded(object sender, RoutedEventArgs e)
        {
            Run();
        }

        void Run()
        {
            int end = (int)(this.Width - inf.ActualWidth);
            Animate(0, end, (object sender, EventArgs e) => {
                Animate(end, 0, (object sr, EventArgs er) => {
                    Animate(0, end, null);
                    Run();
                });
            });
        }
        void Animate(int from, int to, EventHandler hd)
        {
            DoubleAnimation DAnimation = new DoubleAnimation();
            DAnimation.From = from;
            DAnimation.To = to;
            DAnimation.Duration = new Duration(TimeSpan.FromSeconds(8));
            Storyboard.SetTarget(DAnimation, inf);
            Storyboard.SetTargetProperty(DAnimation, new PropertyPath(Canvas.LeftProperty));
            Storyboard story = new Storyboard();
            if (null != hd) story.Completed += hd;
            story.Children.Add(DAnimation);
            story.Begin();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
