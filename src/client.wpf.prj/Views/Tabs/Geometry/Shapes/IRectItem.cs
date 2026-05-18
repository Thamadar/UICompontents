
namespace Client.WPF.Views.Geometry.Shapes
{
    /// <summary>
    /// Прямоугольник.
    /// </summary>
    public interface IRectItem : IShapeItem
    {
        /// <summary>
        /// Ширина.
        /// </summary>
        public double Width { get; set; }
        /// <summary>
        /// Высота.
        /// </summary>
        public double Height { get; set; }
    } 
}
