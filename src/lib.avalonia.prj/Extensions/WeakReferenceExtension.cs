using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.Avalonia.Extensions
{
    public static class WeakReferenceExtension
    { 
        public static bool TryGetTargetOrRemove<T>(this List<WeakReference<T>> value, out T resultTarget, int index)
            where T : class
        {
            if(!value[index].TryGetTarget(out resultTarget))
            {
                value.RemoveAt(index);
                return false;
            }
            return true;
        }
    }
}
