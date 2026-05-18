using Client.WPF.Views.Geometry.Shapes; 
using System.Windows;
using System.Windows.Controls; 

namespace Client.WPF.Views 
{
    public class ShapeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CircleTemplate { get; set; }
        public DataTemplate RectTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if(item is ICircleItem)
                return CircleTemplate;
            if(item is IRectItem)
                return RectTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
