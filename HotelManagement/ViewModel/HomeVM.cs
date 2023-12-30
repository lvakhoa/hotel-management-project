using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.ViewModel;

public partial class HomeVM : ObservableObject
{
    // Property to hold the data. Renamed for clarity.
    [ObservableProperty]
    private HomeProperty homeData;

    [ObservableProperty]
    private bool isLoading;

    public HomeVM()
    {
        HomeData = new HomeProperty(); // Initialize HomeData
        LoadHomeDataAsync(); // Call method to load data
    }

    private async void LoadHomeDataAsync()
    {
        IsLoading = true;
        await Task.Delay(1000); // Simulated delay

        await using var context = new HotelManagementContext();

        HomeData.TotalRevenueMonth = await GetTotalRevenueThisMonth(context);
        HomeData.TotalRevenue = await GetTotalRevenueThisYear(context);
        HomeData.TotalBookingMonth = await GetTotalBookingThisMonth(context);
        HomeData.TotalBooking = await GetTotalBookingThisYear(context);
        HomeData.TotalStaff = await GetTotalStaff(context);
        HomeData.TotalRoom = await GetTotalRoom(context);

        IsLoading = false;
    }
    private async Task<decimal> GetTotalRevenueThisMonth(HotelManagementContext context)
    {
        return (decimal)await context.Invoices
            .Where(i => i.InvoiceDate.Month == DateTime.Now.Month)
            .SumAsync(i => i.TotalAmount);
    }

    private async Task<decimal> GetTotalRevenueThisYear(HotelManagementContext context)
    {
        return (decimal)await context.Invoices
            .Where(i => i.InvoiceDate.Year == DateTime.Now.Year)
            .SumAsync(i => i.TotalAmount);
    }

    private async Task<int> GetTotalBookingThisMonth(HotelManagementContext context)
    {
        return await context.Bookings
            .CountAsync(b => b.CheckInDate.Month == DateTime.Now.Month);
    }

    private async Task<int> GetTotalBookingThisYear(HotelManagementContext context)
    {
        return await context.Bookings
            .CountAsync(b => b.CheckInDate.Year == DateTime.Now.Year);
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


    public class HomeProperty
    {
        public decimal TotalRevenueMonth { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBookingMonth { get; set; }
        public int TotalBooking { get; set; }
        public int TotalStaff { get; set; }
        public int TotalRoom { get; set; }
    }
}