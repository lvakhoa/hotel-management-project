using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.Input;
using HotelManagement.CustomControls.MessageBox;

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
        _ = GetRoomList();
    }

    private async Task GetRoomList()
    {
        List.Clear();
        
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
            orderby roomType.RoomTypeId
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
    private async Task Add_EditRoom()
    {
        await using var context = new HotelManagementContext();
        var room = await context.Rooms.FindAsync(CurrentRoom.ID);

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

            await context.SaveChangesAsync();

            MessageBox.Show(App.ActivatedWindow, "Success", "Edit room successfully",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
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

            await context.Rooms.AddAsync(entity);

            await context.SaveChangesAsync();

            MessageBox.Show(App.ActivatedWindow, "Success", "Add room successfully",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
        }
    }

    #endregion

    #region Delete Command

    [RelayCommand]
    private void Delete(string id)
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Delete Room",
            "Are you sure you want to delete this room?",
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
            var room = context.Rooms.Find(id);

            room.Deleted = true;
            room.DeletedDate = DateTime.Now;
            context.SaveChanges();
        }
    }

    #endregion
    
    #region Restore Command

    [RelayCommand]
    private async Task RestoreLast7Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Room",
            "Restore all rooms that have been deleted in the last 7 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var rooms = await context.Rooms.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-7)).ToListAsync();

            foreach (var room in rooms)
            {
                room.Deleted = false;
                room.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore rooms successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetRoomList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreLast30Days()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Room",
            "Restore all rooms that have been deleted in the last 30 days?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var rooms = await context.Rooms.Where(e => e.DeletedDate >= DateTime.Now.AddDays(-30)).ToListAsync();

            foreach (var room in rooms)
            {
                room.Deleted = false;
                room.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore rooms successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetRoomList();
        }
    }
    
    [RelayCommand]
    private async Task RestoreAll()
    {
        var result = MessageBox.Show(
            App.ActivatedWindow, "Restore Room",
            "Restore all rooms that have been deleted?",
            msgImage: MessageBoxImage.QUESTION, msgButton: MessageBoxButton.YesNo);

        if (result == MessageBoxResult.YES)
        {
            await using var context = new HotelManagementContext();
            var rooms = await context.Rooms.Where(e => e.Deleted == true).ToListAsync();

            foreach (var room in rooms)
            {
                room.Deleted = false;
                room.DeletedDate = null;
            }

            await context.SaveChangesAsync();
            
            MessageBox.Show(
                App.ActivatedWindow, "Success",
                "Restore rooms successfully!",
                msgImage: MessageBoxImage.SUCCESS, msgButton: MessageBoxButton.OK);
                
            await GetRoomList();
        }
    }
    
    #endregion

    public partial class RoomVM : ObservableValidator
    {
        #region Properties

        // ID
        public string? ID { get; set; }

        // RoomNumber
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Room number is required")]
        [RegularExpression(@"^[0-9]{3}$", ErrorMessage = "Room number must be 3 digits")]
        private string? _roomNumber;

        // Notes
        public string? Notes { get; set; }

        // RoomTypeID
        [ObservableProperty] [Required] private string? _roomTypeID;

        #endregion
    }
}