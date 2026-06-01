using Avalonia;  
using Avalonia.Controls.Primitives; 
using Avalonia.Input; 
using Avalonia.Metadata;
using Avalonia.Rendering; 
using Avalonia.VisualTree; 
using Lib.Avalonia.Controls.Helpers; 
using Lib.Avalonia.Helpers; 
using System.Collections.ObjectModel; 
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace Lib.Avalonia.Controls
{
    public class CustomContextMenu : TemplatedControl
    { 
        private ContextMenuControlGroupManager? _contextMenuGroupManager; 

        public static readonly StyledProperty<object?> ContentProperty =
            AvaloniaProperty.Register<CustomContextMenu, object?>(nameof(Content));

        public static readonly StyledProperty<ObservableCollection<IMenuDataItem>> ItemsProperty =
            AvaloniaProperty.Register<CustomContextMenu, ObservableCollection<IMenuDataItem>>(nameof(Items));

        public static readonly StyledProperty<ICommand?> CommandProperty =
            AvaloniaProperty.Register<CustomContextMenu, ICommand?>(nameof(Command));

        public static readonly StyledProperty<bool> IsDropDownOpenProperty =
            AvaloniaProperty.Register<CustomContextMenu, bool>(nameof(IsDropDownOpen), false);

        public static readonly StyledProperty<string?> GroupNameProperty =
            AvaloniaProperty.Register<CustomContextMenu, string?>(nameof(GroupName));

        public ObservableCollection<IMenuDataItem> Items
        {
            get => GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }
         
        /// <summary>
        /// Command.
        /// </summary>
        public ICommand? Command
        {
            get => GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        [Content]
        public object? Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        /// <summary>
        /// Наименование группы menu-кнопок. Используется, чтобы объединить их поведение. Работает по аналогии с RadioButton.
        /// </summary>
        public string? GroupName
        {
            get => GetValue(GroupNameProperty);
            set => SetValue(GroupNameProperty, value);
        }

        /// <summary>
        /// Расскрыто ли меню?
        /// </summary>
        public bool IsDropDownOpen
        {
            get => GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }   

        static CustomContextMenu()
        {
            AffectsMeasure<CustomContextMenu>(IsDropDownOpenProperty); 
        }   
         
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        { 
            _contextMenuGroupManager?.Remove(this, GroupName);
            EnsureGroupManager(e.Root);

            base.OnAttachedToVisualTree(e);
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
             
            _contextMenuGroupManager?.Remove(this, GroupName);
            _contextMenuGroupManager = null;
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);
             
            _contextMenuGroupManager?.OnPointerEntered(this);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            if(Items.Count > 0)
            {
                IsDropDownOpen = !IsDropDownOpen;
            }
            else if(IsDropDownOpen)
            {
                IsDropDownOpen = false;
            }
            else
            { 
                Command?.Execute(null);
            } 
            
            e.Handled = true; 
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if(change.Property == IsDropDownOpenProperty)
            {
                IsCheckedChanged(change.GetNewValue<bool>());
            }
            else if(change.Property == GroupNameProperty)
            {
                var (oldValue, newValue) = change.GetOldAndNewValue<string?>();
                OnGroupNameChanged(oldValue, newValue);
            }
        } 

        private void OnGroupNameChanged(string? oldGroupName, string? newGroupName)
        {
            if(!string.IsNullOrEmpty(oldGroupName))
            {
                _contextMenuGroupManager?.Remove(this, oldGroupName);
            }
            if(!string.IsNullOrEmpty(newGroupName))
            {
                _contextMenuGroupManager?.Add(this);
            }
        }

        private void IsCheckedChanged(bool value)
        {
            if(value)
            { 
                _contextMenuGroupManager?.OnIsDropDownOpenChanged(this);
            }
        }

        [MemberNotNull(nameof(_contextMenuGroupManager))]
        private void EnsureGroupManager(IRenderRoot? root = null)
        {
            _contextMenuGroupManager = ContextMenuControlGroupManager.GetOrCreateForRoot(root ?? this.GetVisualRoot());
            _contextMenuGroupManager.Add(this);
        }  
    }
}
