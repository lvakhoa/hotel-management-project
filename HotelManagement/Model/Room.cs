using System;
using System.Collections.Generic;

namespace HotelManagement;

public partial class Room
{
    public string RoomId { get; set; } = null!;

    public string RoomNumber { get; set; } = null!;

    public string? Notes { get; set; }

    public string RoomTypeId { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual RoomType RoomType { get; set; } = null!;
}
