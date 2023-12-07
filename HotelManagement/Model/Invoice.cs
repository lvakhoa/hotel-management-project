namespace HotelManagement.Model;

public partial class Invoice
{
    public string InvoiceId { get; set; } = null!;

    public string? CustomerId { get; set; } = null!;

    public string StaffId { get; set; } = null!;

    public DateTime InvoiceDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public string PaymentType { get; set; } = null!;

    public bool? Deleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<ServiceUse> ServiceUses { get; set; } = new List<ServiceUse>();

    public virtual Staff Staff { get; set; } = null!;
}
