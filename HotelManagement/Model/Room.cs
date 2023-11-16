using System;
using System.Collections.Generic;

namespace HotelManagement.Model;

public partial class Room
{
    public string RoomId { get; set; } = null!;

    public string RoomNumber { get; set; } = null!;

    public string? Notes { get; set; }

    public string RoomTypeId { get; set; } = null!;

    public bool? Deleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual RoomType RoomType { get; set; } = null!;
}
