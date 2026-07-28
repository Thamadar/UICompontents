using Avalonia;
using Avalonia.Controls.Primitives; 

namespace Lib.Avalonia.Controls
{
    public class SchedulerRowHeader : TemplatedControl
    {
        public static readonly StyledProperty<string> DisplayTimeProperty =
        AvaloniaProperty.Register<SchedulerRowHeader, string>(nameof(DisplayTime));

        public static readonly StyledProperty<bool> IsLastItemProperty =
        AvaloniaProperty.Register<SchedulerRowHeader, bool>(nameof(IsLastItem));

        public string DisplayTime
        {
            get => GetValue(DisplayTimeProperty);
            set => SetValue(DisplayTimeProperty, value);
        }

        /// <summary>
        /// Является ли данный объект последним в коллекции?
        /// </summary>
        public bool IsLastItem
        {
            get => GetValue(IsLastItemProperty);
            set => SetValue(IsLastItemProperty, value);
        }
    }
}
