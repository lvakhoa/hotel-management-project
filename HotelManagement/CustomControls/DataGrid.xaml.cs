using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace HotelManagement.CustomControls;

public partial class DataGrid : UserControl
{
    public DataGrid()
    {
        InitializeComponent();
    }
    
    private void DataGridElement_OnChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
    {
        foreach (var column in DataGrid1.Columns)
        {
            if(column.Header != null)
                column.Header = SplitCamelCase(column.Header.ToString()!);
        }
    }

    private static string SplitCamelCase(string input)
    {
        return Regex.Replace(input, "([a-z0-9])([A-Z])", "$1 $2");
    }
}