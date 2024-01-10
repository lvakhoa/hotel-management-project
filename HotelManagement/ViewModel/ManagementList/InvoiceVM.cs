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

    public ObservableCollection<BookingInvoice> BookingInvoiceList { get; set; }
    
    public ObservableCollection<ServiceInvoice> ServiceInvoiceList { get; set; }
    
    [ObservableProperty] private InvoiceVM _currentInvoice;
    
    [ObservableProperty] private CustomerList.CustomerVM _currentCustomer;

    [ObservableProperty] private bool _isLoading;

    #region Constructor
    public InvoiceList()
    {
        List = new ObservableCollection<InvoiceVM>();
        BookingInvoiceList = new ObservableCollection<BookingInvoice>();
        ServiceInvoiceList = new ObservableCollection<ServiceInvoice>();
        _ = GetInvoiceList();
    }

    private async Task GetInvoiceList()
    {
        List.Clear();
        
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var invoices = await (from invoice in context.Invoices.Include("Customer")
            where invoice.Deleted == false
            select new
            {
                invoice.InvoiceId,
                invoice.CustomerId,
                invoice.Customer.FullName,
                invoice.StaffId,
                invoice.InvoiceDate,
                invoice.TotalAmount,
                invoice.PaymentType
            }).ToListAsync();

        foreach (var item in invoices)
        {
            List.Add(new InvoiceVM()
            {
                StaffName = context.Staff.Find(item.StaffId).FullName,
                InvoiceID = (string)item.InvoiceId,
                CustomerId = item.CustomerId,
                CustomerName = item.FullName,
                StaffId = item.StaffId,
                InvoiceDate = item.InvoiceDate,
                TotalAmount = item.TotalAmount,
                PaymentType = item.PaymentType
            });
        }

        IsLoading = false;
    }
    
    #endregion
    
    #region PrintInvoice
    public void GetInvoiceById(string? id)
    {
        if (BookingInvoiceList.Count() != 0 || ServiceInvoiceList.Count() != 0)
        {
            BookingInvoiceList.Clear();
            ServiceInvoiceList.Clear();
        }
        decimal? totalService = 0;
        decimal? totalBooking = 0;
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
        
        var context = new HotelManagementContext();
        var bookings = (from b in context.Bookings
            join r in context.Rooms on b.RoomId equals r.RoomId
            join rt in context.RoomTypes on r.RoomTypeId equals rt.RoomTypeId
            where b.InvoiceId == id && b.Deleted == false
            select new
            {
                r.RoomNumber,
                rt.RoomTypeName,
                b.TotalAmount,
                b.CheckInDate,
                b.CheckOutDate
            }).ToList();
        foreach (var item in bookings)
        {
            string[] checkin = item.CheckInDate.ToString().Split(' ');
            string[] checkout = item.CheckOutDate.ToString().Split(' ');
            
            BookingInvoiceList.Add(new BookingInvoice()
            {
                RoomType = item.RoomTypeName,
                RoomNumber = item.RoomNumber,
                TotalAmount = item.TotalAmount,
                CheckIn = checkin[0],
                CheckOut = checkout[0]
            });
            totalBooking += item.TotalAmount;
        }
        
        var services = (from su in context.ServiceUses
            join s in context.Services on su.ServiceId equals s.ServiceId
            where su.InvoiceId == id && su.Deleted == false
            select new
            {
                s.ServiceName,
                su.ServiceQuantity,
                su.TotalAmount,
                s.ServicePrice
            }).ToList();
        foreach (var item in services)
        {
            ServiceInvoiceList.Add(new ServiceInvoice()
            {
                ServiceName = item.ServiceName,
                Quantity = item.ServiceQuantity,
                TotalAmount = item.TotalAmount,
                ServicePrice = item.ServicePrice
            });
            totalService += item.TotalAmount;
        }
        
        CurrentInvoice = new InvoiceVM()
        {
            InvoiceID = invoice.InvoiceID,
            CustomerId = invoice.CustomerId,
            StaffId = invoice.StaffId,
            InvoiceDate = invoice.InvoiceDate,
            TotalAmount = invoice.TotalAmount,
            PaymentType = invoice.PaymentType,
            TotalBooking = totalBooking,
            TotalService = totalService,
            TotalAmountTax = invoice.TotalAmount * ((App.Current.Resources["Tax"] as Decimal?) / 100 + 1),
            SaleTax = invoice.TotalAmount * (App.Current.Resources["Tax"] as Decimal? / 100)
        };

        var customer = (from i in context.Invoices
            join c in context.Customers on i.CustomerId equals c.CustomerId
            select c).FirstOrDefault();
        
        CurrentCustomer = new CustomerList.CustomerVM()
        {
            FullName = customer.FullName,
            ID = customer.CustomerId,
            Gender = customer.Gender,
            ContactNumber = customer.ContactNumber
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
            var invoice = context.Invoices.Include("Bookings").Include("ServiceUses").FirstOrDefault(e => e.InvoiceId == id);

            invoice!.Deleted = true;
            invoice.DeletedDate = DateTime.Now;
            
            foreach(var item in invoice.Bookings)
            {
                item.Deleted = true;
                item.DeletedDate = DateTime.Now;
            }
            
            foreach(var item in invoice.ServiceUses)
            {
                item.Deleted = true;
                item.DeletedDate = DateTime.Now;
            }
            
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
        public string? CustomerName { get; set; }

        public string? StaffId { get; set; }
        public string? StaffName { get; set; }

        public DateTime InvoiceDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? PaymentType { get; set; }
        #endregion
        
        public decimal? TotalBooking { get; set; }
        public decimal? TotalService { get; set; }
        public decimal? TotalAmountTax { get; set; }
        public decimal? SaleTax { get; set; }
        
    }
    
    public class ServiceInvoice
    {
        public string? ServiceName { get; set; }
        public int? Quantity { get; set; }
        public decimal? ServicePrice { get; set; }
        public decimal? TotalAmount { get; set; }
    }

    public class BookingInvoice
    {
        public string? RoomNumber { get; set; }
        public string? RoomType { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? CheckIn { get; set; }
        public string? CheckOut { get; set; }
    }
}