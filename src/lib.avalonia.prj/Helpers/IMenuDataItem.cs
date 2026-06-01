using Avalonia.Markup.Xaml.Templates;
using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.Windows.Input;

namespace Lib.Avalonia.Helpers
{
    public interface IMenuDataItem
    { 
        public ObservableCollection<IMenuDataItem>? Childs { get; }

        public ControlTemplate? Icon { get; }

        public ICommand? Command { get; }

        public string? Key { get; set; }

        public string Text { get; set; }   

        public bool IsSeparator { get; }

        /// <summary>
        /// Добавить список элементов IMenuDataItem в коллекцию Childs.
        /// </summary> 
        public void ChildsAddRange(IEnumerable<IMenuDataItem> addChilds);

        /// <summary>
        /// Удалить список элементов IMenuDataItem из коллекции Childs.
        /// </summary> 
        public void ChildsRemoveRange(IEnumerable<IMenuDataItem> removeChilds);
    }
}
