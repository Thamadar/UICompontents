using Client.Avalonia.Services;
using Client.Avalonia.Services.Interfaces;
using Client.Avalonia.Views.Tabs.Scheduler;
using DynamicData;

using Lib.Avalonia;
using Lib.Avalonia.Extensions;
using Lib.Avalonia.Helpers;
using Lib.Avalonia.Services.Dialogs;

using ReactiveUI; 

using System.Collections.ObjectModel; 
using System.Reactive.Linq; 

namespace Client.Avalonia.Views
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ITabService _tabService;
        private ReadOnlyObservableCollection<TabMenu> _totalTabMenu = new(new());

        private ITabVM _currentTabVM;  

        /// <summary>
        /// Горячие клавиши окна.
        /// </summary>
        public ObservableCollection<IHotKey> HotKeys { get; }

        /// <summary>
        /// Все вкладки из меню.
        /// </summary>
        public ReadOnlyObservableCollection<TabMenu> TotalTabMenu => _totalTabMenu;

        /// <summary>
        /// Текущая VM-вкладка.
        /// </summary>
        public ITabVM CurrentTabVM
        {
            get => _currentTabVM;
            set => this.RaiseAndSetIfChanged(ref _currentTabVM, value);
        }

        /// <summary>
        /// Отслеживание состояния "Открыто ли какой-либо IDialog (не окно)?".
        /// </summary>
        public IObservable<bool> IsDialogsOpenedObserve { get; }

        /// <summary>
        /// Хранилище базовых диалоговых панелей.
        /// </summary>
        public DefaultDialogs DefaultDialogs { get; } = new(); 

        /// <summary>
        /// VM диалоговой панели SchedulerEventItemDialog.
        /// </summary>
        public SchedulerEventItemDialogViewModel SchedulerEventItemDialog { get; }

        public MainWindowViewModel()
        {
            _tabService = TabService.Instance;
             
            IsDialogsOpenedObserve = DialogSystem.IsDialogsOpenedObserve;

            SchedulerEventItemDialog = new SchedulerEventItemDialogViewModel();

            HotKeys = new ObservableCollection<IHotKey>();
             
            _tabService
                .ConnectToTotalTabMenu()
                .Bind(out _totalTabMenu)
                .Subscribe()
                .AddTo(_disposables);

            _tabService
                .ConnectToCurrentTabHotKeys()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(changeSet =>
                {   
                    LoadHotKeys(_tabService.GetCurrentTabVMHotKeys());
                })
                .AddTo(_disposables);

            _tabService
                .CurrentSelectedTabVMObservable
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, x => x.CurrentTabVM)
                .AddTo(_disposables); 

            _tabService.LoadTotalTabMenuData();
            _tabService.SelectTabMenu(TabCategoryEnum.Scheduler); 
        }

        /// <summary>
        /// Прогрузка горячих клавиш. 
        /// </summary>
        public void LoadHotKeys(IEnumerable<IHotKey> tabHotKeys)
        {
            HotKeys.Clear();

            HotKeys.AddRange(DefaultWindowKeys());
            HotKeys.AddRange(tabHotKeys);
        }

        /// <inheritdoc/>
        private IEnumerable<IHotKey> DefaultWindowKeys()
        {
            //базовые клавиши всего приложения. Например, f12 - окно "О приложении".
            var defaultKeys = new List<IHotKey>()
            {

            };

            return defaultKeys;
        }
    }
}
