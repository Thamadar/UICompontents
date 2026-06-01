using Avalonia;
using Avalonia.Controls; 
using Avalonia.Media;
using Client.Avalonia.Services;
using Client.Avalonia.Services.Interfaces;
using Client.Avalonia.Views.Geometry.Shapes;
using DynamicData;
using Lib.Avalonia;
using Lib.Avalonia.Extensions;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace Client.Avalonia.Views.Geometry
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
        /// <summary>
        /// Конструктор-заглушка, дабы Designer не падал.
        /// </summary>
        public DisplayViewModel()
            : this(ReactiveCommand.Create<Point>((Point p) => { }))
        {

        }

        public DisplayViewModel(ReactiveCommand<Point, Unit>? createShapeCommand = null)
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

            if(!Design.IsDesignMode && createShapeCommand != null)
            {
                CreateShapeCommand = createShapeCommand;
            }

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
                new CircleItem(100, 200, 50, 0, 1, SolidColorBrush.Parse("#006cb5")),
                new CircleItem(300, 300, 30, 4, 0.5, SolidColorBrush.Parse("#00645f"), SolidColorBrush.Parse("#e85222")),
                new RectItem(500, 600, 30, 30, 4, 0.7, SolidColorBrush.Parse("#00645f"), SolidColorBrush.Parse("#e85222")),
                new RectItem(0, 0, 100, 20, 1, 1, SolidColorBrush.Parse("#00645f"), SolidColorBrush.Parse("#e85222")),
            };

            _shapeService.AddRangeShape(shapes);
        }

        #endregion
    }
}
