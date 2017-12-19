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

namespace AutoMovie
{
    /// <summary>
    /// MovieControl.xaml 的交互逻辑
    /// </summary>
    public partial class MovieControl : UserControl
    {
        public MovieControl()
        {
            InitializeComponent();

            ClipWidth = 200;
            ClipHeight = 112;
        }

        public double Position { get; set; }
        public double Pitch { get; set; }
        public double Yaw { get; set; }

        public double ClipWidth { set; get; }
        public double ClipHeight { set; get; }
        public double ClipX { set; get; }
        public double ClipY { set; get; }

        private void updateViewfinder()
        {
            //裁剪框尺寸
            bdClip.Width = cavPanel.ActualWidth;
            bdClip.Height = cavPanel.ActualHeight;

            //目标位置
            double x = cavPanel.ActualWidth / 2 - rectTarget.Width / 2;
            double y = cavPanel.ActualHeight / 2 - rectTarget.Height / 2;
            Canvas.SetLeft(rectTarget, x);
            Canvas.SetTop(rectTarget, y);

            //裁剪区域位置
            x = bdClip.Width / 2 - ClipWidth/2;
            y = bdClip.Height / 2 - ClipHeight/2;
            ClipX = x + (Position / 100 * ClipWidth);
            ClipY = y - (Pitch / 100 * ClipHeight);

            cavPanel.DataContext = null;
            cavPanel.DataContext = this;
        }

        private void sldTrack_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = (Slider)sender;
            Position = sld.Value;
            updateViewfinder();
        }

        private void sldPitch_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = (Slider)sender;
            Pitch = sld.Value;
            updateViewfinder();
        }

        private void sldYaw_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider sld = (Slider)sender;
            Yaw = sld.Value;
            updateViewfinder();
        }

        private void sldTrack_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Slider sld = (Slider)sender;
            sld.Value = 0;
            updateViewfinder();
        }

        private void sldPitch_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Slider sld = (Slider)sender;
            sld.Value = 0;
            updateViewfinder();
        }

        private void sldYaw_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Slider sld = (Slider)sender;
            sld.Value = 0;
            updateViewfinder();
        }

        private void cavPanel_Loaded(object sender, RoutedEventArgs e)
        {
            updateViewfinder();
        }

        private void cavPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            updateViewfinder();
        }
    }
}
