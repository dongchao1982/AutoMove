using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AutoMovie
{    
    class TimeLineControl
    {
        public bool isPlaying { get; set; }
        public int Index { get; set; }

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        public void getIniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, m_strIniFilePath);
        }

        public string getIniReadValue(string Section, string Key)
        {
            StringBuilder temp = new StringBuilder(500);
            int i = GetPrivateProfileString(Section, Key, "", temp, 500, m_strIniFilePath);
            return temp.ToString();
        }

        public TimeLineControl()
        {
            isPlaying = false;
            Index = 0;

            m_lstLine = new List<KeyValuePair<Motor, TimeLineModel>>();
            m_Timer = new DispatcherTimer();
            m_Timer.Interval = TimeSpan.FromSeconds(0.05);
            m_Timer.Tick += new EventHandler(logicTick);

            m_strIniFilePath = "timeline.ini";
        }

        public void initFormConfig()
        {
            List<string> lst = new List<string>() { "轨道", "旋转", "俯仰", "调焦1", "调焦2" };
            foreach( string strSection in lst)
            {
                add();
                KeyValuePair<Motor, TimeLineModel> part = last();
                part.Key.Name = getIniReadValue(strSection, "name");
                part.Key.Index = Convert.ToInt32(getIniReadValue(strSection, "index"));
                part.Key.Speed = Convert.ToInt32(getIniReadValue(strSection, "speed"));
            }
        }

        public KeyValuePair<Motor, TimeLineModel> last()
        {
            return m_lstLine[m_lstLine.Count - 1];
        }

        public KeyValuePair<Motor, TimeLineModel> add()
        {
            KeyValuePair<Motor, TimeLineModel> value = new KeyValuePair<Motor, TimeLineModel>(new Motor(), new TimeLineModel());
            m_lstLine.Add(value);
            return value;
        }

        public KeyValuePair<Motor, TimeLineModel> get(int index)
        {
            return m_lstLine[index];
        }

        public int count()
        {
            return m_lstLine.Count;
        }

        public bool play()
        {
            if(isPlaying)
            {
                return false;
            }

            bool bMotorMoving = false;
            foreach (KeyValuePair<Motor,TimeLineModel> key in m_lstLine)
            {
                bMotorMoving = bMotorMoving || key.Key.isMoving;
            }
            if(bMotorMoving)
            {
                return false;
            }

            isPlaying = true;
            Index = 0;
            m_LastTimeTickCount = Environment.TickCount;
            m_KeyMoveTime = 0;
            m_Timer.Start();
            return true;
        }

        public void stop()
        {
            foreach (KeyValuePair<Motor, TimeLineModel> key in m_lstLine)
            {
                if (key.Key.isMoving)
                {
                    key.Key.stop();
                }
            }
            isPlaying = false;
            m_Timer.Stop();
        }

        //分段运动逻辑
        void logicTick(object sender, EventArgs e)
        {
            int dt = Environment.TickCount - m_LastTimeTickCount;

            bool bMotorMoving = false;
            foreach (KeyValuePair<Motor, TimeLineModel> key in m_lstLine)
            {
                bMotorMoving = bMotorMoving || key.Key.isMoving;
            }

            //电机全部停止并运动时间超过预计时间的一半（防止还没移动就切换了）
            if (!bMotorMoving && dt >= (m_KeyMoveTime / 2))
            {
                if (Index < count())
                {
                    foreach (KeyValuePair<Motor, TimeLineModel> key in m_lstLine)
                    {
                        Motor motor = key.Key;
                        TimeLineModel model = key.Value;
                        TimeLineKey data = model.get(Index);
                        if(data != null)
                        {
                            if (data.getTime() > m_KeyMoveTime)
                            {
                                m_KeyMoveTime = data.getTime();
                            }
                            motor.setPulseRate(data.Speed);
                            motor.setPosition(data.EndPosition);
                        }
                    }

                    m_LastTimeTickCount = Environment.TickCount;
                    Index++;
                }
                else
                {
                    isPlaying = false;
                    m_Timer.Stop();
                }
            }
        }

        //数据列表
        private List<KeyValuePair<Motor, TimeLineModel>> m_lstLine;
        //播放计时器
        private DispatcherTimer m_Timer;
        
        private int m_LastTimeTickCount;
        private int m_KeyMoveTime;

        private string m_strIniFilePath;
    }
}
