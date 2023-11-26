using System;
using System.Collections.Generic;
using System.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Model;

public partial class HotelManagementContext : DbContext
{
    public HotelManagementContext()
    {
    }

    public HotelManagementContext(DbContextOptions<HotelManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<RoomType> RoomTypes { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<ServiceUse> ServiceUses { get; set; }

    public virtual DbSet<Staff> Staff { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["HotelManagementDB"].ConnectionString ??
                                  throw new InvalidOperationException();

        optionsBuilder.UseSqlServer(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Username).HasName("PK__account__F3DBC573C3AB2CB1");

            entity.ToTable("account");

            entity.HasIndex(e => e.StaffId, "UQ__account__1963DD9DC28BDAE4").IsUnique();

            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
            entity.Property(e => e.ProfilePicture).HasColumnName("profile_picture");
            entity.Property(e => e.StaffId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("staff_id");
            entity.Property(e => e.Status)
                .HasDefaultValueSql("((0))")
                .HasColumnName("status");

            entity.HasOne(d => d.Staff).WithOne(p => p.Account)
                .HasForeignKey<Account>(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("acc_staff_id_fk");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__booking__5DE3A5B13FD7C644");

            entity.ToTable("booking", tb =>
            {
                tb.HasTrigger("CheckGuestQuantity");
                tb.HasTrigger("InsertAmountBooking");
                tb.HasTrigger("UpdateAmountBooking");
            });

            entity.Property(e => e.BookingId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("booking_id");
            entity.Property(e => e.CheckInDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("check_in_date");
            entity.Property(e => e.CheckOutDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("check_out_date");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.GuestQuantity).HasColumnName("guest_quantity");
            entity.Property(e => e.InvoiceId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("invoice_id");
            entity.Property(e => e.RoomId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("room_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("money")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Invoice).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("b_invoice_id_fk");

            entity.HasOne(d => d.Room).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("b_room_id_fk");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PK__customer__CD65CB85EA241678");

            entity.ToTable("customer");

            entity.HasIndex(e => e.ContactNumber, "UQ__customer__A1D1BF21831D3536").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__customer__AB6E61640EAC5AAA").IsUnique();

            entity.HasIndex(e => e.CreditCard, "UQ__customer__C0CC90664A4CED78").IsUnique();

            entity.HasIndex(e => e.IdProof, "UQ__customer__DFB75B10A382F22D").IsUnique();

            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("customer_id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(10)
                .HasColumnName("contact_number");
            entity.Property(e => e.CreditCard)
                .HasMaxLength(45)
                .HasColumnName("credit_card");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(45)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .HasColumnName("gender");
            entity.Property(e => e.IdProof)
                .HasMaxLength(45)
                .HasColumnName("id_proof");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__invoice__F58DFD4983235F70");

            entity.ToTable("invoice");

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("invoice_id");
            entity.Property(e => e.CustomerId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("customer_id");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.InvoiceDate)
                .HasColumnType("smalldatetime")
                .HasColumnName("invoice_date");
            entity.Property(e => e.PaymentType)
                .HasMaxLength(20)
                .HasColumnName("payment_type");
            entity.Property(e => e.StaffId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("staff_id");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("money")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Customer).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("in_customer_id_fk");

            entity.HasOne(d => d.Staff).WithMany(p => p.Invoices)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("in_staff_id_fk");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomId).HasName("PK__room__19675A8A401BF51A");

            entity.ToTable("room");

            entity.Property(e => e.RoomId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("room_id");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.Notes)
                .HasMaxLength(45)
                .HasColumnName("notes");
            entity.Property(e => e.RoomNumber)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("room_number");
            entity.Property(e => e.RoomTypeId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("room_type_id");

            entity.HasOne(d => d.RoomType).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.RoomTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("r_room_type_id_fk");
        });

        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.HasKey(e => e.RoomTypeId).HasName("PK__room_typ__42395E8460623310");

            entity.ToTable("room_type");

            entity.HasIndex(e => e.RoomTypeName, "UQ__room_typ__511E79A8737E6009").IsUnique();

            entity.Property(e => e.RoomTypeId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("room_type_id");
            entity.Property(e => e.BedAmount).HasColumnName("bed_amount");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.RoomPrice)
                .HasColumnType("money")
                .HasColumnName("room_price");
            entity.Property(e => e.RoomTypeDesc)
                .HasMaxLength(100)
                .HasColumnName("room_type_desc");
            entity.Property(e => e.RoomTypeImg).HasColumnName("room_type_img");
            entity.Property(e => e.RoomTypeName)
                .HasMaxLength(20)
                .HasColumnName("room_type_name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__service__3E0DB8AFF2C18B61");

            entity.ToTable("service");

            entity.HasIndex(e => e.ServiceName, "UQ__service__4A8EDF399222ED33").IsUnique();

            entity.Property(e => e.ServiceId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("service_id");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(20)
                .HasColumnName("service_name");
            entity.Property(e => e.ServicePrice)
                .HasColumnType("money")
                .HasColumnName("service_price");
            entity.Property(e => e.ServiceType)
                .HasMaxLength(20)
                .HasColumnName("service_type");
        });

        modelBuilder.Entity<ServiceUse>(entity =>
        {
            entity.HasKey(e => new { e.InvoiceId, e.ServiceId }).HasName("su_booking_service_pk");

            entity.ToTable("service_use", tb =>
            {
                tb.HasTrigger("InsertAmountService");
                tb.HasTrigger("UpdateAmountService");
            });

            entity.Property(e => e.InvoiceId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("invoice_id");
            entity.Property(e => e.ServiceId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("service_id");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.ServiceQuantity).HasColumnName("service_quantity");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("money")
                .HasColumnName("total_amount");

            entity.HasOne(d => d.Invoice).WithMany(p => p.ServiceUses)
                .HasForeignKey(d => d.InvoiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("su_invoice_id_fk");

            entity.HasOne(d => d.Service).WithMany(p => p.ServiceUses)
                .HasForeignKey(d => d.ServiceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("su_service_id_fk");
        });

        modelBuilder.Entity<Staff>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK__staff__1963DD9CFB396E3F");

            entity.ToTable("staff");

            entity.HasIndex(e => e.ContactNumber, "UQ__staff__A1D1BF21B9E63F8F").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__staff__AB6E6164B8072C71").IsUnique();

            entity.Property(e => e.StaffId)
                .HasMaxLength(5)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("staff_id");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("address");
            entity.Property(e => e.Birthday)
                .HasColumnType("date")
                .HasColumnName("birthday");
            entity.Property(e => e.ContactNumber)
                .HasMaxLength(10)
                .HasColumnName("contact_number");
            entity.Property(e => e.Deleted)
                .HasDefaultValueSql("((0))")
                .HasColumnName("deleted");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("datetime")
                .HasColumnName("deleted_date");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(45)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(6)
                .HasColumnName("gender");
            entity.Property(e => e.Position)
                .HasMaxLength(45)
                .HasColumnName("position");
            entity.Property(e => e.Salary)
                .HasColumnType("money")
                .HasColumnName("salary");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}