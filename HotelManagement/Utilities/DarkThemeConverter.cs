using System.Globalization;
using System.Windows.Data;

namespace HotelManagement.Utilities;

public class DarkThemeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString().Equals("Dark", StringComparison.OrdinalIgnoreCase) ?? false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "Dark" : "Light"; 
    }
}