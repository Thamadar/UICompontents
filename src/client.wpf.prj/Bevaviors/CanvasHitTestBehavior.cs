using Client.WPF.Services;
using Client.WPF.Views.Geometry.Shapes;
using Lib.WPF.Extensions;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client.WPF.Bevaviors
{
    public class CanvasHitTestBehavior : Behavior<Canvas>
    {  
        public static readonly DependencyProperty IsHitTestEnabledProperty =
            DependencyProperty.Register(nameof(IsHitTestEnabled), typeof(bool),
                typeof(CanvasHitTestBehavior), new PropertyMetadata(false, OnIsHitTestEnabledChanged));

        public static readonly DependencyProperty CreateCommandProperty =
            DependencyProperty.Register(nameof(CreateCommand), typeof(ICommand),
                typeof(CanvasHitTestBehavior));

        public ICommand CreateCommand
        {
            get => (ICommand)GetValue(CreateCommandProperty);
            set => SetValue(CreateCommandProperty, value);
        }

        public bool IsHitTestEnabled
        {
            get => (bool)GetValue(IsHitTestEnabledProperty);
            set => SetValue(IsHitTestEnabledProperty, value);
        }
          
        private static void OnIsHitTestEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = (CanvasHitTestBehavior)d;
            behavior.OnEnabledChanged((bool)e.NewValue);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            OnEnabledChanged(IsHitTestEnabled);
        }
         
        protected override void OnDetaching()
        {
            base.OnDetaching();
             
        }

        private void OnEnabledChanged(bool newValue)
        {
            if(AssociatedObject == null) return;

            if(newValue)
            { 
                AssociatedObject.PreviewMouseDown += OnCanvasPointerPressed;
            }
            else
            {
                AssociatedObject.PreviewMouseDown -= OnCanvasPointerPressed;
            }
        }
         
        private void OnCanvasPointerPressed(object sender, MouseButtonEventArgs e)
        {
            if(AssociatedObject == null) return;

            var point = e.GetPosition(AssociatedObject);
             
            var hitElement = AssociatedObject.InputHitTest(point) as FrameworkElement;

            if(hitElement != null && hitElement != AssociatedObject)
            {
                ShapePress(hitElement, point);
            }
            else
            {
                CanvasPress(AssociatedObject, point);
            }

        }

        /// <summary>
        /// Реакция на нажатие по полотну Canvas.
        /// </summary> 
        private void CanvasPress(Canvas canvas, Point point)
        {
            CreateCommand?.Execute(point);
        }

        /// <summary>
        /// Реакция на нажатие по графическому элементу
        /// </summary> 
        private void ShapePress(FrameworkElement hitElement, Point point)
        {
            if(hitElement.DataContext is IShapeItem shapeItem)
            {
                ShapeService.Instance.SelectShapeById(shapeItem.Id);
            }
        }
    }
}
