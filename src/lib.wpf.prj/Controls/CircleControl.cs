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
    /// Окружность для графики.
    /// </summary>
    public class CircleControl : Control
    { 
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register(nameof(Fill), typeof(Brush), typeof(CircleControl), new PropertyMetadata(Brushes.Transparent, OnRenderPropertyChanged));

        //public static readonly DependencyProperty BorderBrushProperty =
        //    DependencyProperty.Register(nameof(BorderBrush), typeof(Brush), typeof(CircleControl), new PropertyMetadata(Brushes.Red, OnRenderPropertyChanged));

        //public static readonly DependencyProperty BorderThicknessProperty =
        //    DependencyProperty.Register(nameof(BorderThickness), typeof(double), typeof(CircleControl), new PropertyMetadata(Brushes.Blue, OnRenderPropertyChanged));

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(nameof(Radius), typeof(double), typeof(CircleControl), new PropertyMetadata(20.0, OnRenderPropertyChanged));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(CircleControl), new PropertyMetadata(false, OnRenderPropertyChanged));

        /// <summary>
        /// Заливка окружности.
        /// </summary>
        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        ///// <summary>
        ///// Заливка границы окружности.
        ///// </summary>
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
        /// Радиус окружности.
        /// </summary>
        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

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
            var center = new Point(RenderSize.Width / 2, RenderSize.Height / 2);
            var borderThickness = BorderThickness.Top;

            var pen = new Pen(BorderBrush, borderThickness);
            context.DrawEllipse(Fill, pen, center, Radius, Radius);

            if(IsSelected)
            {
                var rect = new Rect(
                    center.X - Radius - borderThickness / 2 - 2,
                    center.Y - Radius - borderThickness / 2 - 2,
                    Radius * 2 + borderThickness + 4,
                    Radius * 2 + borderThickness + 4);

                var selectPen = new Pen(Brushes.Black, 2)
                {
                    DashCap = PenLineCap.Round,
                    DashStyle = new DashStyle(new double[] { 5, 5 }, 0)
                };
                context.DrawRectangle(null, selectPen, rect); // null заливка
            }
        }

        private static void OnRenderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((CircleControl)d).InvalidateVisual();
        }
    }
}
