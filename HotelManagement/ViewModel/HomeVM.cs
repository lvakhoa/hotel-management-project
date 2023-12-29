using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows.Media;
using RoomMap = HotelManagement.ViewModel.RoomMapVM;

namespace HotelManagement.ViewModel;

public partial class HomeVM : ObservableObject
{
    [ObservableProperty]
    private HomeItemProperty _homeData;
    [ObservableProperty]
    private SeriesCollection _seriesCollection;
    [ObservableProperty]
    private SeriesCollection _pieSeriesCollection;
    public string[] Labels { get; private set; }
    public int[] Values { get; private set; }
    [ObservableProperty]
    private bool _isLoading;
    [ObservableProperty]
    private ObservableCollection<BookingDisplay> _recentBookings;

    public class BookingDisplay
    {
        public string RoomType { get; set; }
        public string RoomNumber { get; set; }
        public string CustomerName { get; set; }
        public DateTime BookingDate { get; set; }
    }
    private async Task LoadRecentBookingsAsync()
    {
        await using var context = new HotelManagementContext();
        var recentBookings = await
            (from b in context.Bookings
             join r in context.Rooms on b.RoomId equals r.RoomId
             join i in context.Invoices on b.InvoiceId equals i.InvoiceId
             join c in context.Customers on i.CustomerId equals c.CustomerId
             join rt in context.RoomTypes on r.RoomTypeId equals rt.RoomTypeId

             orderby i.InvoiceDate descending
             select new BookingDisplay
             {
                 RoomType = rt.RoomTypeName, // Replace with actual property name for room type name
                 RoomNumber = r.RoomNumber, // Replace with actual property name for room number
                 CustomerName = c.FullName, // Replace with actual property name for customer name
                 BookingDate = (DateTime)i.InvoiceDate
             }).Take(3).ToListAsync();

        RecentBookings = new ObservableCollection<BookingDisplay>(recentBookings);
    }

    public HomeVM()
    {
        HomeData = new HomeItemProperty();
        LoadHomeDataAsync();
        _ = LoadRecentBookingsAsync();
    }

    private async void LoadHomeDataAsync()
    {
        IsLoading = true;
        await Task.Delay(100); // Simulated delay
        await using var context = new HotelManagementContext();
        var labels = new string[7];
        for (int i = -6; i <= 0; i++)
        {
            var date1 = DateTime.Today.AddDays(i);
            labels[i + 6] = date1.ToString("dd/MM"); // Format the date as you prefer
        }
        Labels = labels;
        var date = DateTime.Today;
        var rm = new RoomMap();
        await rm.GetRoomList();

        HomeData.TotalBookingMonday = await GetTotalBookingsForDate(context, -6);
        HomeData.TotalBookingTuesday = await GetTotalBookingsForDate(context, -5);
        HomeData.TotalBookingWednesday = await GetTotalBookingsForDate(context, -4);
        HomeData.TotalBookingThursday = await GetTotalBookingsForDate(context, -3);
        HomeData.TotalBookingFriday = await GetTotalBookingsForDate(context, -2);
        HomeData.TotalBookingSaturday = await GetTotalBookingsForDate(context, -1);
        HomeData.TotalBookingSunday = await GetTotalBookingsForDate(context, 0);

        HomeData.TotalAvailableRoom = await GetTotalAvailableRoom(context, rm);
        HomeData.TotalBlockedRoom = await GetTotalBlockedRoom(context, rm);
        HomeData.TotalOccupiedRoom = await GetTotalOccupiedRoom(context, rm);
        HomeData.TotalRevenueToday = await GetTotalRevenueToday(context);
        HomeData.TotalRevenue = await GetTotalRevenue(context);
        HomeData.TotalBookingToday = await GetTotalBookingsForDate(context, 0);
        HomeData.TotalBooking = await GetTotalBooking(context);
        HomeData.TotalCheckinToday = await GetTotalCheckInToday(context);
        HomeData.TotalCheckoutToday = await GetTotalCheckOutToday(context);
        HomeData.TotalStaff = await GetTotalStaff(context);
        HomeData.TotalRoom = await GetTotalRoom(context);
        HomeData.TotalCustomer = await GetTotalCustomer(context);

        InitializeCharts();
        IsLoading = false;
    }

    private void InitializeCharts()
    {
        SeriesCollection = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Total booking",
                Values = new ChartValues<Int32>
                {
                    HomeData.TotalBookingMonday,
                    HomeData.TotalBookingTuesday,
                    HomeData.TotalBookingWednesday,
                    HomeData.TotalBookingThursday,
                    HomeData.TotalBookingFriday,
                    HomeData.TotalBookingSaturday,
                    HomeData.TotalBookingSunday
                },
                DataLabels = true,
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C2FCC1"))
            },
        };
        PieSeriesCollection = new SeriesCollection
        {
            new PieSeries
            {
                Values = new ChartValues<int> { HomeData.TotalAvailableRoom },
                Title = "Available",
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#C2FCC1")),
                DataLabels = false,
                LabelPoint = chartpoint => $"{chartpoint.Y} ({chartpoint.Participation:P})"
            },
            // Other PieSeries follow...new PieSeries
            new PieSeries
            {
                Values = new ChartValues<int> { HomeData.TotalOccupiedRoom },
                Title = "Occupied",
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FCFE7C")),
                DataLabels = false,
                LabelPoint = chartpoint => $"{chartpoint.Y} ({chartpoint.Participation:P})"
            },
            new PieSeries
                // Other PieSeries follow...new PieSeries
            {
                Values = new ChartValues<int> { HomeData.TotalBlockedRoom },
                Title = "Blocked",
                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FCB7B7")),
                DataLabels = false,
                LabelPoint = chartpoint => $"{chartpoint.Y} ({chartpoint.Participation:P})"
            },
                // Other PieSeries follow...
        };
        OnPropertyChanged(nameof(Labels));
    }
    private async Task<decimal> GetTotalRevenueToday(HotelManagementContext context)
    {
        return (decimal)await context.Invoices
            .Where(i => i.InvoiceDate.Day == DateTime.Now.Day)
            .SumAsync(i => i.TotalAmount);
    }

    private async Task<decimal> GetTotalRevenue(HotelManagementContext context)
    {
        return (decimal)await context.Invoices
            .SumAsync(i => i.TotalAmount);
    }

    private async Task<int> GetTotalBookingToday(HotelManagementContext context)
    {
        var today = DateTime.Today;
        var temp = await context.Bookings
            .CountAsync(b => b.CheckInDate.Value.Date == today);
        return temp;
    }

    private async Task<int> GetTotalBookingsForDate(HotelManagementContext context, int x)
    {
        var today = DateTime.Today.AddDays(x);
        var temp = await context.Bookings
                            .CountAsync(b => b.CheckInDate.Value.Date == today);
        return temp;
    }

    private async Task<int> GetTotalBooking(HotelManagementContext context)
    {
        var temp = await context.Bookings
             .CountAsync();
        return temp;
    }

    private async Task<int> GetTotalAvailableRoom(HotelManagementContext context, RoomMap rm)
    {
        await Task.Delay(200);
        var temp = (from r in rm.List where r.Status == "Available" select r).Count();
        return temp;
    }

    private async Task<int> GetTotalBlockedRoom(HotelManagementContext context, RoomMap rm)
    {
        await Task.Delay(200);
        var temp = (from r in rm.List where r.Status == "Out of Order" select r).Count();
        return temp;
    }

    private async Task<int> GetTotalOccupiedRoom(HotelManagementContext context, RoomMap rm)
    {
        await Task.Delay(200);
        var temp = (from r in rm.List where r.Status == "Occupied" select r).Count();
        return temp;
    }

    private async Task<int> GetTotalCheckInToday(HotelManagementContext context)
    {
        var today = DateTime.Today;
        var temp = await context.Bookings
            .CountAsync(b => b.CheckInDate.Value.Date == today);
        return temp;
    }

    private async Task<int> GetTotalCheckOutToday(HotelManagementContext context)
    {
        var today = DateTime.Today;
        var temp = await context.Bookings
            .CountAsync(b => b.CheckOutDate.Value.Date == today);
        return temp;
    }

    private async Task<int> GetTotalStaff(HotelManagementContext context)
    {
        return await context.Staff.CountAsync();
    }

    private async Task<int> GetTotalCustomer(HotelManagementContext context)
    {
        return await context.Customers.CountAsync();
    }

    private async Task<int> GetTotalRoom(HotelManagementContext context)
    {
        return await context.Rooms.CountAsync();
    }

    public partial class HomeItemProperty : ObservableObject
    {
        [ObservableProperty] private int _totalBookingMonday;
        [ObservableProperty] private int _totalBookingTuesday;
        [ObservableProperty] private int _totalBookingWednesday;
        [ObservableProperty] private int _totalBookingThursday;
        [ObservableProperty] private int _totalBookingFriday;
        [ObservableProperty] private int _totalBookingSaturday;
        [ObservableProperty] private int _totalBookingSunday;
        [ObservableProperty] private int _totalOccupiedRoom;
        [ObservableProperty] private int _totalAvailableRoom;
        [ObservableProperty] private int _totalBlockedRoom;
        [ObservableProperty] private int _totalCheckinToday;
        [ObservableProperty] private int _totalCheckoutToday;
        [ObservableProperty] private decimal _totalRevenueToday;
        [ObservableProperty] private decimal _totalRevenue;
        [ObservableProperty] private int _totalBookingToday;
        [ObservableProperty] private int _totalBooking;
        [ObservableProperty] private int _totalStaff;
        [ObservableProperty] private int _totalRoom;
        [ObservableProperty] private int _totalCustomer;
    }
}