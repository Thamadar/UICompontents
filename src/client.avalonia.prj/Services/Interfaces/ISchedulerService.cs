using DynamicData;
using Lib.Avalonia.Controls.Helpers;

namespace Client.Avalonia.Services.Interfaces
{
    public interface ISchedulerService
    {
        /// <summary> 
        /// Подключение к списку всех элементов событий.
        /// </summary> 
        IObservable<IChangeSet<ISchedulerEventItem>> ConnectToTotalSchedulerEventItems();


        /// <summary>
        /// Отслеживание текущего выбранного события.
        /// </summary>
        IObservable<ISchedulerEventItem?> CurrentSelectedSchedulerEventItemObservable { get; }

        /// <summary>
        /// Добавить новое пустое событие.
        /// </summary>
        /// <param name="dateTimeRange">промежуток дата-время события.</param>
        void AddSchedulerItem(SchedulerDateTimeRange dateTimeRange);

        /// <summary>
        /// Изменение StartDate и EndDate у существующего события.
        /// </summary>
        /// <param name="dateTimeRange">промежуток дата-время события.</param>
        void ChangeDateTimeRangeSchedulerItem(SchedulerDateTimeRange dateTimeRange);

        /// <summary>
        /// Удалить событие.
        /// </summary> 
        void RemoveSchedulerItem(ISchedulerEventItem schedulerEventItem);

        /// <summary>
        /// Произвести выбор события.
        /// </summary>
        /// <param name="schedulerEventItem">выбранное событие</param>
        void SelectSchedulerItem(ISchedulerEventItem schedulerEventItem);
    }
}
