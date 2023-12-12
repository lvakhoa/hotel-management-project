using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel;

public partial class RoomList : ObservableObject
{
    public ObservableCollection<RoomVM> List { get; set; }
    public ObservableCollection<RoomVM> Now { get; set; }
    [ObservableProperty]
    private bool _isLoading;
    public RoomList()
    {
        List = new ObservableCollection<RoomVM>();
        Now = new ObservableCollection<RoomVM>();
        GetRoomList();
    }
    private async void GetRoomList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();
        var rooms = await (from room in context.Rooms
                           select new
                           {
                               room.RoomId,
                               room.RoomNumber,
                               room.Notes,
                               room.RoomTypeId
                           }).ToListAsync();
        var today = DateTime.Today;
        var available = await (from r in context.Rooms
                               where r.Bookings.All(b => (b.CheckOutDate < DateTime.Now || b.CheckInDate > DateTime.Now))
                               select r).ToListAsync();
        foreach (var item in rooms)
        {
            List.Add(new RoomVM()
            {
                RoomId = item.RoomId,
                RoomNumber = item.RoomNumber,
                Notes = item.Notes,
                RoomTypeId = item.RoomTypeId
            });
        }
        foreach (var item in List)
        {
            bool exist = false;
            if (item.Notes != null)
            {
                item.Status = "Out of Order";
            }
            else
            {
                foreach (var i in available)
                {
                    if (i.RoomId == item.RoomId)
                    {
                        item.Status = "Available";
                        break;
                    }
                    item.Status = "Occupied";
                }
            }
        }
        IsLoading = false;
    }



    public class RoomVM
    {
        public string? RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? Notes { get; set; }
        public string? RoomTypeId { get; set; }
        public string? Status { get; set; }

    }
}