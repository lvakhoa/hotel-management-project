using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.CustomControls.MessageBox;

namespace HotelManagement.ViewModel.ManagementList;

public partial class ServiceList : ObservableObject
{
    public ObservableCollection<ServiceVM> List { get; set; }

    [ObservableProperty] private List<string> _serviceTypeList;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private ServiceVM _currentService;

    #region Constructor

    public ServiceList()
    {
        List = new ObservableCollection<ServiceVM>();

        ServiceTypeList = new List<string>();
        _ = GetServiceList();
    }

    private async Task GetServiceList()
    {
        List.Clear();
        
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var services = await (from service in context.Services
            where service.Deleted == false
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
                ID = item.ServiceId,
                ServiceName = item.ServiceName,
                ServiceType = item.ServiceType,
                ServicePrice = item.ServicePrice.ToString(CultureInfo.CurrentCulture)
            });
        }

        var serviceTypes = await context.Services.Select(x => x.ServiceType).Distinct().ToListAsync();
        foreach (var item in serviceTypes)
        {
            ServiceTypeList.Add(item);
        }

        IsLoading = false;
    }

    #endregion

    #region EditService

    public void GetServiceById(string? id)
    {
        var service = (from s in List
            where s.ID == id
            select new
            {
                s.ID,
                s.ServiceName,
                s.ServiceType,
                s.ServicePrice
            }).FirstOrDefault();

        if (service != null)
            CurrentService = new ServiceVM()
            {
                ID = service.ID,
                ServiceName = service.ServiceName,
                ServiceType = service.ServiceType,
                ServicePrice = service.ServicePrice
            };

        CurrentService.PropertyChanged += (e, args) => { Add_EditServiceCommand.NotifyCanExecuteChanged(); };
    }

    #endregion

    #region AddService

    public void GenerateServiceId()
    {
        using var context = new HotelManagementContext();
        var lastService = context.Services.OrderByDescending(x => x.ServiceId).FirstOrDefault();

        CurrentService = new ServiceVM();
        if (lastService != null)
        {
            string numericPart = lastService.ServiceId.Substring(1);
            int numericVal = int.Parse(numericPart) + 1;
            CurrentService.ID = $"S{numericVal:D4}";
        }
        else
        {
            CurrentService.ID = "S0001";
        }

        CurrentService.PropertyChanged += (e, args) => { Add_EditServiceCommand.NotifyCanExecuteChanged(); };
    }

    #endregion

    #region Add_Edit Command

    private bool CanAdd_EditService()
    {
        return CurrentService is
        {
            ServiceName: not null,
            ServiceType: not null,
            ServicePrice: not null,
            HasErrors: false
        };
    }

    [RelayCommand(CanExecute = nameof(CanAdd_EditService))]
    private async Task Add_EditService()
    {
        await using var context = new HotelManagementContext();
        var service = await context.Services.FindAsync(CurrentService.ID);

        if (service != null)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.ID == CurrentService.ID)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List[index] = CurrentService;

            service.ServiceId = CurrentService.ID!;
            service.ServiceName = CurrentService.ServiceName!;
            service.ServiceType = CurrentService.ServiceType!;
            service.ServicePrice = decimal.Parse(CurrentService.ServicePrice);
            
            await context.SaveChangesAsync();

            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Edit service successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
        }
        else
        {
            List.Add(new ServiceVM()
            {
                ID = CurrentService.ID,
                ServiceName = CurrentService.ServiceName,
                ServiceType = CurrentService.ServiceType,
                ServicePrice = CurrentService.ServicePrice
            });

            var entity = new Service()
            {
                ServiceId = CurrentService.ID!,
                ServiceName = CurrentService.ServiceName!,
                ServiceType = CurrentService.ServiceType!,
                ServicePrice = decimal.Parse(CurrentService.ServicePrice)
            };

            await context.Services.AddAsync(entity);
            
            await context.SaveChangesAsync();

            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Add service successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
        }

    }

    #endregion

    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Delete Service",
            "Are you sure you want to delete this service?",
            msgImage: MessageBoxImage.WARNING, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.ID == id)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List.RemoveAt(index);

            using var context = new HotelManagementContext();
            var service = context.Services.Find(id);

            service.Deleted = true;
            service.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }

    #endregion
    
    #region Restore Command

    [RelayCommand]
    private async Task RestoreLast7Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Service",
            "Restore all services that have been deleted in the last 7 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var services = await context.Services.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-7)).ToListAsync();

            foreach (var service in services)
            {
                service.Deleted = false;
                service.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore services successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetServiceList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreLast30Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Service",
            "Restore all services that have been deleted in the last 30 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var services = await context.Services.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-30)).ToListAsync();

            foreach (var service in services)
            {
                service.Deleted = false;
                service.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore services successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetServiceList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreAll()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Service",
            "Restore all services that have been deleted?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var services = await context.Services.Where(e => e.Deleted == true).ToListAsync();

            foreach (var service in services)
            {
                service.Deleted = false;
                service.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore services successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetServiceList();
        }
    }
    
    #endregion

    public partial class ServiceVM : ObservableValidator
    {
        #region Properties
        public string? ID { get; set; }

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Service name is required")]
        [CustomValidation(typeof(ServiceVM), "ValidateServiceName")]
        private string? _serviceName;

        [ObservableProperty] [NotifyDataErrorInfo] [Required]
        private string? _serviceType;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Service price is required")]
        [CustomValidation(typeof(ServiceVM), "ValidatePrice")]
        private string? _servicePrice;
        
        #endregion
        
        #region Custom Validation
        public static ValidationResult ValidatePrice(string? price, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (!decimal.TryParse(price, out _))
                return new ValidationResult("Service price must be a number!");

            if (decimal.Parse(price) <= 0)
                return new ValidationResult("Service price must be greater than 0!");

            return ValidationResult.Success!;
        }

        public static ValidationResult ValidateServiceName(string? serviceName, ValidationContext context)
        {
            var instance = context.ObjectInstance as ServiceVM;
            using var hotelContext = new HotelManagementContext();

            return Enumerable.Any(hotelContext.Services,
                item =>
                    string.Equals(item.ServiceName, serviceName?.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                    item.ServiceId != instance.ID)
                ? new ValidationResult("Service name already exists")
                : ValidationResult.Success!;
        }
        #endregion
    }
}