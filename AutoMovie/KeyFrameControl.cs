using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class KeyFrameControl
    {
        bool m_bPlay;
        int m_iIndexKey;

        List<Motor> m_lstMotors;
        KeyFrame m_KeyFrames;

        int m_TimeCount;
        int m_Time;

        public KeyFrameControl()
        {
            m_bPlay = false;
            m_iIndexKey = 0;
        }

        public bool play(List<Motor> lstMotors,KeyFrame frames)
        {
            m_lstMotors = lstMotors;
            m_KeyFrames = frames;

            m_bPlay = true;
            m_iIndexKey = 0;
            m_TimeCount = 0;
            m_Time = 0;

            return false;
        }

        public void logic(int dt)
        {
            if (m_bPlay == false) return;

            bool bMoving = false;
            foreach (Motor m in m_lstMotors)
            {
                bMoving = bMoving || m.Moving;
            }

            m_TimeCount += dt;
            Console.WriteLine("time : {0},{1}", m_TimeCount,dt);
            if (!bMoving && m_TimeCount>(m_Time/2))
            {
                if (m_iIndexKey < m_KeyFrames.getKeyCount())
                {
                    foreach (Motor m in m_lstMotors)
                    {
                        MotorData k = m_KeyFrames.getKeyFormIndex(m_iIndexKey, m.Name);
                        if (k != null)
                        {
                            float pdt = k.Position - m.Position;
                            float time = pdt / k.Speed * 1000;
                            if(Math.Abs(time) > m_Time)
                            {
                                m_Time = (int)Math.Abs(time);
                            }

                            m.setPulseRate(k.Speed);
                            m.move(k.Position);
                        }
                    }

                    Console.WriteLine("Motor time : {0}", m_Time);
                    m_TimeCount = 0;
                    m_iIndexKey++;
                }
                else
                {
                    m_bPlay = false;
                }
            }
        }
    }
}
