using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WPF.Extensions
{
    public static class DisposeExtension
    {
        public static void AddTo(this IDisposable item, ICollection<IDisposable> collection)
        {
            if(!collection.Contains(item))
            {
                collection.Add(item);
            }
        }
        public static void DisposeAll(this ICollection<IDisposable> disposables)
        {
            disposables
                .ToList()
                .ForEach(x => x.Dispose());
            disposables.Clear();
        }
    }

}
