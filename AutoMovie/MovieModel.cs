using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    class MovieModel
    {
        //物体长
        public int Lenght { get; set; }
        //物体宽
        public int Width { get; set; }
        //物体高
        public int Height { get; set; }

        public MovieModel()
        {
            m_editItem = new MovieItem();
            m_lstItems = new List<MovieItem>();
        }

        public MovieItem getEditItem()
        {
            return m_editItem;
        }

        public List<MovieItem> getItems()
        {
            return m_lstItems;
        }

        public bool addItem(MovieItem item)
        {
            if (item!=null)
            {
                MovieItem rt = getItem(item.ID);
                if (rt == null)
                {
                    m_lstItems.Add(item);
                    return true;
                }
            }
            return false;
        }

        public MovieItem getItem(int id)
        {
            foreach(MovieItem item in m_lstItems)
            {
                if(item.ID == id)
                {
                    return item;
                }
            }
            return null;
        }

        public MovieItem getItemOfIndex(int index)
        {
            if(index>=0 && index<m_lstItems.Count)
            {
                return m_lstItems[index];
            }
            return null;
        }

        //镜头列表
        private List<MovieItem> m_lstItems;

        //编辑镜头
        private MovieItem m_editItem;
    }
}
