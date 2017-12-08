using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class MotorData
    {
        public string Name { get; set; }
        public int Speed { get; set; }
        public int Position { get; set; }
    }

    class MotorKey
    {
        public string Name { get; set; }

        public Dictionary<string, MotorData> m_dicKeys;

        public MotorKey()
        {
            m_dicKeys = new Dictionary<string, MotorData>();
        }
    }

    class KeyFrame
    {
        static int m_keyCount = 1;

        public List<MotorKey> m_lstKeys;

        public KeyFrame()
        {
            m_lstKeys = new List<MotorKey>();
        }

        public void clear()
        {
            m_lstKeys.Clear();
        }

        public void addKey()
        {
            MotorKey key = new MotorKey();
            key.Name = "key " + m_keyCount++;
            m_lstKeys.Add(key);
        }

        public void pushKeyAtLast(MotorData key)
        {
            Dictionary<string, MotorData> dic = m_lstKeys[m_lstKeys.Count - 1].m_dicKeys;
            dic[key.Name] = key;
        }

        public MotorData getKeyFormIndex(int index,string name)
        {
            if(index>=0 && index<m_lstKeys.Count)
            {
                Dictionary<string, MotorData> dic = m_lstKeys[index].m_dicKeys;
                if(dic.ContainsKey(name))
                {
                    return dic[name];
                }
            }
            return null;
        }

        public int getKeyCount()
        {
            return m_lstKeys.Count;
        }
    }
}
