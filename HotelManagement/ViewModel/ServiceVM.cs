using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel;

public partial class ServiceList : ObservableObject
{
    public ObservableCollection<ServiceVM> List { get; set; }
    [ObservableProperty]
    private bool _isLoading;
    public ServiceList()
    {
        List = new ObservableCollection<ServiceVM>();

        GetServiceList();
    }
    private async void GetServiceList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();
        var services = await (from service in context.Services
                              select new
                              {
                                  service.ServiceId,
                                  service.ServiceName,
                                  service.ServiceType,
                                  service.ServicePrice
                              }).ToListAsync();
        foreach (var item in services)
        {
            List.Add(new ServiceVM()
            {
                ServiceId = item.ServiceId,
                ServiceName = item.ServiceName,
                ServiceType = item.ServiceType,
                ServicePrice = item.ServicePrice

            });
        }
        IsLoading = false;
    }
    public class ServiceVM
    {
        public string? ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? ServiceType { get; set; }
        public decimal ServicePrice { get; set; }


    }


}

