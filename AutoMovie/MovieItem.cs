using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class MovieItem
    {
        //id，唯一标识
        public int ID { get; set; }
        //拍摄位置，在拍摄过程中会有多个机器，用于描述特定的机器位置
        public String Location { get; set; }
        //焦距
        public String FocalDistance { get; set; }
        //运动类型
        public String MoveType { get; set; }
        //时间
        public double Time { get; set; }

        private MovieLens m_first;
        private MovieLens m_second;

        public MovieItem()
        {
            m_first = new MovieLens();
            m_second = new MovieLens();
        }

        public MovieLens getFirst()
        {
            return m_first;
        }

        public MovieLens getSecond()
        {
            return m_second;
        }
    }
}
