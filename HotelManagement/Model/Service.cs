using System;
using System.Collections.Generic;

namespace HotelManagement.Model;

public partial class Service
{
    public string ServiceId { get; set; } = null!;

    public string ServiceName { get; set; } = null!;

    public string ServiceType { get; set; } = null!;

    public decimal ServicePrice { get; set; }

    public bool? Deleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<ServiceUse> ServiceUses { get; set; } = new List<ServiceUse>();
}
