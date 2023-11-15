using System;
using System.Collections.Generic;

namespace HotelManagement;

public partial class Booking
{
    public string BookingId { get; set; } = null!;

    public string InvoiceId { get; set; } = null!;

    public string RoomId { get; set; } = null!;

    public int GuestQuantity { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Room Room { get; set; } = null!;
}
