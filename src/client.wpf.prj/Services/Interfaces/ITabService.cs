using Client.WPF.Views;
using DynamicData;
using Lib.WPF.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.WPF.Services.Interfaces
{
    //TO DO: IDisposable привентить.
    /// <summary>
    /// Сервис управления вкладками в проекте между VM.
    /// </summary>
    public interface ITabService
    {
        /// <summary> 
        /// Подключение к списку всех вкладок.
        /// </summary> 
        IObservable<IChangeSet<TabMenu>> ConnectToTotalTabMenu();

        /// <summary> 
        /// Подключение к списку текущих горячих клавиш VM-вкладки.
        /// </summary> 
        IObservable<IChangeSet<IHotKey>> ConnectToCurrentTabHotKeys();

        /// <summary>
        /// Отслеживание текущей выбранной VM-вкладки.
        /// </summary>
        IObservable<ITabVM?> CurrentSelectedTabVMObservable { get; }

        IEnumerable<IHotKey> GetCurrentTabVMHotKeys();

        /// <summary>
        /// Получить текущую выделенную VM-вкладку.
        /// </summary> 
        ITabVM? GetCurrentSelectedTabVM();

        /// <summary>
        /// Загрузка самих вкладок (не того, что они отображают).
        /// </summary>
        void LoadTotalTabMenuData();

        /// <summary>
        /// Выбрать влкадку.
        /// </summary> 
        Task SelectTabMenu(TabCategoryEnum tabCategoryEnum);
    }
}
