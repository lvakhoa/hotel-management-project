using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.Model;
using HotelManagement.ViewModel.ManagementList;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.ViewModel;

public partial class NavigationVM : ObservableObject
{
    [ObservableProperty] private StaffList.StaffVM _currentStaff;
    public string Password { get; set; }
    
    [ObservableProperty]
    private object _currentView;
    
    [RelayCommand]
    private void Home() => CurrentView = new HomeVM();
    
    [RelayCommand]
    private void RoomMap() => CurrentView = new RoomMapVM();
    
    [RelayCommand]
    private void Management() => CurrentView = new ManagementVM(CurrentStaff.ID);
    
    [RelayCommand]
    private void Settings() => CurrentView = new SettingsVM(CurrentStaff, Password);

    private async void GetCurrentStaff()
    {
        await using var context = new HotelManagementContext();
        
        string userId = Thread.CurrentPrincipal.Identity.Name;
        
        var user = await (from u in context.Accounts
            where u.StaffId == userId
            select new
            {
                u.Password
            }).FirstOrDefaultAsync();
        
        if (user != null)
            Password = user.Password;
        
        var staff = await (from s in context.Staff
            where s.StaffId == userId
            select new
            {
                s.StaffId, s.FullName, s.Position, s.Address, s.Email, s.Birthday, s.Gender, s.Salary, s.ContactNumber
            }).FirstOrDefaultAsync();
        
        if (staff != null)
        {
            CurrentStaff.ID = staff.StaffId;
            CurrentStaff.FullName = staff.FullName;
            CurrentStaff.Position = staff.Position;
            CurrentStaff.Address = staff.Address;
            CurrentStaff.Email = staff.Email;
            CurrentStaff.Birthday = staff.Birthday;
            CurrentStaff.Gender = staff.Gender;
            CurrentStaff.Salary = staff.Salary.ToString();
            CurrentStaff.ContactNumber = staff.ContactNumber;
        }
    }

    public NavigationVM()
    {
        // Startup Page
        CurrentView = new HomeVM();
        
        CurrentStaff = new StaffList.StaffVM();
        
        GetCurrentStaff();
    }
}