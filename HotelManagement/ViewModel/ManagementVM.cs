using System.Collections.ObjectModel;
using System.Windows.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagement.ViewModel;

internal partial class ManagementVM : ObservableObject
{
    [ObservableProperty] private object _currentManagement;
    public ManagementVM()
    {
        CurrentManagement = new StaffList();
    }
    
    [RelayCommand]
    private void Staff() => CurrentManagement = new StaffList();
    
    [RelayCommand]
    private void Customer() => CurrentManagement = new CustomerList();
}