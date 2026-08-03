using Avalonia;
using Avalonia.Controls; 
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Lib.Avalonia.Comparers;
using Lib.Avalonia.Controls.Helpers;
using Lib.Avalonia.Extensions;
using System;
using System.Collections.Generic; 
using System.Windows.Input; 

namespace Lib.Avalonia.Controls
{ 

    /// <summary>
    /// Тип действия по нажатию по полотну планировщика: создание события, перенос события по курсору, ...
    /// </summary>
    public enum SchedulerActionPressEventTypeEnum
    {
        None,
        /// <summary>
        /// Действие: создание нового элемента SchedulerItemControl, путем нажатия (Released) мышью по полотну
        /// </summary>
        CreateEvent,
        /// <summary>
        /// Действие: изменение даты-время выбранного элемента SchedulerItemControl, путем его перемещения с зажатой мыши (Pressed, Move) по полотну.
        /// </summary>
        ChangeDate
    }

    internal class SchedulerContentPanelControl : Panel
    {
        #region Fields

        private readonly Dictionary<ISchedulerEventItem, SchedulerItemControl> _itemsStorage = new (new SchedulerEventItemIdComparer());
        private readonly SchedulerBackgroundControl _schedulerBackgroundControl = new SchedulerBackgroundControl();

        //Значение суммы пикселей переноса мыши после зажатия ЛКМ по координатам элемента, превышая которое активируется ChangeDate.
        private const double CHANGE_DATE_OFFSET_ITEM_SUM_THRESHOLD = 5;

        //Текущий тип исполняемого действия.
        private SchedulerActionPressEventTypeEnum _currentActionType;
        //Текущее редактируемое событие.
        private SchedulerItemControl? _currentSchedulerItemEdit;
        //Нужен для демонстрации переноса по полотну объекта при попытке сменить дату.
        private SchedulerItemControl? _currentChangeDateItem;

        private bool _layoutDirty;
        //private bool _isDrag; 
        private Point? _pointPressed;
        

        #endregion

        #region Properties 


        public static readonly StyledProperty<ICommand> ClickSchedulerCommandProperty =
        AvaloniaProperty.Register<SchedulerItemControl, ICommand>(nameof(ClickSchedulerCommand)); 

        public static readonly StyledProperty<ICommand> CreateSchedulerEventItemCommandProperty =
        AvaloniaProperty.Register<SchedulerItemControl, ICommand>(nameof(CreateSchedulerEventItemCommand));

        public static readonly StyledProperty<ICommand> EditSchedulerEventItemCommandProperty =
        AvaloniaProperty.Register<SchedulerItemControl, ICommand>(nameof(EditSchedulerEventItemCommand));

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
        /// Нажатие по элементу SchedulerItemControl. Передает ISchedulerEventItem
        /// </summary>
        public ICommand ClickSchedulerCommand
        {
            get => GetValue(ClickSchedulerCommandProperty);
            set => SetValue(ClickSchedulerCommandProperty, value);
        }

        /// <summary>
        /// Вызов создания SchedulerEventItem. Передает SchedulerDateTimeRange.
        /// </summary>
        public ICommand CreateSchedulerEventItemCommand
        {
            get => GetValue(CreateSchedulerEventItemCommandProperty);
            set => SetValue(CreateSchedulerEventItemCommandProperty, value);
        }

        /// <summary>
        /// Вызов редактирования SchedulerEventItem. Передает SchedulerDateTimeRange.
        /// </summary>
        public ICommand EditSchedulerEventItemCommand
        {
            get => GetValue(EditSchedulerEventItemCommandProperty);
            set => SetValue(EditSchedulerEventItemCommandProperty, value);
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

            this.AddHandler(SchedulerContentPanelControl.PointerMovedEvent,    OnBackgroundPointerMoved,    RoutingStrategies.Tunnel);
            this.AddHandler(SchedulerContentPanelControl.PointerPressedEvent,  OnBackgroundPointerPressed,  RoutingStrategies.Tunnel);
            this.AddHandler(SchedulerContentPanelControl.PointerReleasedEvent, OnBackgroundPointerReleased, RoutingStrategies.Tunnel);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            this.RemoveHandler(SchedulerContentPanelControl.PointerMovedEvent,    OnBackgroundPointerMoved);
            this.RemoveHandler(SchedulerContentPanelControl.PointerPressedEvent,  OnBackgroundPointerPressed);
            this.RemoveHandler(SchedulerContentPanelControl.PointerReleasedEvent, OnBackgroundPointerReleased);
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

            //_currentChangeDateItem?.Arrange(_currentChangeDateItem.Bounds);
            _schedulerBackgroundControl.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
           
            _layoutDirty = false;
            return finalSize;
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
                var yTopMargin  = Convert.ToDouble((schedulerEventItem.StartDate.Hour * 60 + schedulerEventItem.StartDate.Minute) * PixelsAtMinute);

                return new Rect(xLeftMargin, yTopMargin, size.Width, size.Height);
            }
        }  

        /// <summary>
        /// Реакция на движение мыши по полотну.
        /// </summary> 
        private void OnBackgroundPointerMoved(object? sender, PointerEventArgs e)
        {
            switch(_currentActionType)
            {
                case SchedulerActionPressEventTypeEnum.ChangeDate:
                    {
                        if(_currentSchedulerItemEdit != null && _pointPressed != null)
                        {
                            var currentPoint = e.GetPosition(this);
                            var pixelsDiff   = Math.Abs(_pointPressed.Value.X - currentPoint.X) + Math.Abs(_pointPressed.Value.Y - currentPoint.Y);
                            if(_currentChangeDateItem == null && pixelsDiff > CHANGE_DATE_OFFSET_ITEM_SUM_THRESHOLD)
                            {
                                var originalItemPoint = e.GetPosition(_currentSchedulerItemEdit);
                                CreateAddCurrentChangeDateItem(originalItemPoint); 
                            }

                            if(_currentChangeDateItem != null)
                            { 
                                var changeItemPoint  = e.GetPosition(_currentChangeDateItem);
                                var resultXTransform = (_currentChangeDateItem.RenderTransform?.Value.M31 ?? 0) + changeItemPoint.X - (_currentChangeDateItem.Bounds.Width  / 2);
                                var resultYTransform = (_currentChangeDateItem.RenderTransform?.Value.M32 ?? 0) + changeItemPoint.Y - (_currentChangeDateItem.Bounds.Height / 2);

                                _currentChangeDateItem.RenderTransform = new TranslateTransform(resultXTransform, resultYTransform);
                            }  
                            e.Handled = true;
                        }
                        break;
                    }
            } 
        }

        /// <summary>
        /// Реакция на нажатия мыши по полотну.
        /// </summary> 
        private void OnBackgroundPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            ResetAction();

            _pointPressed = e.GetPosition(this); 
            var hitElement = this.InputHitTest(_pointPressed.Value) as Control;
            var itemControl = hitElement?.GetParent<SchedulerItemControl>();
            if(itemControl != null)
            {
                SchedulerItemControlPressed(itemControl, e); 
            }
            else
            { 
                _currentActionType = SchedulerActionPressEventTypeEnum.CreateEvent;

                e.Handled = true;
            }
        }

        /// <summary>
        /// Реакция на отжатие мыши по полотну.
        /// </summary> 
        private void OnBackgroundPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            switch(_currentActionType)
            {
                case SchedulerActionPressEventTypeEnum.ChangeDate:
                    {
                        ChangeDateReleased(sender, e); 

                        break;
                    }
                case SchedulerActionPressEventTypeEnum.CreateEvent:
                    {
                        CreateEventReleased(sender, e);
                        break;
                    }
            }

            ResetAction(); 
        }

        /// <summary>
        /// Сброс всех данных, которые необходимы для работы с действием по полотну: нажатие, отжатие мыши и передвижение.
        /// </summary>
        private void ResetAction()
        {
            if(_currentSchedulerItemEdit != null)
            {
                _currentSchedulerItemEdit.Opacity = 1;
            }
            if(_currentChangeDateItem != null)
            {
                this.Children.Remove(_currentChangeDateItem);
            }

            _pointPressed             = null; 
            _currentSchedulerItemEdit = null;
            _currentChangeDateItem    = null;
            _currentActionType        = SchedulerActionPressEventTypeEnum.None;
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

        /// <summary>
        /// Создание и добавление редактируемого события на полотно, дабы не воздействовать на оригинал.
        /// </summary>
        private void CreateAddCurrentChangeDateItem(Point pointAtItem)
        {
            if(_currentSchedulerItemEdit != null &&
               _currentSchedulerItemEdit.DataContext is ISchedulerEventItem currentItemDataContext)
            { 
                //Создаем временный Item, который будем перемещать по полотну, вместо оригинала.
                _currentChangeDateItem = new SchedulerItemControl
                {
                    IsHitTestVisible = false,
                    Focusable = false,
                    DataContext = currentItemDataContext.Clone()
                };
                this.Children.Add(_currentChangeDateItem);

                _currentChangeDateItem.Arrange(new Rect(
                    _currentSchedulerItemEdit.Bounds.X + ((-_currentSchedulerItemEdit.Bounds.Width / 2) + pointAtItem.X),
                    _currentSchedulerItemEdit.Bounds.Y + ((-_currentSchedulerItemEdit.Bounds.Height / 2) + pointAtItem.Y),
                    _currentSchedulerItemEdit.Bounds.Width,
                    _currentSchedulerItemEdit.Bounds.Height));

                //Задать PseudoClass: changeDate?
                _currentSchedulerItemEdit.Opacity = 0.5;
            }
        }

        /// <summary>
        /// Реакция на нажатие по UI-элементу SchedulerItemControl.
        /// </summary> 
        private void SchedulerItemControlPressed(SchedulerItemControl itemControl, PointerPressedEventArgs e)
        { 
            if(itemControl.DataContext is ISchedulerEventItem currentItemDataContext)
            {
                _currentSchedulerItemEdit = itemControl;
                _currentActionType        = SchedulerActionPressEventTypeEnum.ChangeDate;

                ClickSchedulerCommand?.Execute(currentItemDataContext);

                e.Handled = true;
            }
        } 

        /// <summary>
        /// Исполнение реакции на отжатие мыши для типа SchedulerActionPressEventTypeEnum.ChangeDate (изменение даты выбранного SchedulerItemControl).
        /// </summary> 
        private void ChangeDateReleased(object? sender, PointerReleasedEventArgs e)
        {
            if(_currentSchedulerItemEdit != null && _currentChangeDateItem != null &&
                _currentSchedulerItemEdit.DataContext is ISchedulerEventItem itemDataContext)
            {
                if(_currentChangeDateItem.RenderTransform != null)
                {
                    var timeStep = (int)StepTimeRange.TotalMinutes;
                    // Подсчет DateTime, основываясь на параметрах перемещаемого элемента и полотна.
                    var offsetX = (_currentChangeDateItem.Bounds.X + (_currentChangeDateItem.Bounds.Width / 2) + _currentChangeDateItem.RenderTransform.Value.M31);
                    var offsetY = (_currentChangeDateItem.Bounds.Y + _currentChangeDateItem.RenderTransform.Value.M32);

                    var indexItemDay = (int)Math.Floor((decimal)offsetX / PixelsAtDay);
                    // Вычисляем и округляем значение времени начала (startDate) В минутах в меньшую сторону, используя шаг.
                    // Пример:  StepTimeRange = TimeSpan.FromMinutes(30);
                    // 00:40 => 00:30; 00:20 => 00:00; 00:50 => 00:30;  
                    var minuteStartItem = (int)Math.Floor((decimal)offsetY / PixelsAtMinute / timeStep) * timeStep;
                    minuteStartItem     = minuteStartItem > 0 ? minuteStartItem : 0;

                    // Вычисляем и округляем значение времени конца (endDate) В минутах в бОльшую сторону, используя шаг.
                    // Пример:  StepTimeRange = TimeSpan.FromMinutes(30);
                    // 00:29 => 00:30; 00:20 => 00:30; 00:50 => 01:00;  
                    var minuteEndItem = (int)Math.Ceiling((decimal)(_currentSchedulerItemEdit.Bounds.Height + _currentSchedulerItemEdit.Margin.Top + _currentSchedulerItemEdit.Margin.Bottom) / PixelsAtMinute / timeStep) * timeStep;

                    var dateTimeStart = StartDate.AddDays(indexItemDay).AddMinutes(minuteStartItem);
                    var dateTimeEnd   = dateTimeStart.AddMinutes(minuteEndItem);
                    var dateTimeRange = new SchedulerDateTimeRange(dateTimeStart, dateTimeEnd, itemDataContext.Id);

                    EditSchedulerEventItemCommand?.Execute(dateTimeRange);
                } 

                e.Handled = true;
            }
        }

        /// <summary>
        /// Исполнение реакции на отжатие мыши для типа SchedulerActionPressEventTypeEnum.CreateEvent (создание нового элемента SchedulerItemControl).
        /// </summary> 
        private void CreateEventReleased(object? sender, PointerReleasedEventArgs e)
        {
            var point    = e.GetPosition(this);  
            var timeStep = (int)StepTimeRange.TotalMinutes;

            //подсчет DateTime, основываясь на координатах нажатиях полотна.
            var indexPressedDay = (int)Math.Floor((decimal)point.X / PixelsAtDay);
            var pressedTime = (decimal)point.Y / PixelsAtMinute;
            var resultPressedTime = (int)Math.Floor(pressedTime / timeStep) * timeStep;

            var startDate = StartDate.AddDays(indexPressedDay).AddMinutes(resultPressedTime);
            //TO DO: можно сделать так, чтобы можно было зажатием сразу создавать нужный временной диапазон события. Пока просто 60 минут
            var endDate = startDate.AddMinutes(60);

            var pressedDateTimeRange = new SchedulerDateTimeRange(startDate, endDate);

            CreateSchedulerEventItemCommand?.Execute(pressedDateTimeRange);

            e.Handled = true;
        }

        #endregion
    }
}
