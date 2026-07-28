using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace Lib.Avalonia.Controls
{
    /// <summary>
    /// Фон из линий для SchedulerContentPanelControl.
    /// </summary>
    internal class SchedulerBackgroundControl : Control
    {
        public static readonly StyledProperty<double> CellWidthProperty =
        AvaloniaProperty.Register<SchedulerBackgroundControl, double>(nameof(CellWidth), 50);

        public static readonly StyledProperty<double> CellHeightProperty =
            AvaloniaProperty.Register<SchedulerBackgroundControl, double>(nameof(CellHeight), 50);

        public double CellWidth
        {
            get => GetValue(CellWidthProperty);
            set => SetValue(CellWidthProperty, value);
        }

        public double CellHeight
        {
            get => GetValue(CellHeightProperty);
            set => SetValue(CellHeightProperty, value);
        }

        static SchedulerBackgroundControl()
        {
            AffectsRender<SchedulerBackgroundControl>(CellWidthProperty, CellHeightProperty);
        }

        public SchedulerBackgroundControl()
        {
            Margin = new Thickness(-1, -1, 0, 0); 
        }
        
        public override void Render(DrawingContext context)
        {  
            base.Render(context);

            if(CellWidth <= 0 || CellHeight <= 0)
                return;

            var cellWidth  = Math.Ceiling(CellWidth);
            var cellHeight = Math.Ceiling(CellHeight);

            var pen     = new Pen(SolidColorBrush.Parse("#ebebeb"), 1);
            var dashpen = new Pen(SolidColorBrush.Parse("#ebebeb"), 1)
            {
                LineCap = PenLineCap.Round,
                DashStyle = new DashStyle(new double[] { 4, 2 }, 0)
            };
             
            double midOffset = CellHeight / 2.0;

            //сплошные линии по вертикали.
            for(double x = CellWidth; x < Bounds.Width; x += CellWidth)
                context.DrawLine(pen, new Point(x + 0.5, 0), new Point(x + 0.5, Bounds.Height));

            //сплошные лишнии по горизонатали.
            for(double y = cellHeight; y < Bounds.Height; y += cellHeight)
                context.DrawLine(pen, new Point(0, y + 0.5), new Point(Bounds.Width, y + 0.5));

            //пунктирные линии по горизонтали.
            for(double y = midOffset + 1; y <= Bounds.Height; y += cellHeight)
                context.DrawLine(dashpen, new Point(0, y + 0.5), new Point(Bounds.Width, y + 0.5)); 
        }
    }
}
