using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.CustomControls.Datagrid;

public partial class InvoiceDataGrid : UserControl
{
    public InvoiceDataGrid()
    {
        InitializeComponent();
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

            var itemSourceList = new CollectionViewSource() { Source = (DataContext as InvoiceList)!.List };

            ICollectionView itemlist = itemSourceList.View;

            var filter = new Predicate<object>(item => ComplexFilter(item, text));

            itemlist.Filter = filter;

            DataGrid1.ItemsSource = itemlist;
        }
    }
    
    private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var text = SearchBox.Text.ToLower();

        var itemSourceList = new CollectionViewSource() { Source = (DataContext as InvoiceList)!.List };

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
            DataGrid1.ItemsSource = (DataContext as InvoiceList)!.List;
        }
    }

    private bool ComplexFilter(object obj, string text)
    {
        var item = (InvoiceList.InvoiceVM)obj;
        return item.InvoiceID!.ToLower().Contains(text) ||
               item.CustomerId!.ToLower().Contains(text) ||
               item.StaffId!.ToLower().Contains(text);
    }

    private void RestoreBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RestoreMenu.IsOpen = true;
    }
    
    private void OnOpened(object sender, RoutedEventArgs e)
    {
        var contextMenu = (ContextMenu)sender;
        ((MenuItem)contextMenu.Items[0]!).Command = DataContext is InvoiceList invoiceList ? invoiceList.RestoreLast7DaysCommand : null;
        ((MenuItem)contextMenu.Items[1]!).Command = DataContext is InvoiceList invoiceList1 ? invoiceList1.RestoreLast30DaysCommand : null;
        ((MenuItem)contextMenu.Items[2]!).Command = DataContext is InvoiceList invoiceList2 ? invoiceList2.RestoreAllCommand : null;
    }
}