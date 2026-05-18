using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Lib.WPF
{
    public static class ViewLocator
    { 
        public static FrameworkElement Build(object data)
        {
            var typeVM = data.GetType();

            var name = data.GetType().FullName!.Replace("ViewModel", "View");
            var type = Type.GetType(name, (x) => AssemblyResolver(x), (a, str, b) => TypeResolver(typeVM.Assembly, str, b));
            if(TryCreateControl(type, out var control))
            {
                return control;
            }
            else
            {
                return new TextBlock
                {
                    Text = "Not Found: " + name,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }
        }

        public static bool Match(object data)
        {
            return data is ViewModelBase;
        }

        /// <summary>
        /// Assemblies the resolver.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        /// <returns>An Assembly? .</returns>
        private static Assembly? AssemblyResolver(AssemblyName assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(assembly => assembly.GetName().Name == assemblyName.FullName);
        }

        /// <summary>
        /// Types the resolver.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="typeName">The type name.</param>
        /// <param name="b">If true, b.</param>
        /// <returns>A Type? .</returns>
        private static Type? TypeResolver(Assembly? assembly, string typeName, bool b)
        {
            if(assembly is not null)
            {
                return assembly.GetType(typeName);
            }
            else
            { return null; }
        }

        /// <summary>
        /// Tries the create control.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="control">The control.</param>
        /// <returns>A bool.</returns>
        private static bool TryCreateControl(Type? type, out Control control)
        {
            if(type != null)
            {
                control = (Control)Activator.CreateInstance(type)!;
                return true;
            }
            control = default;
            return false;
        }
    }
}
