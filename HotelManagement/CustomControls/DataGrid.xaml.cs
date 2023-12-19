using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using HotelManagement.View.AddView;
using MaterialDesignThemes.Wpf;

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
            if (column.Header != null)
                column.Header = SplitCamelCase(column.Header.ToString()!);
        }
        
        string type = SplitCamelCase(DataContext.GetType().Name).Replace("List", "");
        AddBtn.Content = $"Add {type}";
        
        if(type == "Invoice")
            AddBtn.Visibility = Visibility.Hidden;
    }

    private static string SplitCamelCase(string input)
    {
        return Regex.Replace(input, "([a-z0-9])([A-Z])", "$1 $2");
    }

    private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGridRow dgr) dgr.IsSelected = true;
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        switch (AddBtn.Content.ToString().Trim())
        {
            case "Add Customer":
                var addCustomer = new AddCustomer()
                {
                    ShowInTaskbar = false,
                    Topmost = true,
                    Owner = Application.Current.MainWindow
                };
                addCustomer.ShowDialog();
                break;
            case "Add Service":
                var addService = new AddService()
                {
                    ShowInTaskbar = false,
                    Topmost = true,
                    Owner = Application.Current.MainWindow
                };
                addService.ShowDialog();
                break;
            case "Add Room":
                var addRoom = new AddRoom()
                {
                    ShowInTaskbar = false,
                    Topmost = true,
                    Owner = Application.Current.MainWindow
                };
                addRoom.ShowDialog();
                break;
            case "Add Staff":
                var addStaff = new AddStaff()
                {
                    ShowInTaskbar = false,
                    Topmost = true,
                    Owner = Application.Current.MainWindow
                };
                addStaff.ShowDialog();
                break;
            case "Add Booking":
                var addBooking = new AddBooking()
                {
                    ShowInTaskbar = false,
                    Topmost = true,
                    Owner = Application.Current.MainWindow
                };
                addBooking.ShowDialog();
                break;
        }
    }

    private void ButtonDelete_OnClick(object sender, RoutedEventArgs e)
    {
        
    }
}