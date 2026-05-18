using Lib.WPF;
using Lib.WPF.Extensions;
using ReactiveUI;
using System.Reactive.Linq;
using System.Windows.Media;

namespace Client.WPF.Views.Geometry.Shapes
{
    /// <summary>
    /// Геометрическая фигура. Базовый класс
    /// </summary>
    public class ShapeItem : ViewModelBase, IShapeItem
    {
        private SolidColorBrush? _fill;
        private SolidColorBrush? _borderBrush;

        private bool isSelected;
        private double _xCenter;
        private double _yCenter;
        private double _borderThickness;
        private double _opacity; 

        public Guid Id { get; }

        /// <inheritdoc/>
        public bool IsSelected
        {
            get => isSelected;
            set => this.RaiseAndSetIfChanged(ref isSelected, value);
        }

        /// <inheritdoc/>
        public double XCenter
        {
            get => _xCenter;
            set => this.RaiseAndSetIfChanged(ref _xCenter, value);
        }

        /// <inheritdoc/>
        public double YCenter
        {
            get => _yCenter;
            set => this.RaiseAndSetIfChanged(ref _yCenter, value);
        }

        /// <inheritdoc/>
        public double BorderThickness
        {
            get => _borderThickness;
            set => this.RaiseAndSetIfChanged(ref _borderThickness, value);
        }

        ///<inheritdoc/>
        public double Opacity
        {
            get => _opacity;
            set => this.RaiseAndSetIfChanged(ref _opacity, value);
        }

        /// <inheritdoc/>
        public SolidColorBrush? Fill
        {
            get => _fill;
            set => this.RaiseAndSetIfChanged(ref _fill, value);
        }

        /// <inheritdoc/>
        public SolidColorBrush? BorderBrush
        {
            get => _borderBrush;
            set => this.RaiseAndSetIfChanged(ref _borderBrush, value);
        }

        public ShapeItem(
            double xCenter,
            double yCenter, 
            double borderThickness = 1,
            double opacity = 1,
            SolidColorBrush? fill = null, 
            SolidColorBrush? borderBrush = null)
        {
            Id = Guid.NewGuid();

            XCenter         = xCenter;
            YCenter         = yCenter; 
            BorderThickness = borderThickness;
            Fill            = fill;
            BorderBrush     = borderBrush;

            this.WhenAnyValue(x => x.Opacity)
                .Do(OnOpacityChanged)
                .Subscribe()
                .AddTo(_disposables);

            Opacity = opacity;
        }

        private void OnOpacityChanged(double opacity)
        {
            Fill        = Fill?.ToSolidColorBrush(opacity);
            BorderBrush = BorderBrush?.ToSolidColorBrush(opacity); 
        }
    }
}
