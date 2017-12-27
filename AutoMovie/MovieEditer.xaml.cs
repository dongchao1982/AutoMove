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
        TestBindData m_Ctrl = new TestBindData();

        public TestBindData CTRL
        {
            get
            {
                return m_Ctrl;
            }
            set
            {
                m_Ctrl = value;
            }
        }

        public MovieEditer()
        {
            InitializeComponent();

            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CTRL.Position -= 1;
            CTRL.Yaw += 1;
            CTRL.Pitch += 1;
        }
    }
}
