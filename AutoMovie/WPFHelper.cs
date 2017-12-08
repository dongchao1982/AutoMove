using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AutoMovie
{
    public static class WPFHelper
    {
        //获取父控件
        //调用方法：Grid layoutGrid = VTHelper.GetParentObject<Grid>(this.spDemoPanel, "LayoutRoot");
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }
                parent = VisualTreeHelper.GetParent(parent);
            }
            return null;
         }

        //获取子控件
        //调用方法：StackPanel sp = VTHelper.GetChildObject<StackPanel>(this.LayoutRoot, "spDemoPanel");
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        //获取所有子控件
        //调用方法：List<TextBlock> textblock = VTHelper.GetChildObjects<TextBlock>(this.LayoutRoot, "");
        //在表格中的控件：List<TextBox> listTextBox = GetChildObjects<TextBox>(radGridView_File.ItemContainerGenerator.ContainerFromItem(radGridView_File.SelectedItems[0]), "");
        public static List<T> GetChildObjects<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            List<T> childList = new List<T>();
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T)child).Name == name || string.IsNullOrEmpty(name)))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(GetChildObjects<T>(child, ""));
            }
            return childList;
        }
    } 
}
