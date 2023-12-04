using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.ViewModel;

public partial class CustomerList : ObservableObject
{
    public ObservableCollection<CustomerVM> List { get; set; }

    [ObservableProperty] private bool _isLoading;

    public CustomerList()
    {
        List = new ObservableCollection<CustomerVM>();

        GetCustomerList();
    }

    private async void GetCustomerList()
    {
        IsLoading = true;
        await Task.Delay(2000);
        await using var context = new HotelManagementContext();

        var customers = await (from customer in context.Customers
            select new
            {
                customer.CustomerId, customer.FullName, customer.ContactNumber, customer.Email, customer.Address,
                customer.Gender, customer.CreditCard, customer.IdProof
            }).ToListAsync();

        foreach (var item in customers)
        {
            List.Add(new CustomerVM()
            {
                CustomerID = item.CustomerId, FullName = item.FullName, ContactNumber = item.ContactNumber,
                Email = item.Email, Address = item.Address, Gender = item.Gender, CreditCard = item.CreditCard,
                ProofID = item.IdProof
            });
        }

        IsLoading = false;
    }

    public class CustomerVM
    {
        public string? CustomerID { get; set; }

        public string? FullName { get; set; }

        public string? ContactNumber { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Gender { get; set; }

        public string? CreditCard { get; set; }

        public string? ProofID { get; set; }
    }
}