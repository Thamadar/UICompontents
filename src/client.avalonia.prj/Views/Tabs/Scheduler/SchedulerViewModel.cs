using Avalonia.Input;
using Client.Avalonia.Services;
using Client.Avalonia.Services.Interfaces;
using Client.Avalonia.Views.Tabs.Geometry.Tools;
using DynamicData;
using Lib.Avalonia;
using Lib.Avalonia.Controls.Helpers;
using Lib.Avalonia.Extensions;
using Lib.Avalonia.Helpers;
using ReactiveUI;

namespace Client.Avalonia.Views.Scheduler
{
    public sealed partial class SchedulerViewModel : ViewModelBase, ITabVM
    {
        #region Fields

        private readonly ISchedulerService _schedulerService;

        private IEnumerable<IHotKey> _defaultTabHotKeys = new IHotKey[0];
        private ISchedulerEventItem? _currentSelectedSchedulerEventItem;

        #endregion

        #region Properties

        /// <summary>
        /// Коллекция события, отображаемых планировщиком.
        /// </summary>
        public IObservable<IChangeSet<ISchedulerEventItem>> SchedulerEventItemsObserve { get; }

        /// <summary>
        /// Текущее выбранное событие.
        /// </summary>
        public ISchedulerEventItem? CurrentSelectedSchedulerEventItem
        {
            get => _currentSelectedSchedulerEventItem;
            set => this.RaiseAndSetIfChanged(ref _currentSelectedSchedulerEventItem, value);
        }

        /// <summary>
        /// Дата, с которой начинает планировщик.
        /// </summary>
        public DateTime StartDateTime { get; }

        /// <inheritdoc/>
        public Guid Id { get; }

        #endregion

        #region .ctor

        /// <summary>
        /// Конструктор-заглушка, дабы Designer не падал.
        /// </summary>
        public SchedulerViewModel()
            : this(Guid.NewGuid())
        {

        }
        public SchedulerViewModel(Guid id)
        {
            _schedulerService = SchedulerService.Instance;

            Id = id; 
            StartDateTime = DateTime.Today;

            SchedulerEventItemsObserve = _schedulerService.ConnectToTotalSchedulerEventItems();

            _schedulerService
                .CurrentSelectedSchedulerEventItemObservable
                .BindTo(this, x => x.CurrentSelectedSchedulerEventItem)
                .AddTo(_disposables);

            InitDefaultHotKeys(); 
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

        #endregion

        /// <summary>
        /// Инициализация горячих клавиш данной вкладки.
        /// </summary>
        private void InitDefaultHotKeys()
        {
            _defaultTabHotKeys = new List<IHotKey>
            {
                new HotKey(Key.Delete, Commands.RemoveEventPanelCommand),
            };
        } 
    }
}
