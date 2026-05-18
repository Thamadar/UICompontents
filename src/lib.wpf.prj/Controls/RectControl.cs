using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Lib.WPF.Controls
{   
    /// <summary>
    /// Прямоугольник для графики.
    /// </summary>
    public class RectControl : Control
    {
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(RectControl), new PropertyMetadata(Brushes.Transparent, OnRenderPropertyChanged));

        //public static readonly DependencyProperty BorderBrushProperty =
        //    DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(RectControl), new PropertyMetadata(Brushes.Red, OnRenderPropertyChanged));

        //public static readonly DependencyProperty BorderThicknessProperty =
        //    DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(RectControl), new PropertyMetadata(Brushes.Blue, OnRenderPropertyChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RectControl), new PropertyMetadata(false, OnRenderPropertyChanged));

        /// <summary>
        /// Заливка окружности.
        /// </summary>
        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        /// <summary>
        /// Заливка границы окружности.
        /// </summary>
        //public Brush BorderBrush
        //{
        //    get => (Brush)GetValue(BorderBrushProperty);
        //    set => SetValue(BorderBrushProperty, value);
        //}

        ///// <summary>
        ///// Толщина границы окружности.
        ///// </summary>
        //public double BorderThickness
        //{
        //    get => (double)GetValue(BorderThicknessProperty);
        //    set => SetValue(BorderThicknessProperty, value);
        //} 

        /// <summary>
        /// Выбран ли элемент?
        /// </summary>
        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        protected override void OnRender(DrawingContext context)
        {
            var borderThickness = BorderThickness.Top;
            var pen = new Pen(BorderBrush, borderThickness);
             
            var itemRect = new Rect(
                - (RenderSize.Width  / 2), 
                - (RenderSize.Height / 2),
                RenderSize.Width,
                RenderSize.Height);
            context.DrawRectangle(Fill, pen, itemRect);

            if(IsSelected)
            {
                var dashRect = new Rect(
                    itemRect.X - borderThickness / 2 - 2,
                    itemRect.Y - borderThickness / 2 - 2,
                    itemRect.Width + borderThickness + 4,
                    itemRect.Height + borderThickness + 4);

                var dashPen = new Pen(Brushes.Black, 2)
                {
                    DashCap = PenLineCap.Round,  
                    DashStyle = new DashStyle(new double[] { 5, 5 }, 0)
                };
                context.DrawRectangle(null, dashPen, dashRect); // null заливка
            }
        }

        private static void OnRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((RectControl)d).InvalidateVisual();
        }
    }
}
