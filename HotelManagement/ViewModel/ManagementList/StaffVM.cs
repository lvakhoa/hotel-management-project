using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.ViewModel.ManagementList;

public partial class StaffList : ObservableObject
{
    public ObservableCollection<StaffVM> List { get; set; }
    
    [ObservableProperty]
    private bool _isLoading;
    
    public StaffList()
    {
        List = new ObservableCollection<StaffVM>();
        
        GetStaffList();
    }

    private async void GetStaffList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();
            
        var staffs = await (from staff in context.Staff
            select new
            {
                staff.StaffId, staff.FullName, staff.Position, staff.ContactNumber, staff.Email, staff.Address,
                staff.Birthday, staff.Gender, staff.Salary
            }).ToListAsync();
            
        foreach (var item in staffs)
        {
            List.Add(new StaffVM()
            {
                StaffID = item.StaffId, FullName = item.FullName, Position = item.Position,
                ContactNumber = item.ContactNumber, Email = item.Email, Address = item.Address,
                Birthday = item.Birthday, Gender = item.Gender, Salary = item.Salary
            });
        }

        IsLoading = false;
    }

    public class StaffVM
    {
        public string? StaffID { get; set; }
        public string? FullName { get; set; }
        public string? Position { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public DateTime Birthday { get; set; }
        public string? Gender { get; set; }
        public decimal? Salary { get; set; }
    }
}