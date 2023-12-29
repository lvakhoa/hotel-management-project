using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HotelManagement.View.AddView;
using HotelManagement.ViewModel.ManagementList;
using UIBtn = Wpf.Ui.Controls.Button;

namespace HotelManagement.CustomControls.Datagrid;

public partial class BookingDataGrid : UserControl
{
    public BookingDataGrid()
    {
        InitializeComponent();
    }

    private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var addBooking = new AddBooking(this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this)
        };
        addBooking.ShowDialog();
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn)sender;

        var addBooking = new AddBooking(btn.Tag.ToString(), this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this),
        };
        addBooking.ShowDialog();
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

            var itemSourceList = new CollectionViewSource() { Source = (DataContext as BookingList)!.List };

            ICollectionView itemlist = itemSourceList.View;

            var filter = new Predicate<object>(item => ComplexFilter(item, text));

            itemlist.Filter = filter;

            DataGrid1.ItemsSource = itemlist;
        }
    }
    
    private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var text = SearchBox.Text.ToLower();

        var itemSourceList = new CollectionViewSource() { Source = (DataContext as BookingList)!.List };

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
            DataGrid1.ItemsSource = (DataContext as BookingList)!.List;
        }
    }

    private bool ComplexFilter(object obj, string text)
    {
        var item = (BookingList.BookingVM)obj;
        return item.BookingID!.ToLower().Contains(text) ||
               item.InvoiceID!.ToLower().Contains(text) ||
               item.RoomItem!.RoomID!.ToLower().Contains(text);
    }

    private void RestoreBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RestoreMenu.IsOpen = true;
    }
    
    private void OnOpened(object sender, RoutedEventArgs e)
    {
        var contextMenu = (ContextMenu)sender;
        ((MenuItem)contextMenu.Items[0]!).Command = DataContext is BookingList bookingList ? bookingList.RestoreLast7DaysCommand : null;
        ((MenuItem)contextMenu.Items[1]!).Command = DataContext is BookingList bookingList1 ? bookingList1.RestoreLast30DaysCommand : null;
        ((MenuItem)contextMenu.Items[2]!).Command = DataContext is BookingList bookingList2 ? bookingList2.RestoreAllCommand : null;
    }
}