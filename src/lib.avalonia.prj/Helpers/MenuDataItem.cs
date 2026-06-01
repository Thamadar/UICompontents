using Avalonia.Markup.Xaml.Templates;
using DynamicData;
using ReactiveUI; 
using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.Windows.Input;

namespace Lib.Avalonia.Helpers
{
    public class MenuDataItem : ReactiveObject, IMenuDataItem
    {

        /// <inheritdoc/>
        public ObservableCollection<IMenuDataItem>? Childs { get; }
         
        /// <inheritdoc/>
        public ControlTemplate? Icon { get; }

        /// <inheritdoc/>
        public string? Key { get; set; }

        /// <inheritdoc/>
        public string Text { get; set; }   

        /// <inheritdoc/>
        public ICommand? Command { get; } 

        /// <inheritdoc/>
        public bool IsSeparator { get; }  

        public MenuDataItem(
            string text = "", 
            ControlTemplate? icon = null,
            ICommand? command = null,
            IEnumerable<IMenuDataItem>? childs = null, 
            string? key = null,
            bool isSeparator = false)
        {
            Childs = new ObservableCollection<IMenuDataItem>();

            Text        = text;
            Icon        = icon;
            Command     = command;
            Key         = key;
            IsSeparator = isSeparator; 
            if(childs != null)
            {
                Childs.AddRange(childs);
            }
        }

        /// <inheritdoc/>
        public void ChildsAddRange(IEnumerable<IMenuDataItem> addChilds)
        {
            Childs?.AddRange(addChilds);
        }

        /// <inheritdoc/>
        public void ChildsRemoveRange(IEnumerable<IMenuDataItem> removeChilds)
        { 
            Childs?.RemoveMany(removeChilds);
        }
    }
}
