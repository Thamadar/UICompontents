using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Interactivity;
using DynamicData;
using Lib.Avalonia.Controls.Helpers; 
using Lib.Avalonia.Extensions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows.Input;

namespace Lib.Avalonia.Controls
{
    [TemplatePart("PART_SchedulerContentPanelControl", typeof(SchedulerContentPanelControl))]
    [TemplatePart("PART_ContentScrollViewer", typeof(ScrollViewer))]
    [TemplatePart("PART_ColumnHeaderScrollViewer", typeof(ScrollViewer))]
    [TemplatePart("PART_RowHeaderScrollViewer", typeof(ScrollViewer))]
    [TemplatePart("PART_ColumnHeadersStackPanel", typeof(StackPanel))]
    [TemplatePart("PART_RowHeadersStackPanel", typeof(StackPanel))] 
    public class SchedulerControl : TemplatedControl
    {
        #region Controls

        private SchedulerContentPanelControl? _schedulerContentPanelControl;  
        private ScrollViewer? _contentScrollViewer;
        private ScrollViewer? _columnHeaderScrollViewer;
        private ScrollViewer? _rowHeaderScrollViewer;
        private StackPanel? _columnHeadersStackPanel;
        private StackPanel? _rowHeadersStackPanel;

        #endregion

        #region Fields

        private List<IDisposable> _collectionDisposable = new List<IDisposable>();

        private bool _areControlsAvailable;
        private bool _ignoreScrollEvent;

        //Сколько занимает пикселей минута по оси вертикали.
        private decimal _pixelsPerMinute;
        //Сколько занимает пикселей колона в один день.
        private decimal _pixelsPerDay;
        //Общее количество отображаемых дней на горизонтальной оси.
        private int _totalDays;

        #endregion

        #region Properties

        public static readonly StyledProperty<IObservable<IChangeSet<ISchedulerEventItem>>> SchedulerEventItemsObserveProperty =
            AvaloniaProperty.Register<SchedulerControl, IObservable<IChangeSet<ISchedulerEventItem>>>(nameof(SchedulerEventItemsObserve));

        public static readonly StyledProperty<ISchedulerEventItem?> CurrentSelectedSchedulerEventItemProperty =
        AvaloniaProperty.Register<SchedulerControl, ISchedulerEventItem?>(nameof(CurrentSelectedSchedulerEventItem));

        public static readonly StyledProperty<SchedulerPeriodEnum> SchedulerPeriodProperty =
        AvaloniaProperty.Register<SchedulerControl, SchedulerPeriodEnum>(nameof(SchedulerPeriod));

        public static readonly StyledProperty<DateTime> StartDateProperty =
        AvaloniaProperty.Register<SchedulerControl, DateTime>(nameof(StartDate));

        public static readonly StyledProperty<double?> SchedulerHeightProperty =
        AvaloniaProperty.Register<SchedulerControl, double?>(nameof(SchedulerHeight));

        public static readonly StyledProperty<ICommand> ClickPanelCommandProperty =
        AvaloniaProperty.Register<SchedulerControl, ICommand>(nameof(ClickPanelCommand));

        public static readonly StyledProperty<ICommand> ClickSchedulerCommandProperty =
        AvaloniaProperty.Register<SchedulerControl, ICommand>(nameof(ClickSchedulerCommand));

        /// <summary>
        /// Отслеживание коллекции объектов, хранящих информацию о событии в планировщике.
        /// </summary>
        public IObservable<IChangeSet<ISchedulerEventItem>> SchedulerEventItemsObserve
        {
            get => GetValue(SchedulerEventItemsObserveProperty);
            set => SetValue(SchedulerEventItemsObserveProperty, value);
        }

        /// <summary>
        /// Текущее выбранное событие. Может быть null.
        /// </summary>
        public ISchedulerEventItem? CurrentSelectedSchedulerEventItem
        {
            get => GetValue(CurrentSelectedSchedulerEventItemProperty);
            set => SetValue(CurrentSelectedSchedulerEventItemProperty, value);
        } 

        /// <summary>
        /// Временной период, отображаемый планировщиком.
        /// (пока рабочий только Week).
        /// </summary>
        public SchedulerPeriodEnum SchedulerPeriod
        {
            get => GetValue(SchedulerPeriodProperty);
            set => SetValue(SchedulerPeriodProperty, value);
        }

        /// <summary>
        /// Дата, с которого начинается горизонтальная ось.
        /// </summary>
        public DateTime StartDate
        {
            get => GetValue(StartDateProperty);
            set => SetValue(StartDateProperty, value);
        }

        /// <summary>
        /// Высота всего планировщика.
        /// </summary>
        public double? SchedulerHeight
        {
            get => GetValue(SchedulerHeightProperty);
            set => SetValue(SchedulerHeightProperty, value);
        }

        /// <summary>
        /// Нажатие по холсту. Передает DateTime.
        /// </summary>
        public ICommand ClickPanelCommand
        {
            get => GetValue(ClickPanelCommandProperty);
            set => SetValue(ClickPanelCommandProperty, value);
        }

        /// <summary>
        /// Нажатие по элементу SchedulerItemControl. ПередаетISchedulerEventItem
        /// </summary>
        public ICommand ClickSchedulerCommand
        {
            get => GetValue(ClickSchedulerCommandProperty);
            set => SetValue(ClickSchedulerCommandProperty, value);
        }

        #endregion

        #region Constructors

        public SchedulerControl()
        { 

        }

        #endregion

        #region Methods

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e); 
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e); 
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            _areControlsAvailable = false;

            base.OnApplyTemplate(e);

            _schedulerContentPanelControl = InitSchedulerContentPanelControl(e.NameScope.Find<SchedulerContentPanelControl>("PART_SchedulerContentPanelControl"));

            _contentScrollViewer      = InitScrollViewer(e.NameScope.Find<ScrollViewer>("PART_ContentScrollViewer"));
            _columnHeaderScrollViewer = InitScrollViewer(e.NameScope.Find<ScrollViewer>("PART_ColumnHeaderScrollViewer"));
            _rowHeaderScrollViewer    = InitScrollViewer(e.NameScope.Find<ScrollViewer>("PART_RowHeaderScrollViewer"));

            _columnHeadersStackPanel = e.NameScope.Find<StackPanel>("PART_ColumnHeadersStackPanel");
            _rowHeadersStackPanel    = e.NameScope.Find<StackPanel>("PART_RowHeadersStackPanel"); 

            _areControlsAvailable = true;

            OnColumnHeaderRender();
            OnRowHeaderRender();
             
            BindItems();

            //TO DO: отслеживать обновление, как StartDate. Пока что при изменении ничего не изменится.
            _schedulerContentPanelControl?.SetSchedulerParameters(
                TimeSpan.FromMinutes(30), //TO DO: вынести в StyledProperty?
                _pixelsPerMinute,
                _pixelsPerDay,
                StartDate,
                _totalDays); 
        }

        #region Init
         
        private SchedulerContentPanelControl? InitSchedulerContentPanelControl(SchedulerContentPanelControl? schedulerContentPanelControl)
        {
            if(schedulerContentPanelControl != null)
            {
                var clickPanelBind = new Binding(nameof(ClickPanelCommand))
                {
                    Source = this
                };
                var clickSchedulerBind = new Binding(nameof(ClickSchedulerCommand))
                {
                    Source = this
                };
                var currentSelectedSchedulerEvent= new Binding(nameof(CurrentSelectedSchedulerEventItem))
                {
                    Source = this
                };

                schedulerContentPanelControl.Bind(SchedulerContentPanelControl.ClickPanelCommandProperty, clickPanelBind);
                schedulerContentPanelControl.Bind(SchedulerContentPanelControl.ClickSchedulerCommandProperty, clickSchedulerBind);
                schedulerContentPanelControl.Bind(SchedulerContentPanelControl.CurrentSelectedSchedulerEventItemProperty, currentSelectedSchedulerEvent);

                return schedulerContentPanelControl;
            }
            return schedulerContentPanelControl; 
        }

        private ScrollViewer? InitScrollViewer(ScrollViewer? scrollViewer)
        {
            if(scrollViewer != null)
            {
                scrollViewer.ScrollChanged += OnAnyScrollChanged;
            }
            return scrollViewer;
        }

        #endregion

        /// <summary>
        /// Отрисовка подписей колон.
        /// </summary>
        private void OnColumnHeaderRender()
        { 
            switch(SchedulerPeriod)
            {
                //TO DO: Добавить остальные режимы.
                case SchedulerPeriodEnum.Week:
                    {
                        _pixelsPerDay = 270; 
                        _totalDays = 7;

                        break; 
                    } 
            }

            //Добавление дней по оси горизонтали. (Понедельник, Август 3), (Вторник, Август 4), ...
            _columnHeadersStackPanel?.Children.Clear();
            for(int i = 0; i < _totalDays; i++)
            {
                var schedulerColumnHeader = new SchedulerColumnHeader()
                {
                    Width = Convert.ToDouble(_pixelsPerDay),
                    Date  = this.StartDate + TimeSpan.FromDays(i),
                    IsLastItem = i + 1 >= _totalDays,
                    BorderBrush = this.BorderBrush
                };

                _columnHeadersStackPanel?.Children.Add(schedulerColumnHeader);
            }
        }

        /// <summary>
        /// Отрисовка подписей строк.
        /// </summary>
        private void OnRowHeaderRender()
        {
            var schedulerRowHeaderHeight = 88;
            var schedulerHeight = schedulerRowHeaderHeight * 24m;
            _pixelsPerMinute =  schedulerHeight / 24 / 60; 
            
            //Добавление 24 часов по оси вертикали. 00:00, 01:00, ...
            _rowHeadersStackPanel?.Children.Clear();
            for(int i = 0; i < 24; i++)
            {
                var schedulerRowHeader = new SchedulerRowHeader()
                {
                    DisplayTime = $"{i:00}:00",
                    Height = (double)Math.Ceiling(_pixelsPerMinute * 60),
                    IsLastItem = i+1 >= 24,
                    BorderBrush = this.BorderBrush
                };

                _rowHeadersStackPanel?.Children.Add(schedulerRowHeader);
            }
        }

        private void BindItems()
        {
            _collectionDisposable.DisposeAll();

            if(SchedulerEventItemsObserve != null)
            {
                SchedulerEventItemsObserve
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(changeSet =>
                {
                    foreach(var change in changeSet)
                    {
                        switch(change.Reason)
                        {
                            case ListChangeReason.AddRange:
                                _schedulerContentPanelControl?.AddRangeItems(change.Range);
                                break;
                            case ListChangeReason.Add:
                                _schedulerContentPanelControl?.AddItem(change.Item.Current);
                                break;
                            case ListChangeReason.RemoveRange:
                                _schedulerContentPanelControl?.RemoveRangeItems(change.Range);
                                break; 
                            case ListChangeReason.Remove:
                                _schedulerContentPanelControl?.RemoveItem(change.Item.Current);
                                break;
                            case ListChangeReason.Clear: 
                                _schedulerContentPanelControl?.Clear();
                                break;
                        }
                    }
                })
                .AddTo(_collectionDisposable);
            }
        }

        private void OnAnyScrollChanged(object? sender, ScrollChangedEventArgs e)
        {
            if(_ignoreScrollEvent)
                return;

            _ignoreScrollEvent = true;

            if(sender == _contentScrollViewer && _columnHeaderScrollViewer != null && _rowHeaderScrollViewer != null)
            {
                //Устанавливаем абсолютные значения Offset, синхронизируя с основным скроллом.
                _columnHeaderScrollViewer.Offset = _columnHeaderScrollViewer.Offset.WithX(_contentScrollViewer?.Offset.X ?? 0);
                _rowHeaderScrollViewer.Offset = _rowHeaderScrollViewer.Offset.WithY(_contentScrollViewer?.Offset.Y ?? 0);
            }
            else if(sender == _columnHeaderScrollViewer && _contentScrollViewer != null)
            {
                //Синхронизируем горизонтальный скролл основного контента.
                _contentScrollViewer.Offset = _contentScrollViewer.Offset.WithX(_columnHeaderScrollViewer?.Offset.X ?? 0);
            }
            else if(sender == _rowHeaderScrollViewer && _contentScrollViewer != null)
            {
                //Синхронизируем вертикальный скролл основного контента.
                _contentScrollViewer.Offset = _contentScrollViewer.Offset.WithY(_rowHeaderScrollViewer?.Offset.Y ?? 0);
            }

            _ignoreScrollEvent = false;
        } 

        #endregion
    }
}
