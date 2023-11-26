using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagement.ViewModel;

public partial class NavigationVM : ObservableObject
{
    [ObservableProperty]
    private object _currentView;
    
    [RelayCommand]
    private void Home() => CurrentView = new HomeVM();
    
    [RelayCommand]
    private void RoomMap() => CurrentView = new RoomMapVM();
    
    [RelayCommand]
    private void Management() => CurrentView = new StaffVM();
    
    [RelayCommand]
    private void Inbox() => CurrentView = new InboxVM();
    
    [RelayCommand]
    private void Settings() => CurrentView = new SettingsVM();

    public NavigationVM()
    {
        // Startup Page
        CurrentView = new HomeVM();
    }
}