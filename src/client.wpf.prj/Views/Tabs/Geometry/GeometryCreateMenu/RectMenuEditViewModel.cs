using Client.WPF.Views.Geometry.Shapes;
using Lib.WPF;
using Lib.WPF.Extensions;
using ReactiveUI;
using System.Windows.Media;

namespace Client.WPF.Views
{
    public class RectMenuEditViewModel : ViewModelBase, IShapeCreator
    {
        #region Fields

        private double _width = 20;
        private double _height = 20;
        private double _borderThickness = 1;
        private double _opacity = 1;

        //TO DO: Remove назначения, как появится Control выбора цвета.
        private Brush _fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#006cb5");
        private Brush _borderBrush = Brushes.Red;

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
        public Brush Fill
        {
            get => _fill;
            set => this.RaiseAndSetIfChanged(ref _fill, value);
        }

        /// <summary>
        /// Заливка контура.
        /// </summary>
        public Brush BorderBrush
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
