using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;

namespace Lib.Avalonia.Extensions
{
    public static class ApplicationExtension
    {
        /// <summary>
        /// Поиск ControlTemplate по x:Name в ресурсах приложения.
        /// </summary>  
        public static ControlTemplate? GetTemplateResource(this Application? application, string templateName)
        {
            ControlTemplate? template = null;

            var resourceIcon = application?.FindResource(templateName); 
            template = resourceIcon != null ?
                       resourceIcon as ControlTemplate :
                       null;

            return template;
        }
    }

}
