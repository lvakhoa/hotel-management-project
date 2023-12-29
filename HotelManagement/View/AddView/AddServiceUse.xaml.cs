using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.View.AddView;

public partial class AddServiceUse : Window
{
    private ServiceUseList? ServiceUse => DataContext as ServiceUseList;
    public AddServiceUse(object dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;
        
        ServiceUse!.AddServiceUse();
    }
    
    public AddServiceUse(string serviceId, string invoiceId, object dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;
        
        ServiceUse!.GetServiceUseById(serviceId, invoiceId);

    }

    private void AddServiceUse_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void ServiceBox_OnLoaded(object sender, RoutedEventArgs e)
    {
        var itemVm = ServiceUse!.CurrentServiceUse.ServiceItem;
        var index = -1;
        
        foreach (var item in ServiceUse.ServiceIdList.Where(item => item.ServiceId == itemVm.ServiceId))
        {
            index = ServiceUse.ServiceIdList.IndexOf(item);
            break;
        }

        ServiceBox.SelectedIndex = index;
    }
}