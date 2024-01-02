using System.Windows;
using HotelManagement.ViewModel;
using System.Windows.Controls;

namespace HotelManagement.View;

public partial class RoomMap : UserControl
{
    public RoomMapVM? RoomVM => DataContext as RoomMapVM;
    public RoomMap()
    {
        InitializeComponent();
        _ = RoomVM.GetRoomList();
        ShowListRoom.SelectedIndex = 0;
    }
    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (RoomVM != null)
        {
            RoomVM.SelectFloor(ShowListRoom.SelectedItem.ToString());
        }
    }
    private void FilterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Filter.IsOpen = true;
    }

    private void OccupiedMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (RoomVM != null)
        {
            RoomVM.StatusFilter("Occupied");
        }
    }

    private void AvailableMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (RoomVM != null)
        {
            RoomVM.StatusFilter("Available");
        }
    }

    private void OutOfOrderMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (RoomVM != null)
        {
            RoomVM.StatusFilter("OutOfOrder");
        }
    }

    private void ShowAllMenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        if (RoomVM != null)
        {
            RoomVM.StatusFilter(" ");
        }
    }
}