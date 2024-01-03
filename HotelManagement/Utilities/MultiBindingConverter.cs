using System.Globalization;
using System.Windows.Data;

namespace HotelManagement.Utilities;

public class MultiBindingConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        Tuple<object, object> tuple = new Tuple<object, object>(values[0], values[1]);
        return tuple;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        Tuple<object, object> tuple = value as Tuple<object, object>;
        return new object[] { tuple.Item1, tuple.Item2 };
    }
}