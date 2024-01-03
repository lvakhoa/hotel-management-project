using System;
using System.Collections.Generic;

namespace HotelManagement.Model;

public partial class ServiceUse
{
    public string InvoiceId { get; set; } = null!;

    public string ServiceId { get; set; } = null!;

    public int? ServiceQuantity { get; set; }

    public decimal? TotalAmount { get; set; }

    public bool? Deleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
