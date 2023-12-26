using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HotelManagement.View.AddView;
using HotelManagement.ViewModel.ManagementList;
using UIBtn = Wpf.Ui.Controls.Button;

namespace HotelManagement.CustomControls.Datagrid;

public partial class ServiceUseDataGrid : UserControl
{
    public ServiceUseDataGrid()
    {
        InitializeComponent();
    }
    
    private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var addServiceUse = new AddServiceUse(this.DataContext)
        {
            ShowInTaskbar = false,
            Topmost = true
        };
        addServiceUse.ShowDialog();
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn) sender;
        
        var addServiceUse = new AddServiceUse(btn.Tag.ToString(), btn.Uid ,this.DataContext)
        {
            ShowInTaskbar = false,
            Topmost = true
        };
        addServiceUse.ShowDialog();
    }

    private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is DataGridRow dgr) dgr.IsSelected = true;
    }
    
    private void TextBox_OnSearch(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            var text = SearchBox.Text.ToLower();

            var itemSourceList = new CollectionViewSource() { Source = (DataContext as ServiceUseList)!.List };

            ICollectionView itemlist = itemSourceList.View;

            var filter = new Predicate<object>(item => ComplexFilter(item, text));

            itemlist.Filter = filter;

            DataGrid1.ItemsSource = itemlist;
        }
    }
    
    private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var text = SearchBox.Text.ToLower();

        var itemSourceList = new CollectionViewSource() { Source = (DataContext as ServiceUseList)!.List };

        ICollectionView itemlist = itemSourceList.View;

        var filter = new Predicate<object>(item => ComplexFilter(item, text));

        itemlist.Filter = filter;

        DataGrid1.ItemsSource = itemlist;
    }
    
    private async void SearchBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var text = (sender as TextBox)?.Text;

        if (string.IsNullOrEmpty(text))
        {
            await Task.Delay(100);
            DataGrid1.ItemsSource = (DataContext as ServiceUseList)!.List;
        }
    }

    private bool ComplexFilter(object obj, string text)
    {
        var item = (ServiceUseList.ServiceUseVM)obj;
        return item.InvoiceId!.ToLower().Contains(text) ||
               item.ServiceId!.ToLower().Contains(text);
    }
}