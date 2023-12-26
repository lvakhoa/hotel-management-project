using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.ViewModel.ManagementList;

public partial class StaffList : ObservableObject
{
    public ObservableCollection<StaffVM> List { get; set; }
    
    [ObservableProperty] private List<string> _genderList;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private StaffVM _currentStaff;
    
    #region Constructor
    public StaffList()
    {
        List = new ObservableCollection<StaffVM>();
        GenderList = new List<string>();
        GetStaffList();
    }

    private async void GetStaffList()
    {
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();
        
        var staffs = await (from staff in context.Staff 
            where staff.Deleted == false
            select new
            {
                staff.StaffId, staff.FullName, staff.Position, staff.ContactNumber, staff.Email, staff.Address,
                staff.Birthday, staff.Gender, staff.Salary
            }).ToListAsync();

        foreach (var item in staffs)
        {
            List.Add(new StaffVM()
            {
                ID = item.StaffId, FullName = item.FullName, Position = item.Position,
                ContactNumber = item.ContactNumber, Email = item.Email, Address = item.Address,
                Birthday = item.Birthday, Gender = item.Gender, Salary = item.Salary.ToString()
            });
        }
        
        GenderList.Add("male");
        GenderList.Add("female");

        IsLoading = false;
    }
    #endregion

    #region EditStaff
    public void GetStaffById(string id)
    {
        var staff = (from s in List where s.ID == id select s).FirstOrDefault();
        
        CurrentStaff = new StaffVM()
        {
            ID = staff.ID, FullName = staff.FullName, Position = staff.Position,
            ContactNumber = staff.ContactNumber, Email = staff.Email, Address = staff.Address,
            Birthday = staff.Birthday, Gender = staff.Gender, Salary = staff.Salary
        };
        
        CurrentStaff.PropertyChanged += (_, _) => Add_EditStaffCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region AddStaff
    public void GenerateStaffId()
    {
        using var context = new HotelManagementContext();
        var lastStaff = context.Staff.OrderByDescending(e => e.StaffId).FirstOrDefault();

        CurrentStaff = new StaffVM();
        if (lastStaff != null)
        {
            string numericPart = lastStaff.StaffId.Substring(2);
            int numericVal = int.Parse(numericPart) + 1;
            CurrentStaff.ID = $"ST{numericVal:D3}";
        }
        else
            CurrentStaff.ID = "ST001";
        
        
        CurrentStaff.PropertyChanged += (_, _) => Add_EditStaffCommand.NotifyCanExecuteChanged();
    }
    #endregion

    #region Add_Edit Command
    
    private bool CanAdd_EditStaff()
    {
        return CurrentStaff is
        {
            FullName: not null,
            Position: not null,
            ContactNumber: not null,
            Email: not null,
            Address: not null,
            Gender: not null,
            Salary: not null,
            HasErrors: false
        };
    }
    
    [RelayCommand(CanExecute = nameof(CanAdd_EditStaff))]
    private void Add_EditStaff()
    {
        using var context = new HotelManagementContext();
        var staff = context.Staff.Find(CurrentStaff.ID);

        if (staff != null)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.ID == CurrentStaff.ID)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List[index] = CurrentStaff;
        
            staff.StaffId = CurrentStaff.ID!;
            staff.FullName = CurrentStaff.FullName!;
            staff.Position = CurrentStaff.Position!;
            staff.ContactNumber = CurrentStaff.ContactNumber!;
            staff.Email = CurrentStaff.Email;
            staff.Address = CurrentStaff.Address!;
            staff.Birthday = CurrentStaff.Birthday;
            staff.Gender = CurrentStaff.Gender!;
            staff.Salary = decimal.Parse(CurrentStaff.Salary!);
        }
        else
        {
            List.Add(new StaffVM()
            {
                ID = CurrentStaff.ID, FullName = CurrentStaff.FullName, Position = CurrentStaff.Position,
                ContactNumber = CurrentStaff.ContactNumber, Email = CurrentStaff.Email, Address = CurrentStaff.Address,
                Birthday = CurrentStaff.Birthday, Gender = CurrentStaff.Gender, Salary = CurrentStaff.Salary
            });

            var entity = new Staff()
            {
                StaffId = CurrentStaff.ID!,
                FullName = CurrentStaff.FullName!,
                Position = CurrentStaff.Position!,
                ContactNumber = CurrentStaff.ContactNumber!,
                Email = CurrentStaff.Email!,
                Address = CurrentStaff.Address!,
                Birthday = CurrentStaff.Birthday!,
                Gender = CurrentStaff.Gender!,
                Salary = decimal.Parse(CurrentStaff.Salary!)
            };
            
            context.Staff.Add(entity);
        }
        
        context.SaveChanges();
    }
    #endregion

    #region Delete Command
    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show("Are you sure you want to delete this staff?", "Delete Staff",
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
            if (index != -1)
                List.RemoveAt(index);
        
            using var context = new HotelManagementContext();
            var staff = context.Staff.Find(id);
            staff.Deleted = true;
            staff.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }
    #endregion

    public partial class StaffVM : ObservableValidator
    {
        public string? ID { get; set; }
        // FullName
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required (ErrorMessage = "Full Name is required")]
        private string? _fullName;
        
        // ContactNumber
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required (ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        private string? _contactNumber;
        
        // Email
        [ObservableProperty] 
        [NotifyDataErrorInfo] 
        [Required (ErrorMessage = "Email is required")] 
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        private string? _email;

        // Address
        [ObservableProperty] 
        [NotifyDataErrorInfo] 
        [Required (ErrorMessage = "Address is required")]
        private string? _address;

        // Gender
        [ObservableProperty] 
        [Required] 
        private string? _gender;

        [ObservableProperty]  
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Position is required")]
        private string? _position;

        [ObservableProperty] private DateTime? _birthday;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Salary is required")]
        [CustomValidation(typeof(StaffVM), "ValidateSalary")]
        private string? _salary; 
        
        public static ValidationResult ValidateSalary(string? salary, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (!decimal.TryParse(salary, out _))
                return new ValidationResult("Salary must be a number!");

            if (decimal.Parse(salary) <= 0)
                return new ValidationResult("Salary must be greater than 0!");

            return ValidationResult.Success!;
        }
    }
}