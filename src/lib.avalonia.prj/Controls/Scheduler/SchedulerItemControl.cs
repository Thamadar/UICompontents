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

        public SchedulerItemControl()
        {
            this.GetObservable(IsSelectedProperty)
                .Subscribe(OnIsSelectedChanged);
        } 

        protected override Size MeasureOverride(Size availableSize)
        {
            return availableSize;
        } 

        private void OnIsSelectedChanged(bool isSelected)
            => PseudoClasses.Set(":selected", isSelected);
    }
}
