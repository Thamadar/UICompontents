using Lib.WPF;
using Lib.WPF.Helpers; 

namespace Client.WPF.Views
{
    public class GraphsViewModel : ViewModelBase, ITabVM
    {  
        
        /// <inheritdoc/>
        public Guid Id { get; }

        public GraphsViewModel(Guid id)
        {  
            Id = id; 
        }

        /// <inheritdoc/>
        public IEnumerable<IHotKey> GetTabHotKeys()
        {
            var hotKeys = new List<IHotKey>
            {
                //new HotKey(Key.Delete, Commands.RemoveShapeCommand)
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
