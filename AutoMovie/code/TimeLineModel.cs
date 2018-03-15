using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class TimeLineModel
    {
        public TimeLineModel()
        {
            m_lstKeys = new List<TimeLineKey>();
        }

        public void clear()
        {
            m_lstKeys.Clear();
        }

        public void add(TimeLineKey key)
        {
            m_lstKeys.Add(key);
        }

        public void del(int index)
        {
            if(index>=0 && index<m_lstKeys.Count)
            {
                m_lstKeys.RemoveAt(index);
            }
        }

        public TimeLineKey get(int index)
        {
            if (index >= 0 && index < m_lstKeys.Count)
            {
                return m_lstKeys[index];
            }
            return null;
        }

        public void update(int index, TimeLineKey key)
        {
            if(index >=0 && index<m_lstKeys.Count)
            {
                m_lstKeys[index] = key;
            }
        }

        public int count()
        {
            return m_lstKeys.Count;
        }

        public List<TimeLineKey> gets()
        {
            return m_lstKeys;
        }

        private List<TimeLineKey> m_lstKeys;
    }
}
