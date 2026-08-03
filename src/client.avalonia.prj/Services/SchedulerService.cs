using Avalonia.Media;
using Client.Avalonia.Services.Interfaces;
using DynamicData;
using Lib.Avalonia.Controls.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Client.Avalonia.Services
{
    /// <summary>
    /// Сервис по управлению событиями во вкладке "Планировщик событий".
    /// </summary>
    public class SchedulerService : ISchedulerService
    {

        #region Fields

        private static readonly Lazy<ISchedulerService> _instance = new Lazy<ISchedulerService>(() => new SchedulerService()); 
        private readonly ISourceList<ISchedulerEventItem> _totalSchedulerEventItems;

        private readonly Subject<ISchedulerEventItem?> _currentSelectedSchedulerEventItemSubject; 

        //пул цветов для рандомайзера цвета у нового события. Скорее при дальнейшее разработке будет убрано, так как
        //сразу можно будет настроить цвет события из панели создания/редактирования.
        private static readonly IEnumerable<IBrush> DefaultBrushes = new List<IBrush>
        {
           SolidColorBrush.Parse("#a1adff"),
           SolidColorBrush.Parse("#71d079"),
           SolidColorBrush.Parse("#9a7bd4"),
           SolidColorBrush.Parse("#f7607f"),
           SolidColorBrush.Parse("#ebb46d"),
           SolidColorBrush.Parse("#76c0db"),
           SolidColorBrush.Parse("#d67fc5"),
           SolidColorBrush.Parse("#e7dda4"),
           SolidColorBrush.Parse("#ada4e7")

        };
         
        private ISchedulerEventItem? _currentSelectedSchedulerEventItem;

        #endregion

        #region Properties

        /// <summary>
        /// Экземпляр.
        /// </summary>
        public static ISchedulerService Instance => _instance.Value;

        /// <inheritdoc/>
        public IObservable<ISchedulerEventItem?> CurrentSelectedSchedulerEventItemObservable
            => _currentSelectedSchedulerEventItemSubject.AsObservable();

        /// <summary>
        /// Текущего выбранного события.
        /// </summary>
        public ISchedulerEventItem? CurrentSelectedSchedulerEventItem
        {
            get => _currentSelectedSchedulerEventItem;
            private set
            {
                _currentSelectedSchedulerEventItem = value;
                _currentSelectedSchedulerEventItemSubject.OnNext(value);
            }
        }

        #endregion

        #region Constructors

        public SchedulerService()
        {
            _totalSchedulerEventItems                 = new SourceList<ISchedulerEventItem>();
            _currentSelectedSchedulerEventItemSubject = new Subject<ISchedulerEventItem?>();

            LoadTestData();
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public IObservable<IChangeSet<ISchedulerEventItem>> ConnectToTotalSchedulerEventItems()
        {
            return _totalSchedulerEventItems.Connect();
        }

        /// <inheritdoc/>
        public void AddSchedulerItem(SchedulerDateTimeRange dateTimeRange)
        {
            //TO DO: сделать открывающуюся панель редактирования для нового события, нежели просто добавлять пустые данные.
            //P.S. просто добавляются данные, так как для демонстрации UI/UX вполне достаточно.

            var currentCount = _totalSchedulerEventItems.Count + 1; 
            var rnd = new Random();
            var rndBrush = DefaultBrushes.ElementAt(rnd.Next(DefaultBrushes.Count()));

            var newSchedulerItem = new SchedulerEventItem(dateTimeRange.Start, dateTimeRange.End, $"Событие {currentCount}", "какое-то описание", rndBrush);
             
            if(_totalSchedulerEventItems.Items.Any(x => x.StartDate == newSchedulerItem.StartDate && x.EndDate == newSchedulerItem.EndDate))
            {
                //Пока что идут наслоения, поэтому убран такой сценарий. TO DO: В будущем сделать в SchedulerControl, а именно в панеле, чтобы
                //в таком случае создавался визуально Stack, отображающий все события в том же промежутке.
                return;
            }

            _totalSchedulerEventItems.Add(newSchedulerItem);
            SelectSchedulerItem(newSchedulerItem);
        }

        /// <inheritdoc/>
        public void ChangeDateTimeRangeSchedulerItem(SchedulerDateTimeRange dateTimeRange)
        {
            var changeItem = _totalSchedulerEventItems.Items.FirstOrDefault(x => x.Id == dateTimeRange.ItemId);
            if(changeItem != null)
            {
                changeItem.ChangeDateTimeRange(dateTimeRange.Start, dateTimeRange.End);
                SelectSchedulerItem(changeItem);
            } 
        }

        /// <inheritdoc/>
        public void RemoveSchedulerItem(ISchedulerEventItem schedulerEventItem)
        {
            _totalSchedulerEventItems.Remove(schedulerEventItem);
            SelectSchedulerItem(null);
        }

        /// <inheritdoc/>
        public void SelectSchedulerItem(ISchedulerEventItem? schedulerEventItem)
        { 
            if(schedulerEventItem != null &&
                _totalSchedulerEventItems.Items.Any(x => x.Id == schedulerEventItem.Id))
            {
                CurrentSelectedSchedulerEventItem = schedulerEventItem;
            }
            else
            {
                CurrentSelectedSchedulerEventItem = null;
            }
        }

        /// <summary>
        /// Подзагрузка тестовых данных (события) для демонстрации...
        /// </summary>
        private void LoadTestData()
        {
            var testList = new List<ISchedulerEventItem>()
            {
                new SchedulerEventItem(DateTime.Today, DateTime.Today.AddMinutes(30), "Событие 1", "Some description", SolidColorBrush.Parse("#a1adff")),
                new SchedulerEventItem(DateTime.Today.AddHours(1), DateTime.Today.AddHours(1).AddMinutes(60), "Событие 2", "Some description", SolidColorBrush.Parse("#71d079")),
                new SchedulerEventItem(DateTime.Today.AddHours(5), DateTime.Today.AddHours(5).AddMinutes(90), "Событие 3", "Some description", SolidColorBrush.Parse("#71d079")),
                new SchedulerEventItem(DateTime.Today.AddHours(8), DateTime.Today.AddHours(8).AddMinutes(90), "Собеседование", "Some description", SolidColorBrush.Parse("#9a7bd4")),
                new SchedulerEventItem(DateTime.Today.AddHours(10), DateTime.Today.AddHours(10).AddHours(6), "Разработка проекта UIComponents", "Some description", SolidColorBrush.Parse("#9a7bd4")),
                new SchedulerEventItem(DateTime.Today.AddHours(22), DateTime.Today.AddHours(24), "Прочитать Конан Дойля перед сном", "Some description", SolidColorBrush.Parse("#f7607f")),

                new SchedulerEventItem(DateTime.Today.AddDays(1), DateTime.Today.AddDays(1).AddMinutes(30), "Событие 1", "Some description", SolidColorBrush.Parse("#a1adff")),
                new SchedulerEventItem(DateTime.Today.AddDays(1).AddHours(1), DateTime.Today.AddDays(1).AddHours(1).AddMinutes(60), "Событие 2", "Some description", SolidColorBrush.Parse("#71d079")),
                new SchedulerEventItem(DateTime.Today.AddDays(1).AddHours(5), DateTime.Today.AddDays(1).AddHours(5).AddMinutes(90), "Событие 3", "Some description", SolidColorBrush.Parse("#f7607f")),
                new SchedulerEventItem(DateTime.Today.AddDays(1).AddHours(10), DateTime.Today.AddDays(1).AddHours(10).AddMinutes(90), "Собеседование", "Some description", SolidColorBrush.Parse("#9a7bd4")),

                new SchedulerEventItem(DateTime.Today.AddDays(2), DateTime.Today.AddDays(2).AddMinutes(30), "Событие 1", "Some description", SolidColorBrush.Parse("#a1adff")),
                new SchedulerEventItem(DateTime.Today.AddDays(2).AddHours(1), DateTime.Today.AddDays(2).AddHours(1).AddMinutes(60), "Событие 2", "Some description", SolidColorBrush.Parse("#71d079")),
                new SchedulerEventItem(DateTime.Today.AddDays(2).AddHours(5), DateTime.Today.AddDays(2).AddHours(5).AddMinutes(90), "Событие 3", "Some description", Brushes.DimGray),
                new SchedulerEventItem(DateTime.Today.AddDays(2).AddHours(22), DateTime.Today.AddDays(2).AddHours(24), "Дочитать первый том Конан Дойля перед сном", "Some description", SolidColorBrush.Parse("#f7607f")),

                new SchedulerEventItem(DateTime.Today.AddDays(5), DateTime.Today.AddDays(5).AddHours(12), "Сон", "Some description", SolidColorBrush.Parse("#ebb46d"))
            };

            _totalSchedulerEventItems.AddRange(testList);
        }

        #endregion

    }
}
