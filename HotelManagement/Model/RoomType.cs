using System;
using System.Collections.Generic;

namespace HotelManagement.Model;

public partial class RoomType
{
    public string RoomTypeId { get; set; } = null!;

    public string RoomTypeName { get; set; } = null!;

    public int Capacity { get; set; }

    public int BedAmount { get; set; }

    public decimal RoomPrice { get; set; }

    public string RoomTypeDesc { get; set; } = null!;

    public byte[]? RoomTypeImg { get; set; }

    public bool? Deleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
}
