using Lib.WPF.Helpers;
using System.Windows;

namespace Client.WPF.Views
{
    /// <summary>
    /// Интерфейс для VM, отображающей содержимое вкладки.
    /// </summary>
    public interface ITabVM : IDisposable
    {
        /// <summary>
        /// Идентификатор. Связывает ITabVM и ITabMenu.
        /// </summary>
        Guid Id { get; } 

        /// <summary>
        /// Загрузка содержимого вкладки.
        /// </summary> 
        Task LoadTab();

        /// <summary>
        /// Освобождение памяти.
        /// </summary>
        Task DisposeTab();

        /// <summary>
        /// Получение горячих клавиш данной вкладки.
        /// </summary> 
        IEnumerable<IHotKey> GetTabHotKeys();
    }
}
