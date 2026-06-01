using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System.Linq;

namespace Lib.Avalonia.Extensions
{
    public static class VisualExtension
    {
        public static T? GetParentByName<T>(this Visual control, string name)
        where T : StyledElement
        {
            var currentParent = control.Parent;
            while(currentParent != null &&
                 (currentParent is not T || currentParent.Name != name))
            {
                currentParent = currentParent.Parent;
            }

            return currentParent != null ? (T)currentParent : default;
        }

        public static T? GetParent<T>(this Visual control)
            where T : StyledElement
        {
            var currentParent = control.Parent;
            while(currentParent != null &&
                  currentParent is not T)
            {
                currentParent = currentParent.Parent;
            }

            return currentParent != null ? (T)currentParent : default;
        }

        public static Window GetParentWindow(Visual visual)
        {
            while(visual != null)
            {
                if(visual is Window window)
                {
                    return window;
                }
                visual = (Visual)visual.Parent;
            }
            return null;
        }

        /// <summary>
        /// Рекурсивный поиск потомков типа T в визуальном дереве.
        /// </summary> 
        public static IEnumerable<T> FindDescendants<T>(Visual root)
            where T : Visual
        {
            if(root == null)
                yield break;

            foreach(var child in root.GetVisualChildren())
            {
                if(child is T t)
                    yield return t;

                // Рекурсивный вызов для поиска потомков
                foreach(var descendant in FindDescendants<T>(child))
                    yield return descendant;
            }
        }

        /// <summary>
        ///  Рекурсивный поиск родителя T.
        /// </summary> 
        //public static T FindParent<T>(this Visual child)
        //    where T : class
        //{
        //    var parent = child.GetVisualParent();
        //    while(parent != null)
        //    {
        //        if(parent is T tParent)
        //        {
        //            return tParent;
        //        }
        //        parent = parent.GetVisualParent();
        //    }
        //    return null;
        //}

        /// <summary>
        /// Рекурсивный поиск, при этом не влезая в дочерние Popup. Является ли потомком переданный searchItem типа T.
        /// </summary> 
        public static IEnumerable<bool> FindDescendantControlExceptPopup<T>(Visual root, T searchItem)
            where T : Visual
        {
            if(root == null)
                yield break;

            var visualChildrens = root.GetVisualChildren();
            if(visualChildrens.Count() > 0)
            {
                foreach(var child in visualChildrens)
                {
                    if(child is OverlayPopupHost)
                        continue;

                    if(child is T t && t == searchItem)
                        yield return true;

                    // Рекурсивный вызов для поиска потомков
                    foreach(var descendant in FindDescendantControlExceptPopup<T>(child, searchItem))
                        yield return descendant;
                }
            }
            else
            {
                foreach(var child in root.GetLogicalChildren().Select(x => x as Visual))
                {
                    if(child is OverlayPopupHost)
                        continue;

                    if(child is T t && t == searchItem)
                        yield return true;

                    // Рекурсивный вызов для поиска потомков
                    foreach(var descendant in FindDescendantControlExceptPopup<T>(child, searchItem))
                        yield return descendant;
                }
            }
        }

        public static bool IsParent(this Visual control, Visual parent)
        {
            var result = control == parent;
            var currentParent = control.Parent;
            while(!result && currentParent != null)
            {
                result = currentParent == parent;
                currentParent = currentParent.Parent;
            }

            return result;
        }
    }
}
