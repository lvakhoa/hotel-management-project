using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media;

namespace HotelManagement.ViewModel;

public partial class HomeVM : ObservableObject
{
    // Property to hold the data. Renamed for clarity.
    [ObservableProperty]
    private HomeItemProperty _homeData;
    public SeriesCollection SeriesCollection { get; private set; }
    public SeriesCollection PieSeriesCollection { get; private set; }
    public string[] Labels { get; private set; }
    [ObservableProperty]
    private bool _isLoading;

    private void InitializeCharts()
    {
        SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total booking",
                    Values = new ChartValues<int>
                    {
                HomeData.TotalBookingMonday,
                HomeData.TotalBookingTuesday,
                HomeData.TotalBookingWednesday,
                HomeData.TotalBookingThursday,
                HomeData.TotalBookingFriday,
                HomeData.TotalBookingSaturday,
                HomeData.TotalBookingSunday
            },
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66CDAA"))
                },
                //new ColumnSeries
                //{
                //    Title = "Checkout",
                //    Values = new ChartValues<int> { 7, 6, 9, 10, 11, 5, 8 },
                //    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCC"))
                //}
            };

        Labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

        PieSeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Values = new ChartValues<int> { 3 },
                    Title = "Available",
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E4FFE0")),
                    DataLabels = true,
                    LabelPoint = chartpoint => $"{chartpoint.Y} ({chartpoint.Participation:P})"
                },
            // Other PieSeries follow...new PieSeries
              new PieSeries
                {
                    Values = new ChartValues<int> { 3 },
                    Title = "Occupied",
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE0FC")),
                    DataLabels = true,
                    LabelPoint = chartpoint => $"{chartpoint.Y} ({chartpoint.Participation:P})"
                },
                new PieSeries
                // Other PieSeries follow...new PieSeries
                {
                    Values = new ChartValues<int> { 3 },
                    Title = "Blocked",
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F8EFE2")),
                    DataLabels = true,
                    LabelPoint = chartpoint => $"{chartpoint.Y} ({chartpoint.Participation:P})"
                },
                // Other PieSeries follow...
            };
    }
    public HomeVM()
    {
        HomeData = new HomeItemProperty(); // Initialize HomeData
        LoadHomeDataAsync(); // Call method to load data
        InitializeCharts();
    }
    private (DateTime, DateTime) GetLastWeekRange()
    {
        var today = DateTime.Today;
        var endOfLastWeek = today.AddDays(-(int)today.DayOfWeek);
        var startOfLastWeek = endOfLastWeek.AddDays(-6); // Assuming week starts on Monday

        return (startOfLastWeek, endOfLastWeek);
    }

    private async Task<int> GetTotalBookingsForDay(DayOfWeek day, DateTime startDate, DateTime endDate, HotelManagementContext context)
    {
        var bookings = await context.Bookings
                                    .Where(b => b.CheckInDate.Date >= startDate &&
                                                b.CheckInDate.Date <= endDate)
                                    .ToListAsync();

        return bookings.Count(b => b.CheckInDate.DayOfWeek == day);
    }

    private async void LoadHomeDataAsync()
    {
        IsLoading = true;
        await Task.Delay(1000); // Simulated delay

        await using var context = new HotelManagementContext();
        var (startOfLastWeek, endOfLastWeek) = GetLastWeekRange();
        HomeData.TotalBookingMonday = await GetTotalBookingsForDay(DayOfWeek.Monday, startOfLastWeek, endOfLastWeek, context);
        HomeData.TotalBookingTuesday = await GetTotalBookingsForDay(DayOfWeek.Tuesday, startOfLastWeek, endOfLastWeek, context);
        HomeData.TotalBookingWednesday = await GetTotalBookingsForDay(DayOfWeek.Wednesday, startOfLastWeek, endOfLastWeek, context);
        HomeData.TotalBookingThursday = await GetTotalBookingsForDay(DayOfWeek.Thursday, startOfLastWeek, endOfLastWeek, context);
        HomeData.TotalBookingFriday = await GetTotalBookingsForDay(DayOfWeek.Friday, startOfLastWeek, endOfLastWeek, context);
        HomeData.TotalBookingSaturday = await GetTotalBookingsForDay(DayOfWeek.Saturday, startOfLastWeek, endOfLastWeek, context);
        HomeData.TotalBookingSunday = await GetTotalBookingsForDay(DayOfWeek.Sunday, startOfLastWeek, endOfLastWeek, context);
        HomeData.TotalRevenueToday = await GetTotalRevenueToday(context);
        HomeData.TotalRevenue = await GetTotalRevenue(context);
        HomeData.TotalBookingToday = await GetTotalBookingToday(context);
        HomeData.TotalBooking = await GetTotalBooking(context);
        HomeData.TotalStaff = await GetTotalStaff(context);
        HomeData.TotalRoom = await GetTotalRoom(context);
        HomeData.TotalCustomer = await GetTotalCustomer(context);


        IsLoading = false;
    }

    private async Task<decimal> GetTotalRevenueToday(HotelManagementContext context)
    {
        return (decimal)await context.Invoices
            .Where(i => i.InvoiceDate.Month == DateTime.Now.Day)
            .SumAsync(i => i.TotalAmount);
    }

    private async Task<decimal> GetTotalRevenue(HotelManagementContext context)
    {
        return (decimal)await context.Invoices

            .SumAsync(i => i.TotalAmount);
    }

    private async Task<int> GetTotalBookingToday(HotelManagementContext context)
    {
        return await context.Bookings
            .CountAsync(b => b.CheckInDate.Month == DateTime.Now.Day);
    }

    private async Task<int> GetTotalBooking(HotelManagementContext context)
    {
        var temp = await context.Bookings
             .CountAsync();
        return temp;
    }

    // Define methods for total staff, total customers, total transactions, and total rooms
    // Replace these methods with the correct logic based on your database schema
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
        [ObservableProperty]
        private int _totalBookingMonday;
        [ObservableProperty]
        private int _totalBookingTuesday;
        [ObservableProperty]
        private int _totalBookingWednesday;
        [ObservableProperty]
        private int _totalBookingThursday;
        [ObservableProperty]
        private int _totalBookingFriday;
        [ObservableProperty]
        private int _totalBookingSaturday;
        [ObservableProperty]
        private int _totalBookingSunday;
        [ObservableProperty]
        private int _totalCheckinToday;
        [ObservableProperty]
        private int _totalCheckoutToday;

        [ObservableProperty]
        private decimal _totalRevenueToday;

        [ObservableProperty]
        private decimal _totalRevenue;

        [ObservableProperty]
        private int _totalBookingToday;

        [ObservableProperty]
        private int _totalBooking;

        [ObservableProperty]
        private int _totalStaff;

        [ObservableProperty]
        private int _totalRoom;
        [ObservableProperty]
        private int _totalCustomer;


    }
}