using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.CustomControls.MessageBox;

namespace HotelManagement.ViewModel.ManagementList;

public partial class InvoiceList : ObservableObject
{
    public ObservableCollection<InvoiceVM> List { get; set; }

    [ObservableProperty] private InvoiceVM _currentInvoice;

    [ObservableProperty] private bool _isLoading;

    #region Constructor
    public InvoiceList()
    {
        List = new ObservableCollection<InvoiceVM>();

        _ = GetInvoiceList();
    }

    private async Task GetInvoiceList()
    {
        List.Clear();
        
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var invoices = await (from invoice in context.Invoices
            where invoice.Deleted == false
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
    
    #endregion

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
        var result = MessageBox.Show(
            App.ActivatedWindow, "Delete Invoice",
            "Are you sure you want to delete this invoice?",
            msgImage: MessageBoxImage.WARNING, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.InvoiceID == id)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List.RemoveAt(index);

            using var context = new HotelManagementContext();
            var invoice = context.Invoices.Find(id);

            invoice!.Deleted = true;
            invoice.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }

    #endregion
    
    #region Restore Command

    [RelayCommand]
    private async Task RestoreLast7Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Invoice",
            "Restore all invoices that have been deleted in the last 7 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var invoices = await context.Invoices.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-7)).ToListAsync();

            foreach (var invoice in invoices)
            {
                invoice.Deleted = false;
                invoice.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore invoices successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetInvoiceList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreLast30Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Invoice",
            "Restore all invoices that have been deleted in the last 30 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var invoices = await context.Invoices.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-30)).ToListAsync();

            foreach (var invoice in invoices)
            {
                invoice.Deleted = false;
                invoice.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore invoices successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetInvoiceList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreAll()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Invoice",
            "Restore all invoices that have been deleted?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var invoices = await context.Invoices.Where(e => e.Deleted == true).ToListAsync();

            foreach (var invoice in invoices)
            {
                invoice.Deleted = false;
                invoice.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore invoices successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetInvoiceList();
        }
    }
    
    #endregion

    public class InvoiceVM
    {
        #region Properties
        public string? InvoiceID { get; set; }

        public string? CustomerId { get; set; }

        public string? StaffId { get; set; }

        public DateTime InvoiceDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? PaymentType { get; set; }
        #endregion
    }
}