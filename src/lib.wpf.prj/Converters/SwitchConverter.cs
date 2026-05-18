using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Lib.WPF.Converters
{
    public class SwitchConverter : IValueConverter
    {
        public object Default { get; set; }

        public IValueConverter PreConverter { get; set; }
        public IValueConverter PostConverter { get; set; }

        public object? PostConverterParameter { get; set; }
        public object? PreConverterParameter { get; set; }

        public CaseSet Cases { get; set; } = new();
        public bool TypeMode { get; set; }

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(TypeMode)
            {
                value = value?.GetType().Name;
            }
            else
            {
                value = PreConverter?.Convert(value, targetType, PreConverterParameter, culture) ?? value;
            }

            var pair = Cases.FirstOrDefault(x => Equals(TypeMode ? (x.KeyType?.Name ?? "") : x.Key, value) || SafeCompareAsStrings(TypeMode ? (x.KeyType?.Name ?? "") : x.Key, value));
            value = pair == null ? Default : pair.Value;
            return PostConverter?.Convert(value, targetType, PostConverterParameter, culture) ?? value;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(TypeMode)
            {
                value = value?.GetType().Name;
            }
            else
            {
                value = PreConverter?.ConvertBack(value, targetType, PreConverterParameter, culture) ?? value;
            }

            var pair = Cases.FirstOrDefault(x => Equals(x.Value, value) || SafeCompareAsStrings(x.Value, value));
            value = pair == null ? Default : pair.Key;
            return PostConverter?.ConvertBack(value, targetType, PostConverterParameter, culture) ?? value;
        }

        private static bool SafeCompareAsStrings(object? a, object? b)
        {
            if(a == null && b == null) return true;
            if(a == null || b == null) return false;
            return string.Equals(a.ToString(), b.ToString(), StringComparison.OrdinalIgnoreCase);
        }
    }

    public class CaseSet : List<Case> { }

    public class Case
    {
        public object? Key { get; set; }
        public Type? KeyType { get; set; }
        public object? Value { get; set; }
    }

}
