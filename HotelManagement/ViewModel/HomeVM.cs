using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Windows.Media;

namespace HotelManagement.ViewModel;

public partial class HomeVM : ObservableObject
{

    [ObservableProperty]
    private HomeItemProperty _homeData;
    public SeriesCollection SeriesCollection { get; private set; }
    public SeriesCollection PieSeriesCollection { get; private set; }
    public string[] Labels { get; private set; }
    [ObservableProperty]
    private bool _isLoading;
    public HomeVM()
    {
        HomeData = new HomeItemProperty();
        LoadHomeDataAsync();
        InitializeCharts();
    }
    private void InitializeCharts()
    {
        SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Total booking",
                    Values = new ChartValues<int>
                    {//10, 50, 39, 50, 12, 30, 20
               (int) HomeData.TotalBookingMonday,
               (int) HomeData.TotalBookingTuesday,
                (int)HomeData.TotalBookingWednesday,
                (int)HomeData.TotalBookingThursday,
               (int) HomeData.TotalBookingFriday,
                (int)HomeData.TotalBookingSaturday,
                (int)HomeData.TotalBookingSunday
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

        //Labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

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





    private async void LoadHomeDataAsync()
    {
        IsLoading = true;
        await Task.Delay(1000); // Simulated delay

        await using var context = new HotelManagementContext();


        var labels = new string[7];
        for (int i = -6; i <= 0; i++)
        {
            var date1 = DateTime.Today.AddDays(i);
            labels[i + 6] = date1.ToString("dd/MM"); // Format the date as you prefer
        }

        var date = DateTime.Today;
        HomeData.TotalBookingMonday = await GetTotalBookingsForDate(context, -6);
        HomeData.TotalBookingTuesday = await GetTotalBookingsForDate(context, -5);
        HomeData.TotalBookingWednesday = await GetTotalBookingsForDate(context, -4);
        HomeData.TotalBookingThursday = await GetTotalBookingsForDate(context, -3);
        HomeData.TotalBookingFriday = await GetTotalBookingsForDate(context, -2);
        HomeData.TotalBookingSaturday = await GetTotalBookingsForDate(context, -1);
        HomeData.TotalBookingSunday = await GetTotalBookingsForDate(context, 0);

        HomeData.TotalRevenueToday = await GetTotalRevenueToday(context);
        HomeData.TotalRevenue = await GetTotalRevenue(context);
        HomeData.TotalBookingToday = await GetTotalBookingsForDate(context, 0);
        HomeData.TotalBooking = await GetTotalBooking(context);
        HomeData.TotalCheckinToday = await GetTotalCheckInToday(context);
        HomeData.TotalCheckoutToday = await GetTotalCheckOutToday(context);
        HomeData.TotalStaff = await GetTotalStaff(context);
        HomeData.TotalRoom = await GetTotalRoom(context);
        HomeData.TotalCustomer = await GetTotalCustomer(context);
        Labels = labels;

        IsLoading = false;
        InitializeCharts();
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
    //private async Task<int> GetTotalAvailableRoom(HotelManagementContext context)
    //{
    //    var temp = await context.Bookings
    //         .CountAsync();
    //    return temp;
    //}
    //private async Task<int> GetTotalBlockedRoom(HotelManagementContext context)
    //{
    //    var temp = await context.Bookings
    //         .CountAsync();
    //    return temp;
    //}
    //private async Task<int> GetTotalOccupiedRoom(HotelManagementContext context)
    //{
    //    var temp = await context.Roô
    //         .CountAsync(b=>b.);
    //    return temp;
    //}
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
        private decimal _totalOccupiedRoom;
        [ObservableProperty]
        private decimal _totalAvailableRoom;
        [ObservableProperty]
        private decimal _totalBlockedRoom;
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