using System.Windows;
using System.Windows.Input;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.View.AddView;

public partial class AddRoom : Window
{
    public AddRoom(object dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;

        (DataContext as RoomList).GenerateRoomId();
    }
    
    public AddRoom(string? id, object dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;
        
        if(id != null)
            (DataContext as RoomList).GetRoomById(id);

    }

    private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void AddRoom_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }
    
    private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }
}