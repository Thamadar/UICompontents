using Lib.Avalonia.Services.Dialogs.ViewDialog;

using ReactiveUI; 

namespace Client.Avalonia.Views.Tabs.Scheduler
{
    public sealed partial class SchedulerEventItemDialogViewModel
    {
        public sealed class SchedulerEventItemDialogViewModelCommands
        {
            public IReactiveCommand AcceptCommand { get; }
            public IReactiveCommand CancelCommand { get; }

            public SchedulerEventItemDialogViewModelCommands(SchedulerEventItemDialogViewModel vm)
            {
                AcceptCommand = ReactiveCommand.Create(() => { vm.Close(new SchedulerEventItemDialogResult(DialogResultEnum.Accept)); });
                CancelCommand = ReactiveCommand.Create(() => { vm.Close(new SchedulerEventItemDialogResult(DialogResultEnum.Cancel)); });
            }
        }

        private SchedulerEventItemDialogViewModelCommands? _commands;

        public SchedulerEventItemDialogViewModelCommands Commands => _commands ??= new(this);
    }

}
