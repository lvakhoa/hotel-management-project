using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace HotelManagement.ViewModel.ManagementList;

public partial class AccountList : ObservableObject
{
    public ObservableCollection<AccountVM> List { get; set; }

    [ObservableProperty]
    private bool _isLoading;

    public AccountList()
    {
        List = new ObservableCollection<AccountVM>();

        GetAccountList();
    }

    private async void GetAccountList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();

        var accounts = await (from account in context.Accounts
                              select new
                              {
                                  account.Username,
                                  account.Password,
                                  account.ProfilePicture,
                                  account.StaffId,
                                  account.Status
                              }).ToListAsync();

        foreach (var item in accounts)
        {
            List.Add(new AccountVM()
            {
                Username = item.Username,
                Password = item.Password,
                ProfilePicture = item.ProfilePicture,
                StaffId = item.StaffId,
                Status = item.Status,

            });
        }

        IsLoading = false;
    }

    public class AccountVM
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public byte[]? ProfilePicture { get; set; }
        public string? StaffId { get; set; }
        public bool? Status { get; set; }

    }
}