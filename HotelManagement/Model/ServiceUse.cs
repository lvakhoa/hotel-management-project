using System;
using System.Collections.Generic;

namespace HotelManagement;

public partial class ServiceUse
{
    public string InvoiceId { get; set; } = null!;

    public string ServiceId { get; set; } = null!;

    public int? ServiceQuantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
