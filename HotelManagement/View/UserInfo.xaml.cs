using System.Windows;
using System.Windows.Input;
using HotelManagement.ViewModel;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.View;

public partial class UserInfo : Window
{
    public UserInfo(object dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
        (DataContext as SettingsVM).UpdateStaff();
    }

    private void CloseButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void UserInfo_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }
}