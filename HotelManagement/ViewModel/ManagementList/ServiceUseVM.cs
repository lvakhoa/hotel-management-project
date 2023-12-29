using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.CustomControls.MessageBox;

namespace HotelManagement.ViewModel.ManagementList;

public partial class ServiceUseList : ObservableObject
{
    public ObservableCollection<ServiceUseVM> List { get; set; }

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private ServiceUseVM _currentServiceUse;

    [ObservableProperty] private List<string> _invoiceIdList;

    [ObservableProperty] private List<ServiceInfo> _serviceIdList;

    #region Constructor

    public ServiceUseList()
    {
        List = new ObservableCollection<ServiceUseVM>();
        
        _ = GetServiceUseList();

        InvoiceIdList = new List<string>();
        ServiceIdList = new List<ServiceInfo>();
    }

    private async Task GetServiceUseList()
    {
        List.Clear();
        
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var serviceUses = await (from serviceUse in context.ServiceUses
                join service in context.Services on serviceUse.ServiceId equals service.ServiceId
                where serviceUse.Deleted == false
                select new
                {
                    serviceUse.InvoiceId,
                    serviceUse.ServiceId,
                    service.ServiceName,
                    serviceUse.ServiceQuantity,
                    serviceUse.TotalAmount
                }
            ).ToListAsync();

        foreach (var item in serviceUses)
        {
            List.Add(new ServiceUseVM()
            {
                InvoiceId = item.InvoiceId,
                ServiceItem = new ServiceInfo()
                {
                    ServiceId = item.ServiceId,
                    ServiceName = item.ServiceName
                },
                ServiceQuantity = item.ServiceQuantity.ToString(),
                TotalAmount = item.TotalAmount,
            });
        }

        IsLoading = false;

        InvoiceIdList = await (from invoice in context.Invoices
            where invoice.Deleted == false
            orderby invoice.InvoiceId
            select invoice.InvoiceId).ToListAsync();

        ServiceIdList = await (from service in context.Services
                where service.Deleted == false
                orderby service.ServiceId
                select new ServiceInfo() { ServiceId = service.ServiceId, ServiceName = service.ServiceName })
            .ToListAsync();
    }

    #endregion

    #region AddServiceUse

    public void AddServiceUse()
    {
        CurrentServiceUse = new ServiceUseVM() {ServiceItem = new ServiceInfo()};
        CurrentServiceUse.PropertyChanged += (_, _) => Add_EditServiceUseCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region EditServiceUse

    public void GetServiceUseById(string serviceId, string invoiceId)
    {
        using var context = new HotelManagementContext();
        var serviceUse = (from s in List where s.InvoiceId == invoiceId && s.ServiceItem.ServiceId == serviceId select s)
            .FirstOrDefault();

        CurrentServiceUse = new ServiceUseVM()
        {
            InvoiceId = serviceUse.InvoiceId,
            ServiceItem = new ServiceInfo()
            {
                ServiceId = serviceUse.ServiceItem.ServiceId,
                ServiceName = serviceUse.ServiceItem.ServiceName
            },
            ServiceQuantity = serviceUse.ServiceQuantity,
            TotalAmount = serviceUse.TotalAmount
        };

        CurrentServiceUse.PropertyChanged += (_, _) => Add_EditServiceUseCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region Add_Edit Command

    private bool CanAdd_EditServiceUse()
    {
        return CurrentServiceUse is
        {
            InvoiceId: not null,
            ServiceItem: not null,
            ServiceQuantity: not null,
            HasErrors: false
        };
    }

    [RelayCommand(CanExecute = nameof(CanAdd_EditServiceUse))]
    private async Task Add_EditServiceUse()
    {
        await using var context = new HotelManagementContext();
        var serviceUse = await context.ServiceUses.FindAsync(CurrentServiceUse.InvoiceId, CurrentServiceUse.ServiceItem!.ServiceId);

        if (serviceUse != null)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.InvoiceId == CurrentServiceUse.InvoiceId &&
                    item.ServiceItem!.ServiceId == CurrentServiceUse.ServiceItem!.ServiceId)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
            {
                List[index] = CurrentServiceUse;
            }

            serviceUse.InvoiceId = CurrentServiceUse.InvoiceId!;
            serviceUse.ServiceId = CurrentServiceUse.ServiceItem.ServiceId!;
            serviceUse.ServiceQuantity = int.Parse(CurrentServiceUse.ServiceQuantity!);
            serviceUse.TotalAmount = CurrentServiceUse.TotalAmount;

            context.ServiceUses.Update(serviceUse);
            
            await context.SaveChangesAsync();
            
            var totalAmount = await (from s in context.ServiceUses
                where s.ServiceId == CurrentServiceUse.ServiceItem.ServiceId && s.InvoiceId == CurrentServiceUse.InvoiceId
                select s.TotalAmount).FirstOrDefaultAsync();
            
            List[index].TotalAmount = totalAmount;
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Edit service use successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
        }
        else
        {
            List.Add(new ServiceUseVM()
            {
                InvoiceId = CurrentServiceUse.InvoiceId,
                ServiceItem = new ServiceInfo()
                {
                    ServiceId = CurrentServiceUse.ServiceItem.ServiceId,
                    ServiceName = CurrentServiceUse.ServiceItem.ServiceName
                },
                ServiceQuantity = CurrentServiceUse.ServiceQuantity,
                TotalAmount = CurrentServiceUse.TotalAmount
            });

            var entity = new ServiceUse()
            {
                InvoiceId = CurrentServiceUse.InvoiceId!,
                ServiceId = CurrentServiceUse.ServiceItem!.ServiceId,
                ServiceQuantity = int.Parse(CurrentServiceUse.ServiceQuantity!),
                TotalAmount = CurrentServiceUse.TotalAmount
            };

            await context.ServiceUses.AddAsync(entity);
            
            await context.SaveChangesAsync();
            
            var totalAmount = await (from s in context.ServiceUses
                where s.ServiceId == CurrentServiceUse.ServiceItem.ServiceId && s.InvoiceId == CurrentServiceUse.InvoiceId
                select s.TotalAmount).FirstOrDefaultAsync();
            
            List[^1].TotalAmount = totalAmount;
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Add service use successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
            
        }
    }

    #endregion

    #region Delete Command

    [RelayCommand]
    private void Delete(object parameter)
    {
        var param = (Tuple<object, object>)parameter;
        var serviceId = (string)param.Item1;
        var invoiceId = (string)param.Item2;

        var result = MessageBox.Show(
            App.ActivatedWindow, "Delete Service Use",
            "Are you sure you want to delete this service use?",
            msgImage: MessageBoxImage.WARNING, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.InvoiceId == invoiceId && item.ServiceItem!.ServiceId == serviceId)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List.RemoveAt(index);

            using var context = new HotelManagementContext();
            var serviceUse = context.ServiceUses.Find(invoiceId, serviceId);

            serviceUse.Deleted = true;
            serviceUse.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }

    #endregion
    
    #region Restore Command

    [RelayCommand]
    private async Task RestoreLast7Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Service Use",
            "Restore all service uses that have been deleted in the last 7 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var serviceUses = await context.ServiceUses.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-7)).ToListAsync();

            foreach (var serviceUse in serviceUses)
            {
                serviceUse.Deleted = false;
                serviceUse.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore service uses successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetServiceUseList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreLast30Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Service Use",
            "Restore all service uses that have been deleted in the last 30 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var serviceUses = await context.ServiceUses.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-30)).ToListAsync();

            foreach (var serviceUse in serviceUses)
            {
                serviceUse.Deleted = false;
                serviceUse.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore service uses successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetServiceUseList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreAll()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Service Use",
            "Restore all service uses that have been deleted?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var serviceUses = await context.ServiceUses.Where(e => e.Deleted == true).ToListAsync();

            foreach (var serviceUse in serviceUses)
            {
                serviceUse.Deleted = false;
                serviceUse.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore service uses successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetServiceUseList();
        }
    }
    
    #endregion

    public partial class ServiceUseVM : ObservableValidator
    {
        #region Properties
        
        [ObservableProperty] [NotifyDataErrorInfo] [Required]
        private string? _invoiceId;

        [ObservableProperty] [NotifyDataErrorInfo] [Required]
        private ServiceInfo _serviceItem;
        
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Service quantity is required")]
        [CustomValidation(typeof(ServiceUseVM), nameof(ValidateQuantity))]
        private string? _serviceQuantity;

        [ObservableProperty] private decimal? _totalAmount;
        
        #endregion
        
        #region Custom Validation

        public static ValidationResult ValidateQuantity(string? quantity, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (string.IsNullOrEmpty(quantity))
                return new ValidationResult("Service quantity is required!");

            if (!int.TryParse(quantity, out _))
                return new ValidationResult("Service quantity must be a number!");

            if (int.Parse(quantity) <= 0)
                return new ValidationResult("Service quantity must be greater than 0!");

            return ValidationResult.Success!;
        }
        
        #endregion
    }
}

public class ServiceInfo
{
    public string? ServiceId { get; set; }
    public string? ServiceName { get; set; }
}