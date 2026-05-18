using Client.WPF.Views.Geometry.Shapes;

namespace Client.WPF.Views
{
    public interface IShapeCreator
    {
        /// <summary>
        /// Создать геом. фигуру
        /// </summary> 
        public IShapeItem Create(double x, double y);
    }
}
