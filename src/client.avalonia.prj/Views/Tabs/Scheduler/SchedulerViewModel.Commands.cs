using Lib.Avalonia.Controls.Helpers;
using ReactiveUI;
using System.Windows.Input;

namespace Client.Avalonia.Views.Scheduler
{
    public sealed partial class SchedulerViewModel
    {
        public sealed class SchedulerViewModelCommands
        {
            public ICommand CreateEventPanelCommand { get; }
            public ICommand ClickSchedulerEventItemCommand { get; }
            public ICommand RemoveEventPanelCommand { get; } 
            //public ICommand OpenCreateEventPanelCommand { get; }

            public SchedulerViewModelCommands(SchedulerViewModel vm)
            {
                CreateEventPanelCommand        = ReactiveCommand.Create<DateTime>(vm.OnCreateEventPanel);
                ClickSchedulerEventItemCommand = ReactiveCommand.Create<ISchedulerEventItem>(vm.OnClickSchedulerEventItem);

                RemoveEventPanelCommand = ReactiveCommand.Create(vm.OnRemoveSelectedSchedulerEventItem);
            }
        }

        private SchedulerViewModelCommands? _commands;

        public SchedulerViewModelCommands Commands => _commands ??= new(this);

        #region Methods 

        ///// <summary>
        ///// Открытие панели добавления нового события.
        ///// </summary>
        ///// <param name="startDateTime">дата-время области, по которой произведено нажатие.</param>
        //private void OnOpenCreateEventPanel(DateTime startDateTime)
        //{
            
        //}

        /// <summary>
        /// Создание нового события.
        /// </summary> 
        private void OnCreateEventPanel(DateTime startDateTime)
        {
            _schedulerService.AddSchedulerItem(startDateTime);
        }

        /// <summary>
        /// Реакция на нажатие элемент UI - событие.
        /// </summary> 
        private void OnClickSchedulerEventItem(ISchedulerEventItem schedulerEventItem)
        {
            _schedulerService.SelectSchedulerItem(schedulerEventItem);
        } 

        /// <summary>
        /// Удаление текущего выбранного события.
        /// </summary>
        private void OnRemoveSelectedSchedulerEventItem()
        {
            var currentEventItem = CurrentSelectedSchedulerEventItem;
            if(currentEventItem != null)
            {
                _schedulerService.RemoveSchedulerItem(currentEventItem);
            }
        }
        #endregion
    }
}
