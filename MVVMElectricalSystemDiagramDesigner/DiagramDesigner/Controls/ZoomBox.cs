using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Input;

namespace DiagramDesigner
{
    public class ZoomBox : Control
    {
        private Thumb zoomThumb;
        private Canvas zoomCanvas;
        private Slider zoomSlider;
        private ScaleTransform scaleTransform;

        #region DPs

        #region ScrollViewer
        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ZoomBox));
        #endregion

        #region DesignerCanvas


        public static readonly DependencyProperty DesignerCanvasProperty =
            DependencyProperty.Register("DesignerCanvas", typeof(DesignerCanvas), typeof(ZoomBox),
                new(null, new(OnDesignerCanvasChanged)));


        public DesignerCanvas DesignerCanvas
        {
            get => (DesignerCanvas)GetValue(DesignerCanvasProperty);
            set => SetValue(DesignerCanvasProperty, value);
        }


        private static void OnDesignerCanvasChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomBox target = (ZoomBox)d;
            DesignerCanvas oldDesignerCanvas = (DesignerCanvas)e.OldValue;
            DesignerCanvas newDesignerCanvas = target.DesignerCanvas;
            target.OnDesignerCanvasChanged(oldDesignerCanvas, newDesignerCanvas);
        }


        protected virtual void OnDesignerCanvasChanged(DesignerCanvas oldDesignerCanvas, DesignerCanvas newDesignerCanvas)
        {
            if (oldDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated -= new(this.DesignerCanvas_LayoutUpdated);
                newDesignerCanvas.PreviewMouseWheel -= new(this.DesignerCanvas_PreviewMouseWheel);
                newDesignerCanvas.LayoutTransform = this.scaleTransform;
            }

            if (newDesignerCanvas != null)
            {
                newDesignerCanvas.LayoutUpdated += new(this.DesignerCanvas_LayoutUpdated);
                newDesignerCanvas.PreviewMouseWheel += new(this.DesignerCanvas_PreviewMouseWheel);
                newDesignerCanvas.LayoutTransform = this.scaleTransform;
            }
        }

        #endregion

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.ScrollViewer == null)
                return;

            this.zoomThumb = Template.FindName("PART_ZoomThumb", this) as Thumb;
            if (this.zoomThumb == null)
                throw new("PART_ZoomThumb template is missing!");

            this.zoomCanvas = Template.FindName("PART_ZoomCanvas", this) as Canvas;
            if (this.zoomCanvas == null)
                throw new("PART_ZoomCanvas template is missing!");

            this.zoomSlider = Template.FindName("PART_ZoomSlider", this) as Slider;
            if (this.zoomSlider == null)
                throw new("PART_ZoomSlider template is missing!");

            this.zoomThumb.DragDelta += new(this.Thumb_DragDelta);
            this.zoomSlider.ValueChanged += new(this.ZoomSlider_ValueChanged);
            this.scaleTransform = new();
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            double scale = e.NewValue / e.OldValue;
            double halfViewportHeight = this.ScrollViewer.ViewportHeight / 2;
            double newVerticalOffset = ((this.ScrollViewer.VerticalOffset + halfViewportHeight) * scale - halfViewportHeight);
            double halfViewportWidth = this.ScrollViewer.ViewportWidth / 2;
            double newHorizontalOffset = ((this.ScrollViewer.HorizontalOffset + halfViewportWidth) * scale - halfViewportWidth);
            this.scaleTransform.ScaleX *= scale;
            this.scaleTransform.ScaleY *= scale;
            this.ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            this.ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            this.InvalidateScale(out double scale, out _, out double _);
            this.ScrollViewer.ScrollToHorizontalOffset(this.ScrollViewer.HorizontalOffset + e.HorizontalChange / scale);
            this.ScrollViewer.ScrollToVerticalOffset(this.ScrollViewer.VerticalOffset + e.VerticalChange / scale);
        }

        private void DesignerCanvas_LayoutUpdated(object sender, EventArgs e)
        {
            this.InvalidateScale(out double scale, out double xOffset, out double yOffset);
            this.zoomThumb.Width = this.ScrollViewer.ViewportWidth * scale;
            this.zoomThumb.Height = this.ScrollViewer.ViewportHeight * scale;
            Canvas.SetLeft(this.zoomThumb, xOffset + this.ScrollViewer.HorizontalOffset * scale);
            Canvas.SetTop(this.zoomThumb, yOffset + this.ScrollViewer.VerticalOffset * scale);
        }

        private void DesignerCanvas_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift))
                return;
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double value = Math.Min(e.Delta / 12, 10);
                this.zoomSlider.Value += value;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                this.ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset - e.Delta);
                e.Handled = true;
            }
            else
                this.ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset- e.Delta);
        }

        private void InvalidateScale(out double scale, out double xOffset, out double yOffset)
        {
            double w = DesignerCanvas.ActualWidth * this.scaleTransform.ScaleX;
            double h = DesignerCanvas.ActualHeight * this.scaleTransform.ScaleY;

            // zoom canvas size
            double x = this.zoomCanvas.ActualWidth;
            double y = this.zoomCanvas.ActualHeight;
            double scaleX = x / w;
            double scaleY = y / h;
            scale = (scaleX < scaleY) ? scaleX : scaleY;
            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;
        }
    }
}


