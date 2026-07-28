using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using DynamicData;
using Lib.Avalonia.Comparers;
using Lib.Avalonia.Controls.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Lib.Avalonia.Controls
{ 
    internal class SchedulerContentPanelControl : Panel
    {
        #region Fields

        private readonly Dictionary<ISchedulerEventItem, SchedulerItemControl> _itemsStorage = new (new SchedulerEventItemIdComparer());
        private readonly SchedulerBackgroundControl _schedulerBackgroundControl = new SchedulerBackgroundControl();

        private bool _layoutDirty;

        #endregion

        #region Properties 


        public static readonly StyledProperty<ICommand> ClickSchedulerCommandProperty =
        AvaloniaProperty.Register<SchedulerItemControl, ICommand>(nameof(ClickSchedulerCommand)); 

        public static readonly StyledProperty<ICommand> ClickPanelCommandProperty =
        AvaloniaProperty.Register<SchedulerItemControl, ICommand>(nameof(ClickPanelCommand));

        public static readonly StyledProperty<ISchedulerEventItem?> CurrentSelectedSchedulerEventItemProperty =
        AvaloniaProperty.Register<SchedulerItemControl, ISchedulerEventItem?>(nameof(CurrentSelectedSchedulerEventItem));

        /// <summary>
        /// Текущее выбранное событие. Может быть null.
        /// </summary>
        public ISchedulerEventItem? CurrentSelectedSchedulerEventItem
        {
            get => GetValue(CurrentSelectedSchedulerEventItemProperty);
            set => SetValue(CurrentSelectedSchedulerEventItemProperty, value);
        }

        /// <summary>
        /// Нажатие по элементу SchedulerItemControl. ПередаетISchedulerEventItem
        /// </summary>
        public ICommand ClickSchedulerCommand
        {
            get => GetValue(ClickSchedulerCommandProperty);
            set => SetValue(ClickSchedulerCommandProperty, value);
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
        /// Шаг временного промежутка, по которому можно менять SchedulerItemControl. Нужно для дальнейшего управления.
        /// Пример: 30 минут. Значит можно увеличить/уменьшить событие на этот шаг. 21:00 => 20:30 или 21:30.
        /// </summary>
        public TimeSpan StepTimeRange { get; private set; } = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Сколько по вертикале пикселей в одной минуте.
        /// </summary>
        public decimal PixelsAtMinute { get; private set; } = 1;

        /// <summary>
        /// Сколько по горизонтале пикселей в одном дне.
        /// </summary>
        public decimal PixelsAtDay { get; private set; } = 1;

        /// <summary>
        /// Дата, с которого начинается горизонтальная ось.
        /// </summary>
        public DateTime StartDate { get; private set; } = DateTime.Now;

        /// <summary>
        /// Сколько всего дней на горизонтальной оси.
        /// </summary>
        public int TotalDays { get; private set; } = 7;

        #endregion

        public SchedulerContentPanelControl()
        {
            this.GetObservable(CurrentSelectedSchedulerEventItemProperty)
                .Subscribe(OnCurrentSelectedSchedulerEventItemChanged);

            Children.Add(_schedulerBackgroundControl); 
        }

        #region Methods  

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            this.PointerPressed += OnBackgroundPointerPressed;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            this.PointerPressed -= OnBackgroundPointerPressed;
        }

        protected override Size MeasureOverride(Size availableSize)
        {  
            foreach(var keyValue in _itemsStorage)
            {
                keyValue.Value.Measure(CalculateSchedulerItemSize(keyValue.Key));
            }   

            _layoutDirty = false;
            return new Size(Convert.ToDouble(PixelsAtDay * TotalDays), Convert.ToDouble(PixelsAtMinute * 60 * 24));
        }

        protected override Size ArrangeOverride(Size finalSize)
        { 
            if(_itemsStorage.Count == 0)
                return finalSize;

            foreach(var keyValue in _itemsStorage)
            {
                keyValue.Value.Arrange(CalculateSchedulerItemRect(keyValue.Key, keyValue.Value.DesiredSize));
            }

            _schedulerBackgroundControl.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
           
            _layoutDirty = false;
            return finalSize;
        }
         
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == ClickSchedulerCommandProperty)
            {
                UpdateClickCommandSchedulerItemControls();
            }
        } 

        public void AddRangeItems(IEnumerable<ISchedulerEventItem> schedulerEventItems)
        {
            foreach(var item in schedulerEventItems)
                AddItem(item, true);

            if(_layoutDirty)
            {
                InvalidateMeasure();
                InvalidateArrange();
                _layoutDirty = false;
            }
        }

        public void RemoveRangeItems(IEnumerable<ISchedulerEventItem> schedulerEventItems)
        { 
            foreach(var item in schedulerEventItems)
                RemoveItem(item, true);
             
            if(_layoutDirty)
            {
                InvalidateMeasure();
                InvalidateArrange();
                _layoutDirty = false;
            }
        }

        public void AddItem(ISchedulerEventItem schedulerEventItem, bool ignoreMeasureArrange = false)
        {
            if(_itemsStorage.ContainsKey(schedulerEventItem))
                return;

            var control = new SchedulerItemControl 
            { 
                ClickSchedulerCommand = this.ClickSchedulerCommand,
                DataContext           = schedulerEventItem 
            };

            _itemsStorage[schedulerEventItem] = control;
            Children.Add(control);

            if(ignoreMeasureArrange)
            {
                _layoutDirty = true;
            }
            else
            {
                InvalidateMeasure();
                InvalidateArrange(); 
            } 
        }

        public void RemoveItem(ISchedulerEventItem schedulerEventItem, bool ignoreMeasureArrange = false)
        {
            if(!_itemsStorage.TryGetValue(schedulerEventItem, out var control))
                return;

            Children.Remove(control);
            _itemsStorage.Remove(schedulerEventItem);
             
            if(ignoreMeasureArrange)
            {
                _layoutDirty = true;
            }
            else
            {
                InvalidateMeasure();
                InvalidateArrange();
            }
        }

        public void Clear()
        {
            foreach(var keyValue in _itemsStorage)
            {
                Children.Remove(keyValue.Value);
                _itemsStorage.Remove(keyValue.Key);
            } 
            _layoutDirty = false;

            InvalidateMeasure();
            InvalidateArrange();
        }


        /// <summary>
        /// Задать параметры для полотна, отрисовывающего объекты событий, основанных на указанных данных значениях.
        /// </summary>
        /// <param name="stepTimeRange">шаг временного промежутка, по которому можно менять SchedulerItemControl</param>
        /// <param name="pixelsAtMinute">сколько по вертикале пикселей в одной минуте</param>
        /// <param name="pixelsAtDay">сколько по горизонтале пикселей в одном дне</param>
        /// <param name="startDate">дата, с которого начинается горизонтальная ось</param>
        /// <param name="totalDays">сколько всего дней на горизонтальной оси</param>
        public void SetSchedulerParameters(
            TimeSpan stepTimeRange,
            decimal pixelsAtMinute,
            decimal pixelsAtDay,
            DateTime startDate,
            int totalDays)
        {
            StepTimeRange  = stepTimeRange;
            PixelsAtMinute = pixelsAtMinute;
            PixelsAtDay    = pixelsAtDay;
            StartDate      = startDate;
            TotalDays      = totalDays;

            _schedulerBackgroundControl.CellWidth  = Convert.ToDouble(PixelsAtDay); 
            _schedulerBackgroundControl.CellHeight = Convert.ToDouble(PixelsAtMinute * 60);

            InvalidateMeasure();
            InvalidateArrange();
        }


        /// <summary>
        /// Высчитывание размера для SchedulerItemControl.
        /// </summary> 
        private Size CalculateSchedulerItemSize(ISchedulerEventItem schedulerEventItem)
        {
            var minutesPeriod = (int)(schedulerEventItem.EndDate - schedulerEventItem.StartDate).TotalMinutes;

            var itemWidth  = Convert.ToDouble(PixelsAtDay);
            var itemHeight = Convert.ToDouble(minutesPeriod * PixelsAtMinute); 

            return new Size(itemWidth, itemHeight);
        }

        /// <summary>
        /// Высчитывание расположения для SchedulerItemControl.
        /// </summary> 
        private Rect CalculateSchedulerItemRect(ISchedulerEventItem schedulerEventItem, Size size)
        {
            var minutesPeriod = (int)(schedulerEventItem.EndDate - schedulerEventItem.StartDate).TotalMinutes;
            int itemDayindex  = (int)(schedulerEventItem.StartDate - StartDate).TotalDays;

            if(itemDayindex < 0 || itemDayindex >= TotalDays)
            {
                return new Rect(0, 0, 0, 0);
            }
            else
            {
                var xLeftMargin = Convert.ToDouble(itemDayindex * PixelsAtDay);
                var yTopMargin = Convert.ToDouble((schedulerEventItem.StartDate.Hour * 60 + schedulerEventItem.StartDate.Minute) * PixelsAtMinute);

                return new Rect(xLeftMargin, yTopMargin, size.Width, size.Height);
            }
        } 

        /// <summary>
        /// Обновление ICommand у всех существующих SchedulerItemControl.
        /// </summary> 
        private void UpdateClickCommandSchedulerItemControls()
        {
            foreach(var keyValue in _itemsStorage)
            {
                keyValue.Value.ClickSchedulerCommand = ClickSchedulerCommand;
            }
        }

        /// <summary>
        /// Реакция на нажатие по полотну.
        /// </summary> 
        private void OnBackgroundPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var point    = e.GetPosition(this);  
            var timeStep = (int)StepTimeRange.TotalMinutes;

            //подсчет DateTime, основываясь на координатах нажатиях полотна.
            var indexPressedDay   = (int)Math.Floor((decimal)point.X / PixelsAtDay);
            var pressedTime       = (decimal)point.Y / PixelsAtMinute;
            var resultPressedTime = (int)Math.Floor(pressedTime / timeStep) * timeStep;

            var pressedDateTime = StartDate.AddDays(indexPressedDay).AddMinutes(resultPressedTime);

            ClickPanelCommand?.Execute(pressedDateTime);

            e.Handled = true;
        }

        /// <summary>
        /// Реакция на изменение текущего выбранного события.
        /// </summary>
        /// <param name="schedulerEventItem">выбранное событие</param>
        private void OnCurrentSelectedSchedulerEventItemChanged(ISchedulerEventItem? schedulerEventItem)
        {
            foreach(var item in _itemsStorage)
            {
                item.Value.IsSelected = schedulerEventItem != null && item.Key.Id == schedulerEventItem.Id;
            }
        }

        #endregion
    }
}
