using Lib.WPF.Extensions;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Lib.WPF.Controls
{
    /// <summary>
    /// Аналог ViewLocator'а из Avalonia. Задаете DataContent => данный контрол отображает подходящий View, относительно 
    /// указанной VM.
    /// </summary>
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class ViewLocatorControl : Control
    { 
         
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(FrameworkElement), typeof(ViewLocatorControl));

        /// <summary>
        /// Отображаемая View.
        /// </summary>
        public FrameworkElement Content
        {
            get => (FrameworkElement)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        } 

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);
             
            if(oldParent == null)
            {
                DataContextChanged += OnDataContextChanged;
            }

            else
            {
                DataContextChanged -= OnDataContextChanged;
            } 
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue != null)
            {
                var viewControl = ViewLocator.Build(e.NewValue); 
                if(viewControl != null)
                {
                    Content = viewControl;
                }
            }
        } 
    }
}
