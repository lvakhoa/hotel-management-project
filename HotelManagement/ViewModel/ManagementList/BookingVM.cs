using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.CustomControls.MessageBox;

namespace HotelManagement.ViewModel.ManagementList;

public partial class BookingList : ObservableObject
{
    public ObservableCollection<BookingVM> List { get; set; }

    [ObservableProperty] private List<RoomInfo> _roomIdList;

    [ObservableProperty] private List<string> _invoiceIdList;

    [ObservableProperty] private List<string> _customerIdList;

    [ObservableProperty] private List<string> _staffIdList;

    [ObservableProperty] private List<string> _paymentTypeList;

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private BookingVM _currentBooking;

    #region Constructor

    public BookingList()
    {
        List = new ObservableCollection<BookingVM>();
        RoomIdList = new List<RoomInfo>();
        CustomerIdList = new List<string>();
        StaffIdList = new List<string>();
        PaymentTypeList = new List<string>();
        _ = GetBookingList();
    }

    private async Task GetBookingList()
    {
        List.Clear();
        RoomIdList.Clear();
        CustomerIdList.Clear();
        StaffIdList.Clear();
        PaymentTypeList.Clear();
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var bookings = await context.Bookings.Include("Invoice").Where(x => (bool)!x.Deleted).ToListAsync();

        foreach (var item in bookings)
        {
            List.Add(new BookingVM()
            {
                BookingID = item.BookingId,
                InvoiceID = item.InvoiceId,
                RoomItem = new RoomInfo()
                {
                    RoomID = item.RoomId,
                    RoomType = (from r in context.Rooms
                        join rt in context.RoomTypes on r.RoomTypeId equals rt.RoomTypeId
                        where r.RoomId == item.RoomId
                        select rt.RoomTypeName).FirstOrDefault()
                },
                RoomNumber = (from r in context.Rooms
                    where r.RoomId == item.RoomId
                    select r.RoomNumber).FirstOrDefault(),
                CustomerID = item.Invoice.CustomerId,
                StaffID = item.Invoice.StaffId,
                GuestQuantity = item.GuestQuantity.ToString(),
                CheckInDate = item.CheckInDate,
                CheckOutDate = item.CheckOutDate,
                PaymentMethod = item.Invoice.PaymentType,
                TotalAmount = item.TotalAmount,
                DepositFee = (item.CheckInDate - item.Invoice.InvoiceDate)!.Value.Days / 15 * 20
            });
        }

        var roomIds = await (from r in context.Rooms
            join rt in context.RoomTypes on r.RoomTypeId equals rt.RoomTypeId
            where r.Deleted == false && string.IsNullOrEmpty(r.Notes) && string.IsNullOrWhiteSpace(r.Notes)
            let b = from b in r.Bookings
                where b.Deleted == false
                select b.CheckOutDate
            where !b.Any() || b.Max() < DateTime.Now
            orderby r.RoomId
            select new RoomInfo() { RoomID = r.RoomId, RoomType = rt.RoomTypeName }).Distinct().ToListAsync();

        foreach (var item in roomIds)
        {
            RoomIdList.Add(item);
        }

        var customerIds = await context.Customers.Where(x => x.Deleted == false).OrderBy(x => x.CustomerId).Select(x => x.CustomerId).ToListAsync();
        foreach (var item in customerIds)
        {
            CustomerIdList.Add(item);
        }

        var staffIds = await context.Accounts.OrderBy(x => x.StaffId).Select(x => x.StaffId).ToListAsync();
        foreach (var item in staffIds)
        {
            StaffIdList.Add(item);
        }

        var paymentTypes = await context.Invoices.Select(x => x.PaymentType).Distinct().ToListAsync();
        foreach (var item in paymentTypes)
        {
            PaymentTypeList.Add(item);
        }

        IsLoading = false;
    }

    #endregion

    #region EditBooking

    public void GetBookingById(string? id)
    {
        CurrentBooking = new BookingVM() { RoomItem = new RoomInfo() };

        using var context = new HotelManagementContext();

        InvoiceIdList = new List<string>();

        var invoiceIds = context.Invoices.Select(x => x.InvoiceId).ToList();
        foreach (var item in invoiceIds)
        {
            InvoiceIdList.Add(item);
        }

        var booking = (from b in List
            where b.BookingID == id
            select new
            {
                b.BookingID,
                b.InvoiceID,
                b.RoomItem,
                b.CustomerID,
                b.StaffID,
                b.GuestQuantity,
                b.CheckInDate,
                b.CheckOutDate,
                b.PaymentMethod,
                b.TotalAmount,
                b.DepositFee
            }).FirstOrDefault();

        if (booking != null)
        {
            CurrentBooking.BookingID = booking.BookingID;
            CurrentBooking.InvoiceID = booking.InvoiceID;
            CurrentBooking.RoomItem.RoomID = booking.RoomItem!.RoomID;
            CurrentBooking.RoomItem.RoomType = booking.RoomItem.RoomType;
            CurrentBooking.CustomerID = booking.CustomerID;
            CurrentBooking.StaffID = booking.StaffID;
            CurrentBooking.GuestQuantity = booking.GuestQuantity;
            CurrentBooking.CheckInDate = booking.CheckInDate;
            CurrentBooking.CheckOutDate = booking.CheckOutDate;
            CurrentBooking.PaymentMethod = booking.PaymentMethod;
            CurrentBooking.TotalAmount = booking.TotalAmount;
            CurrentBooking.DepositFee = booking.DepositFee;
        }

        CurrentBooking.PropertyChanged += (sender, args) => { Add_EditBookingCommand.NotifyCanExecuteChanged(); };
    }

    #endregion

    #region AddBooking

    public async void GenerateBookingId()
    {
        CurrentBooking = new BookingVM() { RoomItem = new RoomInfo() };

        await using var context = new HotelManagementContext();

        InvoiceIdList = new List<string>();

        var invoiceIds = await context.Invoices.Select(x => x.InvoiceId).ToListAsync();
        foreach (var item in invoiceIds)
        {
            InvoiceIdList.Add(item);
        }

        var lastInvoice = await context.Invoices.OrderByDescending(x => x.InvoiceId).FirstOrDefaultAsync();
        var lastBooking = await context.Bookings.OrderByDescending(x => x.BookingId).FirstOrDefaultAsync();

        if (lastBooking != null)
        {
            string numericPart = lastBooking.BookingId.Substring(1);
            int numericVal = int.Parse(numericPart) + 1;
            CurrentBooking.BookingID = $"B{numericVal:D4}";
        }
        else
        {
            CurrentBooking.BookingID = "B0001";
        }

        if (lastInvoice != null)
        {
            string numericPart = lastInvoice.InvoiceId.Substring(1);
            int numericVal = int.Parse(numericPart) + 1;
            CurrentBooking.InvoiceID = $"I{numericVal:D4}";
        }
        else
        {
            CurrentBooking.InvoiceID = "I0001";
        }

        InvoiceIdList.Add(CurrentBooking.InvoiceID);

        CurrentBooking.PropertyChanged += (sender, args) => { Add_EditBookingCommand.NotifyCanExecuteChanged(); };
    }

    #endregion

    #region Add_Edit Command

    private bool CanAdd_EditBooking()
    {
        return CurrentBooking is
        {
            InvoiceID: not null, RoomItem: not null, PaymentMethod: not null, CustomerID: not null, StaffID: not null,
            HasErrors: false
        };
    }

    [RelayCommand(CanExecute = nameof(CanAdd_EditBooking))]
    private async Task Add_EditBooking()
    {
        await using var context = new HotelManagementContext();
        var booking = await context.Bookings.FindAsync(CurrentBooking.BookingID);
        var invoice = await context.Invoices.FindAsync(CurrentBooking.InvoiceID);

        if (booking != null && invoice != null)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.BookingID == CurrentBooking.BookingID)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List[index] = CurrentBooking;

            booking.BookingId = CurrentBooking.BookingID!;
            booking.InvoiceId = CurrentBooking.InvoiceID!;
            booking.RoomId = CurrentBooking.RoomItem!.RoomID!;
            booking.GuestQuantity = int.Parse(CurrentBooking.GuestQuantity!);
            booking.CheckInDate = CurrentBooking.CheckInDate;
            booking.CheckOutDate = CurrentBooking.CheckOutDate;
            booking.TotalAmount = CurrentBooking.TotalAmount;
            invoice.PaymentType = CurrentBooking.PaymentMethod == "Cash" ? "Cash" : "Credit card";
            invoice.StaffId = CurrentBooking.StaffID!;

            await context.SaveChangesAsync();

            var totalAmount = await (from b in context.Bookings
                where b.BookingId == CurrentBooking.BookingID
                select b.TotalAmount).FirstOrDefaultAsync();

            List[index].TotalAmount = totalAmount;
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Edit booking successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);

            await GetBookingList();
        }
        else if (booking == null && invoice != null)
        {
            if (CurrentBooking.CheckInDate < DateTime.Now)
            {
                MessageBox.Show(
                    App.ActivatedWindow, "Error",
                    "Can't add booking with check-in date before today!",
                    msgImage: MessageBoxImage.ERROR, msgButton: MessageBoxButton.OK);
                return;
            }
            
            var totalAmount = CalculateTotalAmount();

            List.Add(new BookingVM()
            {
                BookingID = CurrentBooking.BookingID,
                InvoiceID = CurrentBooking.InvoiceID,
                RoomItem = new RoomInfo()
                {
                    RoomID = CurrentBooking.RoomItem!.RoomID,
                    RoomType = CurrentBooking.RoomItem.RoomType
                },
                CustomerID = CurrentBooking.CustomerID,
                StaffID = CurrentBooking.StaffID,
                GuestQuantity = CurrentBooking.GuestQuantity,
                CheckInDate = CurrentBooking.CheckInDate,
                CheckOutDate = CurrentBooking.CheckOutDate,
                PaymentMethod = CurrentBooking.PaymentMethod,
                TotalAmount = totalAmount
            });

            var bookingEntity = new Booking()
            {
                BookingId = CurrentBooking.BookingID!,
                InvoiceId = CurrentBooking.InvoiceID!,
                RoomId = CurrentBooking.RoomItem.RoomID!,
                GuestQuantity = int.Parse(CurrentBooking.GuestQuantity!),
                CheckInDate = CurrentBooking.CheckInDate,
                CheckOutDate = CurrentBooking.CheckOutDate,
                TotalAmount = CurrentBooking.TotalAmount
            };

            await context.Bookings.AddAsync(bookingEntity);

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Add booking successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
            
            await GetBookingList();
        }
        else
        {
            var totalAmount = CalculateTotalAmount();

            List.Add(new BookingVM()
            {
                BookingID = CurrentBooking.BookingID,
                InvoiceID = CurrentBooking.InvoiceID,
                RoomItem = new RoomInfo()
                {
                    RoomID = CurrentBooking.RoomItem!.RoomID,
                    RoomType = CurrentBooking.RoomItem.RoomType
                },
                CustomerID = CurrentBooking.CustomerID,
                StaffID = CurrentBooking.StaffID,
                GuestQuantity = CurrentBooking.GuestQuantity,
                CheckInDate = CurrentBooking.CheckInDate,
                CheckOutDate = CurrentBooking.CheckOutDate,
                PaymentMethod = CurrentBooking.PaymentMethod,
                TotalAmount = totalAmount
            });

            var invoiceEntity = new Invoice()
            {
                InvoiceId = CurrentBooking.InvoiceID!,
                CustomerId = CurrentBooking.CustomerID!,
                StaffId = CurrentBooking.StaffID!,
                InvoiceDate = DateTime.Now,
                TotalAmount = 0,
                PaymentType = CurrentBooking.PaymentMethod == "Cash" ? "Cash" : "Credit card"
            };

            await context.Invoices.AddAsync(invoiceEntity);

            var bookingEntity = new Booking()
            {
                BookingId = CurrentBooking.BookingID!,
                InvoiceId = CurrentBooking.InvoiceID!,
                RoomId = CurrentBooking.RoomItem.RoomID!,
                GuestQuantity = int.Parse(CurrentBooking.GuestQuantity!),
                CheckInDate = CurrentBooking.CheckInDate,
                CheckOutDate = CurrentBooking.CheckOutDate,
                TotalAmount = CurrentBooking.TotalAmount
            };

            await context.Bookings.AddAsync(bookingEntity);

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Add booking successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
            
            await GetBookingList();
        }
    }

    private decimal CalculateTotalAmount()
    {
        using var context = new HotelManagementContext();
        var room = context.Rooms.Find(CurrentBooking.RoomItem!.RoomID);
        var roomType = context.RoomTypes.Find(room!.RoomTypeId);

        decimal totalAmount =
            roomType!.RoomPrice * CurrentBooking.CheckOutDate!.Value.Subtract(CurrentBooking.CheckInDate!.Value).Days;

        return totalAmount;
    }

    #endregion

    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Delete Booking",
            "Are you sure you want to delete this booking?",
            msgImage: MessageBoxImage.WARNING, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.BookingID == id)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }

            if (index != -1)
                List.RemoveAt(index);

            using var context = new HotelManagementContext();
            var booking = context.Bookings.Find(id);

            booking!.Deleted = true;
            booking.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }

    #endregion
    
    #region Restore Command

    [RelayCommand]
    private async Task RestoreLast7Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Booking",
            "Restore all bookings that have been deleted in the last 7 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var bookings = await context.Bookings.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-7)).ToListAsync();

            foreach (var booking in bookings)
            {
                booking.Deleted = false;
                booking.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore bookings successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetBookingList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreLast30Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Booking",
            "Restore all bookings that have been deleted in the last 30 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var bookings = await context.Bookings.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-30)).ToListAsync();

            foreach (var booking in bookings)
            {
                booking.Deleted = false;
                booking.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore bookings successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetBookingList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreAll()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Booking",
            "Restore all bookings that have been deleted?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var bookings = await context.Bookings.Where(e => e.Deleted == true).ToListAsync();

            foreach (var booking in bookings)
            {
                booking.Deleted = false;
                booking.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore bookings successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetBookingList();
        }
    }
    
    #endregion

    public partial class BookingVM : ObservableValidator
    {
        #region Properties

        // BookingID
        [ObservableProperty] private string? _bookingID;

        // InvoiceID
        private string? _invoiceID;

        [Required]
        public string? InvoiceID
        {
            get => _invoiceID;
            set => SetProperty(ref _invoiceID, value);
        }

        // RoomID
        private RoomInfo? _roomItem;

        [Required]
        public RoomInfo? RoomItem
        {
            get => _roomItem;
            set
            {
                SetProperty(ref _roomItem, value);
                ValidateProperty(GuestQuantity, nameof(GuestQuantity));
            }
        }

        // CustomerID
        private string? _customerID;

        [Required]
        public string? CustomerID
        {
            get => _customerID;
            set => SetProperty(ref _customerID, value);
        }

        // StaffID
        private string? _staffID;

        [Required]
        public string? StaffID
        {
            get => _staffID;
            set => SetProperty(ref _staffID, value);
        }

        [ObservableProperty] private string? _roomNumber;
        
        // GuestQuantity
        private string? _guestQuantity;

        [Required(ErrorMessage = "Guest quantity is required!")]
        [CustomValidation(typeof(BookingVM), nameof(ValidateGuestQuantity))]
        public string? GuestQuantity
        {
            get => _guestQuantity;
            set => SetProperty(ref _guestQuantity, value, true);
        }

        // CheckInDate
        private DateTime? _checkInDate;

        [CustomValidation(typeof(BookingVM), nameof(ValidateCheckInDate))]
        public DateTime? CheckInDate
        {
            get => _checkInDate;
            set
            {
                SetProperty(ref _checkInDate, value, true);
                ValidateProperty(CheckOutDate, nameof(CheckOutDate));
                ChangeDepositFee(value);
            }
        }

        // CheckOutDate
        private DateTime? _checkOutDate;

        [CustomValidation(typeof(BookingVM), nameof(ValidateCheckOutDate))]
        public DateTime? CheckOutDate
        {
            get => _checkOutDate;
            set
            {
                SetProperty(ref _checkOutDate, value, true);
                ValidateProperty(CheckInDate, nameof(CheckInDate));
            }
        }

        // PaymentMethod
        private string? _paymentMethod;

        [Required(ErrorMessage = "Payment method is required!")]
        public string? PaymentMethod
        {
            get => _paymentMethod;
            set => SetProperty(ref _paymentMethod, value, true);
        }

        // TotalAmount
        public decimal? TotalAmount { get; set; }

        [ObservableProperty] private decimal? _depositFee = 0;

        #endregion

        #region Custom Validation

        /** -- Custom Validation -- **/
        // ValidateGuestQuantity
        public static ValidationResult ValidateGuestQuantity(string? guestQuantity, ValidationContext context)
        {
            var instance = context.ObjectInstance as BookingVM;
            using var hotelContext = new HotelManagementContext();
            var capacity =
                (from room in hotelContext.Rooms
                    join roomType in hotelContext.RoomTypes on room.RoomTypeId equals roomType.RoomTypeId
                    where room.RoomId == instance.RoomItem.RoomID
                    select roomType.Capacity).FirstOrDefault();

            if (instance.RoomItem.RoomID == null)
                return new ValidationResult("Select Room ID before changing guest quantity!");

            if (string.IsNullOrEmpty(guestQuantity))
                return new ValidationResult("Guest quantity is required!");

            if (!int.TryParse(guestQuantity, out _))
                return new ValidationResult("Guest quantity must be a number!");

            if (int.Parse(guestQuantity) > capacity || int.Parse(guestQuantity) < 1)
            {
                if (capacity == 1)
                    return new ValidationResult("Guest quantity must be 1");
                return new ValidationResult($"Guest quantity must be between 1 and {capacity}");
            }

            return ValidationResult.Success!;
        }

        // ValidateCheckInDate
        public static ValidationResult ValidateCheckInDate(DateTime? checkInDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as BookingVM;

            if (checkInDate > DateTime.Now.AddMonths(2))
                return new ValidationResult("Check-in date must be within 2 months from now");

            if (checkInDate >= instance?.CheckOutDate)
                return new ValidationResult("Check-in date must be before check-out date");

            return ValidationResult.Success!;
        }

        // ValidateCheckOutDate
        public static ValidationResult ValidateCheckOutDate(DateTime? checkOutDate, ValidationContext context)
        {
            var instance = context.ObjectInstance as BookingVM;

            if (checkOutDate <= instance?.CheckInDate)
                return new ValidationResult("Check-out date must be after check-in date");

            return ValidationResult.Success!;
        }

        #endregion

        public new void ValidateAllProperties()
        {
            base.ValidateAllProperties();
        }

        private void ChangeDepositFee(DateTime? checkIn)
        {
            using var context = new HotelManagementContext();
            var booking = context.Bookings.Find(BookingID);
            var invoice = context.Invoices.Find(InvoiceID);

            if (checkIn < DateTime.Now)
                DepositFee = 0;
            else if (booking == null)
                DepositFee = (checkIn - DateTime.Now).Value.Days / 15 * 20;
            else
                DepositFee = (checkIn - invoice.InvoiceDate).Value.Days / 15 * 20;
        }
    }
}

public class RoomInfo
{
    public string? RoomID { get; set; }
    public string? RoomType { get; set; }
}