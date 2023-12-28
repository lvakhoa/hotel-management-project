using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.CustomControls.MessageBox;

namespace HotelManagement.ViewModel.ManagementList;

public partial class CustomerList : ObservableObject
{
    public ObservableCollection<CustomerVM> List { get; set; }

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private List<string> _genderList;

    [ObservableProperty] private CustomerVM _currentCustomer;

    #region Constructor

    public CustomerList()
    {
        List = new ObservableCollection<CustomerVM>();
        GenderList = new List<string>();
        GetCustomerList();
    }

    private async void GetCustomerList()
    {
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var customers = await (from customer in context.Customers
            where customer.Deleted == false
            select new
            {
                customer.CustomerId,
                customer.FullName,
                customer.ContactNumber,
                customer.Email,
                customer.Address,
                customer.Gender,
                customer.CreditCard,
                customer.IdProof
            }).ToListAsync();

        foreach (var item in customers)
        {
            List.Add(new CustomerVM()
            {
                ID = item.CustomerId,
                FullName = item.FullName,
                ContactNumber = item.ContactNumber,
                Email = item.Email,
                Address = item.Address,
                Gender = item.Gender,
                CreditCard = item.CreditCard,
                ProofID = item.IdProof
            });
        }

        IsLoading = false;

        GenderList.Add("male");
        GenderList.Add("female");
    }

    #endregion

    #region EditCustomer

    public void GetCustomerById(string id)
    {
        using var context = new HotelManagementContext();
        var customer = (from c in List where c.ID == id select c).FirstOrDefault();

        CurrentCustomer = new CustomerVM()
        {
            ID = customer.ID, FullName = customer.FullName,
            ContactNumber = customer.ContactNumber, Email = customer.Email, Address = customer.Address,
            Gender = customer.Gender, CreditCard = customer.CreditCard, ProofID = customer.ProofID
        };

        CurrentCustomer.PropertyChanged += (sender, args) => { Add_EditCustomerCommand.NotifyCanExecuteChanged(); };
    }

    #endregion

    #region AddCustomer

    public void GenerateCustomerId()
    {
        using var context = new HotelManagementContext();
        var lastCustomer = context.Customers.OrderByDescending(e => e.CustomerId).FirstOrDefault();

        CurrentCustomer = new CustomerVM();
        if (lastCustomer != null)
        {
            string numericPart = lastCustomer.CustomerId.Substring(1);
            int numericVal = int.Parse(numericPart) + 1;
            CurrentCustomer.ID = $"C{numericVal:D4}";
        }
        else
        {
            CurrentCustomer.ID = "C0001";
        }

        CurrentCustomer.PropertyChanged += (sender, args) => { Add_EditCustomerCommand.NotifyCanExecuteChanged(); };
    }

    #endregion

    #region Add_Edit Command

    private bool CanAdd_EditCustomer()
    {
        return CurrentCustomer is
        {
            FullName: not null,
            ContactNumber: not null,
            Email: not null,
            Address: not null,
            Gender: not null,
            CreditCard: not null,
            ProofID: not null,
            HasErrors: false
        };
    }

    [RelayCommand(CanExecute = nameof(CanAdd_EditCustomer))]
    private async Task Add_EditCustomer()
    {
        await using var context = new HotelManagementContext();
        var customer = await context.Customers.FindAsync(CurrentCustomer.ID);
        if (customer != null)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.ID == CurrentCustomer.ID)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List[index] = CurrentCustomer;

            customer.CustomerId = CurrentCustomer.ID;
            customer.FullName = CurrentCustomer.FullName;
            customer.ContactNumber = CurrentCustomer.ContactNumber;
            customer.Email = CurrentCustomer.Email;
            customer.Address = CurrentCustomer.Address;
            customer.Gender = CurrentCustomer.Gender;
            customer.CreditCard = CurrentCustomer.CreditCard;
            customer.IdProof = CurrentCustomer.ProofID;

            await context.SaveChangesAsync();

            MessageBox.Show(
                App.ActivatedWindow, "Success", "Edit customer successfully",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
        }
        else
        {
            List.Add(new CustomerVM()
            {
                ID = CurrentCustomer.ID, FullName = CurrentCustomer.FullName,
                ContactNumber = CurrentCustomer.ContactNumber, Email = CurrentCustomer.Email,
                Address = CurrentCustomer.Address,
                Gender = CurrentCustomer.Gender, CreditCard = CurrentCustomer.CreditCard,
                ProofID = CurrentCustomer.ProofID
            });

            var entity = new Customer()
            {
                CustomerId = CurrentCustomer.ID,
                FullName = CurrentCustomer.FullName,
                ContactNumber = CurrentCustomer.ContactNumber,
                Email = CurrentCustomer.Email,
                Address = CurrentCustomer.Address,
                Gender = CurrentCustomer.Gender,
                CreditCard = CurrentCustomer.CreditCard,
                IdProof = CurrentCustomer.ProofID
            };

            await context.Customers.AddAsync(entity);

            await context.SaveChangesAsync();

            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Add customer successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
        }
    }

    #endregion

    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Delete Customer",
            "Are you sure you want to delete this customer?",
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
            var customer = context.Customers.Find(id);

            customer.Deleted = true;
            customer.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }

    #endregion

    public partial class CustomerVM : ObservableValidator
    {
        #region Properties

        // ID
        public string? ID { get; set; }

        // FullName
        [ObservableProperty] [NotifyDataErrorInfo] [Required(ErrorMessage = "Full Name is required")]
        private string? _fullName;

        // ContactNumber
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Contact Number is required")]
        [Phone(ErrorMessage = "Invalid Contact Number")]
        [CustomValidation(typeof(CustomerVM), "ValidateContactNumber")]
        private string? _contactNumber;

        // Email
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [CustomValidation(typeof(CustomerVM), "ValidateEmail")]
        private string? _email;

        // Address
        [ObservableProperty] [NotifyDataErrorInfo] [Required(ErrorMessage = "Address is required")]
        private string? _address;

        // Gender
        [ObservableProperty] [Required] private string? _gender;

        // CreditCard
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Credit Card is required")]
        [RegularExpression(@"^\d{4}-\d{4}-\d{4}-\d{4}$", ErrorMessage = "Valid Proof ID format is xxxx-xxxx-xxxx-xxxx")]
        [CustomValidation(typeof(CustomerVM), "ValidateCreditCard")]
        private string? _creditCard;

        // ProofID
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Proof ID is required")]
        [CustomValidation(typeof(CustomerVM), "ValidateProofId")]
        private string? _proofID;

        #endregion

        #region Custom Validation

        public static ValidationResult ValidateContactNumber(string? contactNumber, ValidationContext context)
        {
            var instance = context.ObjectInstance as CustomerVM;
            using var hotelContext = new HotelManagementContext();

            return Enumerable.Any(hotelContext.Customers,
                item => string.Equals(item.ContactNumber, contactNumber?.Trim(),
                    StringComparison.CurrentCultureIgnoreCase) && item.CustomerId != instance.ID)
                ? new ValidationResult("Contact Number already exists")
                : ValidationResult.Success!;
        }

        public static ValidationResult ValidateEmail(string? email, ValidationContext context)
        {
            var instance = context.ObjectInstance as CustomerVM;
            using var hotelContext = new HotelManagementContext();

            return Enumerable.Any(hotelContext.Customers,
                item => string.Equals(item.Email, email?.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                        item.CustomerId != instance.ID)
                ? new ValidationResult("Email already exists")
                : ValidationResult.Success!;
        }

        public static ValidationResult ValidateCreditCard(string? creditCard, ValidationContext context)
        {
            var instance = context.ObjectInstance as CustomerVM;
            using var hotelContext = new HotelManagementContext();

            return Enumerable.Any(hotelContext.Customers,
                item => string.Equals(item.CreditCard, creditCard?.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                        item.CustomerId != instance.ID)
                ? new ValidationResult("Credit Card already exists")
                : ValidationResult.Success!;
        }

        public static ValidationResult ValidateProofId(string? proofID, ValidationContext context)
        {
            var instance = context.ObjectInstance as CustomerVM;
            using var hotelContext = new HotelManagementContext();

            return Enumerable.Any(hotelContext.Customers,
                item => string.Equals(item.IdProof, proofID?.Trim(), StringComparison.CurrentCultureIgnoreCase) &&
                        item.CustomerId != instance.ID)
                ? new ValidationResult("Proof ID already exists")
                : ValidationResult.Success!;
        }

        #endregion
    }
}