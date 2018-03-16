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
            if (m_lstKeys.Count > 0)
            {
                TimeLineKey preKey = m_lstKeys[m_lstKeys.Count - 1];

                int distance = (key.Position - preKey.Position);
                key.Time = (double)Math.Abs(distance) / (double)key.Speed;
            }
            else
            {
                key.Time = 0;
            }
            m_lstKeys.Add(key);
        }

        public void move(int oldIndex,int newIndex)
        {
            if(oldIndex>=0 && oldIndex<m_lstKeys.Count && newIndex >= 0 && newIndex < m_lstKeys.Count)
            {
                TimeLineKey key = m_lstKeys[oldIndex];
                TimeLineKey newKey = new TimeLineKey();
                newKey.Name = key.Name;
                newKey.Position = key.Position;
                newKey.Speed = key.Speed;
                newKey.Time = 0;
                m_lstKeys.Insert(newIndex, newKey);
                m_lstKeys.Remove(key);

                m_lstKeys[0].Time = 0;
                for (int i = 1; i < m_lstKeys.Count; ++i)
                {
                    TimeLineKey preKey = m_lstKeys[i - 1];
                    TimeLineKey curKey = m_lstKeys[i];
                    int distance = (curKey.Position - preKey.Position);
                    curKey.Time = (double)Math.Abs(distance) / (double)curKey.Speed;
                }
            }
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
