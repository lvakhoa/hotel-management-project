using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.ViewModel;

internal partial class SettingsVM : ObservableObject
{
    [ObservableProperty] private string? _fullName;
    [ObservableProperty] private string? _position;

    public SettingsVM(StaffList.StaffVM CurrentStaff)
    {
        FullName = CurrentStaff.FullName;
        Position = CurrentStaff.Position;
    }
}