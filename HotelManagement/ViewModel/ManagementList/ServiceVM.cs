using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

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
        GetServiceList();
    }

    private async void GetServiceList()
    {
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
        if(lastService != null)
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
    private void Add_EditService()
    {
        using var context = new HotelManagementContext();
        var service = context.Services.Find(CurrentService.ID);

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
            
            if(index != -1)
                List[index] = CurrentService;
            
            service.ServiceId = CurrentService.ID!;
            service.ServiceName = CurrentService.ServiceName!;
            service.ServiceType = CurrentService.ServiceType!;
            service.ServicePrice = decimal.Parse(CurrentService.ServicePrice);
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
            
            context.Services.Add(entity);
        }
        
        context.SaveChanges();
    }
    #endregion
    
    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show("Are you sure you want to delete this service?", "Delete Service",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
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
            
            if(index != -1)
                List.RemoveAt(index);
            
            using var context = new HotelManagementContext();
            var service = context.Services.Find(id);
            
            service.Deleted = true;
            service.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }
    #endregion

    public partial class ServiceVM : ObservableValidator
    {
        public string? ID { get; set; }

        [ObservableProperty] 
        [NotifyDataErrorInfo] 
        [Required(ErrorMessage = "Service name is required")]
        private string? _serviceName;

        [ObservableProperty] 
        [NotifyDataErrorInfo] 
        [Required]
        private string? _serviceType;
        
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Service price is required")]
        [CustomValidation(typeof(ServiceVM), "ValidatePrice")]
        private string? _servicePrice;
        
        public static ValidationResult ValidatePrice(string? price, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (!decimal.TryParse(price, out _))
                return new ValidationResult("Service price must be a number!");

            if (decimal.Parse(price) <= 0)
                return new ValidationResult("Service price must be greater than 0!");

            return ValidationResult.Success!;
        }
    }
}