using Microsoft.Win32;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AutoMovie
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Output { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            m_TimeLineControl = new TimeLineControl();
            m_TimeLineControl.initFormConfig();
            lstboxMotor.DataContext = m_TimeLineControl.getMotors();

            refreshPort();

            m_SerialPortControl = new SerialPortControl();

            m_btnInitial.IsEnabled = !m_SerialPortControl.isOK();
            m_btnDispose.IsEnabled = m_SerialPortControl.isOK();
        }

        private void refreshPort()
        {
            string[] portName = SerialPortControl.getPortName();
            m_comboBox.Items.Clear();
            foreach(string name in portName)
            {
                m_comboBox.Items.Add(name);
            }
            m_comboBox.SelectedIndex = m_comboBox.Items.Count - 1;
        }

        private void portDataReceived(object sender, string cmd)
        {
            string instruction = cmd.Substring(0, 2);
            if (instruction == "mp")
            {
                string param = cmd.Substring(2, cmd.Length - 4).Trim();
                int index = param[0] - '0';
                Motor motor = m_TimeLineControl.findMotor(index);
                if (motor != null)
                {
                    param = param.Substring(1);
                    int position = Convert.ToInt32(param);
                    motor.Position = position;
                    RefreshPosition(motor);
                }
            }
            else if (instruction == "ms")
            {
                string param = cmd.Substring(2, cmd.Length - 4).Trim();
                for (int i = 0; i < 8; ++i)
                {
                    Motor motor = m_TimeLineControl.findMotor(i + 1);
                    if (motor != null)
                    {
                        bool bMoving = param[i] == '1' ? true : false;
                        if (motor.isMoving != bMoving)
                        {
                            Action<Motor, bool> updateAction = new Action<Motor, bool>(delegate (Motor m, bool b)
                            {
                                m.isMoving = b;
                                for (int idx = 0; idx < m_TimeLineControl.count(); ++idx)
                                {
                                    var item = lstboxMotor.ItemContainerGenerator.ContainerFromIndex(idx);
                                    TextBlock txtIndex = WPFHelper.GetChildObject<TextBlock>(item, "txtBlockIndex");
                                    if (txtIndex.Text == m.Index.ToString())
                                    {
                                        Button btn = WPFHelper.GetChildObject<Button>(item, "btnMoving");
                                        btn.IsEnabled = m.isMoving;
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
        }

        public void output(string str)
        {
            Output += str;
            Action<TextBox> updateAction = new Action<TextBox>(delegate (TextBox textBox)
            {
                textBox.Text = Output;
                textBox.ScrollToLine(this.m_output.LineCount - 1);
            });
            this.Dispatcher.BeginInvoke(updateAction, m_output);
        }

        private void RefreshPosition(Motor motor)
        {
            Action<Motor> updateAction = new Action<Motor>(delegate (Motor motor1)
            {
                for(int idx=0;idx<m_TimeLineControl.count();++idx)
                {
                    var item = lstboxMotor.ItemContainerGenerator.ContainerFromIndex(idx);
                    TextBlock txtIndex = WPFHelper.GetChildObject<TextBlock>(item, "txtBlockIndex");
                    if(txtIndex.Text == motor1.Index.ToString())
                    {
                        TextBlock txtPosition = WPFHelper.GetChildObject<TextBlock>(item, "txtBlockPosition");
                        txtPosition.Text = Convert.ToString(motor1.Position);
                    }
                }
            });
            this.Dispatcher.BeginInvoke(updateAction, motor);

            if (m_MotorDlg != null && m_MotorDlg.m_motor.Index == motor.Index)
            {
                m_MotorDlg.setPosition();
            }
        }

        private void NameMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(tb, "itemPanel");
            Motor motor = m_TimeLineControl.findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                MotorDlg dlg = new MotorDlg(motor);
                dlg.Owner = this;
                m_MotorDlg = dlg;
                dlg.ShowDialog();
                m_MotorDlg = null;
            }
        }

        private void ButtonMouseDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "itemPanel");
            Motor motor = m_TimeLineControl.findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                Motor.eMoveType eMoveType = Motor.convertMoveType(btn.Name);
                motor.setpMoveStart(eMoveType);
            }
        }

        private void ButtonMouseUp(object sender, MouseButtonEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "itemPanel");
            Motor motor = m_TimeLineControl.findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                motor.setpMoveStop();
            }
        }

        private void MotorHomeClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "itemPanel");
            Motor motor = m_TimeLineControl.findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                motor.setPosition(0);
            }
        }

        private void MotorToggleClick(object sender, RoutedEventArgs e)
        {
            ToggleButton btn = (ToggleButton)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "itemPanel");
            Motor motor = m_TimeLineControl.findMotor((string)itemPanel.Tag);
            if (motor != null)
            {
                lstboxMotor.DataContext = null;
                lstboxMotor.DataContext = m_TimeLineControl.getMotors();
            }
        }

        private void MotorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Motor motor = (Motor)lstboxMotor.SelectedItem;
            if (motor != null)
            {
                TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                lstBoxMotorKey.ItemsSource = null;
                lstBoxMotorKey.ItemsSource = model.gets();
            }
        }

        private void RefreshPortClick(object sender, RoutedEventArgs e)
        {
            refreshPort();
        }

        private void InitialSerialPortClick(object sender, RoutedEventArgs e)
        {
            if(m_comboBox.SelectedItem==null)
            {
                return;
            }

            string portName = m_comboBox.SelectedItem.ToString();
            m_SerialPortControl.initial(portName, portDataReceived);
            m_TimeLineControl.initSerialPort(m_SerialPortControl);

            if(m_SerialPortControl.isOK())
            {
                output("hello auto movie.\r\n");
            }

            //update ui
            m_btnInitial.IsEnabled = !m_SerialPortControl.isOK();
            m_btnDispose.IsEnabled = m_SerialPortControl.isOK();
        }

        private void DisposeSerialPortClick(object sender, RoutedEventArgs e)
        {
            output("bye.\r\n");
            m_SerialPortControl.dispose();
            m_TimeLineControl.initSerialPort(m_SerialPortControl);

            //update ui
            m_btnInitial.IsEnabled = !m_SerialPortControl.isOK();
            m_btnDispose.IsEnabled = m_SerialPortControl.isOK();
        }

        private void StopAllMotorClick(object sender, RoutedEventArgs e)
        {
            m_SerialPortControl.stopAll();
            output("stop all motor.\r\n");
        }

        private void ClearClick(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(btn, "itemPanel");
            Motor motor = m_TimeLineControl.findMotor((string)itemPanel.Tag);
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

        private void updateUI()
        {
            Motor m = (Motor)lstboxMotor.SelectedItem;
            if (m != null)
            {
                TimeLineModel md = m_TimeLineControl.getTimeLineModel(m);
                lstBoxMotorKey.ItemsSource = null;
                lstBoxMotorKey.ItemsSource = md.gets();
            }
        }

        private void AddKeyClick(object sender, RoutedEventArgs e)
        {
            foreach(Motor motor in m_TimeLineControl.getMotors())
            {
                TimeLineKey key = new TimeLineKey();
                key.Speed = motor.Speed;
                key.StartPositon = 0;
                key.EndPosition = motor.Position;

                TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                model.add(key);
            }

            updateUI();
        }

        private void DelKeyClick(object sender, RoutedEventArgs e)
        {
            if(lstBoxMotorKey.SelectedIndex!=-1)
            {
                int idx = lstBoxMotorKey.SelectedIndex;
                foreach (Motor motor in m_TimeLineControl.getMotors())
                {
                    TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                    model.del(idx);
                }

                updateUI();
            }
        }

        private void UpdateKeyClick(object sender, RoutedEventArgs e)
        {
            if(lstBoxMotorKey.SelectedIndex!=-1)
            {
                int idx = lstBoxMotorKey.SelectedIndex;
                foreach (Motor motor in m_TimeLineControl.getMotors())
                {
                    TimeLineKey key = new TimeLineKey();
                    key.Speed = motor.Speed;
                    key.StartPositon = 0;
                    key.EndPosition = motor.Position;

                    TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                    model.update(idx,key);
                }

                updateUI();
            }
        }

        private void ClearKeyClick(object sender, RoutedEventArgs e)
        {
            foreach (Motor motor in m_TimeLineControl.getMotors())
            {
                TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                model.clear();
            }

            updateUI();
        }

        private void GoHomeClick(object sender, RoutedEventArgs e)
        {
            foreach (Motor motor in m_TimeLineControl.getMotors())
            {
                TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);

                if(model.count()>0)
                {
                    TimeLineKey key = model.get(0);
                    motor.setPulseRate(key.Speed);
                    motor.setPosition(key.EndPosition);
                }               
            }
        }

        private void PlayClick(object sender, RoutedEventArgs e)
        {
            m_TimeLineControl.play();
        }

        private void SaveFileClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = @"C:\";
            dlg.Filter = "移动脚本|*.amf";
            if (dlg.ShowDialog() == true)
            {
                m_TimeLineControl.saveFile(dlg.FileName);
            }
        }

        private void ReadFileClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = @"C:\";
            dlg.Filter = "移动脚本|*.amf";
            if (dlg.ShowDialog() == true)
            {
                m_TimeLineControl.readFile(dlg.FileName);

                updateUI();
            }
        }

        private SerialPortControl m_SerialPortControl = null;
        private TimeLineControl m_TimeLineControl = null;
        private MotorDlg m_MotorDlg = null;
    }
}
