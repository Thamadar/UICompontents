
using Avalonia.Input;

using Client.Avalonia.Services;
using Client.Avalonia.Services.Interfaces;
using Client.Avalonia.Views.Tabs.Geometry.Tools;

using Lib.Avalonia;
using Lib.Avalonia.Extensions;
using Lib.Avalonia.Helpers;

using ReactiveUI; 

namespace Client.Avalonia.Views.Geometry
{
    /// <summary>
    /// VM, отвечающая за вкладку "Графический редактор".
    /// //TO DO: переименовать класс. GeometryViewModel => GhaphicEditorViewModel.
    /// </summary>
    public sealed partial class GeometryViewModel : ViewModelBase, ITabVM
    {
        #region Fields

        private readonly IShapeService _shapeService;
        private readonly IToolService _toolService;

        private IEnumerable<IHotKey> _defaultTabHotKeys = new IHotKey[0];
        private ITool _currentSelectedTool;

        #endregion

        #region Properties

        public GeometryManagementPanelViewModel GeometryManagementPanelViewModel { get; }
        public DisplayViewModel DisplayViewModel { get; }
        public ToolsPanelViewModel ToolsPanelViewModel { get; }

        /// <summary>
        /// Текущий выбранный инструмент (заливка, текст, перемещение, ...) .
        /// </summary>
        public ITool CurrentSelectedTool
        {
            get => _currentSelectedTool;
            set => this.RaiseAndSetIfChanged(ref _currentSelectedTool, value);
        }

        /// <inheritdoc/>
        public Guid Id { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Конструктор-заглушка, дабы Designer не падал.
        /// </summary>
        public GeometryViewModel()
            : this(Guid.NewGuid())
        {

        }

        public GeometryViewModel(Guid id)
        {
            _shapeService = ShapeService.Instance;
            _toolService  = ToolService.Instance;

            Id = id; 

            GeometryManagementPanelViewModel = new GeometryManagementPanelViewModel();
            DisplayViewModel                 = new DisplayViewModel();
            ToolsPanelViewModel              = new ToolsPanelViewModel();
             
            _toolService
                .CurrentSelectedToolObservable
                .BindTo(this, x => x.CurrentSelectedTool)
                .AddTo(_disposables); 

            InitDefaultHotKeys();

            _toolService.SelectTool(ToolTypeEnum.Moving);
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public IEnumerable<IHotKey> GetTabHotKeys() => _defaultTabHotKeys; 

        /// <inheritdoc/>
        public Task DisposeTab()
        {
            //Какие-нибудь отписки, дочерние Dispose, await...

            Dispose();

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task LoadTab()
        {
            //Какие-нибудь подписки, await...

            return Task.CompletedTask;
        }

        /// <summary>
        /// Инициализация горячих клавиш данной вкладки.
        /// </summary>
        private void InitDefaultHotKeys()
        {
            _defaultTabHotKeys = new List<IHotKey>
            {
                new HotKey(Key.Delete, Commands.RemoveShapeCommand),

                new HotKey(Key.N, GeometryManagementPanelViewModel.Commands.NewFileCommand, keyModifiers: KeyModifiers.Control),
                new HotKey(Key.O, GeometryManagementPanelViewModel.Commands.OpenFileCommand, keyModifiers: KeyModifiers.Control),
                new HotKey(Key.S, GeometryManagementPanelViewModel.Commands.SaveFileCommand, keyModifiers: KeyModifiers.Control),

                new HotKey(Key.Z, GeometryManagementPanelViewModel.Commands.StepBackEditCommand, keyModifiers:KeyModifiers.Control),
                new HotKey(Key.Y, GeometryManagementPanelViewModel.Commands.StepForwardEditCommand, keyModifiers:KeyModifiers.Control),
                new HotKey(Key.J, GeometryManagementPanelViewModel.Commands.CloneEditCommand, keyModifiers:KeyModifiers.Control),

                new HotKey(Key.U, Commands.SelectShapesToolCommand),
            };
        } 

        #endregion

    }
}
