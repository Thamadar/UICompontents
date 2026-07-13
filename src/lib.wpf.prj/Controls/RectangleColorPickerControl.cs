using ReactiveUI; 

using System.Reactive; 
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Lib.WPF.Controls
{
    [TemplatePart(Name = "PART_MainBorder", Type = typeof(Border))]
    [TemplatePart(Name = "PART_ItemsControl", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class RectangleColorPickerControl : Control
    {

        private Border? _mainBorder;
        private ItemsControl? _itemsControl;
        private Popup? _popup;
          
        private bool _areControlsAvailable;


        private static readonly IEnumerable<Brush> DefaultBrushes = new List<Brush>
        {
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Yellow),
                new SolidColorBrush(Colors.Purple),
                new SolidColorBrush(Colors.Orange),
                new SolidColorBrush(Colors.White),
        };

        public static readonly DependencyProperty SelectedBrushProperty =
            DependencyProperty.Register(nameof(SelectedBrush), typeof(Brush), typeof(RectangleColorPickerControl), new FrameworkPropertyMetadata(
            default(Brush),
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault
        )); 

        public static readonly DependencyProperty AvailableBrushesProperty =
            DependencyProperty.Register(nameof(AvailableBrushes), typeof(IEnumerable<Brush>), typeof(RectangleColorPickerControl),
                new FrameworkPropertyMetadata(
                    defaultValue: DefaultBrushes,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
         
        public static readonly DependencyProperty WidthButtonProperty =
            DependencyProperty.Register(nameof(WidthButton), typeof(double), typeof(RectangleColorPickerControl));

        public static readonly DependencyProperty HeightButtonProperty =
            DependencyProperty.Register(nameof(HeightButton), typeof(double), typeof(RectangleColorPickerControl));

        public static readonly DependencyProperty SpacingBrushesProperty =
            DependencyProperty.Register(nameof(SpacingBrushes), typeof(double), typeof(RectangleColorPickerControl));

        public static readonly DependencyProperty IsAvaivableOpenPopupProperty =
            DependencyProperty.Register(nameof(IsAvaivableOpenPopup), typeof(bool), typeof(RectangleColorPickerControl));

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(nameof(IsPopupOpen), typeof(bool), typeof(RectangleColorPickerControl)); 
          
        public static readonly DependencyProperty PickColorCommandProperty =
            DependencyProperty.Register(nameof(PickColorCommand), typeof(ICommand), typeof(RectangleColorPickerControl));
         
        public ICommand PickColorCommand
        {
            get => (ICommand)GetValue(PickColorCommandProperty);
            set => SetValue(PickColorCommandProperty, value);
        }

        /// <summary>
        /// Список доступных цветов для выбора.
        /// </summary>
        public IEnumerable<Brush> AvailableBrushes
        {
            get => (IEnumerable<Brush>)GetValue(AvailableBrushesProperty);
            set => SetValue(AvailableBrushesProperty, value);
        } 

        /// <summary>
        /// Выбранный цвет.
        /// </summary>
        public Brush? SelectedBrush
        {
            get => (Brush?)GetValue(SelectedBrushProperty);
            set => SetValue(SelectedBrushProperty, value);
        } 

        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        /// <summary>
        /// Может ли открываться Popup?
        /// </summary>
        public bool IsAvaivableOpenPopup
        {
            get => (bool)GetValue(IsAvaivableOpenPopupProperty);
            set => SetValue(IsAvaivableOpenPopupProperty, value);
        }

        /// <summary>
        /// Ширина кнопки, по которой открывается список цветов.
        /// </summary>
        public double WidthButton
        {
            get => (double)GetValue(WidthButtonProperty);
            set => SetValue(WidthButtonProperty, value);
        }

        /// <summary>
        /// Высота кнопки, по которой открывается список цветов.
        /// </summary>
        public double HeightButton
        {
            get => (double)GetValue(HeightButtonProperty);
            set => SetValue(HeightButtonProperty, value);
        }

        /// <summary>
        /// Отступы между цветами в списке.
        /// </summary>
        public double SpacingBrushes
        {
            get => (double)GetValue(SpacingBrushesProperty);
            set => SetValue(SpacingBrushesProperty, value);
        }

        public RectangleColorPickerControl()
        {
            PickColorCommand = ReactiveCommand.Create<Brush>(PickColor); 
        }

        public override void OnApplyTemplate()
        {
            _areControlsAvailable = false;

            base.OnApplyTemplate();

            var mainBorder   = GetTemplateChild("PART_MainBorder") as Border;
            var popup        = GetTemplateChild("PART_Popup") as Popup;
            var itemsControl = GetTemplateChild("PART_ItemsControl") as ItemsControl;

            if(mainBorder != null)
            {
                _mainBorder = InitMainBorder(mainBorder);
            }
            if(popup != null)
            {
                _popup = InitPopup(popup);
            }
            if(itemsControl != null)
            {
                _itemsControl = InitItemsControl(itemsControl);
            }

            if(SelectedBrush == null)
            {
                SelectedBrush = DefaultBrushes.FirstOrDefault();
            }

            _areControlsAvailable = true;
        }

        private Border InitMainBorder(Border border)
        {
            var binding = new Binding(nameof(SelectedBrush))
            {
                Source = this,
                Mode = BindingMode.TwoWay
            };
            border.SetBinding(Border.BackgroundProperty, binding);
            border.MouseDown += OpenPopup;

            return border;
        }

        private Popup InitPopup(Popup popup)
        {
            var binding = new Binding(nameof(IsPopupOpen))
            {
                Source = this,
                Mode = BindingMode.TwoWay
            };
            popup.SetBinding(Popup.IsOpenProperty, binding);

            return popup;
        }

        private ItemsControl InitItemsControl(ItemsControl itemsControl)
        {
            var binding = new Binding(nameof(AvailableBrushes))
            {
                Source = this,
                Mode = BindingMode.TwoWay
            };
            itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, binding);  

            return itemsControl;
        }

        private void OpenPopup(object? sender, MouseButtonEventArgs e)
        {
            if(IsAvaivableOpenPopup)
            {
                IsPopupOpen = !IsPopupOpen;
            }
        }

        private void PickColor(Brush brush)
        {
            SelectedBrush = brush;
            IsPopupOpen = false;
        }
    }
}
