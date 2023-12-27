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
}