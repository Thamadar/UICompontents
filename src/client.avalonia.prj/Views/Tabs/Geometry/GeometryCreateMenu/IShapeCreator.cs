using Client.Avalonia.Views.Geometry.Shapes;

namespace Client.Avalonia.Views.Geometry
{
    public interface IShapeCreator
    {
        /// <summary>
        /// Создать геом. фигуру
        /// </summary> 
        public IShapeItem Create(double x, double y);
    }
}
