using ReactiveUI;
using System.Windows.Media;

namespace Client.WPF.Views.Geometry.Shapes
{
    /// <summary>
    /// Окружность.
    /// </summary>
    public class CircleItem : ShapeItem, ICircleItem
    {
        private double _radius;

        /// <inheritdoc/>
        public double Radius
        {
            get => _radius;
            set => this.RaiseAndSetIfChanged(ref _radius, value);
        }

        public CircleItem(
            double xCenter,
            double yCenter,
            double radius,
            double borderThickness = 1,
            double opacity = 1.0,
            SolidColorBrush? fill = null,
            SolidColorBrush? borderBrush = null) 
            : base(xCenter, yCenter, borderThickness, opacity,fill, borderBrush)
        {
            Radius = radius;
        }
    }
}
