using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AutoMovie
{    
    class TimeLineControl
    {
        public bool isPlaying { get; set; }
        public int Index { get; set; }

        public TimeLineControl()
        {
            isPlaying = false;
            Index = 0;

            m_lstNames = new List<string>() { "轨道", "旋转", "俯仰", "调焦1", "调焦2" };
            INI.IniFilePath = AppDomain.CurrentDomain.BaseDirectory + "timeline.ini";
            m_dicLine = new Dictionary<Motor, TimeLineModel>();

            m_Timer = new DispatcherTimer();
            m_Timer.Interval = TimeSpan.FromSeconds(0.05);
            m_Timer.Tick += new EventHandler(logicTick);
            m_Timer.Start();
        }

        public void initFormConfig()
        {
            foreach( string strSection in m_lstNames)
            {
                Motor motor = new Motor();
                motor.Name = INI.ReadIni(strSection, "name");
                if(motor.Name.Length>0)
                {
                    motor.Alias = strSection;
                    try
                    {
                        motor.Index = Convert.ToInt32(INI.ReadIni(strSection, "index"));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }

                    try
                    {
                        motor.Speed = Convert.ToInt32(INI.ReadIni(strSection, "speed"));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e);
                        continue;
                    }

                    TimeLineModel model = new TimeLineModel();
                    m_dicLine[motor] = model;
                }
            }
        }

        public void initSerialPort(SerialPortControl ctrl)
        {
            foreach (Motor m in m_dicLine.Keys)
            {
                m.Port = ctrl.Port;
                m.stop();
                m.getPosition();
                m.setPulseRate(m.Speed);
            }
            m_SerialPortControl = ctrl;
        }

        public Motor findMotor(String name)
        {
            foreach(Motor m in m_dicLine.Keys)
            {
                if (m.Name == name)
                {
                    return m;
                }
            }
            return null;
        }

        public Motor findMotor(int idx)
        {
            foreach (Motor m in m_dicLine.Keys)
            {
                if (m.Index == idx)
                {
                    return m;
                }
            }
            return null;
        }

        public Motor findMotorOfAlias(String alias)
        {
            foreach (Motor m in m_dicLine.Keys)
            {
                if (m.Alias == alias)
                {
                    return m;
                }
            }
            return null;
        }

        public TimeLineModel getTimeLineModel(Motor motor)
        {
            return m_dicLine[motor];
        }

        public List<Motor> getMotors()
        {
            return m_dicLine.Keys.ToList();
        }

        public List<TimeLineModel> getModels()
        {
            return m_dicLine.Values.ToList();
        }
        
        public int count()
        {
            return m_dicLine.Count;
        }

        public void clear()
        {
            foreach(TimeLineModel model in m_dicLine.Values)
            {
                model.clear();
            }
        }

        public bool play()
        {
            if(isPlaying)
            {
                return false;
            }

            bool bMotorMoving = false;
            foreach (Motor m in m_dicLine.Keys)
            {
                bMotorMoving = bMotorMoving || m.isMoving;
            }
            if(bMotorMoving)
            {
                return false;
            }

            isPlaying = true;
            Index = 0;
            m_LastTimeTickCount = Environment.TickCount;
            m_KeyMoveTime = 0;
            return true;
        }

        public void stop()
        {
            foreach (Motor m in m_dicLine.Keys)
            {
                if (m.isMoving)
                {
                    m.stop();
                }
            }
            isPlaying = false;
        }

        public void reset()
        {

        }

        //分段运动逻辑
        void logicTick(object sender, EventArgs e)
        {
            if(m_SerialPortControl!=null && m_SerialPortControl.isOK())
            {
                m_SerialPortControl.inquiryState();
            }

            if(isPlaying)
            {
                int dt = Environment.TickCount - m_LastTimeTickCount;

                bool bMotorMoving = false;
                foreach (Motor m in m_dicLine.Keys)
                {
                    bMotorMoving = bMotorMoving || m.isMoving;
                }

                //电机全部停止并运动时间超过预计时间的一半（防止还没移动就切换了）
                if (!bMotorMoving && dt >= (m_KeyMoveTime / 2))
                {
                    foreach (Motor motor in m_dicLine.Keys)
                    {
                        TimeLineModel model = m_dicLine[motor];
                        if (Index < model.count())
                        {
                            TimeLineKey data = model.get(Index);
                            if (data != null)
                            {
                                if (data.Time > m_KeyMoveTime)
                                {
                                    m_KeyMoveTime = data.Time;
                                }
                                motor.setPulseRate(data.Speed);
                                motor.setPosition(data.EndPosition);
                            }
                        }
                        else
                        {
                            isPlaying = false;
                        }
                    }

                    m_LastTimeTickCount = Environment.TickCount;
                    Index++;
                }
            }
        }

        public void saveFile(String filename)
        {
            FileStream file = new FileStream(filename, FileMode.OpenOrCreate);
            using (StreamWriter sw = new StreamWriter(file))
            {
                int line = 0;
                foreach (Motor motor in m_dicLine.Keys)
                {
                    line++;
                    sw.WriteLine("#" + line);
                    TimeLineModel model = getTimeLineModel(motor);
                    foreach (TimeLineKey key in model.gets())
                    {
                        sw.WriteLine(motor.Name);
                        sw.WriteLine(key.Speed);
                        sw.WriteLine(key.EndPosition);
                    }
                }
                sw.Flush();
            }
            file.Close();
        }

        public void readFile(String filename)
        {
            if (isPlaying)
            {
                return;
            }

            clear();

            StreamReader sr = new StreamReader(filename, Encoding.UTF8);
            String line;
            String alias;
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Length > 0)
                {
                    if (line[0] == '#')
                    {
                        //不处理
                    }
                    else
                    {
                        alias = line;
                        Motor motor = findMotorOfAlias(alias);
                        if(motor!=null)
                        {
                            TimeLineKey key = new TimeLineKey();
                            try
                            {
                                line = sr.ReadLine();
                                key.Speed = Convert.ToInt32(line);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e);
                                key.Speed = 10000;
                            }

                            try
                            {
                                line = sr.ReadLine();
                                key.EndPosition = Convert.ToInt32(line);
                            }
                            catch(Exception e)
                            {
                                Console.WriteLine(e);
                                key.EndPosition = 0;
                            }

                            TimeLineModel model = getTimeLineModel(motor);
                            model.add(key);
                        }
                    }
                }
            }
        }

        //电机名字列表
        private List<string> m_lstNames;
        //数据列表
        private Dictionary<Motor, TimeLineModel> m_dicLine;
        //播放计时器
        private DispatcherTimer m_Timer;
        private int m_LastTimeTickCount;
        private int m_KeyMoveTime;

        SerialPortControl m_SerialPortControl;
    }
}
