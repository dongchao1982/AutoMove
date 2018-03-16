﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

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

            this.DataContext = new MainModel();
        }

        public MainModel Model()
        {
            return (MainModel)this.DataContext;
        }

        private void refreshPort()
        {
            string[] portName = SerialPortControl.getPortName();
            comboBoxPort.Items.Clear();
            foreach(string name in portName)
            {
                comboBoxPort.Items.Add(name);
            }
            comboBoxPort.SelectedIndex = comboBoxPort.Items.Count - 1;
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

        private void RefreshPortClick(object sender, RoutedEventArgs e)
        {
            refreshPort();
        }

        private void InitialSerialPortClick(object sender, RoutedEventArgs e)
        {
            if(comboBoxPort.SelectedItem==null)
            {
                return;
            }

            string portName = comboBoxPort.SelectedItem.ToString();
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
            m_TimeLineControl.stop();
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
            int index = lstvKeyName.SelectedIndex;
            Model().lstKeyData.Clear();
            if (index != -1)
            {
                foreach (Motor motor in m_TimeLineControl.getMotors())
                {
                    TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                    Model().lstKeyData.Add(model.gets()[index]);
                }
            }
        }

        private void AddKeyClick(object sender, RoutedEventArgs e)
        {
            foreach(Motor motor in m_TimeLineControl.getMotors())
            {
                TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);

                TimeLineKey key = new TimeLineKey();
                key.Name = motor.Name;
                key.Speed = motor.Speed;
                key.Position = motor.Position;
                
                model.add(key);
            }

            string keyName = "帧" + m_KeyCount++;
            Model().lstKeyName.Add(keyName);

            updateUI();
        }

        private void DelKeyClick(object sender, RoutedEventArgs e)
        {
            int idx = lstvKeyName.SelectedIndex;
            if (idx != -1)
            {
                MessageBoxResult confirmToDel = MessageBox.Show("确认要删除关键帧？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (confirmToDel == MessageBoxResult.Yes)
                {
                    foreach (Motor motor in m_TimeLineControl.getMotors())
                    {
                        TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                        model.del(idx);
                    }

                    Model().lstKeyName.RemoveAt(idx);

                    updateUI();
                }   
            }
        }

        private void UpdateKeyClick(object sender, RoutedEventArgs e)
        {
            if(lstvKeyName.SelectedIndex!=-1)
            {
                int idx = lstvKeyName.SelectedIndex;
                foreach (Motor motor in m_TimeLineControl.getMotors())
                {
                    TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                    model.update(idx,motor.Position,motor.Speed);
                }
                updateUI();
            }
        }

        private void ClearKeyClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult confirmToDel = MessageBox.Show("确认要清空关键帧？", "提示", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (confirmToDel == MessageBoxResult.Yes)
            {
                foreach (Motor motor in m_TimeLineControl.getMotors())
                {
                    TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                    model.clear();
                }

                Model().lstKeyName.Clear();
                m_KeyCount = 1;

                updateUI();
            }
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
                    motor.setPosition(key.Position);
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

                m_KeyCount = 1;
                Model().lstKeyName.Clear();
                for(int i=0;i<m_TimeLineControl.getModels()[0].count();++i)
                {
                    Model().lstKeyName.Add("帧" + m_KeyCount++);
                }

                updateUI();
            }
        }

        private SerialPortControl m_SerialPortControl = null;
        private TimeLineControl m_TimeLineControl = null;
        private MotorDlg m_MotorDlg = null;
        private int m_KeyCount = 1;

        private void ButtonClipClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dlg.Filter = "视频文件|*.mov";
            if (dlg.ShowDialog() == true)
            {
                OpenFileDialog dlg2 = new OpenFileDialog();
                dlg2.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                dlg2.Filter = "配置文件|*.txt";
                if(dlg2.ShowDialog() == true)
                {
                    String movFilename = dlg.FileName;
                    String cfgFilename = dlg2.FileName;

                    Cmd cmd = new Cmd();
                    String strcmd;

                    FileStream file = new FileStream("FileList.txt", FileMode.OpenOrCreate);
                    using (StreamWriter sw = new StreamWriter(file))
                    {
                        StreamReader sr = new StreamReader(cfgFilename, Encoding.UTF8);
                        String line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            String strFile = line.Trim();

                            line = sr.ReadLine();
                            String strBegin = line.Trim();

                            line = sr.ReadLine();
                            String strEnd = line.Trim();

                            strcmd = "ffmpeg -y -i " + movFilename + " -ss " + strBegin + " -to " + strEnd + " -movflags +faststart -c copy " + strFile;
                            cmd.RunCmd(strcmd);

                            sw.Write("file '");
                            sw.Write(AppDomain.CurrentDomain.BaseDirectory+strFile);
                            sw.WriteLine("'");
                            sw.Flush();
                        }
                    }
                    file.Close();

                    strcmd = "ffmpeg -y -f concat -safe 0 -i FileList.txt -movflags +faststart -c copy " + System.IO.Path.GetFileName(dlg.FileName);
                    Console.WriteLine(strcmd);
                    cmd.RunCmd(strcmd);
                }
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MovieEditer me = new MovieEditer();
            me.ShowDialog();
        }

        private void lstvKeyName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            updateUI();
        }

        private void lstvKeyName_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(lstvKeyName.SelectedIndex!=-1)
            {
                foreach (Motor motor in m_TimeLineControl.getMotors())
                {
                    TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                    TimeLineKey key = model.get(lstvKeyName.SelectedIndex);
                    motor.setPulseRate(key.Speed);
                    motor.setPosition(key.Position);
                }
            }
        }

        private void SpeedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox text = (TextBox)sender;
            StackPanel itemPanel = WPFHelper.GetParentObject<StackPanel>(text, "itemPanel");
            Motor motor = m_TimeLineControl.findMotor((string)itemPanel.Tag);
            if (text.Text.Length > 0)
            {
                int Speed = Convert.ToInt32(text.Text);
                motor.setPulseRate(Speed);
            }
            else
            {
                motor.setPulseRate(0);
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

        delegate Point GetPositionDelegate(IInputElement element);

        ListViewItem GetListViewItem(int index)
        {
            if (lstvKeyName.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return lstvKeyName.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }

        private bool IsMouseOverTarget(Visual target, GetPositionDelegate getPosition)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            Point mousePos = getPosition((IInputElement)target);
            return bounds.Contains(mousePos);
        }

        private int GetCurrentIndex(GetPositionDelegate getPosition)
        {
            int index = -1;
            for (int i = 0; i < lstvKeyName.Items.Count; ++i)
            {
                ListViewItem item = GetListViewItem(i);
                if (item != null && IsMouseOverTarget(item, getPosition))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void lstvKeyName_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(System.Collections.IList)))
            {
                System.Collections.IList peopleList = e.Data.GetData(typeof(System.Collections.IList)) as System.Collections.IList;
                //index为放置时鼠标下元素项的索引
                int index = GetCurrentIndex(new GetPositionDelegate(e.GetPosition));
                if (index > -1 && peopleList.Count > 0)
                {
                    string item = peopleList[0] as string;
                    //拖动元素集合的第一个元素索引
                    int OldFirstIndex = Model().lstKeyName.IndexOf(item);
                    if (OldFirstIndex != index)
                    {
                        //交换数据
                        Model().lstKeyName.Move(OldFirstIndex, index);
                        foreach (Motor motor in m_TimeLineControl.getMotors())
                        {
                            TimeLineModel model = m_TimeLineControl.getTimeLineModel(motor);
                            model.move(OldFirstIndex, index);
                        }
                        updateUI();
                    }
                }
            }
        }

        private void lstvKeyName_MouseMove(object sender, MouseEventArgs e)
        {
            ListView listview = sender as ListView;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Collections.IList list = listview.SelectedItems as System.Collections.IList;
                DataObject data = new DataObject(typeof(System.Collections.IList), list);
                if (list.Count > 0)
                {
                    DragDrop.DoDragDrop(listview, data, DragDropEffects.Move);
                }
            }
        }
    }
}
