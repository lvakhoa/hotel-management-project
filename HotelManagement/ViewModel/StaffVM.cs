using System.Collections.ObjectModel;
using HotelManagement.Model;

namespace HotelManagement.ViewModel;

public class StaffList : ObservableCollection<StaffList.StaffVM>
{
    public StaffList() : base()
    {
        using var context = new HotelManagementContext();
        foreach (var item in context.Staff)
        {
            Add(new StaffVM()
            {
                StaffId = item.StaffId, FullName = item.FullName, Position = item.Position,
                ContactNumber = item.ContactNumber, Email = item.Email, Address = item.Address,
                Birthday = item.Birthday, Gender = item.Gender, Salary = item.Salary
            });
        }
    }
    
    public class StaffVM
    {
        public string? StaffId { get; set; }
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