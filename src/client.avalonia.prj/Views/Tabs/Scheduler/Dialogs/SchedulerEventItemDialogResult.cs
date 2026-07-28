using Lib.Avalonia.Services.Dialogs.ViewDialog; 

namespace Client.Avalonia.Views.Tabs.Scheduler 
{
    public class SchedulerEventItemDialogResult : ViewDialogDataResult
    {
        public SchedulerEventItemDialogResult(DialogResultEnum dialogResult)
            : base(dialogResult)
        { }
    }
}
