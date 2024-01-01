using System.Globalization;
using System.Windows.Data;

namespace HotelManagement.Utilities;

public class LightThemeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString().Equals("Light", StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Light" : "Dark"; 
    }
}