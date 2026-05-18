using Lib.WPF;
using ReactiveUI; 

namespace Client.WPF.Views
{
    /// <summary>
    /// Вкладка для меню.
    /// </summary>
    public class TabMenu : ViewModelBase, ITabMenu
    {
        private bool _isSelected;

        /// <inheritdoc/>
        public Guid Id { get; }

        /// <inheritdoc/>
        public string DisplayName { get; }

        /// <inheritdoc/>
        public TabCategoryEnum TabCategory { get; }

        /// <inheritdoc/>
        public bool IsSelected
        {
            get => _isSelected;
            set => this.RaiseAndSetIfChanged(ref _isSelected, value);
        }

        public TabMenu(string displayName, TabCategoryEnum tabCategory)
        {
            Id = Guid.NewGuid();

            DisplayName = displayName;
            TabCategory = tabCategory;
        }

    }
}
