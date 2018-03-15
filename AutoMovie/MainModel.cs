using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    public class MainModel
    {
        public MainModel()
        {
            mKeyName = new ObservableCollection<string>();
            mKeyData = new ObservableCollection<TimeLineKey>();
        }

        private ObservableCollection<string> mKeyName;
        public ObservableCollection<string> lstKeyName
        {
            get { return mKeyName; }
        }

        private ObservableCollection<TimeLineKey> mKeyData;
        public ObservableCollection<TimeLineKey> lstKeyData
        {
            get { return mKeyData; }
        }
    }
}
