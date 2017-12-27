using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    public partial class TestBindData : INotifyPropertyChanged
    {
        int _pos;
        int _pitch;
        int _yaw;

        public TestBindData()
        {
            _pos = 0;
            _pitch = 0;
            _yaw = 0;
        }

        public int Position
        {
            set
            {
                _pos = value;
                OnPropertyChanged("Position");
            }
            get
            {
                return _pos;
            }
        }

        public int Pitch
        {
            set
            {
                _pitch = value;
                OnPropertyChanged("Pitch");
            }
            get
            {
                return _pitch;
            }
        }

        public int Yaw
        {
            set
            {
                _yaw = value;
                OnPropertyChanged("Yaw");
            }
            get
            {
                return _yaw;
            }
        }

        // Declare event
        public event PropertyChangedEventHandler PropertyChanged;
        // OnPropertyChanged method to update property value in binding
        private void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }
    }
}
