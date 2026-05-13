using Lib.Avalonia.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Avalonia.Views
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
