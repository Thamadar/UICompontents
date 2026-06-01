using Avalonia.Media;
using Client.Avalonia.Views.Geometry.Shapes;
using Lib.Avalonia;
using Lib.Avalonia.Extensions;
using ReactiveUI; 

namespace Client.Avalonia.Views.Geometry
{
    public class CircleMenuEditViewModel : ViewModelBase, IShapeCreator
    {

        #region Fields

        private double _radius = 20;
        private double _borderThickness = 1;
        private double _opacity = 1;
        private IBrush _fill;
        private IBrush _borderBrush;

        #endregion

        #region Properties

        /// <summary>
        /// Выбранный радиус.
        /// </summary>
        public double Radius
        {
            get => _radius;
            set => this.RaiseAndSetIfChanged(ref _radius, value);
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
            return new CircleItem(x, y, Radius, BorderThickness, Opacity, Fill.ToSolidColorBrush(), BorderBrush.ToSolidColorBrush());
        }

        #endregion
    }
}
