using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Rendering;
using Avalonia.VisualTree;

using Lib.Avalonia.Controls.Helpers;
using Lib.Avalonia.Extensions;
using Lib.Avalonia.Helpers;

using System; 
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Input;

namespace Lib.Avalonia.Controls
{
    public class CustomMenuItem : TemplatedControl
    {
        private CustomMenuItem? _itemParent;
        private CustomMenuItemControlManager? _customMenuItemControlManager;

        public static readonly StyledProperty<ObservableCollection<IMenuDataItem>> ItemsProperty =
            AvaloniaProperty.Register<CustomContextMenu, ObservableCollection<IMenuDataItem>>(nameof(Items));

        public static readonly StyledProperty<ControlTemplate?> IconProperty =
            AvaloniaProperty.Register<CustomMenuItem, ControlTemplate?>(nameof(Icon));

        public static readonly StyledProperty<ICommand?> CommandProperty =
            AvaloniaProperty.Register<CustomMenuItem, ICommand?>(nameof(Command));

        public static readonly StyledProperty<string?> KeyProperty =
            AvaloniaProperty.Register<CustomMenuItem, string?>(nameof(Key));

        public static readonly StyledProperty<string> TextProperty =
            AvaloniaProperty.Register<CustomMenuItem, string>(nameof(Text)); 

        public static readonly StyledProperty<bool> IsSubMenuOpenProperty =
            AvaloniaProperty.Register<CustomMenuItem, bool>(nameof(IsSubMenuOpen));

        public static readonly StyledProperty<bool> IsSeparatorProperty =
            AvaloniaProperty.Register<CustomMenuItem, bool>(nameof(IsSeparator)); 

        public static readonly DirectProperty<CustomMenuItem, CustomMenuItem?> ItemParentProperty =
            AvaloniaProperty.RegisterDirect<CustomMenuItem, CustomMenuItem?>(
            nameof(ItemParent),
            x => x.ItemParent,
            (x, v) => x.ItemParent = v);

        /// <summary>
        /// Список меню. Может быть пустым.
        /// </summary>
        public ObservableCollection<IMenuDataItem> Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        /// <summary>
        /// Иконка объекта меню. Может быть пустым.
        /// </summary>
        public ControlTemplate? Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }
        
        /// <summary>
        /// Родитель данного item'а. Может быть пустым.
        /// Устанавливается автоматически.
        /// </summary>
        public CustomMenuItem? ItemParent
        {
            get => _itemParent;
            set => SetAndRaise(ItemParentProperty, ref _itemParent, value);
        }

        /// <summary>
        /// Command.
        /// </summary>
        public ICommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Горячие клавиши. Может быть пустым
        /// </summary>
        public string? Key
        {
            get => GetValue(KeyProperty);
            set => SetValue(KeyProperty, value);
        } 

        /// <summary>
        /// Выводимый текст.
        /// </summary>
        public string Text
        {
            get => GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        } 
         
        /// <summary>
        /// Открыта ли панель-меню данного Item'а?
        /// </summary>
        public bool IsSubMenuOpen
        {
            get => GetValue(IsSubMenuOpenProperty);
            set => SetValue(IsSubMenuOpenProperty, value);
        } 

        //to do: change add/remove class
        /// <summary>
        /// Существует ли данный Item в качестве разделителя в списке меню?
        /// </summary>
        public bool IsSeparator
        {
            get => GetValue(IsSeparatorProperty);
            set => SetValue(IsSeparatorProperty, value);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        { 
            base.OnPointerPressed(e);

            if(!IsSeparator)
            {
                _customMenuItemControlManager?.OnPointedPressed(this); 
                if(Items.Count() == 0)
                {
                    Command?.Execute(null);  
                } 
            }

            e.Handled = true;
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);
             
            if(!IsSeparator)
            {
                _customMenuItemControlManager?.OnPointerEntered(this);
            }

            e.Handled = true;
        }
         
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            ItemParent = this.GetParent<CustomMenuItem>();

            _customMenuItemControlManager?.Remove(this);
            EnsureMenuManager(e.Root);

            base.OnAttachedToVisualTree(e);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            _customMenuItemControlManager?.Remove(this);
            _customMenuItemControlManager = null;
        }

        [MemberNotNull(nameof(_customMenuItemControlManager))]
        private void EnsureMenuManager(IRenderRoot? root = null)
        {
            _customMenuItemControlManager = CustomMenuItemControlManager.GetOrCreateForRoot(root ?? this.GetVisualRoot());
            _customMenuItemControlManager.Add(this);
        }
    }
}
