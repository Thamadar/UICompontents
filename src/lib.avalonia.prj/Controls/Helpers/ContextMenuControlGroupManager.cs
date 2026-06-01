using Avalonia.Rendering; 
using System;
using System.Collections.Generic; 
using System.Runtime.CompilerServices; 
using Lib.Avalonia.Extensions;

namespace Lib.Avalonia.Controls.Helpers
{
    internal class ContextMenuControlGroupManager
    {
        private static readonly ContextMenuControlGroupManager s_default = new();
        private static readonly ConditionalWeakTable<IRenderRoot, ContextMenuControlGroupManager> s_registeredVisualRoots = new();

        private bool _ignoreIsDropDownOpenChanges;
        private readonly Dictionary<string, List<WeakReference<CustomContextMenu>>> _registeredGroups = new(); 

        public static ContextMenuControlGroupManager GetOrCreateForRoot(IRenderRoot? root)
        {
            if(root == null)
                return s_default;
            return s_registeredVisualRoots.GetValue(root, key => new ContextMenuControlGroupManager());
        }

        public void Add(CustomContextMenu menu)
        {
            var groupName = menu.GroupName;
            if(groupName is not null)
            {
                if(!_registeredGroups.TryGetValue(groupName, out var group))
                {
                    group = new List<WeakReference<CustomContextMenu>>();
                    _registeredGroups.Add(groupName, group);
                }

                group.Add(new WeakReference<CustomContextMenu>(menu));
            }
        }

        public void Remove(CustomContextMenu menu, string? oldGroupName)
        {
            if(!string.IsNullOrEmpty(oldGroupName) && _registeredGroups.TryGetValue(oldGroupName, out var group))
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
                    _registeredGroups.Remove(oldGroupName);
                }
            }
        }
          
        /// <summary>
        /// Реакция на наведение по CustomContextMenu.
        /// </summary>
        /// <param name="menuEnterer"></param>
        public void OnPointerEntered(CustomContextMenu menuEnterer)
        {
            if(_ignoreIsDropDownOpenChanges)
            {
                return;
            }

            _ignoreIsDropDownOpenChanges = true;
            try
            {
                var groupName = menuEnterer.GroupName;
                if(!string.IsNullOrEmpty(groupName))
                {
                    if(_registeredGroups.TryGetValue(groupName, out var group))
                    {
                        if(!IsAnyOpened(group))
                            return;

                        CloseAllItemsExcept(group, menuEnterer);
                         
                        if(group.Count == 0)
                        {
                            _registeredGroups.Remove(groupName);
                        }
                         
                        menuEnterer.IsDropDownOpen = true; 
                        //if(menuEnterer.Items.Count > 0)
                        //{
                        //    menuEnterer.IsDropDownOpen = true;
                        //} 
                    }
                }
            }
            finally
            {
                _ignoreIsDropDownOpenChanges = false;
            } 
        }

        /// <summary>
        /// Реакция на изменение свойства OnIsDropDownOpenChanged в CustomContextMenu.
        /// </summary>
        /// <param name="menu"></param>
        public void OnIsDropDownOpenChanged(CustomContextMenu menu)
        {
            if(_ignoreIsDropDownOpenChanges)
            {
                return;
            }

            _ignoreIsDropDownOpenChanges = true;
            try
            {
                var groupName = menu.GroupName;
                if(!string.IsNullOrEmpty(groupName))
                {
                    if(_registeredGroups.TryGetValue(groupName, out var group))
                    {
                        CloseAllItemsExcept(group, menu); 
                        if(group.Count == 0)
                        {
                            _registeredGroups.Remove(groupName);
                        } 
                    }
                } 
            }
            finally
            {
                _ignoreIsDropDownOpenChanges = false;
            }
        }
         
        /// <summary>
        /// Закрыть все CustomContextMenu из группы, кроме указанного в параметре.
        /// </summary>
        /// <param name="group">группа</param>
        /// <param name="menu">исключение</param>
        private void CloseAllItemsExcept(List<WeakReference<CustomContextMenu>> group, CustomContextMenu menu)
        {
            var i = 0;
            while(i < group.Count)
            {
                if(group.TryGetTargetOrRemove(out var current, i))
                {

                    if(current.IsDropDownOpen)
                    {
                        if(current != menu)
                        {
                            current.IsDropDownOpen = false;
                        }

                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Открыт ли какой-либо CustomContextMenu из указанной группы?
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private bool IsAnyOpened(List<WeakReference<CustomContextMenu>> group)
        {
            var i = 0;
            var isAnyOpened = false;
            while(i < group.Count)
            {
                if(group.TryGetTargetOrRemove(out var current, i))
                {
                    if(current.IsDropDownOpen)
                    {
                        isAnyOpened = true;
                        return isAnyOpened;
                    }
                }
                i++;
            }

            return isAnyOpened;
        }
    } 
}
