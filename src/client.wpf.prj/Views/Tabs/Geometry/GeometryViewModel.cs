using Client.WPF.Services;
using Client.WPF.Services.Interfaces;
using Lib.WPF;
using Lib.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client.WPF.Views 
{
    /// <summary>
    /// VM, отвечающая за вкладку "Графический редактор".
    /// //TO DO: переименовать класс. GeometryViewModel => GhaphicEditorViewModel.
    /// </summary>
    public sealed partial class GeometryViewModel : ViewModelBase, ITabVM
    {
        private readonly IShapeService _shapeService;

        public GeometryCreateMenuViewModel GeometryCreateMenuViewModel { get; }
        public DisplayViewModel DisplayViewModel { get; }

        /// <inheritdoc/>
        public Guid Id { get; }
         
        public GeometryViewModel(Guid id)
        {
            _shapeService = ShapeService.Instance;

            Id = id;

            GeometryCreateMenuViewModel = new GeometryCreateMenuViewModel();
            DisplayViewModel = new DisplayViewModel(Commands.CreateShapeCommand);
        }

        /// <inheritdoc/>
        public IEnumerable<IHotKey> GetTabHotKeys()
        {
            var hotKeys = new List<IHotKey>
            {
                new HotKey(Key.Delete, Commands.RemoveShapeCommand)
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
    }
}
