using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagement.ViewModel.ManagementList;

public partial class RoomList : ObservableObject
{
    public ObservableCollection<RoomVM> List { get; set; }

    [ObservableProperty] private bool _isLoading;
    
    [ObservableProperty] private List<string>? _roomTypeList;

    [ObservableProperty] private RoomVM _currentRoom;

    #region Constructor

    public RoomList()
    {
        List = new ObservableCollection<RoomVM>();
        RoomTypeList = new List<string>();
        GetRoomList();
    }

    private async void GetRoomList()
    {
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();

        var rooms = await (from room in context.Rooms
            where room.Deleted == false
            select new
            {
                room.RoomId,
                room.RoomNumber,
                room.Notes,
                room.RoomTypeId
            }).ToListAsync();
        
        foreach (var item in rooms)
        {
            List.Add(new RoomVM()
            {
                ID = item.RoomId,
                RoomNumber = item.RoomNumber,
                Notes = item.Notes,
                RoomTypeID = item.RoomTypeId
            });
        }
        
        RoomTypeList = await (from roomType in context.RoomTypes
            where roomType.Deleted == false
            select roomType.RoomTypeId).ToListAsync();
        
        IsLoading = false;
    }

    #endregion

    #region EditRoom
    public void GetRoomById(string? id)
    {
        var room = (from r in List where r.ID == id select r).FirstOrDefault();
        
        CurrentRoom = new RoomVM()
        {
            ID = room.ID,
            RoomNumber = room.RoomNumber,
            Notes = room.Notes,
            RoomTypeID = room.RoomTypeID
        };

        CurrentRoom.PropertyChanged += (e, args) => { Add_EditRoomCommand.NotifyCanExecuteChanged(); };
    }
    #endregion
    
    #region AddRoom
    public void GenerateRoomId()
    {
        using var context = new HotelManagementContext();
        var lastRoom = context.Rooms.OrderByDescending(x => x.RoomId).FirstOrDefault();

        CurrentRoom = new RoomVM();
        if (lastRoom != null)
        {
            string numericPart = lastRoom.RoomId.Substring(1);
            int numericVal = int.Parse(numericPart) + 1;
            CurrentRoom.ID = $"R{numericVal:D4}";
        }
        else
        {
            CurrentRoom.ID = "R0001";
        }
        
        CurrentRoom.PropertyChanged += (e, args) => { Add_EditRoomCommand.NotifyCanExecuteChanged(); };
    }
    #endregion
    
    #region Add_Edit Command
    
    private bool CanAdd_EditRoom()
    {
        return CurrentRoom is
        {
            RoomNumber: not null, RoomTypeID: not null, HasErrors: false
        };
    }
    
    [RelayCommand(CanExecute = nameof(CanAdd_EditRoom))]
    private void Add_EditRoom()
    {
        using var context = new HotelManagementContext();
        var room = context.Rooms.Find(CurrentRoom.ID);

        if (room != null)
        {
            int index = -1;
            foreach (var item in List)
            {
                if (item.ID == CurrentRoom.ID)
                {
                    index = List.IndexOf(item);
                    break;
                }
            }
            
            if (index != -1)
                List[index] = CurrentRoom;
            
            room.RoomId = CurrentRoom.ID;
            room.RoomNumber = CurrentRoom.RoomNumber;
            room.Notes = CurrentRoom.Notes;
            room.RoomTypeId = CurrentRoom.RoomTypeID;
        }
        else
        {
            List.Add(new RoomVM()
            {
                ID = CurrentRoom.ID,
                RoomNumber = CurrentRoom.RoomNumber,
                Notes = CurrentRoom.Notes,
                RoomTypeID = CurrentRoom.RoomTypeID
            });

            var entity = new Room()
            {
                RoomId = CurrentRoom.ID,
                RoomNumber = CurrentRoom.RoomNumber,
                Notes = CurrentRoom.Notes,
                RoomTypeId = CurrentRoom.RoomTypeID
            };

            context.Rooms.Add(entity);
        }

        context.SaveChanges();
    }
    #endregion
    
    #region Delete Command
    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show("Are you sure you want to delete this room?", "Delete Room",
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
            var room = context.Rooms.Find(id);

            room.Deleted = true;
            room.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }
    #endregion
    
    public partial class RoomVM : ObservableValidator
    {
        // ID
        public string? ID { get; set; }
        
        // RoomNumber
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required (ErrorMessage = "Room number is required")]
        [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "Room number must be 3 digits")]
        private string? _roomNumber;
        
        // Notes
        public string? Notes { get; set; }
        
        // RoomTypeID
        [ObservableProperty] 
        [Required]
        private string? _roomTypeID;
    }
}