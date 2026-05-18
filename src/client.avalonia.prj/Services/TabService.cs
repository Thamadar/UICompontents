using Client.Avalonia.Services.Interfaces;
using Client.Avalonia.Views; 
using DynamicData;
using DynamicData.Binding;
using Lib.Avalonia.Extensions;
using Lib.Avalonia.Helpers;
using ReactiveUI; 
using System.Collections.ObjectModel; 
using System.Reactive.Linq;
using System.Reactive.Subjects; 

namespace Client.Avalonia.Services 
{
    public class TabService : ITabService
    {
        #region Fields

        private static readonly Lazy<ITabService> _instance = new Lazy<ITabService>(() => new TabService());
        private ReadOnlyObservableCollection<TabMenu> _tabMenuObservable = new(new());

        private List<IDisposable> _disposables = new List<IDisposable>();
        private readonly ISourceList<TabMenu> _totalTabMenu;
        private readonly ISourceList<IHotKey> _totalHotKeys;
        private readonly Subject<ITabVM?> _currentSelectedTabVMSubject;

        //private IList<IDisposable> _disposables = new List<IDisposable>();
        private ITabVM? _currentSelectedTabVM;

        #endregion

        #region Properties

        /// <summary>
        /// Экземпляр.
        /// </summary>
        public static ITabService Instance => _instance.Value;

        /// <inheritdoc/>
        public IObservable<ITabVM?> CurrentSelectedTabVMObservable => _currentSelectedTabVMSubject.AsObservable();

        /// <summary>
        /// Текущая выбранная геом. фигура
        /// </summary>
        private ITabVM? CurrentSelectedTabVM
        {
            get => _currentSelectedTabVM;
            set
            {
                _currentSelectedTabVM = value;
                _currentSelectedTabVMSubject.OnNext(value);
            }
        }

        #endregion

        #region .ctor

        private TabService()
        {
            _totalTabMenu                = new SourceList<TabMenu>();
            _totalHotKeys                = new SourceList<IHotKey>();
            _currentSelectedTabVMSubject = new Subject<ITabVM?>();
             
            _totalTabMenu
                .Connect()  
                .Bind(out _tabMenuObservable)
                .Subscribe()
                .AddTo(_disposables);

            _tabMenuObservable 
                .ToObservableChangeSet(item => item.WhenPropertyChanged(prop => prop.IsSelected))
                .AutoRefresh(un => un.IsSelected)
                .Subscribe(changeSet =>     
                {
                    foreach(var change in changeSet)
                    {
                        if(change.Current.IsSelected && !(CurrentSelectedTabVM != null && CurrentSelectedTabVM.Id.Equals(change.Current.Id)))
                        {
                            //TO DO: await
                            SelectTabMenu(change.Current.TabCategory);
                        } 
                    } 
                })
                .AddTo(_disposables); 
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public IObservable<IChangeSet<TabMenu>> ConnectToTotalTabMenu()
        {
            return _totalTabMenu.Connect();
        }

        /// <inheritdoc/>
        public IObservable<IChangeSet<IHotKey>> ConnectToCurrentTabHotKeys()
        {
            return _totalHotKeys.Connect();
        }

        /// <inheritdoc/>
        public ITabVM? GetCurrentSelectedTabVM() => CurrentSelectedTabVM;

        public IEnumerable<IHotKey> GetCurrentTabVMHotKeys() => _totalHotKeys.Items;

        /// <inheritdoc/>
        public void LoadTotalTabMenuData()
        {
            var tabMenu = new List<TabMenu>()
            {
                new TabMenu("Графики", TabCategoryEnum.Graphs), 
                new TabMenu("Граф. редактор", TabCategoryEnum.GraphicEditor)
            };

            _totalTabMenu.AddRange(tabMenu);
        }

        /// <inheritdoc/>
        public async Task SelectTabMenu(TabCategoryEnum tabCategoryEnum)
        {
            var selectedTabMenu = _totalTabMenu.Items.FirstOrDefault(x => x.TabCategory.Equals(tabCategoryEnum));
            
            if(selectedTabMenu != null && !selectedTabMenu.Id.Equals(CurrentSelectedTabVM?.Id))
            { 
                if(CurrentSelectedTabVM != null)
                { 
                    selectedTabMenu.IsSelected = false;
                    await CurrentSelectedTabVM.DisposeTab();
                }  

                var tabVM = CreateTabVM(selectedTabMenu);

                CurrentSelectedTabVM = tabVM; 

                if(CurrentSelectedTabVM != null)
                {
                    selectedTabMenu.IsSelected = true;
                    await CurrentSelectedTabVM.LoadTab();
                }

                //TO DO: вынести
                if(CurrentSelectedTabVM!=null)
                {
                    _totalHotKeys.Clear();
                    _totalHotKeys.AddRange(CurrentSelectedTabVM.GetTabHotKeys());
                } 
            } 
        }
         
        /// <summary>
        /// Создание свежей вкладки, а именно VM, возвращая ITabVM.
        /// </summary> 
        private ITabVM? CreateTabVM(TabMenu tabMenu)
        { 
            return tabMenu.TabCategory switch
            {
                TabCategoryEnum.GraphicEditor => new GeometryViewModel(tabMenu.Id),
                TabCategoryEnum.Graphs        => new GraphsViewModel(tabMenu.Id),

                //TO DO: остальные типы...
                _ => throw new ArgumentOutOfRangeException("Tab create invalid")
            }; 
        }

        #endregion
    }
}
