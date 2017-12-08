using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AutoMovie
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Motor> m_lstMotors = null;
        private SerialPort m_port = null;
        int m_portFrequency = 57600;

        public string Output { get; set; }
        private string m_strOutputLine;

        private MotorDlg m_motorDlg = null;

        private DispatcherTimer m_timer = new DispatcherTimer();

        private KeyFrame m_KeyFrame = null;

        private KeyFrameControl m_KeyFrameCtrl = null;

        private int m_OldTimeTickCount = 0;

        public MainWindow()
        {
            InitializeComponent();

            m_lstMotors = new List<Motor>()
                {
                    new Motor() { Name = "轨道",Index = 7,Position=0,Enable=true,Speed=10000,Moving=false },
                    new Motor() { Name = "云台旋转",Index = 5,Position=0,Enable=true,Speed=10000,Moving=false },
                    new Motor() { Name = "云台俯仰",Index = 6,Position=0,Enable=true,Speed=10000,Moving=false },
                    new Motor() { Name = "调焦1",Index = 2,Position=0,Enable=true,Speed=1000,Moving=false },
                    new Motor() { Name = "调焦2",Index = 3,Position=0,Enable=true,Speed=1000,Moving=false },
                };
            m_lstboxMotor.DataContext = m_lstMotors;

            refreshPort();

            m_btnInitial.IsEnabled = m_port == null;
            m_btnDispose.IsEnabled = m_port != null;

            m_strOutputLine = "";

            m_timer.Tick += new EventHandler(LogicUpdate);
            m_timer.Interval = TimeSpan.FromSeconds(0.05);

            m_KeyFrame = new KeyFrame();
            m_KeyFrameCtrl = new KeyFrameControl();
        }

        void LogicUpdate(object sender, EventArgs e)
        {
            int dt = Environment.TickCount - m_OldTimeTickCount;

            if (m_port!=null)
            {
                m_port.WriteLine("ms\r\n");
            }

            m_KeyFrameCtrl.logic(dt);

            m_OldTimeTickCount = Environment.TickCount;
        }

        private Motor findMotor(String name)
        {
            Motor motor = null;
            m_lstMotors.ForEach(
                delegate (Motor m)
                {
                    if(m.Name == name)
                    {
                        motor = m;
                    }
                });
            return motor;
        }

        private Motor findMotor(int idx)
        {
            Motor motor = null;
            m_lstMotors.ForEach(
                delegate (Motor m)
                {
                    if (m.Index == idx)
                    {
                        motor = m;
                    }
                });
            return motor;
        }

        private void refreshPort()
        {
            string[] ports = SerialPort.GetPortNames();
            for (int i = 0; i < ports.Length; ++i)
            {
                m_comboList.Items.Add(ports[i]);
            }
            m_comboList.SelectedIndex = m_comboList.Items.Count - 1;
        }

        /// <summary>
        /// 初始化串口实例
        /// </summary>
        private void InitialSerialPort()
        {
            try
            {
                string portName = m_comboList.SelectedItem.ToString();
                m_port = new SerialPort(portName, m_portFrequency);
                m_port.Encoding = Encoding.ASCII;
                m_port.DataReceived += portDataReceived;
                m_port.Open();

                m_port.WriteLine("hi\r\n");

                m_lstMotors.ForEach(
                delegate (Motor m)
                {
                    m.Port = m_port;
                    m.stop();
                    m.getPosition();
                    m.setPulseRate(m.Speed);
                });

                m_OldTimeTickCount = Environment.TickCount;
                m_timer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化串口发生错误：" + ex.Message, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                m_port = null;
            }
        }

        /// <summary>
        /// 关闭并销毁串口实例
        /// </summary>
        private void DisposeSerialPort()
        {
            if (m_port != null)
            {
                try
                {
                    m_lstMotors.ForEach(
                    delegate (Motor m)
                    {
                        m.Port = null;
                    });

                    if (m_port.IsOpen)
                    {
                        m_port.Close();
                    }
                    m_port.Dispose();
                    m_port = null;
                    m_timer.Stop();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("关闭串口发生错误：" + ex.Message, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                    m_port = null;
                }
            }
        }

        void portDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string value = ReadSerialData();
            m_strOutputLine += value;
            int iEndPos = m_strOutputLine.IndexOf("\r\n");
            while (iEndPos != -1)
            {
                string cmd = m_strOutputLine.Substring(0, iEndPos + 2);
                m_strOutputLine = m_strOutputLine.Substring(iEndPos + 2);

                string instruction = cmd.Substring(0, 2);
                if(instruction == "mp")
                {
                    string param = cmd.Substring(2, cmd.Length - 4).Trim();
                    int index = param[0] - '0';
                    Motor motor = findMotor(index);
                    if (motor != null)
                    {
                        param = param.Substring(1);
                        int position = Convert.ToInt32(param);
                        motor.Position = position;
                        RefreshPosition(motor);
                    }
                }
                else if(instruction == "ms")
                {
                    string param = cmd.Substring(2, cmd.Length - 4).Trim();
                    for(int i = 0; i < 8; ++i)
                    {
                        Motor motor = findMotor(i + 1);
                        if (motor != null)
                        {
                            bool bMoving = param[i] == '1' ? true : false;
                            if (motor.Moving != bMoving)
                            {
                                Action<Motor, bool> updateAction = new Action<Motor, bool>(delegate (Motor m, bool b)
                                {
                                    m.Moving = b;
                                    for (int idx = 0; idx < m_lstMotors.Count; ++idx)
                                    {
                                        var item = m_lstboxMotor.ItemContainerGenerator.ContainerFromIndex(idx);
                                        TextBlock txtIndex = WPFHelper.GetChildObject<TextBlock>(item, "txtBlockIndex");
                                        if (txtIndex.Text == m.Index.ToString())
                                        {
                                            Button btn = WPFHelper.GetChildObject<Button>(item, "btnMoving");
                                            btn.IsEnabled = m.Moving;
                                        }
                                    }
                                });
                                this.Dispatcher.BeginInvoke(updateAction, motor, bMoving);
                            }
                        }
                    }
                }
                else
                {
                    //output(cmd);
                }

                iEndPos = m_strOutputLine.IndexOf("\r\n");
            }
        }

        /// <summary>
        /// 从串口读取数据并转换为字符串形式
        /// </summary>
        /// <returns></returns>
        private string ReadSerialData()
        {
            string value = "";
            try
            {
                if (m_port != null && m_port.BytesToRead > 0)
                {
                    value = m_port.ReadExisting();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取串口数据发生错误：" + ex.Message, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return value;
        }

        public void output(string str)
        {
            Output += str;
            RefreshOutput();
        }

        private void RefreshOutput()
        {
            Action<TextBox> updateAction = new Action<TextBox>(delegate (TextBox output)
            {
                output.Text = Output;
                output.ScrollToLine(this.m_output.LineCount - 1);
            });
            this.Dispatcher.BeginInvoke(updateAction, m_output);
        }

        private void RefreshPosition(Motor motor)
        {
            Action<Motor> updateAction = new Action<Motor>(delegate (Motor motor1)
            {
                for(int idx=0;idx<m_lstMotors.Count;++idx)
                {
                    var item = m_lstboxMotor.ItemContainerGenerator.ContainerFromIndex(idx);
                    TextBlock txtIndex = WPFHelper.GetChildObject<TextBlock>(item, "txtBlockIndex");
                    if(txtIndex.Text == motor1.Index.ToString())
                    {
                        TextBlock txtPosition = WPFHelper.GetChildObject<TextBlock>(item, "txtBlockPosition");
                        txtPosition.Text = Convert.ToString(motor1.Position);
                    }
                }
            });
            this.Dispatcher.BeginInvoke(updateAction, motor);

            if (m_motorDlg != null && m_motorDlg.m_motor.Index == motor.Index)
            {
                m_motorDlg.setPosition();
            }
        }

        private void NameMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(tb, "ItemPanel");
            Motor motor = findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                MotorDlg dlg = new MotorDlg(motor);
                dlg.Owner = this;
                m_motorDlg = dlg;
                dlg.ShowDialog();
                m_motorDlg = null;
            }
        }

        private void ButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "ItemPanel");
            Motor motor = findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                int moveType = -1;
                if (btn.Name == "LeftMove")
                {
                    moveType = 1;
                }
                else if (btn.Name == "LeftMoveStep")
                {
                    moveType = 2;
                }
                else if (btn.Name == "RightMoveStep")
                {
                    moveType = 3;
                }
                else if (btn.Name == "RightMove")
                {
                    moveType = 4;
                }

                if (moveType != -1)
                {
                    motor.moveStart(moveType);
                }
            }
        }

        private void ButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "ItemPanel");
            Motor motor = findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                motor.moveStop();
                motor.stop();
            }
        }

        private void MotorHomeClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "ItemPanel");
            Motor motor = findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                motor.move(0);
            }
        }

        private void MotorToggleClick(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "ItemPanel");
            Motor motor = findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
            }
        }

        private void RefreshPortClick(object sender, RoutedEventArgs e)
        {
            //断开连接
            DisposeSerialPort();

            refreshPort();
        }

        private void InitialSerialPortClick(object sender, RoutedEventArgs e)
        {
            InitialSerialPort();
            m_btnInitial.IsEnabled = m_port == null;
            m_btnDispose.IsEnabled = m_port != null;
        }

        private void DisposeSerialPortClick(object sender, RoutedEventArgs e)
        {
            DisposeSerialPort();
            m_btnInitial.IsEnabled = m_port == null;
            m_btnDispose.IsEnabled = m_port != null;

            output("bye.\r\n");
        }

        private void StopAllMotorClick(object sender, RoutedEventArgs e)
        {
            if(m_port!=null)
            {
                m_port.WriteLine("sa\r\n");
                output("stop all motor.\r\n");
            }
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "ItemPanel");
            Motor motor = findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                motor.clearPosition();
                output("clear motor(" + motor.Name + ") position.\r\n");
            }
        }

        private void OutputClearClick(object sender, RoutedEventArgs e)
        {
            Output = "";
            m_output.Text = "";
        }

        private void AddKeyClick(object sender, RoutedEventArgs e)
        {
            m_KeyFrame.addKey();
            foreach(Motor m in m_lstMotors)
            {
                MotorData key = new MotorData();
                key.Name = m.Name;
                key.Position = m.Position;
                key.Speed = m.Speed;
                m_KeyFrame.pushKeyAtLast(key);
            }

            lstBoxKeyFrame.ItemsSource = null;
            lstBoxKeyFrame.ItemsSource = m_KeyFrame.m_lstKeys;
            lstBoxKeyFrame.SelectedIndex = m_KeyFrame.m_lstKeys.Count-1;
        }

        private void DelKeyClick(object sender, RoutedEventArgs e)
        {
            if(lstBoxKeyFrame.SelectedIndex!=-1)
            {
                m_KeyFrame.m_lstKeys.RemoveAt(lstBoxKeyFrame.SelectedIndex);
                lstBoxKeyFrame.ItemsSource = null;
                lstBoxKeyFrame.ItemsSource = m_KeyFrame.m_lstKeys;
            }
        }

        private void ClearKeyClick(object sender, RoutedEventArgs e)
        {
            lstBoxKeyFrame.ItemsSource = null;
            lstBoxMotorKey.ItemsSource = null;
            m_KeyFrame.clear();
        }

        private void GoHomeClick(object sender, RoutedEventArgs e)
        {
            if(m_KeyFrame.getKeyCount()>0)
            {
                foreach (Motor m in m_lstMotors)
                {
                    MotorData k = m_KeyFrame.getKeyFormIndex(0, m.Name);
                    if(k!=null)
                    {
                        m.setPulseRate(k.Speed);
                        m.move(k.Position);
                    }
                }
            }
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            //有运动的电机不准许播放
            foreach(Motor m in m_lstMotors)
            {
                if(m.Moving)
                {
                    return;
                }
            }

            //播放start
            if(m_KeyFrame.getKeyCount()>0)
            {
                m_KeyFrameCtrl.play(m_lstMotors, m_KeyFrame);
            }
        }

        private void lstBoxKeyFrame_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ListBox lst = (ListBox)sender;
            lstBoxMotorKey.ItemsSource = null;
            if (lst.SelectedIndex!=-1)
            {
                Dictionary<string, MotorData> dic = m_KeyFrame.m_lstKeys[lst.SelectedIndex].m_dicKeys;
                lstBoxMotorKey.ItemsSource = dic;
            }
        }

        private void lstBoxKeyFrame_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBox lst = (ListBox)sender;
            if (lst.SelectedIndex != -1)
            {
                Dictionary<string, MotorData> dic = m_KeyFrame.m_lstKeys[lst.SelectedIndex].m_dicKeys;
                foreach(var item in dic){
                    Motor m = findMotor(item.Key);
                    if(m!=null)
                    {
                        m.setPulseRate(item.Value.Speed);
                        m.move(item.Value.Position);
                    }
                }
            }
        }
    }
}
