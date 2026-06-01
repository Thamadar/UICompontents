using Avalonia.Media;
using Client.Avalonia.Views.Geometry.Shapes;
using Lib.Avalonia;
using Lib.Avalonia.Extensions;
using ReactiveUI;

namespace Client.Avalonia.Views.Geometry
{
    public class RectMenuEditViewModel : ViewModelBase, IShapeCreator
    {
        #region Fields

        private double _width = 20;
        private double _height = 20;
        private double _borderThickness = 1;
        private double _opacity = 1;

        private IBrush _fill;
        private IBrush _borderBrush;

        #endregion

        #region Properties

        /// <summary>
        /// Выбранная ширина.
        /// </summary>
        public double Width
        {
            get => _width;
            set => this.RaiseAndSetIfChanged(ref _width, value);
        }

        /// <summary>
        /// Выбранная длина.
        /// </summary>
        public double Height
        {
            get => _height;
            set => this.RaiseAndSetIfChanged(ref _height, value);
        }

        /// <summary>
        /// Толщина контура.
        /// </summary>
        public double BorderThickness
        {
            get => _borderThickness;
            set => this.RaiseAndSetIfChanged(ref _borderThickness, value);
        }

        /// <summary>
        /// Непрозрачность.
        /// </summary>
        public double Opacity
        {
            get => _opacity;
            set => this.RaiseAndSetIfChanged(ref _opacity, value);
        }

        /// <summary>
        /// Заливка.
        /// </summary>
        public IBrush Fill
        {
            get => _fill;
            set => this.RaiseAndSetIfChanged(ref _fill, value);
        }

        /// <summary>
        /// Заливка контура.
        /// </summary>
        public IBrush BorderBrush
        {
            get => _borderBrush;
            set => this.RaiseAndSetIfChanged(ref _borderBrush, value);
        }

        #endregion


        #region Methods 

        /// <inheritdoc/>
        public IShapeItem Create(double x, double y)
        {
            return new RectItem(x, y, Width, Height, BorderThickness, Opacity, Fill.ToSolidColorBrush(), BorderBrush.ToSolidColorBrush());
        }

        #endregion
    }
}
