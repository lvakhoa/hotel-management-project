using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;

namespace HotelManagement.ViewModel;

public partial class PrintInvoiceVM : ObservableObject
{
    public Customer Info { get; set; }
    public PrintInvoiceVM()
    {
        Info = new Customer();
        Info.FullName = "Le Trung Kien";
    }
}