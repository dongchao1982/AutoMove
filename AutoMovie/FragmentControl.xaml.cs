using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoMovie
{
    /// <summary>
    /// FragmentControl.xaml 的交互逻辑
    /// </summary>
    public class FragmentItem
    {
        public BitmapImage Image { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }

    public partial class FragmentControl : UserControl
    {
        private ObservableCollection<FragmentItem> _FragmentList;
        public ObservableCollection<FragmentItem> FragmentList
        {
            get { return _FragmentList; }
        }

        public FragmentControl()
        {
            InitializeComponent();

            _FragmentList = new ObservableCollection<FragmentItem>();
            FragmentItem item = new FragmentItem();
            item.Image = new BitmapImage(new Uri(@"Image/image.jpg", UriKind.Relative));
            item.Width = 120;
            item.Height = 80;
            _FragmentList.Add(item);
            item = new FragmentItem();
            item.Image = new BitmapImage(new Uri(@"Image/image.jpg", UriKind.Relative));
            item.Width = 80;
            item.Height = 80;
            _FragmentList.Add(item);

            this.DataContext = this;
        }
    }
}
