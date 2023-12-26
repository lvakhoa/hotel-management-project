using System;
using System.Collections.Generic;

namespace HotelManagement.Model;

public partial class Staff
{
    public string StaffId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Position { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string Address { get; set; } = null!;

    public DateTime? Birthday { get; set; }

    public string Gender { get; set; } = null!;

    public decimal? Salary { get; set; }

    public bool? Deleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
