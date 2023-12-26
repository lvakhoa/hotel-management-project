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
using System.Windows.Shapes;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.View.AddView
{
    /// <summary>
    /// Interaction logic for Addroom_Editroom.xaml
    /// </summary>
    public partial class AddRoomType : Window
    {
        public AddRoomType(object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;
            
            (DataContext as RoomtypeList).GenerateRoomTypeId();
        }
        
        public AddRoomType(string? id, object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;
            
            if(id != null)
                (DataContext as RoomtypeList).GetRoomTypeById(id);

        }

        private void AddRoomType_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
