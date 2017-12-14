using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoMovie
{
    public delegate void SerialPortCommandEventHandler(object sender, string cmd);

    class SerialPortControl
    {
        static int PortFrequency = 57600;

        public SerialPort Port { get; set; }

        public static string[] getPortName()
        {
            return SerialPort.GetPortNames();
        }

        public bool isOK()
        {
            return Port != null && Port.IsOpen;
        }

        public void inquiryState()
        {
            if (isOK())
            {
                Port.WriteLine("ms\r\n");
            }
        }

        public void stopAll()
        {
            if (isOK())
            {
                Port.WriteLine("sa\r\n");
            }
        }

        //初始化串口实例
        public void initial(string portName,SerialPortCommandEventHandler hander)
        {
            try
            {
                m_cmdHandler = hander;

                Port = new SerialPort(portName, PortFrequency);
                Port.Encoding = Encoding.ASCII;
                Port.DataReceived += receivedData;
                Port.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化串口发生错误：" + ex.Message, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                Port = null;
            }
        }

        // 关闭并销毁串口实例
        public void dispose()
        {
            if (Port != null)
            {
                try
                {
                    if (Port.IsOpen)
                    {
                        Port.Close();
                    }
                    Port.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("关闭串口发生错误：" + ex.Message, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                Port = null;
            }
        }

        public string readData()
        {
            string value = "";
            try
            {
                if (Port != null && Port.BytesToRead > 0)
                {
                    value = Port.ReadExisting();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("读取串口数据发生错误：" + ex.Message, "提示信息", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            return value;
        }

        void receivedData(object sender, SerialDataReceivedEventArgs e)
        {
            string value = readData();
            m_strReceivedBuffer += value;

            int iEndPos = m_strReceivedBuffer.IndexOf("\r\n");
            while (iEndPos != -1)
            {
                string cmd = m_strReceivedBuffer.Substring(0, iEndPos + 2);
                m_cmdHandler(this, cmd);

                m_strReceivedBuffer = m_strReceivedBuffer.Substring(iEndPos + 2);
                iEndPos = m_strReceivedBuffer.IndexOf("\r\n");
            }
        }

        private string m_strReceivedBuffer;
        private SerialPortCommandEventHandler m_cmdHandler;
    }
}
