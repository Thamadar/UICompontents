using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Lib.Avalonia.Controls.Helpers;
using System;
using System.Windows.Input;

namespace Lib.Avalonia.Controls
{
    /// <summary>
    /// Компонент, отрисовывающий событие на холсте.
    /// </summary>
    [PseudoClasses(":selected")]
    public class SchedulerItemControl : TemplatedControl
    {
        public static readonly StyledProperty<ICommand> ClickSchedulerCommandProperty =
        AvaloniaProperty.Register<SchedulerItemControl, ICommand>(nameof(ClickSchedulerCommand));

        public static readonly StyledProperty<bool> IsSelectedProperty =
            AvaloniaProperty.Register<SchedulerItemControl, bool>(nameof(IsSelected));
        
        /// <summary>
        /// Выбран ли данный элемент?
        /// </summary>
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        /// <summary>
        /// Нажатие по элементу SchedulerItemControl. ПередаетISchedulerEventItem
        /// </summary>
        public ICommand ClickSchedulerCommand
        {
            get => GetValue(ClickSchedulerCommandProperty);
            set => SetValue(ClickSchedulerCommandProperty, value);
        }

        public SchedulerItemControl()
        {
            this.GetObservable(IsSelectedProperty)
                .Subscribe(OnIsSelectedChanged);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            this.PointerPressed += OnItemPointerPressed;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            this.PointerPressed -= OnItemPointerPressed;
        }

        protected override Size MeasureOverride(Size availableSize)
        { 
            return availableSize;
        }

        /// <summary>
        /// Реакция на нажатие по элементу.
        /// </summary> 
        private void OnItemPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if(DataContext is ISchedulerEventItem scheduler)
            { 
                ClickSchedulerCommand?.Execute(scheduler);
                e.Handled = true;
            }  
        }

        private void OnIsSelectedChanged(bool isSelected)
            => PseudoClasses.Set(":selected", isSelected);
    }
}
