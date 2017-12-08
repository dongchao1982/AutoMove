using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AutoMovie
{
    public class Motor
    {
        //电机名称
        public string Name { get; set; }
        //电机索引
        public int Index { get; set; }
        //电机位置
        public int Position { get; set; }
        //是否可控制
        public bool Enable { get; set; }
        //速度
        public int Speed { get; set; }
        //移动状态
        public bool Moving { get; set; }
        //端口对象
        public SerialPort Port { get; set; }
        //移动类型
        public int MoveType { get; set; }
        //计时器
        private DispatcherTimer m_timer = new DispatcherTimer();

        public Motor()
        {
            MoveType = -1;
            m_timer.Tick += new EventHandler(ControlMotorMove);
            m_timer.Interval = TimeSpan.FromSeconds(0.1);
        }

        public void moveStart(int type)
        {
            if (type >= 1 && type <= 4)
            {
                MoveType = type;
                m_timer.Start();
            }
        }

        public void moveStop()
        {
            m_timer.Stop();
            MoveType = -1;
        }

        void ControlMotorMove(object sender, EventArgs e)
        {
            switch (MoveType)
            {
                case 1:
                    setp(int.MinValue);
                    break;
                case 2:
                    setpMin(int.MinValue);
                    break;
                case 3:
                    setpMin(int.MaxValue);
                    break;
                case 4:
                    setp(int.MaxValue);
                    break;
            }
        }

        public void command(string cmd)
        {
            string value = cmd + "\r\n";
            if (Port != null)
            {
                Port.WriteLine(value);
            }
        }

        public void stop()
        {
            string cmd = "sm " + Index;
            command(cmd);
        }

        public void getPosition()
        {
            string cmd = "mp " + Index;
            command(cmd);
        }

        public void setPulseRate(int value)
        {
            string cmd = "pr " + Index + " " + value;
            command(cmd);
        }

        //清除位置不移动
        public void clearPosition()
        {
            string cmd = "zm " + Index;
            command(cmd);
        }

        public void move(int pos)
        {
            string cmd = "mm " + Index + " " + pos;
            command(cmd);
        }

        public void setp(int pos)
        {
            string cmd = "jm " + Index + " " + pos;
            command(cmd);
        }

        public void setpMin(int pos)
        {
            string cmd = "im " + Index + " " + pos;
            command(cmd);
        }
    }
}
