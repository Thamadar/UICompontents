using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media; 

namespace Lib.Avalonia.Controls
{
    /// <summary>
    /// Прямоугольник для графики.
    /// </summary>
    public class RectControl : Control
    {

        public static readonly StyledProperty<IBrush> FillProperty =
       AvaloniaProperty.Register<RectControl, IBrush>(nameof(Fill), defaultValue: Brushes.Transparent);

        public static readonly StyledProperty<IBrush> BorderBrushProperty =
       AvaloniaProperty.Register<RectControl, IBrush>(nameof(BorderBrush), defaultValue: Brushes.Red);

        public static readonly StyledProperty<double> BorderThicknessProperty =
       AvaloniaProperty.Register<RectControl, double>(nameof(BorderThickness), defaultValue: 1);
         
        public static readonly StyledProperty<bool> IsSelectedProperty =
       AvaloniaProperty.Register<CircleControl, bool>(nameof(IsSelected));

        /// <summary>
        /// Заливка окружности.
        /// </summary>
        public IBrush Fill
        {
            get => GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        /// <summary>
        /// Заливка границы окружности.
        /// </summary>
        public IBrush BorderBrush
        {
            get => GetValue(BorderBrushProperty);
            set => SetValue(BorderBrushProperty, value);
        }

        /// <summary>
        /// Выбран ли элемент?
        /// </summary>
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Толщина границы окружности.
        /// </summary>
        public double BorderThickness
        {
            get => GetValue(BorderThicknessProperty);
            set => SetValue(BorderThicknessProperty, value);
        }
         
        static RectControl()
        {
            AffectsRender<RectControl>(WidthProperty, HeightProperty, IsSelectedProperty, FillProperty, BorderBrushProperty, BorderThicknessProperty);
        }

        public override void Render(DrawingContext context)
        { 
            var pen = new Pen(BorderBrush, BorderThickness);

            var itemRect = new Rect(Bounds.X - (Width / 2), Bounds.Y - (Height / 2), Width, Height);
            context.DrawRectangle(Fill, pen, itemRect);

            if(IsSelected)
            {
                var dashRect = new Rect(
                    itemRect.X - BorderThickness / 2 - 2,
                    itemRect.Y - BorderThickness / 2 - 2,
                    itemRect.Width + BorderThickness + 4,
                    itemRect.Height + BorderThickness + 4);

                var dashPen = new Pen(Brushes.Black, 2)
                {
                    LineCap = PenLineCap.Round,
                    DashStyle = new DashStyle(new double[] { 5, 5 }, 0)
                };
                context.DrawRectangle(dashPen, dashRect);
            }
        }

    }
}
