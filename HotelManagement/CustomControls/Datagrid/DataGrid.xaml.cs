using System.Windows;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using HotelManagement.View.AddView;
using HotelManagement.ViewModel.ManagementList;
using MaterialDesignThemes.Wpf;
using UIBtn = Wpf.Ui.Controls.Button;

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

    private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn) sender;
        
        switch (AddBtn.Content.ToString().Trim())
        {
            case "Add Customer":
                var addCustomer = new AddCustomer(this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addCustomer.ShowDialog();
                break;
            case "Add Service":
                var addService = new AddService(this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addService.ShowDialog();
                break;
            case "Add Room":
                var addRoom = new AddRoom(this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addRoom.ShowDialog();
                break;
            case "Add Roomtype":
                var addRoomType = new AddRoomType(this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addRoomType.ShowDialog();
                break;
            case "Add Staff":
                var addStaff = new AddStaff(this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addStaff.ShowDialog();
                break;
            case "Add Booking":
                var addBooking = new AddBooking(this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addBooking.ShowDialog();
                break;
        }
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn) sender;
        
        switch (AddBtn.Content.ToString().Trim())
        {
            case "Add Customer":
                var addCustomer = new AddCustomer(btn.Tag.ToString(), this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addCustomer.ShowDialog();
                break;
            case "Add Service":
                var addService = new AddService(btn.Tag.ToString(), this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addService.ShowDialog();
                break;
            case "Add Room":
                var addRoom = new AddRoom(btn.Tag.ToString(), this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addRoom.ShowDialog();
                break;
            case "Add Roomtype":
                var addRoomType = new AddRoomType(btn.Tag.ToString(), this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addRoomType.ShowDialog();
                break;
            case "Add Staff":
                var addStaff = new AddStaff(btn.Tag.ToString(), this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addStaff.ShowDialog();
                break;
            case "Add Booking":
                var addBooking = new AddBooking(btn.Tag.ToString(), this.DataContext)
                {
                    ShowInTaskbar = false,
                    Topmost = true
                };
                addBooking.ShowDialog();
                break;
        }
    }
}