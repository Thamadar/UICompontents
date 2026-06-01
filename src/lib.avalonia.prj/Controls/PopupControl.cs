using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity; 
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive; 
using System.Threading.Tasks;
using System.Windows.Input;

using Lib.Avalonia.Extensions;
using Avalonia.VisualTree;
using Avalonia.LogicalTree;

namespace Lib.Avalonia.Controls
{ 
    public class PopupControl : Popup
    { 
        private static readonly List<int> _opened = new();

        private RoutingStrategies _clickOutStrategy = RoutingStrategies.Bubble;

        private bool _ignoreClickOut;
        private bool _isHidden;
        private bool _treeModeClose;
        private int _idPopup = -1;


        public static readonly StyledProperty<ICommand?> CloseCommandProperty =
            AvaloniaProperty.Register<PopupControl, ICommand?>(nameof(CloseCommand));

        public static readonly DirectProperty<PopupControl, RoutingStrategies> ClickOutStrategyProperty
            = AvaloniaProperty.RegisterDirect<PopupControl, RoutingStrategies>(
            nameof(ClickOutStrategy),
            x => x.ClickOutStrategy,
            (x, v) => x.ClickOutStrategy = v);

        public static readonly DirectProperty<PopupControl, bool> IsHiddenProperty = AvaloniaProperty.RegisterDirect<PopupControl, bool>(
            nameof(IsHidden),
            x => x.IsHidden,
            (x, v) => x.IsHidden = v);

        public static readonly DirectProperty<PopupControl, bool> TreeModeCloseProperty = AvaloniaProperty.RegisterDirect<PopupControl, bool>(
            nameof(TreeModeClose),
            x => x.TreeModeClose,
            (x, v) => x.TreeModeClose = v);

        public static readonly DirectProperty<PopupControl, bool> IgnoreClickOutProperty = AvaloniaProperty.RegisterDirect<PopupControl, bool>(
            nameof(IgnoreClickOut),
            x => x.IgnoreClickOut,
            (x, v) => x.IgnoreClickOut = v); 

        /// <summary>
        ///  Спрятан ли PopupControl?
        /// </summary>
        public bool IsHidden
        {
            get => _isHidden;
            set => SetAndRaise(IsHiddenProperty, ref _isHidden, value);
        }

        /// <summary>
        /// Включен ли режим древовидного закрытия PopupControl?
        /// </summary>
        public bool TreeModeClose
        {
            get => _treeModeClose;
            set => SetAndRaise(TreeModeCloseProperty, ref _treeModeClose, value);
        }

        /// <summary>
        /// Игнорировать ли нажатия вне PopupControl?
        /// </summary>
        public bool IgnoreClickOut
        {
            get => _ignoreClickOut;
            set => SetAndRaise(IgnoreClickOutProperty, ref _ignoreClickOut, value);
        }

        public RoutingStrategies ClickOutStrategy
        {
            get => _clickOutStrategy;
            set => SetAndRaise(ClickOutStrategyProperty, ref _clickOutStrategy, value);
        }

        /// <summary>
        /// Command закрытия Popup.
        /// </summary>
        public ICommand? CloseCommand
        {
            get => GetValue(CloseCommandProperty);
            set => SetValue(CloseCommandProperty, value);
        }

        public PopupControl()
        {
            Topmost = false;

            CloseCommand = ReactiveCommand.CreateFromTask(Close);

            this.GetObservable(IsOpenProperty)
                .Subscribe(OnIsOpenChanged); 
        }

        #region Methods

        protected Task Close()
        {
            IsOpen = false;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Метод закрытия вложенного внутри PopupControl данного PopupControl, если он открыт.
        /// </summary>
        private void CloseNestedPopups()
        {
            var root = this.Child;

            if(root == null)
                return;

            foreach(var nestedPopup in VisualExtension.FindDescendants<PopupControl>(root))
            {
                if(nestedPopup.IsOpen)
                {
                    nestedPopup.Close();
                }
            }
        }

        private static int AddToOpened()
        {
            var counter = _opened.Count > 0 ? _opened.Last() + 1 : 0;

            _opened.Add(counter);

            return counter;
        }

        private static void RemoveFromOpened(int id)
        {
            if(_opened.Contains(id))
            {
                _opened.Remove(id);
            }
        }

        /// <summary>
        /// Нажатие по родителю-окну (клик, когда отжали ПКМ).
        /// </summary> 
        private void EvOnReleasedWindow(object? sender, PointerReleasedEventArgs e)
        {
            if(e.InitialPressMouseButton != MouseButton.Left)
            {
                return;
            }
            if(e.Source is Control control)
            {
                var toggleButton = control
                    .GetVisualAncestors()
                    .Where(x => x is ToggleButton)
                    .Select(x => x as ToggleButton)
                    .FirstOrDefault();

                if(toggleButton != null)
                {
                    var flag = VisualExtension.FindDescendantControlExceptPopup<ToggleButton>(this, toggleButton);
                    if(!flag.FirstOrDefault())
                    {
                        this.Close();
                    }

                    return;
                }
            }
        }

        /// <summary>
        /// Нажатие по родителю-окну (клик, когда нажали ПКМ).
        /// </summary> 
        private void EvOnPressWindow(object? sender, PointerPressedEventArgs e)
        {
            if(e.Source is Control control)
            {
                if(!e.GetCurrentPoint(control).Properties.IsLeftButtonPressed)
                {
                    return;
                }

                var controlLogicalAncestors = control.GetLogicalAncestors();
                if(controlLogicalAncestors.Where(x => x is ToggleButton).Count() == 0 &&
                   controlLogicalAncestors.Where(x => x is Button).Count() > 0)
                {
                    var button = controlLogicalAncestors.Where(x => x is Button).Select(x => x as Button).FirstOrDefault();
                    if(IsHidden)
                    {
                        return;
                    }

                    var flag = VisualExtension.FindDescendantControlExceptPopup<Button>(this, button);
                    if(!flag.FirstOrDefault())
                    {
                        this.Close();
                    }

                    return;
                }
                else if(controlLogicalAncestors.Where(x => x is TextBox).Count() > 0)
                {
                    var textBox = controlLogicalAncestors.Where(x => x is TextBox).Select(x => x as TextBox).FirstOrDefault();
                    if(IsHidden)
                    {
                        return;
                    }

                    var flag = VisualExtension.FindDescendantControlExceptPopup<TextBox>(this, textBox);
                    if(!flag.FirstOrDefault())
                    {
                        this.Close();
                    }

                    return;
                }
                else if(controlLogicalAncestors.Where(x => x is TabControl).Count() > 0 &&
                    this.GetLogicalAncestors().Where(x => x is TabControl).Count() == 0)
                {
                    var tabControl = controlLogicalAncestors.Where(x => x is TabControl).Select(x => x as TabControl).FirstOrDefault();
                    if(IsHidden)
                    {
                        return;
                    }

                    var flag = VisualExtension.FindDescendantControlExceptPopup<TabControl>(this, tabControl);
                    if(!flag.FirstOrDefault())
                    {
                        this.Close();
                    }

                    return;
                }

                if((e.Handled ||
                    IgnoreClickOut ||
                    this.GetLogicalAncestors().Where(x => x is ComboBox && x == control).Count() > 0))
                {
                    return;
                }

                if(controlLogicalAncestors.Where(x => x is ComboBox).Count() > 0)
                {
                    var comboBox = controlLogicalAncestors.Where(x => x is ComboBox).Select(x => x as ComboBox).FirstOrDefault();
                    if(IsHidden)
                    {
                        return;
                    }

                    var flag = VisualExtension.FindDescendantControlExceptPopup<ComboBox>(this, comboBox);
                    if(!flag.FirstOrDefault())
                    {
                        this.Close();
                    }

                    return;
                }

                var lastOpened = _opened.LastOrDefault();
                if(TreeModeClose &&
                    lastOpened != _idPopup)
                    return;

                if(!control.IsParent(this))
                {
                    var result = CloseCommand?.CanExecute(Unit.Default) ?? false;
                    if(result)
                    {
                        CloseCommand?.Execute(Unit.Default);
                    }
                }
            }
        }

        private void OnIsOpenChanged(bool isOpen)
        {
            var evPressed = PointerPressedEvent;
            var evReleased = PointerReleasedEvent;

            var parentWindow = default(Window);
            parentWindow = VisualExtension.GetParentWindow(this);
            if(parentWindow == null &&
               Application.Current != null &&
               Application.Current.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                parentWindow = desktop.MainWindow;
            }

            if(isOpen)
            {
                parentWindow?.AddHandler(evPressed, EvOnPressWindow, ClickOutStrategy, true);
                parentWindow?.AddHandler(evReleased, EvOnReleasedWindow, ClickOutStrategy, true);

                if(TreeModeClose)
                    _idPopup = AddToOpened();
            }
            else
            {
                if(TreeModeClose)
                {
                    RemoveFromOpened(_idPopup);
                }

                parentWindow?.RemoveHandler(evPressed, EvOnPressWindow);
                parentWindow?.RemoveHandler(evReleased, EvOnReleasedWindow);
                CloseNestedPopups();
            }
        }

        #endregion

    }
}
