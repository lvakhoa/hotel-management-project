using System.Windows;
using System.Windows.Input;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.View.AddView;

public partial class AddServiceUse : Window
{
    public AddServiceUse(object dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;
        
        (DataContext as ServiceUseList).AddServiceUse();
    }
    
    public AddServiceUse(string serviceId, string invoiceId, object dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;
        
        (DataContext as ServiceUseList).GetServiceUseById(serviceId, invoiceId);

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
}