using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel.ManagementList;

public partial class RoomtypeList : ObservableObject
{
    public ObservableCollection<RoomtypeVM> List { get; set; }
    [ObservableProperty]
    private bool _isLoading;
    public RoomtypeList()
    {
        List = new ObservableCollection<RoomtypeVM>();
        GetRoomtypeList();
    }
    private async void GetRoomtypeList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();
        var roomtypes = await (from roomtype in context.RoomTypes
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
                RoomTypeId = item.RoomTypeId,
                RoomTypeName = item.RoomTypeName,
                Capacity = item.Capacity,
                BedAmount = item.BedAmount,
                RoomPrice = item.RoomPrice,
                RoomTypeDesc = item.RoomTypeDesc,
                RoomTypeImg = item.RoomTypeImg
            });
        }
        IsLoading = false;

    }
    public class RoomtypeVM
    {
        public string? RoomTypeId { get; set; }
        public string? RoomTypeName { get; set; }
        public int Capacity { get; set; }
        public int BedAmount { get; set; }
        public decimal RoomPrice { get; set; }
        public string? RoomTypeDesc { get; set; }
        public byte[]? RoomTypeImg { get; set; }
    }
}