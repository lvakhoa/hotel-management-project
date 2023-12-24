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
                    Title = "Checkin",
                    Values = new ChartValues<int> { 10, 50, 39, 50, 12, 30, 20 },
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66CDAA"))
                },
                new ColumnSeries
                {
                    Title = "Checkout",
                    Values = new ChartValues<int> { 7, 6, 9, 10, 11, 5, 8 },
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCC"))
                }
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

    private async void LoadHomeDataAsync()
    {
        IsLoading = true;
        await Task.Delay(1000); // Simulated delay

        await using var context = new HotelManagementContext();

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