using Lib.Avalonia.Controls.Helpers; 
using System.Collections.Generic; 

namespace Lib.Avalonia.Comparers
{
    public class SchedulerEventItemIdComparer : IEqualityComparer<ISchedulerEventItem>
    {
        public bool Equals(ISchedulerEventItem? x, ISchedulerEventItem? y)
            => ReferenceEquals(x, y) || (x is not null && y is not null && x.Id == y.Id);

        public int GetHashCode(ISchedulerEventItem obj)
            => obj.Id.GetHashCode();
    }
}
