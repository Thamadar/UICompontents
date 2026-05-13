using Client.Avalonia.Services;
using Client.Avalonia.Services.Interfaces;
using Lib.Avalonia;
using Lib.Avalonia.Helpers;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Avalonia.Views
{
    /// <summary>
    /// VM, отвечающая за вкладку "Графики". 
    /// </summary>
    public sealed partial class GraphsViewModel : ViewModelBase, ITabVM
    {

        #region Properties

        public GeometryCreateMenuViewModel GeometryCreateMenuViewModel { get; }
        public DisplayViewModel DisplayViewModel { get; }

        /// <inheritdoc/>
        public Guid Id { get; }

        #endregion

        #region .ctor

        /// <summary>
        /// Конструктор-заглушка, дабы Designer не падал.
        /// </summary>
        public GraphsViewModel()
            : this(Guid.NewGuid())
        {

        }
        public GraphsViewModel(Guid id)
        { 
            Id = id;
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public IEnumerable<IHotKey> GetTabHotKeys()
        {
            var hotKeys = new List<IHotKey>
            {

            };

            return hotKeys;
        }

        /// <inheritdoc/>
        public Task DisposeTab()
        {
            //Какие-нибудь отписки, дочерние Dispose, await...

            Dispose();

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task LoadTab()
        {
            //Какие-нибудь подписки, await...

            return Task.CompletedTask;
        }

        #endregion

    }
}
