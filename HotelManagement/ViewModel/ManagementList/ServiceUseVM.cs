using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagement.ViewModel.ManagementList;

public partial class ServiceUseList : ObservableObject
{
    public ObservableCollection<ServiceUseVM> List { get; set; }

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private ServiceUseVM _currentServiceUse;

    [ObservableProperty] private List<string> _invoiceIdList;

    [ObservableProperty] private List<string> _serviceIdList;

    #region Constructor

    public ServiceUseList()
    {
        GetServiceUseList();

        InvoiceIdList = new List<string>();
        ServiceIdList = new List<string>();
    }

    private async void GetServiceUseList()
    {
        List = new ObservableCollection<ServiceUseVM>();

        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var serviceUses = await (from serviceUse in context.ServiceUses
                where serviceUse.Deleted == false
                select new
                {
                    serviceUse.InvoiceId,
                    serviceUse.ServiceId,
                    serviceUse.ServiceQuantity,
                    serviceUse.TotalAmount
                }
            ).ToListAsync();

        foreach (var item in serviceUses)
        {
            List.Add(new ServiceUseVM()
            {
                InvoiceId = item.InvoiceId,
                ServiceId = item.ServiceId,
                ServiceQuantity = item.ServiceQuantity.ToString(),
                TotalAmount = item.TotalAmount,
            });
        }

        IsLoading = false;

        InvoiceIdList = await (from invoice in context.Invoices
            where invoice.Deleted == false
            select invoice.InvoiceId).ToListAsync();

        ServiceIdList = await (from service in context.Services
            where service.Deleted == false
            select service.ServiceId).ToListAsync();
    }

    #endregion

    #region AddServiceUse

    public void AddServiceUse()
    {
        CurrentServiceUse = new ServiceUseVM();
        CurrentServiceUse.PropertyChanged += (_, _) => Add_EditServiceUseCommand.NotifyCanExecuteChanged();
    }

    #endregion

    #region EditServiceUse

    public void GetServiceUseById(string serviceId, string invoiceId)
    {
        using var context = new HotelManagementContext();
        var serviceUse = (from s in List where s.InvoiceId == invoiceId && s.ServiceId == serviceId select s)
            .FirstOrDefault();

        CurrentServiceUse = new ServiceUseVM()
        {
            InvoiceId = serviceUse.InvoiceId,
            ServiceId = serviceUse.ServiceId,
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
            ServiceId: not null,
            ServiceQuantity: not null,
            HasErrors: false
        };
    }

    [RelayCommand(CanExecute = nameof(CanAdd_EditServiceUse))]
    private async Task Add_EditServiceUse()
    {
        await using var context = new HotelManagementContext();
        var serviceUse = await context.ServiceUses.FindAsync(CurrentServiceUse.InvoiceId, CurrentServiceUse.ServiceId);

        if (serviceUse != null)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.InvoiceId == CurrentServiceUse.InvoiceId &&
                    item.ServiceId == CurrentServiceUse.ServiceId)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List[index] = CurrentServiceUse;

            serviceUse.InvoiceId = CurrentServiceUse.InvoiceId!;
            serviceUse.ServiceId = CurrentServiceUse.ServiceId!;
            serviceUse.ServiceQuantity = int.Parse(CurrentServiceUse.ServiceQuantity!);
            serviceUse.TotalAmount = CurrentServiceUse.TotalAmount;

            context.ServiceUses.Update(serviceUse);
        }
        else
        {
            List.Add(new ServiceUseVM()
            {
                InvoiceId = CurrentServiceUse.InvoiceId,
                ServiceId = CurrentServiceUse.ServiceId,
                ServiceQuantity = CurrentServiceUse.ServiceQuantity,
                TotalAmount = CurrentServiceUse.TotalAmount
            });

            var entity = new ServiceUse()
            {
                InvoiceId = CurrentServiceUse.InvoiceId!,
                ServiceId = CurrentServiceUse.ServiceId!,
                ServiceQuantity = int.Parse(CurrentServiceUse.ServiceQuantity!),
                TotalAmount = CurrentServiceUse.TotalAmount
            };

            await context.ServiceUses.AddAsync(entity);
        }

        await context.SaveChangesAsync();
    }

    #endregion

    #region Delete Command

    [RelayCommand]
    private void Delete(object parameter)
    {
        var param = (Tuple<object, object>)parameter;
        var serviceId = (string)param.Item1;
        var invoiceId = (string)param.Item2;

        var result = MessageBox.Show("Are you sure you want to delete this service use?", "Delete Service Use",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.InvoiceId == invoiceId && item.ServiceId == serviceId)
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

    public partial class ServiceUseVM : ObservableValidator
    {
        [ObservableProperty] [NotifyDataErrorInfo] [Required]
        private string? _invoiceId;

        [ObservableProperty] [NotifyDataErrorInfo] [Required]
        private string? _serviceId;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required (ErrorMessage = "Service quantity is required")]
        [CustomValidation(typeof(ServiceUseVM), nameof(ValidateQuantity))]
        private string? _serviceQuantity;

        public decimal? TotalAmount { get; set; }

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
    }
}