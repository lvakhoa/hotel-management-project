using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using HotelManagement.CustomControls.MessageBox;

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
        _ = GetStaffList();
    }

    private async Task GetStaffList()
    {
        List.Clear();
        
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
    private async Task Add_EditStaff()
    {
        await using var context = new HotelManagementContext();
        var staff = await context.Staff.FindAsync(CurrentStaff.ID);

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
            
            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Edit staff successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
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

            await context.Staff.AddAsync(entity);
            
            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Add staff successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);

        }
    }

    #endregion

    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Delete Staff",
            "Are you sure you want to delete this staff?",
            msgImage: MessageBoxImage.WARNING, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
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
    
    #region Restore Command

    [RelayCommand]
    private async Task RestoreLast7Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Staff",
            "Restore all staffs that have been deleted in the last 7 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var staffs = await context.Staff.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-7)).ToListAsync();

            foreach (var staff in staffs)
            {
                staff.Deleted = false;
                staff.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore staffs successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetStaffList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreLast30Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Staff",
            "Restore all staff that have been deleted in the last 30 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var staffs = await context.Staff.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-30)).ToListAsync();

            foreach (var staff in staffs)
            {
                staff.Deleted = false;
                staff.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore staffs successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetStaffList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreAll()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Staff",
            "Restore all staffs that have been deleted?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var staffs = await context.Staff.Where(e => e.Deleted == true).ToListAsync();

            foreach (var staff in staffs)
            {
                staff.Deleted = false;
                staff.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore staffs successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetStaffList();
        }
    }
    
    #endregion

    public partial class StaffVM : ObservableValidator
    {
        #region Properties
        public string? ID { get; set; }

        // FullName
        private string? _fullName;

        [Required(ErrorMessage = "Full Name is required")]
        public string? FullName
        {
            get => _fullName;
            set => SetProperty(ref _fullName, value, true);
        }

        // ContactNumber
        private string? _contactNumber;

        [Required(ErrorMessage = "Contact Number is required")]
        [Phone(ErrorMessage = "Invalid Contact Number")]
        [CustomValidation(typeof(StaffVM), "ValidateContactNumber")]
        public string? ContactNumber
        {
            get => _contactNumber;
            set => SetProperty(ref _contactNumber, value, true);
        }

        // Email
        private string? _email;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [CustomValidation(typeof(StaffVM), "ValidateEmail")]
        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value, true);
        }

        // Address
        private string? _address;

        [Required(ErrorMessage = "Address is required")]
        public string? Address
        {
            get => _address;
            set => SetProperty(ref _address, value, true);
        }

        // Gender
        [ObservableProperty] [Required] private string? _gender;

        // Position
        private string? _position;

        [Required(ErrorMessage = "Position is required")]
        public string? Position
        {
            get => _position;
            set => SetProperty(ref _position, value, true);
        }

        [ObservableProperty] private DateTime? _birthday;

        // Salary
        private string? _salary;

        [Required(ErrorMessage = "Salary is required")]
        [CustomValidation(typeof(StaffVM), "ValidateSalary")]
        public string? Salary
        {
            get => _salary;
            set => SetProperty(ref _salary, value, true);
        }
        
        #endregion
        
        #region Custom Validation

        public static ValidationResult ValidateSalary(string? salary, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (!decimal.TryParse(salary, out _))
                return new ValidationResult("Salary must be a number!");

            if (decimal.Parse(salary) <= 0)
                return new ValidationResult("Salary must be greater than 0!");

            return ValidationResult.Success!;
        }

        public static ValidationResult ValidateContactNumber(string? contactNumber, ValidationContext context)
        {
            var instance = context.ObjectInstance as StaffVM;
            using var hotelContext = new HotelManagementContext();

            return Enumerable.Any(hotelContext.Staff,
                item => string.Equals(item.ContactNumber, contactNumber?.Trim(),
                    StringComparison.CurrentCultureIgnoreCase) && item.StaffId != instance.ID)
                ? new ValidationResult("Contact Number already exists")
                : ValidationResult.Success!;
        }

        public static ValidationResult ValidateEmail(string? email, ValidationContext context)
        {
            var instance = context.ObjectInstance as StaffVM;
            using var hotelContext = new HotelManagementContext();

            return Enumerable.Any(hotelContext.Staff,
                item => string.Equals(item.Email, email?.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                        item.StaffId != instance.ID)
                ? new ValidationResult("Email already exists")
                : ValidationResult.Success!;
        }
        
        #endregion
    }
}