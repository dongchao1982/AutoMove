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
            TargetWidth = 50;
            TargetHeight = 50;

            this.SetBinding(this.sldTrack, Slider.ValueProperty, "Position");
            this.SetBinding(this.sldPitch, Slider.ValueProperty, "Pitch");
            this.SetBinding(this.sldYaw, Slider.ValueProperty, "Yaw");
        }

        private void SetBinding(FrameworkElement obj, DependencyProperty p, string path)
        {
            Binding b = new Binding(path) { Source = this };
            obj.SetBinding(p, b);
        }

        public static readonly DependencyProperty PositionValueProperty =
            DependencyProperty.Register(
                "Position", typeof(double), typeof(MovieControl),
                new FrameworkPropertyMetadata((double)0, new PropertyChangedCallback(OnPositionValueChanged),
                                              new CoerceValueCallback(PositionCoerceValue)));

        private static object PositionCoerceValue(DependencyObject obj, object value)
        {
            double newValue = (double)value;
            MovieControl control = (MovieControl)obj;
            newValue = Math.Max(control.sldTrack.Minimum, Math.Min(control.sldTrack.Maximum, newValue));
            return newValue;
        }

        private static void OnPositionValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MovieControl control = (MovieControl)obj;
            RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(
                (double)args.OldValue, (double)args.NewValue, PositionValueChangedEvent);
            control.OnValueChanged(e);
        }

        public static readonly RoutedEvent PositionValueChangedEvent = EventManager.RegisterRoutedEvent(
            "PositionValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>), typeof(MovieControl));

        protected virtual void OnValueChanged(RoutedPropertyChangedEventArgs<double> args)
        {
            RaiseEvent(args);
        }

        //轨道位置
        public double Position
        {
            get
            {
                return (double)GetValue(PositionValueProperty);
            }
            set
            {
                SetValue(PositionValueProperty, value);
            }
        }

        public static readonly DependencyProperty PitchValueProperty =
            DependencyProperty.Register(
                "Pitch", typeof(double), typeof(MovieControl),
                new FrameworkPropertyMetadata((double)0, new PropertyChangedCallback(OnPitchValueChanged),
                                              new CoerceValueCallback(PitchCoerceValue)));

        private static object PitchCoerceValue(DependencyObject obj, object value)
        {
            double newValue = (double)value;
            MovieControl control = (MovieControl)obj;
            newValue = Math.Max(control.sldPitch.Minimum, Math.Min(control.sldPitch.Maximum, newValue));
            return newValue;
        }

        private static void OnPitchValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MovieControl control = (MovieControl)obj;
            RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(
                (double)args.OldValue, (double)args.NewValue, PitchValueChangedEvent);
            control.OnValueChanged(e);
        }

        public static readonly RoutedEvent PitchValueChangedEvent = EventManager.RegisterRoutedEvent(
            "PitchValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>), typeof(MovieControl));


        //俯仰角（点头、抬头）
        public double Pitch
        {
            get
            {
                return (double)GetValue(PitchValueProperty);
            }
            set
            {
                SetValue(PitchValueProperty, value);
            }
        }

        public static readonly DependencyProperty YawValueProperty =
            DependencyProperty.Register(
                "Yaw", typeof(double), typeof(MovieControl),
                new FrameworkPropertyMetadata((double)0, new PropertyChangedCallback(OnYawValueChanged),
                                              new CoerceValueCallback(YawCoerceValue)));

        private static object YawCoerceValue(DependencyObject obj, object value)
        {
            double newValue = (double)value;
            MovieControl control = (MovieControl)obj;
            newValue = Math.Max(control.sldYaw.Minimum, Math.Min(control.sldYaw.Maximum, newValue));
            return newValue;
        }

        private static void OnYawValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MovieControl control = (MovieControl)obj;
            RoutedPropertyChangedEventArgs<double> e = new RoutedPropertyChangedEventArgs<double>(
                (double)args.OldValue, (double)args.NewValue, YawValueChangedEvent);
            control.OnValueChanged(e);
        }

        public static readonly RoutedEvent YawValueChangedEvent = EventManager.RegisterRoutedEvent(
            "YawValueChanged", RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<double>), typeof(MovieControl));

        //偏航角（左右摇动）
        public double Yaw
        {
            get
            {
                return (double)GetValue(YawValueProperty);
            }
            set
            {
                SetValue(YawValueProperty, value);
            }
        }

        //剪裁宽度
        public double ClipWidth { set; get; }
        //剪裁高度
        public double ClipHeight { set; get; }
        //剪裁x位置
        public double ClipX { set; get; }
        //剪裁y位置
        public double ClipY { set; get; }

        //目标尺寸宽度
        public double TargetWidth { set; get; }
        //目标尺寸高度
        public double TargetHeight { set; get; }

        public void updateUI()
        {
            Action updateAction = new Action(delegate ()
            {
                updateViewfinder();
            });
            this.Dispatcher.BeginInvoke(updateAction);
        }

        private void updateViewfinder()
        {
            rectTarget.Width = TargetWidth;
            rectTarget.Height = TargetHeight;

            //裁剪框尺寸（与背景尺寸相同）
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
