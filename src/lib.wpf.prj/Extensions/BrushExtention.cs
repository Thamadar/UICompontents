using System.Windows.Media;

namespace Lib.WPF.Extensions
{
    public static class BrushExtention
    {
        public static SolidColorBrush? ToSolidColorBrush(this Brush brush, double opacity = 1)
        {
            if(brush is SolidColorBrush solid)
            {
                return new SolidColorBrush(Color.FromArgb((byte)(255 * opacity), solid.Color.R, solid.Color.G, solid.Color.B));
            }

            return null;
        }
    }
}
