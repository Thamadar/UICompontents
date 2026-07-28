using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System; 

namespace Lib.Avalonia.Controls
{
    public class SchedulerColumnHeader : TemplatedControl
    { 
        public static readonly StyledProperty<DateTime> DateProperty =
        AvaloniaProperty.Register<SchedulerColumnHeader, DateTime>(nameof(Date)); 

        public static readonly StyledProperty<bool> IsLastItemProperty =
        AvaloniaProperty.Register<SchedulerColumnHeader, bool>(nameof(IsLastItem));

        public DateTime Date
        {
            get => GetValue(DateProperty);
            set => SetValue(DateProperty, value);
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
