using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagement.ViewModel.ManagementList;

public partial class InvoiceList : ObservableObject
{
    public ObservableCollection<InvoiceVM> List { get; set; }
    
    [ObservableProperty] private InvoiceVM _currentInvoice;

    [ObservableProperty] private bool _isLoading;

    public InvoiceList()
    {
        List = new ObservableCollection<InvoiceVM>();

        GetInvoiceList();
    }

    private async void GetInvoiceList()
    {
        IsLoading = true;
        await Task.Delay(1000);
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
    
    #region EditRoomType

    public void GetInvoiceById(string? id)
    {
        var invoice = (from i in List
            where i.InvoiceID == id
            select new
            {
                i.InvoiceID,
                i.CustomerId,
                i.StaffId,
                i.InvoiceDate,
                i.TotalAmount,
                i.PaymentType,
            }).FirstOrDefault();
        
        CurrentInvoice = new InvoiceVM()
        {
            InvoiceID = invoice.InvoiceID,
            CustomerId = invoice.CustomerId,
            StaffId = invoice.StaffId,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            PaymentType = invoice.PaymentType,
        };
    }
    #endregion
    
    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show("Are you sure you want to delete this invoice?", "Delete Invoice",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            int index = -1;
            foreach(var item in List)
            {
                if (item.InvoiceID == id)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }
            
            if(index != -1)
                List.RemoveAt(index);
            
            using var context = new HotelManagementContext();
            var invoice = context.Invoices.Find(id);
            
            invoice!.Deleted = true;
            invoice.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }
    #endregion

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