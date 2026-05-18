using Client.WPF.Services;
using Client.WPF.Services.Interfaces;
using Client.WPF.Views.Geometry.Shapes;
using DynamicData;
using Lib.WPF;
using Lib.WPF.Extensions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Client.WPF.Views 
{
    public class DisplayViewModel : ViewModelBase
    {
        #region Fields

        private ReadOnlyObservableCollection<IShapeItem> _totalShapes = new(new());
        private readonly IShapeService _shapeService;
        private IShapeItem? _selectedShape;

        #endregion

        #region Properties 

        /// <summary>
        /// Текущая выбранная геом. фигура.
        /// </summary>
        public IShapeItem? SelectedShape
        {
            get => _selectedShape;
            set => this.RaiseAndSetIfChanged(ref _selectedShape, value);
        }

        /// <summary>
        /// Геометрические фигуры.
        /// </summary>
        public ReadOnlyObservableCollection<IShapeItem> TotalShapes => _totalShapes;
        public ReactiveCommand<Point, Unit> CreateShapeCommand { get; }

        #endregion

        #region .ctor 

        public DisplayViewModel(ReactiveCommand<Point, Unit>? createShapeCommand)
        {
            _shapeService = ShapeService.Instance;

            _shapeService
                .ConnectToTotalShapes()
                .Bind(out _totalShapes)
                .Subscribe()
                .AddTo(_disposables);

            _shapeService
                .CurrentSelectedShapeObservable
                .BindTo(this, x => x.SelectedShape)
                .AddTo(_disposables);

            CreateShapeCommand = createShapeCommand;

            LoadDefaultShapes();
        }

        #endregion

        #region Methods 

        /// <summary>
        /// Загрузка базовых геом. фигур (для теста).
        /// </summary>
        private void LoadDefaultShapes()
        {
            _shapeService.RemoveAllShapes();

            var shapes = new List<IShapeItem>()
            {
                new CircleItem(100, 200, 50, 0, 1, (SolidColorBrush)new BrushConverter().ConvertFrom("#006cb5")),
                new CircleItem(300, 300, 30, 4, 0.5, (SolidColorBrush)new BrushConverter().ConvertFrom("#00645f"), (SolidColorBrush)new BrushConverter().ConvertFrom("#e85222")),
                new RectItem(500, 600, 30, 30, 4, 0.7, (SolidColorBrush)new BrushConverter().ConvertFrom("#00645f"), (SolidColorBrush)new BrushConverter().ConvertFrom("#e85222")),
                new RectItem(0, 0, 100, 20, 1, 1,(SolidColorBrush)new BrushConverter().ConvertFrom("#00645f"), (SolidColorBrush)new BrushConverter().ConvertFrom("#e85222")),
           };

            _shapeService.AddRangeShape(shapes);
        }

        #endregion
    }
}
