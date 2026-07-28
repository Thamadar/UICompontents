using Avalonia.Controls;
using Client.Avalonia.Views.Tabs.Scheduler.Dialogs; 
using Lib.Avalonia.Services.Dialogs.ViewDialog;
using ReactiveUI; 

namespace Client.Avalonia.Views.Tabs.Scheduler 
{
    /// <summary>
    /// Панель создания/редактирования элементов события.
    /// Пока что не добавлена. Отложена работа из-за избыточности реализации...
    /// </summary>
    public sealed partial class SchedulerEventItemDialogViewModel
        : BaseViewDialogViewModel<SchedulerEventItemDialogParameters, SchedulerEventItemDialogResult>
    {
        #region Fields

	    private SchedulerEventItemDialogTypeEnum _schedulerEventItemDialogType;

        #endregion

        #region Properties

        /// <summary>
        /// Тип SchedulerEventItemDialogType.
        /// </summary>
        public SchedulerEventItemDialogTypeEnum SchedulerEventItemDialogType
        {
            get => _schedulerEventItemDialogType;
            private set => this.RaiseAndSetIfChanged(ref _schedulerEventItemDialogType, value);
        }

        #endregion

        #region Constructors

        public SchedulerEventItemDialogViewModel()
              : base()
        {
            if(Design.IsDesignMode)
            {
                var messageBoxParameter = new SchedulerEventItemDialogParameters(
                    SchedulerEventItemDialogTypeEnum.Create,
                    "SomeTest",
                    "Какой-то текст");

                new Thread(() =>
                {
                    _ = Open(messageBoxParameter);
                }).Start();
            }
        }

        #endregion

        #region Methods   

        /// <inheritdoc/>
        public override Task<SchedulerEventItemDialogResult> Open(SchedulerEventItemDialogParameters data)
        {
            SchedulerEventItemDialogType = data.SchedulerEventItemDialogType;

            return base.Open(data);
        }

        /// <inheritdoc/>
        public override async Task HardClose(bool ignoreHardCloseWarning = false)
        {
            if(!await OpenHardCloseWarning(ignoreHardCloseWarning))
            {
                return;
            }

            await Close(new SchedulerEventItemDialogResult(DialogResultEnum.Close));
        }


        #endregion
    }
}
