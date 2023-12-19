using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel.ManagementList;

public partial class InvoiceList : ObservableObject
{
    public ObservableCollection<InvoiceVM> List { get; set; }

    [ObservableProperty] private bool _isLoading;

    public InvoiceList()
    {
        List = new ObservableCollection<InvoiceVM>();

        GetInvoiceList();
    }

    private async void GetInvoiceList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();

        var invoices = await (from invoice in context.Invoices
                              select new
                              {
                                  invoice.InvoiceId,
                                  invoice.CustomerId,
                                  invoice.StaffId,
                                  invoice.InvoiceDate,
                                  invoice.TotalAmount,
                                  invoice.PaymentType


                              }).ToListAsync();

        foreach (var item in invoices)
        {
            List.Add(new InvoiceVM()
            {
                InvoiceID = (string)item.InvoiceId,
                CustomerId = item.CustomerId,
                StaffId = item.StaffId,
                InvoiceDate = item.InvoiceDate,
                TotalAmount = item.TotalAmount,
                PaymentType = item.PaymentType
            });
        }

        IsLoading = false;
    }

    public class InvoiceVM
    {
        public string? InvoiceID { get; set; }

        public string? CustomerId { get; set; }

        public string? StaffId { get; set; }

        public DateTime InvoiceDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? PaymentType { get; set; }

    }
}