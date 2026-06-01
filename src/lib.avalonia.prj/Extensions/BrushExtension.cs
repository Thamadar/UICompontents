using Avalonia.Media;

namespace Lib.Avalonia.Extensions
{
    public static class BrushExtension
    {
        public static SolidColorBrush? ToSolidColorBrush(this IBrush brush, double opacity = 1)
        {
            if(brush is SolidColorBrush solid)
            {
                return new SolidColorBrush(Color.FromArgb((byte)(255 * opacity), solid.Color.R, solid.Color.G, solid.Color.B)); 
            }

            return null;
        }
    }
}
