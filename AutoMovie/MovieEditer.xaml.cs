using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace AutoMovie
{
    /// <summary>
    /// MovieEditer.xaml 的交互逻辑
    /// </summary>
    public partial class MovieEditer : Window
    {
        AdornerLayer myAdornerLayer;

        public MovieEditer()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            myAdornerLayer = AdornerLayer.GetAdornerLayer(FCItem);
            myAdornerLayer.Add(new ResizeAdorner(FCItem));
        }
    }
}
