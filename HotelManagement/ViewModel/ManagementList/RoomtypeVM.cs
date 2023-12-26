﻿using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagement.ViewModel.ManagementList;

public partial class RoomtypeList : ObservableObject
{
    public ObservableCollection<RoomtypeVM> List { get; set; }

    [ObservableProperty] private bool _isLoading;

    [ObservableProperty] private RoomtypeVM _currentRoomType;

    #region Constructor

    public RoomtypeList()
    {
        List = new ObservableCollection<RoomtypeVM>();
        GetRoomtypeList();
    }

    private async void GetRoomtypeList()
    {
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var roomtypes = await (from roomtype in context.RoomTypes
                where roomtype.Deleted == false
                select new
                {
                    roomtype.RoomTypeId,
                    roomtype.RoomTypeName,
                    roomtype.Capacity,
                    roomtype.BedAmount,
                    roomtype.RoomPrice,
                    roomtype.RoomTypeDesc,
                    roomtype.RoomTypeImg
                }
            ).ToListAsync();
        
        foreach (var item in roomtypes)
        {
            List.Add(new RoomtypeVM()
            {
                ID = item.RoomTypeId,
                RoomTypeName = item.RoomTypeName,
                Capacity = item.Capacity.ToString(),
                BedAmount = item.BedAmount.ToString(),
                RoomPrice = item.RoomPrice.ToString(CultureInfo.CurrentCulture),
                RoomTypeDesc = item.RoomTypeDesc,
                RoomTypeImg = item.RoomTypeImg
            });
        }

        IsLoading = false;
    }

    #endregion
    
    #region EditRoomType

    public void GetRoomTypeById(string? id)
    {
        var roomtype = (from r in List
            where r.ID == id
            select new
            {
                r.ID,
                r.RoomTypeName,
                r.Capacity,
                r.BedAmount,
                r.RoomPrice,
                r.RoomTypeDesc,
                r.RoomTypeImg
            }).FirstOrDefault();
        
        CurrentRoomType = new RoomtypeVM()
        {
            ID = roomtype.ID,
            RoomTypeName = roomtype.RoomTypeName,
            Capacity = roomtype.Capacity,
            BedAmount = roomtype.BedAmount,
            RoomPrice = roomtype.RoomPrice,
            RoomTypeDesc = roomtype.RoomTypeDesc,
            RoomTypeImg = roomtype.RoomTypeImg
        };
        
        CurrentRoomType.PropertyChanged += (e, args) => { Add_EditRoomTypeCommand.NotifyCanExecuteChanged(); };
    }
    #endregion
    
    #region AddRoomType
    public void GenerateRoomTypeId()
    {
        using var context = new HotelManagementContext();
        var lastRoomType = context.RoomTypes.OrderByDescending(r => r.RoomTypeId).FirstOrDefault();

        CurrentRoomType = new RoomtypeVM();
        if (lastRoomType != null)
        {
            string numericPart = lastRoomType.RoomTypeId.Substring(2);
            int numericVal = int.Parse(numericPart) + 1;
            CurrentRoomType.ID = $"RT{numericVal:D3}";
        }
        else
        {
            CurrentRoomType.ID = "RT001";
        }
        
        CurrentRoomType.PropertyChanged += (e, args) => { Add_EditRoomTypeCommand.NotifyCanExecuteChanged(); };
    }
    #endregion
    
    #region Add_Edit Command

    private bool CanAdd_EditRoomType()
    {
        return CurrentRoomType is
        {
            RoomTypeName: not null,
            Capacity: not null,
            BedAmount: not null,
            RoomPrice: not null,
            RoomTypeDesc: not null,
            HasErrors: false
        };
    }
    
    [RelayCommand(CanExecute = nameof(CanAdd_EditRoomType))]
    private void Add_EditRoomType()
    {
        using var context = new HotelManagementContext();
        var roomType = context.RoomTypes.Find(CurrentRoomType.ID);

        if (roomType != null)
        {
            int index = -1;
            foreach(var item in List)
            {
                if (item.ID == CurrentRoomType.ID)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }
            
            if(index != -1)
                List[index] = CurrentRoomType;
            
            roomType.RoomTypeId = CurrentRoomType.ID!;
            roomType.RoomTypeName = CurrentRoomType.RoomTypeName!;
            roomType.Capacity = int.Parse(CurrentRoomType.Capacity!);
            roomType.BedAmount = int.Parse(CurrentRoomType.BedAmount!);
            roomType.RoomPrice = decimal.Parse(CurrentRoomType.RoomPrice);
            roomType.RoomTypeDesc = CurrentRoomType.RoomTypeDesc!;
            roomType.RoomTypeImg = CurrentRoomType.RoomTypeImg;
        }
        else
        {
            List.Add(new RoomtypeVM()
            {
                ID = CurrentRoomType.ID,
                RoomTypeName = CurrentRoomType.RoomTypeName,
                Capacity = CurrentRoomType.Capacity,
                BedAmount = CurrentRoomType.BedAmount,
                RoomPrice = CurrentRoomType.RoomPrice,
                RoomTypeDesc = CurrentRoomType.RoomTypeDesc,
                RoomTypeImg = CurrentRoomType.RoomTypeImg
            });

            var entity = new RoomType()
            {
                RoomTypeId = CurrentRoomType.ID!,
                RoomTypeName = CurrentRoomType.RoomTypeName!,
                Capacity = int.Parse(CurrentRoomType.Capacity!),
                BedAmount = int.Parse(CurrentRoomType.BedAmount!),
                RoomPrice = decimal.Parse(CurrentRoomType.RoomPrice),
                RoomTypeDesc = CurrentRoomType.RoomTypeDesc!,
                RoomTypeImg = CurrentRoomType.RoomTypeImg
            };
            
            context.RoomTypes.Add(entity);
        }
        
        context.SaveChanges();
    }
    #endregion
    
    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show("Are you sure you want to delete this room type?", "Delete Room Type",
            MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            int index = -1;
            foreach(var item in List)
            {
                if (item.ID == id)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }
            
            if(index != -1)
                List.RemoveAt(index);
            
            using var context = new HotelManagementContext();
            var roomType = context.RoomTypes.Find(id);
            
            roomType!.Deleted = true;
            roomType.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }
    #endregion

    public partial class RoomtypeVM : ObservableValidator
    {
        // ID
        public string? ID { get; set; }
        
        // RoomTypeName
        [ObservableProperty] 
        [NotifyDataErrorInfo] 
        [Required(ErrorMessage = "Room type name is required")]
        private string? _roomTypeName;
        
        // Capacity
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Capacity is required")]
        [CustomValidation(typeof(RoomtypeVM), nameof(ValidateCapacity))]
        private string? _capacity;
        
        // BedAmount
        [ObservableProperty] 
        [NotifyDataErrorInfo] 
        [Required(ErrorMessage = "Bed amount is required")]
        [CustomValidation(typeof(RoomtypeVM), nameof(ValidateBedAmount))]
        private string? _bedAmount;
        
        // RoomPrice
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Room price is required")]
        [CustomValidation(typeof(RoomtypeVM), nameof(ValidatePrice))]
        private string _roomPrice = "1";
        
        // RoomTypeDesc
        [ObservableProperty] 
        [NotifyDataErrorInfo] 
        [Required(ErrorMessage = "Room type description is required")]
        private string? _roomTypeDesc;
        
        // RoomTypeImg
        public byte[]? RoomTypeImg { get; set; }
        
        
        public static ValidationResult ValidateCapacity(string? capacity, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (string.IsNullOrEmpty(capacity))
                return new ValidationResult("Capacity is required!");

            if (!int.TryParse(capacity, out _))
                return new ValidationResult("Capacity must be a number!");
            
            if (int.Parse(capacity) <= 0)
                return new ValidationResult("Capacity must be greater than 0!");

            return ValidationResult.Success!;
        }
        
        public static ValidationResult ValidateBedAmount(string? bedAmount, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (string.IsNullOrEmpty(bedAmount))
                return new ValidationResult("Bed amount is required!");

            if (!int.TryParse(bedAmount, out _))
                return new ValidationResult("Bed amount must be a number!");
            
            if (int.Parse(bedAmount) <= 0)
                return new ValidationResult("Bed amount must be greater than 0!");

            return ValidationResult.Success!;
        }
        
        public static ValidationResult ValidatePrice(string? price, ValidationContext context)
        {
            using var hotelContext = new HotelManagementContext();

            if (!decimal.TryParse(price, out _))
                return new ValidationResult("Room price must be a number!");

            if (decimal.Parse(price) <= 0)
                return new ValidationResult("Room price must be greater than 0!");

            return ValidationResult.Success!;
        }
    }
}