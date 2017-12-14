using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AutoMovie
{
    /// <summary>
    /// MotorDlg.xaml 的交互逻辑
    /// </summary>
    public partial class MotorDlg : Window
    {
        public Motor m_motor;

        public int Speed { get; set; }

        public int Position
        {
            get { return m_motor.Position; }
        }

        public MotorDlg(Motor motor)
        {
            InitializeComponent();

            m_motor = motor;
            this.Title = motor.Name;
            this.Speed = motor.Speed;

            this.DataContext = this;
        }

        public void setPosition()
        {
            Action updateAction = new Action(delegate ()
            {
                labPosition.Content = m_motor.Position;
            });
            this.Dispatcher.BeginInvoke(updateAction);
        }

        private void ButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            
            Button btn = (Button)sender;
            if (m_motor != null)
            {
                Motor.eMoveType eMoveType = Motor.convertMoveType(btn.Name);
                m_motor.setpMoveStart(eMoveType);
            }
        }

        private void ButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            m_motor.setpMoveStop();
            m_motor.stop();
        }

        private void OK(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            m_motor.Speed = Speed;
            m_motor.setPulseRate(m_motor.Speed); // 设置速度

            this.DialogResult = true;
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            m_motor.setPulseRate(m_motor.Speed); // 恢复速度
        }

        private void SpeedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            if(text.Text.Length>0)
            {
                Speed = Convert.ToInt32(text.Text);
                m_motor.setPulseRate(Speed);
            }
            else
            {
                m_motor.setPulseRate(0);
            }
        }

        private void SpeedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox txt = sender as TextBox;

            //屏蔽非法按键
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Decimal || e.Key.ToString() == "Tab")
            {
                if (txt.Text.Contains(".") && e.Key == Key.Decimal)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else if (((e.Key >= Key.D0 && e.Key <= Key.D9) || e.Key == Key.OemPeriod) && e.KeyboardDevice.Modifiers != ModifierKeys.Shift)
            {
                if (txt.Text.Contains(".") && e.Key == Key.OemPeriod)
                {
                    e.Handled = true;
                    return;
                }
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                if (e.Key.ToString() != "RightCtrl")
                {
                }
            }
        }
    }
}
