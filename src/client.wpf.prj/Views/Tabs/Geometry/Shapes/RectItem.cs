using ReactiveUI;
using System.Windows.Media;

namespace Client.WPF.Views.Geometry.Shapes
{
    /// <summary>
    /// Прямоугольник
    /// </summary>
    public class RectItem : ShapeItem, IRectItem
    {
        private double _width;
        private double _height;

        /// <inheritdoc/>
        public double Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }
        /// <inheritdoc/>
        public double Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        public RectItem(
            double xCenter,
            double yCenter,
            double width,
            double height,
            double borderThickness = 1,
            double opacity = 1.0,
            SolidColorBrush? fill = null,
            SolidColorBrush? borderBrush = null)
            : base(xCenter, yCenter, borderThickness, opacity, fill, borderBrush)
        {
            Width  = width;
            Height = height;
        }
    } 
}
