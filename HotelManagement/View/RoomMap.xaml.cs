using HotelManagement.Model;
using HotelManagement.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.Messaging;
using System.Net;
using System.Collections.ObjectModel;

namespace HotelManagement.View;

public partial class RoomMap : UserControl
{
    public RoomMapVM? RoomVM => DataContext as RoomMapVM;
    public RoomMap()
    {
        InitializeComponent();
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