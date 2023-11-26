using System.Collections.ObjectModel;
using System.Windows.Documents;
using HotelManagement.Model;

namespace HotelManagement.ViewModel;

public class StaffVM 
{
    public ObservableCollection<Staff> Staffs { get; set; }

    public StaffVM()
    {
        using var context = new HotelManagementContext();
        Staffs = new ObservableCollection<Staff>(context.Staff);
    }
}