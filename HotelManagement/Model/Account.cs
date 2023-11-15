using System;
using System.Collections.Generic;

namespace HotelManagement;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public byte[]? ProfilePicture { get; set; }

    public string StaffId { get; set; } = null!;

    public virtual Staff Staff { get; set; } = null!;
}
