using Avalonia.Rendering;
using Avalonia.Threading;
using Lib.Avalonia.Extensions;
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Runtime.CompilerServices;

namespace Lib.Avalonia.Controls.Helpers
{
    internal class CustomMenuItemControlManager
    {
        private static readonly CustomMenuItemControlManager s_default = new();
        private static readonly ConditionalWeakTable<IRenderRoot, CustomMenuItemControlManager> s_registeredVisualRoots = new();

        private static DispatcherTimer? _timer;

        private readonly Dictionary<CustomMenuItem, List<WeakReference<CustomMenuItem>>> _registeredGroups = new();
        private readonly CustomMenuItem _defaultNullParent = new CustomMenuItem(); 
        private readonly TimeSpan _timerDelay = TimeSpan.FromMilliseconds(200);

        private CustomMenuItem? _selectedItem; 

        private bool _ignoreCheckedChanges; 

        public static CustomMenuItemControlManager GetOrCreateForRoot(IRenderRoot? root)
        {
            if(root == null)
                return s_default;
            return s_registeredVisualRoots.GetValue(root, key => new CustomMenuItemControlManager());
        }

        /// <summary>
        /// Реакция на нажатие по CustomMenuItem.
        /// </summary> 
        public void OnPointedPressed(CustomMenuItem menu)
        {
            _selectedItem = menu;

            BreakEnteredActionTimer(); 
            OnPointedPressedAction();
        }

        /// <summary>
        /// Реакция на наведение на CustomMenuItem.
        /// </summary> 
        public void OnPointerEntered(CustomMenuItem menu)
        {
            _selectedItem = menu;

            BreakEnteredActionTimer();

            _timer = new DispatcherTimer
            {
                Interval = _timerDelay
            };
            _timer.Tick += (sender, e) =>
            {
                BreakEnteredActionTimer();
                OnPointedEnteredAction();
            };

            _timer.Start(); 
        } 

        public void Add(CustomMenuItem menu)
        {
            var parent = menu.ItemParent ?? _defaultNullParent; 

            if(!_registeredGroups.TryGetValue(parent, out var menuGroup))
            {
                menuGroup = new List<WeakReference<CustomMenuItem>>();
                _registeredGroups.Add(parent, menuGroup);
            }

            menuGroup.Add(new WeakReference<CustomMenuItem>(menu));
        }

        public void Remove(CustomMenuItem menu)
        {
            var parent = menu.ItemParent ?? _defaultNullParent; 

            if(_registeredGroups.TryGetValue(parent, out var group))
            {
                int i = 0;
                while(i < group.Count)
                {
                    if(!group[i].TryGetTarget(out var item) || item == menu)
                    {
                        group.RemoveAt(i);
                        continue;
                    }

                    i++;
                }

                if(group.Count == 0)
                {
                    _registeredGroups.Remove(parent);
                }
            }
        }

        /// <summary>
        /// Действие, исполняемое при исполнении таймера, реагирующего на нажатие по CustomMenuItem.
        /// </summary>
        private void OnPointedPressedAction()
        {
            var menu = _selectedItem;

            if(_ignoreCheckedChanges || menu == null)
            {
                return;
            }

            var parent = menu.ItemParent ?? _defaultNullParent;

            _ignoreCheckedChanges = true;

            try
            {
                if(_registeredGroups.TryGetValue(parent, out var group))
                {
                    if(!IsAnyOpened(group))
                    {
                        OpenCloseSubMenu(menu, !menu.IsSubMenuOpen);
                        return;
                    }

                    CloseAllItemsExcept(group, menu);
                    if(group.Count == 0)
                    {
                        _registeredGroups.Remove(parent);
                    }
                    OpenCloseSubMenu(menu, !menu.IsSubMenuOpen);
                }
            }
            finally
            {
                _ignoreCheckedChanges = false;
            }
        }

        /// <summary>
        /// Действие, исполняемое при исполнении таймера, реагирующего на наведение на CustomMenuItem.
        /// </summary>
        private void OnPointedEnteredAction()
        {
            var menu = _selectedItem; 
             
            if(_ignoreCheckedChanges || menu == null)
            {
                return;
            }

            var parent = menu.ItemParent ?? _defaultNullParent;

            _ignoreCheckedChanges = true;

            try
            { 
                if(_registeredGroups.TryGetValue(parent, out var group))
                { 
                    if(!IsAnyOpened(group))
                    {
                        if(!menu.IsSubMenuOpen)
                        {
                            OpenCloseSubMenu(menu, true);
                        } 
                        return;
                    }

                    CloseAllItemsExcept(group, menu); 
                    if(group.Count == 0)
                    {
                        _registeredGroups.Remove(parent);
                    }

                    if(!menu.IsSubMenuOpen)
                    {
                        OpenCloseSubMenu(menu, true);
                    } 
                }
            }
            finally
            {
                _ignoreCheckedChanges = false;
            }
        }

        /// <summary>
        /// Останавливает и отменяет таймер, реагирующий на наведение на CustomMenuItem.
        /// </summary>
        private void BreakEnteredActionTimer()
        { 
            _timer?.Stop();
            _timer = null;
        }

        /// <summary>
        /// Закрыть все CustomMenuItem'ы из группы, кроме указанного в параметре.
        /// </summary>
        /// <param name="group">группа</param>
        /// <param name="menu">исключение</param>
        private void CloseAllItemsExcept(List<WeakReference<CustomMenuItem>> group, CustomMenuItem menu)
        {
            var i = 0;
            while(i < group.Count)
            {
                if(group.TryGetTargetOrRemove(out var current, i))
                {

                    if(current.IsSubMenuOpen)
                    {
                        if(current != menu)
                        {
                            current.IsSubMenuOpen = false;
                        }

                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Открыт ли какой-либо CustomMenuItem из указанной группы?
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private bool IsAnyOpened(List<WeakReference<CustomMenuItem>> group)
        {
            var i = 0;
            var isAnyOpened = false;
            while(i < group.Count)
            {
                if(group.TryGetTargetOrRemove(out var current, i))
                {
                    if(current.IsSubMenuOpen)
                    {
                        isAnyOpened = true;
                        return isAnyOpened;
                    }
                }
                i++;
            }

            return isAnyOpened;
        } 

        /// <summary>
        /// Открыть/Закрыть указанный CustomMenuItem.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="open"></param>
        private void OpenCloseSubMenu(CustomMenuItem menu, bool open)
        {
            if(!menu.IsSeparator && menu.Items.Count() > 0)
            {
                menu.IsSubMenuOpen = open;
            }
        }
    }
}
