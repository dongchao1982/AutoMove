using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AutoMovie
{
    class MoviePlayControl
    {
        public bool isPlaying { get; set; }
        public int Index { get; set; }
        public TimeLineControl TimeLineCtrl { get; set; }

        public MoviePlayControl()
        {
            m_Timer = new DispatcherTimer();
            m_Timer.Interval = TimeSpan.FromSeconds(0.05);
            m_Timer.Tick += new EventHandler(logicTick);
        }

        public bool play()
        {
            if (isPlaying)
            {
                return false;
            }

            isPlaying = true;
            Index = 0;
            TimeLineCtrl.reset();
            m_Timer.Start();

            return true;
        }

        public void stop()
        {

        }

        void logicTick(object sender, EventArgs e)
        {
            if (TimeLineCtrl.isPlaying == false)
            {
                //构建镜头时间轴数据
                MovieItem item = m_model.getItemOfIndex(Index);


                //开启一个镜头的录制
            }
        }

        private void buildTimeLine(MovieItem item)
        {            
        }

        private MovieModel m_model;
        private DispatcherTimer m_Timer;
    }
}
