using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class TimeLineKey
    {
        public int Speed { get; set; }
        public int StartPositon { get; set; }
        public int EndPosition { get; set; }

        public int getDistance()
        {
            return EndPosition - StartPositon;
        }

        public int getTime()
        {
            return Math.Abs(getDistance()) / Speed * 1000;
        }
    }
}
