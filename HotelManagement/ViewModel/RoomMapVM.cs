using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;

namespace HotelManagement.ViewModel;

public partial class RoomMapVM : ObservableObject
{
    public ObservableCollection<RoomMap> List { get; set; }
    public ObservableCollection<string> Floors { get; set; }
    public ICollectionView RoomView { get; set; }
    public CollectionViewSource GroupFloor { get; set; }
    
    [ObservableProperty]
    private string _filterName;
    [ObservableProperty]
    private bool _isLoading;
    public RoomMapVM()
    {
        List = new ObservableCollection<RoomMap>();
        Floors = new ObservableCollection<string>();
        GroupFloor = new CollectionViewSource();
        GroupFloor.Source = List;
        GroupFloor.GroupDescriptions.Add(new PropertyGroupDescription("Floor"));
        RoomView = GroupFloor.View;
        Floors.Insert(0, "Show All");
    }

    public async Task GetRoomList()
    {
        IsLoading = true;
        await Task.Delay(1000);
        await using var context = new HotelManagementContext();
        var rooms = await (from room in context.Rooms
                           join type in context.RoomTypes on room.RoomTypeId equals type.RoomTypeId
                           select new
                           {
                               room.RoomId,
                               room.RoomNumber,
                               room.Notes,
                               room.RoomTypeId,
                               room.Deleted,
                               type.Capacity,
                               type.RoomTypeName
                           }).ToListAsync();

        var available = await (from r in context.Rooms
            join rt in context.RoomTypes on r.RoomTypeId equals rt.RoomTypeId
            where r.Deleted == false && string.IsNullOrEmpty(r.Notes) && string.IsNullOrWhiteSpace(r.Notes)
            let b = from b in r.Bookings
                where b.Deleted == false
                select b.CheckOutDate
            where !b.Any() || b.Max() < DateTime.Now
            orderby r.RoomId
            select r).Distinct().ToListAsync();
        
        foreach (var item in rooms)
        {
            if (item.Deleted == false)
            {
                List.Add(new RoomMap()
                {
                    RoomId = item.RoomId,
                    RoomNumber = item.RoomNumber,
                    Notes = item.Notes,
                    RoomTypeId = item.RoomTypeId,
                    RoomType = item.RoomTypeName,
                    Capacity = item.Capacity,
                    Floor = item.RoomNumber[0].ToString()
                });
            }
        }

        var floor = List.Select(x => x.Floor).Distinct().ToList();
        foreach (string f in floor)
        {
            if (f != null)
                Floors.Add("Floor " + f);
        }

        foreach (var item in List)
        {
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
    public void StatusFilter(string? status)
    {
        if (RoomView != null)
        {
            switch (status)
            {
                case "Available":
                    RoomView.Filter = item => { return ((RoomMap)item).Status == "Available"; };
                    FilterName = "Filter: " + status;
                    break;
                case "Occupied":
                    RoomView.Filter = item => { return ((RoomMap)item).Status == "Occupied"; };
                    FilterName = "Filter: " + status;
                    break;
                case "OutOfOrder":
                    RoomView.Filter = item => { return ((RoomMap)item).Status == "Out of Order"; };
                    FilterName = "Filter: Out Of Order";
                    break;
                default:
                    RoomView.Filter = null;
                    FilterName = String.Empty;
                    break;
            }
        }
    }
    public void SelectFloor(string? selectedfloor)
    {
        if (RoomView != null)
        {
            if (selectedfloor != "Show All")
            {
                RoomView.Filter = item =>
                {
                    string[] floor = selectedfloor.Split(' ');
                    return ((RoomMap)item).Floor == floor[1];
                };
            }
            else
            {
                RoomView.Filter = null;
            }

        }
    }
    public class RoomMap
    {
        public string? RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string? Notes { get; set; }
        public string? RoomTypeId { get; set; }
        public string? Status { get; set; }
        public string? Floor { get; set; }
        public string? RoomType { get; set; }
        public int Capacity { get; set; }
        public bool Deleted { get; set; }
    }
}