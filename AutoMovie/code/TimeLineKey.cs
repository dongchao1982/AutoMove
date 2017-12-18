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

        public int Distance
        {
            get
            {
                return EndPosition - StartPositon;
            }
        }

        public int Time
        {
            get
            {
                return (int)(((double)Math.Abs(Distance) / (double)Speed * 1000.0));
            }
        }
    }
}
