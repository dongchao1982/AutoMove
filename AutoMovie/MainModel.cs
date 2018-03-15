using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class MainModel
    {
        MainModel()
        {
            m_TimeLineControl = new TimeLineControl();
        }

        private TimeLineControl m_TimeLineControl = null;
    }
}
