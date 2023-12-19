using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel.ManagementList;

public partial class BookingList : ObservableObject
{
    public ObservableCollection<BookingVM>? List { get; set; }
    [ObservableProperty]
    private bool _isLoading;
    public BookingList()
    {
        List = new ObservableCollection<BookingVM>();
        GetBookingList();

    }
    private async void GetBookingList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();
        var bookings = await (from booking in context.Bookings
                              select new
                              {
                                  booking.BookingId,
                                  booking.InvoiceId,
                                  booking.RoomId,
                                  booking.GuestQuantity,
                                  booking.CheckInDate,
                                  booking.CheckOutDate,
                                  booking.TotalAmount
                              }).ToListAsync();
        foreach (var item in bookings)
        {
            List.Add(new BookingVM()
            {
                BookingId = item.BookingId,
                InvoiceId = item.InvoiceId,
                RoomId = item.RoomId,
                QuestQuanity = item.GuestQuantity,
                CheckInDate = item.CheckInDate,
                CheckOutDate = item.CheckOutDate,
                TotalAmount = item.TotalAmount
            });

        }
        IsLoading = false;
    }
    public class BookingVM
    {
        public string? BookingId { get; set; }
        public string? InvoiceId { get; set; }
        public string? RoomId { get; set; }
        public int QuestQuanity { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal? TotalAmount { get; set; }

    }
}