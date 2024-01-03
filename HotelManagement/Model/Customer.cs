using System;
using System.Collections.Generic;

namespace HotelManagement.Model;

public partial class Customer
{
    public string CustomerId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string ContactNumber { get; set; } = null!;

    public string? Email { get; set; }

    public string Address { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public string? CreditCard { get; set; }

    public string IdProof { get; set; } = null!;

    public bool? Deleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
