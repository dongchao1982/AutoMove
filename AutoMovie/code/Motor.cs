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
        //移动类型枚举
        public enum eMoveType
        {
            Null        = 0,
            Left        = 1, //左移动
            Right       = 4, //右移动
            LeftMin     = 2, //左移动（以最小的速度）
            RightMin    = 3, //右移动（以最小的速度）
        }

        /*控制变量*/
        //可控制
        public bool isEnable { get; set; }
        //反转电机方向
        public bool isReverse { get; set; }
        //移动状态
        public bool isMoving { get; set; }

        /*属性*/
        //名称
        public string Name { get; set; }
        //索引
        public int Index { get; set; }
        //位置
        public int Position { get; set; }
        //速度
        public int Speed { get; set; }
        //时间
        public float Time { get; }

        //端口对象
        public SerialPort Port { get; set; }

        public Motor()
        {
            m_Timer = new DispatcherTimer();
            m_Timer.Interval = TimeSpan.FromSeconds(0.1);
            m_EventHandler = null;
        }

        //步进移动启动
        public void setpMoveStart(eMoveType type)
        {
            if (m_EventHandler != null)
            {
                m_Timer.Tick -= m_EventHandler;
                m_EventHandler = null;
            }

            switch(type)
            {
                case eMoveType.Left:
                    m_EventHandler = isReverse? new EventHandler(moveRightTick) : new EventHandler(moveLeftTick);
                    break;
                case eMoveType.Right:
                    m_EventHandler = isReverse ? new EventHandler(moveLeftTick) : new EventHandler(moveRightTick);
                    break;
                case eMoveType.LeftMin:
                    m_EventHandler = isReverse ? new EventHandler(moveRightMinTick) : new EventHandler(moveLeftMinTick);
                    break;
                case eMoveType.RightMin:
                    m_EventHandler = isReverse ? new EventHandler(moveLeftMinTick) : new EventHandler(moveRightMinTick);
                    break;
            }
            if (m_EventHandler != null)
            {
                m_Timer.Tick += m_EventHandler;
            }
        }

        //步进移动停止
        public void setpMoveStop()
        {
            stop();
            m_Timer.Stop();
        }

        //停止移动
        public void stop()
        {
            string cmd = "sm " + Index;
            command(cmd);
        }

        //设置速度
        public void setPulseRate(int value)
        {
            string cmd = "pr " + Index + " " + value;
            command(cmd);
        }

        //获取位置
        public void getPosition()
        {
            string cmd = "mp " + Index;
            command(cmd);
        }

        //设置位置
        public void setPosition(int pos)
        {
            string cmd = "mm " + Index + " " + pos;
            command(cmd);
        }

        //清除位置，但不移动电机
        public void clearPosition()
        {
            string cmd = "zm " + Index;
            command(cmd);
        }

        //向电机控制设备发送指令
        public void command(string cmd)
        {
            string value = cmd + "\r\n";
            if (Port != null && Port.IsOpen)
            {
                Port.WriteLine(value);
            }
        }

        //移动类型转换函数
        static public eMoveType convertMoveType(string name)
        {
            eMoveType type = eMoveType.Null;
            if (name == "LeftMove")
            {
                type = eMoveType.Left;
            }
            else if (name == "RightMove")
            {
                type = eMoveType.Right;
            }
            else if (name == "LeftMoveMin")
            {
                type = eMoveType.LeftMin;
            }
            else if (name == "RightMoveMin")
            {
                type = eMoveType.RightMin;
            }
            return type;
        }

        //移动事件处理函数
        private void moveLeftTick(object sender, EventArgs e)
        {
            string cmd = "jm " + Index + " " + int.MinValue;
            command(cmd);
        }

        //移动事件处理函数
        private void moveRightTick(object sender, EventArgs e)
        {
            string cmd = "jm " + Index + " " + int.MaxValue;
            command(cmd);
        }

        //移动事件处理函数
        private void moveLeftMinTick(object sender, EventArgs e)
        {
            string cmd = "im " + Index + " " + int.MinValue;
            command(cmd);
        }

        //移动事件处理函数
        private void moveRightMinTick(object sender, EventArgs e)
        {
            string cmd = "im " + Index + " " + int.MaxValue;
            command(cmd);
        }

        //控制电机移动计时器
        private DispatcherTimer m_Timer;
        //控制电机移动的事件处理函数
        private EventHandler m_EventHandler;
    }
}
