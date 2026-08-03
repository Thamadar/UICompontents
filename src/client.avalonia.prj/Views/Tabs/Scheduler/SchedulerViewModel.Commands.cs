using Lib.Avalonia.Controls.Helpers;
using ReactiveUI;
using System.Windows.Input;

namespace Client.Avalonia.Views.Scheduler
{
    public sealed partial class SchedulerViewModel
    {
        public sealed class SchedulerViewModelCommands
        {
            public ICommand CreateSchedulerEventItemCommand { get; }
            public ICommand EditSchedulerEventItemCommand { get; } 
            public ICommand ClickSchedulerEventItemCommand { get; }
            public ICommand RemoveEventPanelCommand { get; } 
            //public ICommand OpenCreateEventPanelCommand { get; }

            public SchedulerViewModelCommands(SchedulerViewModel vm)
            {
                CreateSchedulerEventItemCommand = ReactiveCommand.Create<SchedulerDateTimeRange>(vm.OnCreateEventPanel);
                EditSchedulerEventItemCommand   = ReactiveCommand.Create<SchedulerDateTimeRange>(vm.OnChangeDateTimeRangeEventPanel);
                ClickSchedulerEventItemCommand  = ReactiveCommand.Create<ISchedulerEventItem>(vm.OnClickSchedulerEventItem);

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
        private void OnCreateEventPanel(SchedulerDateTimeRange dateTimeRange)
        {
            _schedulerService.AddSchedulerItem(dateTimeRange);
        }

        /// <summary>
        /// Изменение StartDate и EndDate у существующего события.
        /// </summary> 
        private void OnChangeDateTimeRangeEventPanel(SchedulerDateTimeRange dateTimeRange)
        {
            _schedulerService.ChangeDateTimeRangeSchedulerItem(dateTimeRange);
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
