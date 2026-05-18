using Lib.WPF.Helpers;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Lib.WPF.Behaviors
{
    /// <summary>
    /// Behavior, с помощью которого можно подключить горячие клавиши к Window,
    /// используя свойство HotKeys.
    /// </summary>
    public class AddWindowHotKeysBehavior : Behavior<Window>
    {
        public static readonly DependencyProperty HotKeysProperty =
            DependencyProperty.Register(nameof(HotKeys), typeof(ObservableCollection<IHotKey>), typeof(AddWindowHotKeysBehavior));

        /// <summary>
        /// Список горячих клавиш интерфейса IHotKey.
        /// </summary>
        public ObservableCollection<IHotKey> HotKeys
        {
            get => (ObservableCollection<IHotKey>)GetValue(HotKeysProperty);
            set => SetValue(HotKeysProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.PreviewKeyDown += OnPreviewKeyDown; 
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewKeyDown -= OnPreviewKeyDown;

        } 

        /// <summary>
        /// Исполнение события нажатия на клавишу. Поиск совпадений в списке горячих клавиш и их выполнение.
        /// </summary> 
        private void OnPreviewKeyDown(object? sender, KeyEventArgs e)
        {
            var textBox = e.Source as TextBox;
            if(HotKeys == null ||
                HotKeys.Count() == 0 ||
                textBox != null) 
                return;

            var pressedModifiers = Keyboard.Modifiers;  

            var tempKeyBindingItems = new List<IHotKey>();
            foreach(var item in HotKeys)
            {
                if(item.Key == e.Key &&
                   item.KeyModifiers == pressedModifiers)
                    tempKeyBindingItems.Add(item);
            }

            foreach(var keyBinding in tempKeyBindingItems)
            {
                keyBinding.Command?.Execute(keyBinding.CommandParameter);
                e.Handled = true;
            }
        }
    }
}
