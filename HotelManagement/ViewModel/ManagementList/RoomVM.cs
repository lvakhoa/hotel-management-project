using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel.ManagementList;

public partial class RoomList : ObservableObject
{
    public ObservableCollection<RoomVM> List { get; set; }
    [ObservableProperty]
    private bool _isLoading;
    public RoomList()
    {
        List = new ObservableCollection<RoomVM>();
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
        IsLoading = false;
    }



    public class RoomVM
    {
        public string? RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? Notes { get; set; }
        public string RoomTypeId { get; set; }

    }
}