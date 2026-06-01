using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using DynamicData;

namespace Client.Avalonia.Behaviors
{
    /// <summary>
    /// Behavior, скрывающий последний элемент "разделитель" (Separator) в ItemsControl.
    /// </summary>
    public class HideBottomSeparatorBehavior : Behavior<Separator>
    {  
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject?.AddHandler(Control.LoadedEvent, OnAssociatedControlLoaded);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject?.RemoveHandler(Control.PointerPressedEvent, OnAssociatedControlLoaded);
        }

        private void OnAssociatedControlLoaded(object? sender, RoutedEventArgs e)
        {
            Dispatcher.UIThread.Post(UpdateSeparatorVisibility);
        }

        private void UpdateSeparatorVisibility()
        {
            if(AssociatedObject == null)
                return; 

            var itemsControl = AssociatedObject.FindAncestorOfType<ItemsControl>();
            var itemParent = AssociatedObject.FindAncestorOfType<ContentPresenter>();
            if(itemsControl != null)
            {   
                var indexItem = itemsControl.IndexFromContainer(itemParent);
                int lastIndex = itemsControl.Items.Count() - 1;

                // Скрыть нижний Separator только у последнего элемента.
                if(indexItem == lastIndex)
                {
                    AssociatedObject.IsVisible = false;
                }
                else
                {
                    AssociatedObject.IsVisible = true;
                } 
            }
        }
    }
}
