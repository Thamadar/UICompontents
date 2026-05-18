using System.Windows.Media;

namespace Client.WPF.Views.Geometry.Shapes
{
    /// <summary>
    /// Геометрическая фигура.
    /// </summary>
    public interface IShapeItem : IDisposable
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Выбран ли элемент?
        /// </summary>
        public bool IsSelected { get; set; }

        /// <summary>
        /// Координата X.
        /// </summary>
        public double XCenter { get; set; }

        /// <summary>
        /// Координата Y.
        /// </summary>
        public double YCenter { get; set; }

        /// <summary>
        /// Толщина контура по всей фигуре.
        /// </summary>
        public double BorderThickness { get; set; }

        /// <summary>
        /// Прозрачность.
        /// </summary>
        public double Opacity { get; set; }

        /// <summary>
        /// Цвет заливки.
        /// </summary>
        public SolidColorBrush? Fill { get; set; }

        /// <summary>
        /// Цвет контура.
        /// </summary>
        public SolidColorBrush? BorderBrush { get; set; }

    }
}
