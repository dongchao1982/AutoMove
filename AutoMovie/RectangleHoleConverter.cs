namespace AutoMovie
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;

    /// <summary>
    /// 矩形空洞的转换器
    /// </summary>
    public class RectangleHoleConverter : IMultiValueConverter
    {
        /// <summary>
        /// 转换成矩形空洞
        /// </summary>
        /// <param name="values">
        /// 转换值列表,第一个表示起始宽度,第二个表示起始高度,
        /// 第三个表示总宽度,第四个表示总高度
        /// 第五个表示宿主宽度,第六个表示宿主宽度
        /// </param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 6 || values.HaveNullItem()
                || !values.IsAllInstanceOfType(typeof(double)))
            {
                return DependencyProperty.UnsetValue;
            }

            var maskStartWidth = (double)values[0];
            var maskStartHeight = (double)values[1];
            var maskTotalWidth = (double)values[2];
            var maskTotalHeight = (double)values[3];
            var hostWidth = (double)values[4];
            var hostHeight = (double)values[5];
            if (hostWidth == 0.0 || hostHeight == 0.0)
            {
                return null;
            }

            var maskRectangle = new RectangleGeometry(new Rect(new Size(hostWidth, hostHeight)));
            var maskEllipse = new RectangleGeometry(new Rect(
                new Point(maskStartWidth, maskStartHeight),
                new Size(maskTotalWidth, maskTotalHeight)));
            var combinedGeometry = Geometry.Combine(maskRectangle, maskEllipse, GeometryCombineMode.Exclude, null);

            return combinedGeometry;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { Binding.DoNothing };
        }
    }
}