using Client.WPF.Services;
using Client.WPF.Services.Interfaces;
using DynamicData;
using Lib.WPF;
using Lib.WPF.Extensions;
using Lib.WPF.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.WPF.Views
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

        public MainWindowViewModel()
        {
            _tabService = TabService.Instance;

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
                    //TO DO сделать грамотнее, нежели тянуть ещё раз метод

                    LoadHotKeys(_tabService.GetCurrentTabVMHotKeys());
                })
                .AddTo(_disposables);

            _tabService
                .CurrentSelectedTabVMObservable
                .ObserveOn(RxApp.MainThreadScheduler)
                .BindTo(this, x => x.CurrentTabVM)
                .AddTo(_disposables);

            _tabService.LoadTotalTabMenuData();
            _tabService.SelectTabMenu(TabCategoryEnum.GraphicEditor);
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

        private IEnumerable<IHotKey> DefaultWindowKeys()
        {
            //базовые клавиши всего приложения. Например, f12 - О приложении.
            var defaultKeys = new List<IHotKey>()
            {

            };

            return defaultKeys;
        }
    }
}
