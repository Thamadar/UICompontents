

namespace Client.WPF.Views.Geometry.Shapes
{
    /// <summary>
    /// Окружность.
    /// </summary>
    public interface ICircleItem : IShapeItem
    {
        /// <summary>
        /// Радиус.
        /// </summary>
        public double Radius { get; set; }
    }
}
