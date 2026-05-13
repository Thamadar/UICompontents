using Avalonia.Controls;
using Client.Avalonia.Services.Interfaces;
using Client.Avalonia.Views.Geometry.Shapes;
using DynamicData; 
using System.Reactive.Linq;
using System.Reactive.Subjects; 

namespace Client.Avalonia.Services
{
    public class ShapeService : IShapeService
    {
        #region Fields

        private static readonly Lazy<IShapeService> _instance = new Lazy<IShapeService>(() => new ShapeService());

        private readonly ISourceList<IShapeItem> _totalShapes;
        private readonly Subject<IShapeItem?> _currentSelectedShapeSubject;

        //private IList<IDisposable> _disposables = new List<IDisposable>();
        private IShapeItem? _currentSelectedShape;

        #endregion

        #region Properties

        /// <summary>
        /// Экземпляр.
        /// </summary>
        public static IShapeService Instance => _instance.Value;

        /// <inheritdoc/>
        public IObservable<IShapeItem?> CurrentSelectedShapeObservable => _currentSelectedShapeSubject.AsObservable();
        
        /// <summary>
        /// Текущая выбранная геом. фигура
        /// </summary>
        private IShapeItem? CurrentSelectedShape
        {
            get => _currentSelectedShape;
            set
            {
                _currentSelectedShape = value;
                _currentSelectedShapeSubject.OnNext(value);
            }
        }

        #endregion

        #region .ctor

        private ShapeService()
        {
            _totalShapes = new SourceList<IShapeItem>();

            _currentSelectedShapeSubject = new Subject<IShapeItem?>(); 
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public IObservable<IChangeSet<IShapeItem>> ConnectToTotalShapes()
        {
            return _totalShapes.Connect();
        }

        /// <inheritdoc/>
        public IShapeItem? GetCurrentSelectedShape() => CurrentSelectedShape;

        /// <inheritdoc/>
        public void AddShape(IShapeItem shapeItem)
        {
            if(!_totalShapes.Items.Any(x => x.Id.Equals(shapeItem.Id)))
            {
                _totalShapes.Add(shapeItem);
            }
        }

        /// <inheritdoc/>
        public void AddRangeShape(IEnumerable<IShapeItem> shapeItems)
        {
            foreach(var shapeItem in shapeItems)
            {  
                if(!_totalShapes.Items.Any(x => x.Id.Equals(shapeItem.Id)))
                {
                    _totalShapes.Add(shapeItem);
                }
            } 
        } 

        /// <inheritdoc/>
        public void RemoveShapeById(Guid guid)
        {
            var shapeItem = _totalShapes.Items.FirstOrDefault(x => x.Id.Equals(guid));
            if(shapeItem != null)
            {
                CheckAndDeselectShape(shapeItem);
                shapeItem.Dispose();
                _totalShapes.Remove(shapeItem);
            }
        } 

        /// <inheritdoc/>
        public void RemoveAllShapes()
        {
            for(int i = _totalShapes.Count; i != 0; i++)
            {
                var shapeItem = _totalShapes.Items.Last();

                CheckAndDeselectShape(shapeItem);
                shapeItem.Dispose();
                _totalShapes.Remove(shapeItem);
            }
        }

        /// <inheritdoc/>
        public void SelectShapeById(Guid? guid = null)
        {  
            foreach(var shape in _totalShapes.Items)
            {
                shape.IsSelected = false;
            }

            if(guid == null)
            {
                CurrentSelectedShape = null;
                return;
            }

            var selectedShape = _totalShapes.Items.FirstOrDefault(x => x.Id.Equals(guid)); 
            if(selectedShape != null && !selectedShape.Id.Equals(CurrentSelectedShape?.Id))
            { 
                selectedShape.IsSelected = true;

                CurrentSelectedShape = selectedShape;
                return;
            }
            CurrentSelectedShape = null;
        }

        private void CheckAndDeselectShape(IShapeItem shapeItem)
        {
            if(CurrentSelectedShape != null && CurrentSelectedShape.Id.Equals(shapeItem.Id))
            {
                CurrentSelectedShape.IsSelected = false;
                CurrentSelectedShape = null;
            }
        }

        #endregion
    }
}
