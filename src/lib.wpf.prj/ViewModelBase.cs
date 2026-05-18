using Lib.WPF.Extensions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WPF
{
    /// <summary>Базовый класс для ViewModel.</summary>
    public class ViewModelBase : ReactiveObject, IDisposable
    {
        private int _disposed;
        protected IList<IDisposable> _disposables = new List<IDisposable>();

        protected virtual void Dispose(bool disposing)
        {
            if(Interlocked.Exchange(ref _disposed, 1) == 1)
                return;
            try
            {
                _disposables.DisposeAll();
            }
            catch(Exception) { }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
