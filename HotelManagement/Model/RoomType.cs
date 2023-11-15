using System;
using System.Collections.Generic;

namespace HotelManagement;

public partial class RoomType
{
    public string RoomTypeId { get; set; } = null!;

    public string RoomTypeName { get; set; } = null!;

    public int Capacity { get; set; }

    public int BedAmount { get; set; }

    public decimal RoomPrice { get; set; }

    public string RoomTypeDesc { get; set; } = null!;

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
