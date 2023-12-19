using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel.ManagementList;

public partial class ServiceUseList : ObservableObject
{
    public ObservableCollection<ServiceUseVM> List { get; set; }
    [ObservableProperty]
    private bool _isLoading;
    public ServiceUseList()
    {
        List = new ObservableCollection<ServiceUseVM>();
        GetServiceUseList();
    }
    private async void GetServiceUseList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();
        var serviceuses = await (from serviceuse in context.ServiceUses
                                 select new
                                 {
                                     serviceuse.InvoiceId,
                                     serviceuse.ServiceId,
                                     serviceuse.ServiceQuantity,
                                     serviceuse.TotalAmount
                                 }
                            ).ToListAsync();
        foreach (var item in serviceuses)
        {
            List.Add(new ServiceUseVM()
            {
                ServiceId = item.ServiceId,
                InvoiceId = item.InvoiceId,
                ServiceQuanity = item.ServiceQuantity,
                TotalAmount = item.TotalAmount,
            });
        }

        IsLoading = false;
    }
    public class ServiceUseVM
    {
        public string? InvoiceId { get; set; }
        public string? ServiceId { get; set; }
        public int? ServiceQuanity { get; set; }
        public decimal? TotalAmount { get; set; }

    }
}