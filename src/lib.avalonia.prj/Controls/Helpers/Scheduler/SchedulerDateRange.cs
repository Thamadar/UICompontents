using System; 

namespace Lib.Avalonia.Controls.Helpers
{
    public record SchedulerDateTimeRange(DateTime Start, DateTime End, Guid? ItemId = null);
}
