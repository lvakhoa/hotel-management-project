using System.Runtime.InteropServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.Model;
using HotelManagement.ViewModel.ManagementList;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace HotelManagement.ViewModel;

internal partial class SettingsVM : ObservableObject
{
    [ObservableProperty] private StaffList.StaffVM _staff;
    [ObservableProperty] private string _staffAvatar;

    public SettingsVM(StaffList.StaffVM CurrentStaff)
    {
        Staff = new StaffList.StaffVM();
        Staff = CurrentStaff;
        UpdateStaff();
    }
    public void UpdateStaff()
    {
        var context = new HotelManagementContext();
        var s = context.Staff.Find(Staff.ID);
        if (s != null)
        {
            Staff.FullName = s.FullName;
            Staff.ContactNumber = s.ContactNumber;
            Staff.Birthday = s.Birthday;
            Staff.Address = s.Address;
            Staff.Gender = s.Gender;
        }
        AvatarLoad(Staff);
    }

    private void AvatarLoad(StaffList.StaffVM Staff)
    {
        if (Staff.Gender == "female")
        {
            StaffAvatar = "/HotelManagement;component/Assets/Images/female_placeholder_photo.png";
        }
        else
        {
            StaffAvatar = "/HotelManagement;component/Assets/Images/male_placeholder_photo.png";
        }
    }
    private bool Can_EditUserInfo()
    {
        return Staff is
        {
            ID: not null,
            FullName: not null,
            Birthday: not null,
            ContactNumber: not null,
            HasErrors: false
        };
    }
    
    [RelayCommand(CanExecute = nameof(Can_EditUserInfo))]
    private void EditUserInfo()
    {
        using var context = new HotelManagementContext();
        var staff = context.Staff.Find(Staff.ID);
        if (staff != null)
        {
            staff.Birthday = Staff.Birthday;
            staff.ContactNumber = Staff.ContactNumber;
            staff.Address = Staff.Address;
            staff.FullName = Staff.FullName;
            context.SaveChanges();
        }
        
    }
}