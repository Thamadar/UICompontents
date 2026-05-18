using Lib.WPF;
using Lib.WPF.Extensions;
using ReactiveUI; 
using System.Reactive.Linq;

namespace Client.WPF.Views
{
    public enum ShapeCreateEnum
    {
        Rect,
        Circle,
    } 

    public class GeometryCreateMenuViewModel : ViewModelBase
    {

        #region Fields

        private ShapeCreateEnum _selectedShapeCreate;
        private IShapeCreator _currentShapeCreaterVM;

        #endregion

        #region Properties

        /// <summary>
        /// Текущий выбранный тип создания элемента.
        /// </summary>
        public ShapeCreateEnum SelectedShapeCreate
        {
            get => _selectedShapeCreate;
            set => this.RaiseAndSetIfChanged(ref _selectedShapeCreate, value);
        } 

        /// <summary>
        /// Текущая VM редактора.
        /// </summary>
        public IShapeCreator CurrentShapeCreaterVM
        {
            get => _currentShapeCreaterVM;
            set => this.RaiseAndSetIfChanged(ref _currentShapeCreaterVM, value);
        }
         
        public CircleMenuEditViewModel CircleMenuEditViewModel { get; }
        public RectMenuEditViewModel RectMenuEditViewModel { get; }

        #endregion

        #region .ctor

        public GeometryCreateMenuViewModel()
        {
            CircleMenuEditViewModel = new CircleMenuEditViewModel();
            RectMenuEditViewModel   = new RectMenuEditViewModel();

            this.WhenAnyValue(x => x.SelectedShapeCreate)
                .Do(OnselectedShapeCreate)
                .Subscribe()
                .AddTo(_disposables);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Реакция на изменения значения "Текущий тип редактируемой фигуры".
        /// </summary> 
        public void OnselectedShapeCreate(ShapeCreateEnum shapeCreateEnum)
        {
            switch(shapeCreateEnum)
            {
                case ShapeCreateEnum.Rect:
                    CurrentShapeCreaterVM = RectMenuEditViewModel;
                    break;
                case ShapeCreateEnum.Circle:
                    CurrentShapeCreaterVM = CircleMenuEditViewModel;
                    break;
            }
        }

        #endregion

    }
}
