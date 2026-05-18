using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lib.WPF.Converters
{
    /// <summary>
    /// Converter, преобразующий Enum => Boolean.
    /// ConverterParameter={x:Static enum:MyEnum.SomeEnumValue} => value?.Equals(parameter) == true.
    /// </summary>
    public class EnumToBooleanConverter : IValueConverter
    {
        public static readonly EnumToBooleanConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.Equals(parameter) == true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is bool isSelected && isSelected)
                return Enum.Parse(targetType, parameter.ToString());
            throw new NotImplementedException();
        }
    }
}
