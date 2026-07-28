using Lib.Avalonia.Services.Dialogs.ViewDialog; 

namespace Client.Avalonia.Views.Tabs.Scheduler.Dialogs
{
    public class SchedulerEventItemDialogParameters : ViewDialogParameters
    {
        public SchedulerEventItemDialogTypeEnum SchedulerEventItemDialogType { get; }

        /// <summary>
        /// Параметры MessageBox
        /// </summary>
        /// <param name="schedulerEventItemDialogType">тип SchedulerEventItemDialog'а.</param>
        /// <param name="header">заголовок.</param>
        /// <param name="message">сообщение. Может быть пустым.</param>
        /// <param name="ignoreClickOut">игнорировать ли нажатия вне области? По умолчанию true.</param>
        /// <param name="hardCloseWarning">включить ли функцию "Отображать предупреждение Yes/No при попытке закрыть?". По умолчанию false.</param> 
        public SchedulerEventItemDialogParameters(
            SchedulerEventItemDialogTypeEnum schedulerEventItemDialogType,
            string header,
            string? message = null,
            bool ignoreClickOut = true,
            bool hardCloseWarning = false)
            : base(header, message, ignoreClickOut, hardCloseWarning)
        {
            SchedulerEventItemDialogType = schedulerEventItemDialogType;
        }
    }
}
