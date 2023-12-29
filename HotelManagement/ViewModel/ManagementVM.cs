using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.Model;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.ViewModel;

internal partial class ManagementVM : ObservableObject
{
    [ObservableProperty] private object _currentManagement;

    public bool IsManager { get; set; } = false;
    public ManagementVM(string? staffID)
    {
        CurrentManagement = new CustomerList();
        
        using var context = new HotelManagementContext();
        var staff = context.Staff.Find(staffID);
        
        if(staff is { Position: "Hotel Manager" })
            IsManager = true;
    }

    [RelayCommand]
    private void Staff() => CurrentManagement = new StaffList();

    [RelayCommand]
    private void Customer() => CurrentManagement = new CustomerList();
    
    [RelayCommand]
    private void Account() => CurrentManagement = new AccountList();
    
    [RelayCommand]
    private void Booking() => CurrentManagement = new BookingList();
    
    [RelayCommand]
    private void Invoice() => CurrentManagement = new InvoiceList();
    
    [RelayCommand]
    private void Room() => CurrentManagement = new RoomList();
    
    [RelayCommand]
    private void Roomtype() => CurrentManagement = new RoomtypeList();
    
    [RelayCommand]
    private void Service() => CurrentManagement = new ServiceList();
    
    [RelayCommand]
    private void ServiceUse() => CurrentManagement = new ServiceUseList();

}