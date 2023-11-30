using System.Windows;
using System.Windows.Controls;

namespace HotelManagement.CustomControls;

public partial class DataGrid : UserControl
{
    public DataGrid()
    {
        InitializeComponent();
    }
    
    private void DataGrid_OnLoaded(object sender, RoutedEventArgs e)
    {
        foreach (var column in DataGrid1.Columns)
        {
            column.Header = SplitCamelCase(column.Header.ToString());
        }
    }

    private static string SplitCamelCase(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, "([a-z0-9])([A-Z])", "$1 $2");
    }
}