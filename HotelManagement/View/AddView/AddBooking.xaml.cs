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
using Wpf.Ui.Controls;

namespace HotelManagement.View.AddView
{
    /// <summary>
    /// Interaction logic for Addbooking.xaml
    /// </summary>
    public partial class AddBooking : Window
    {
        private BookingList? Booking => DataContext as BookingList;
        
        public AddBooking(object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;

            Booking!.GenerateBookingId();
        }
        
        public AddBooking(string? id, object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;
            
            if(id != null)
                Booking!.GetBookingById(id);
        }

        private void AddBooking_OnMouseDown(object sender, MouseButtonEventArgs e)
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

        private void AddBooking_OnLoaded(object sender, RoutedEventArgs e)
        {
            if(CheckInBox.SelectedDate == null)
            {
                CheckInBox.BlackoutDates.AddDatesInPast();
                CheckInBox.SelectedDate = DateTime.Now;
            } 
            if(CheckOutBox.SelectedDate == null)
            {
                CheckOutBox.BlackoutDates.AddDatesInPast();
                CheckOutBox.SelectedDate = DateTime.Now.AddDays(1);
            }    
        }

        private void RoomIdBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var itemVm = Booking!.CurrentBooking.RoomItem;
            var index = -1;
        
            foreach (var item in Booking.RoomIdList.Where(item => item.RoomID == itemVm.RoomID))
            {
                index = Booking.RoomIdList.IndexOf(item);
                break;
            }

            RoomIdBox.SelectedIndex = index;
        }
    }
}
