using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AutoMovie
{
    public class ResizeAdorner : Adorner
    {
        const double THUMB_SIZE     = 10;
        const double MINIMAL_SIZE   = 10;
        const double MOVE_OFFSET    = 20;

        Thumb moveAndRotateThumb;
        Thumb middleLeftThumb;
        Thumb middleRightThumb;

        Rectangle thumbRectangle;

        VisualCollection visualCollection;

        public ResizeAdorner(UIElement adorned) : base(adorned)
        {
            visualCollection = new VisualCollection(this);

            //框住控件的矩形区域
            thumbRectangle = new Rectangle()
            {
                Width = AdornedElement.RenderSize.Width,
                Height = AdornedElement.RenderSize.Height,
                Fill = Brushes.Transparent,
                Stroke = Brushes.Red,
                StrokeThickness = (double)1
            };
            visualCollection.Add(thumbRectangle);

            //创建控制句柄
            middleLeftThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Left, VerticalAlignment.Center);
            visualCollection.Add(middleLeftThumb);
            middleRightThumb = GetResizeThumb(Cursors.SizeWE, HorizontalAlignment.Right, VerticalAlignment.Center);
            visualCollection.Add(middleRightThumb);

            visualCollection.Add(moveAndRotateThumb = GetMoveAndRotateThumb());
        }

        private Thumb GetResizeThumb(Cursor cur, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            var thumb = new Thumb()
            {
                Width = THUMB_SIZE,
                Height = THUMB_SIZE,
                HorizontalAlignment = horizontal,
                VerticalAlignment = vertical,
                Cursor = cur,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetThumbTemple(new SolidColorBrush(Colors.White))
                }
            };
            thumb.DragDelta += (s, e) =>
            {
                var element = AdornedElement as FrameworkElement;
                if (element == null)
                    return;
                this.ElementResize(element);

                switch (thumb.HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        if (element.Width - e.HorizontalChange > MINIMAL_SIZE)
                        {
                            element.Width -= e.HorizontalChange;
                            thumbRectangle.Width -= e.HorizontalChange;
                            Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                        }
                        break;
                    case HorizontalAlignment.Right:
                        if (element.Width + e.HorizontalChange > MINIMAL_SIZE)
                        {
                            element.Width += e.HorizontalChange;
                            thumbRectangle.Width += e.HorizontalChange;
                        }
                        break;
                }
                e.Handled = true;
            };
            return thumb;
        }

        private void ElementResize(FrameworkElement frameworkElement)
        {
            if (Double.IsNaN(frameworkElement.Width))
                frameworkElement.Width = frameworkElement.RenderSize.Width;
            if (Double.IsNaN(frameworkElement.Height))
                frameworkElement.Height = frameworkElement.RenderSize.Height;
        }

        // get Thumb Temple
        private FrameworkElementFactory GetThumbTemple(Brush back)
        {
            back.Opacity = 1;
            var fef = new FrameworkElementFactory(typeof(Ellipse));
            fef.SetValue(Ellipse.FillProperty, back);
            fef.SetValue(Ellipse.StrokeProperty, Brushes.Green);
            fef.SetValue(Ellipse.StrokeThicknessProperty, (double)1);
            return fef;
        }

        private Thumb GetMoveAndRotateThumb()
        {
            var thumb = new Thumb()
            {
                Width = THUMB_SIZE,
                Height = THUMB_SIZE,
                //Cursor = wpfDecorator.CursorHelper.CreateCursor(@"..\..\wpfAdorner\旋转.png", 8, 8),
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetThumbTemple(GetMoveEllipseBack())
                }
            };
            thumb.DragDelta += (s, e) =>
            {
                var element = AdornedElement as FrameworkElement;
                if (element == null)
                    return;

                Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
            };
            return thumb;
        }

        private Brush GetMoveEllipseBack()
        {
            string lan = "M841.142857 570.514286c0 168.228571-153.6 336.457143-329.142857 336.457143s-329.142857-153.6-329.142857-336.457143c0-182.857143 153.6-336.457143 329.142857-336.457143v117.028571l277.942857-168.228571L512 0v117.028571c-241.371429 0-438.857143 197.485714-438.857143 453.485715S270.628571 1024 512 1024s438.857143-168.228571 438.857143-453.485714h-109.714286z m0 0";
            var converter = TypeDescriptor.GetConverter(typeof(Geometry));
            var geometry = (Geometry)converter.ConvertFrom(lan);
            TileBrush bsh = new DrawingBrush(new GeometryDrawing(Brushes.Transparent, new Pen(Brushes.Black, 2), geometry));
            bsh.Stretch = Stretch.Fill;
            return bsh;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double offset = (THUMB_SIZE / 2);
            Size sz = new Size(THUMB_SIZE, THUMB_SIZE);

            middleLeftThumb.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));
            middleRightThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height / 2 - THUMB_SIZE / 2), sz));

            moveAndRotateThumb.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width / 2 - THUMB_SIZE / 2, -MOVE_OFFSET), sz));

            thumbRectangle.Arrange(new Rect(new Point(-offset, -offset), new Size(Width = AdornedElement.RenderSize.Width + THUMB_SIZE, Height = AdornedElement.RenderSize.Height + THUMB_SIZE)));

            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        protected override int VisualChildrenCount
        {
            get { return visualCollection.Count; }
        }
    }
}
