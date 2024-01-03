using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HotelManagement.View.AddView;
using HotelManagement.ViewModel.ManagementList;
using UIBtn = Wpf.Ui.Controls.Button;

namespace HotelManagement.CustomControls.Datagrid;

public partial class RoomTypeDataGrid : UserControl
{
    public RoomTypeDataGrid()
    {
        InitializeComponent();
    }
    
    private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var addRoomType = new AddRoomType(this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this)
        };
        addRoomType.ShowDialog();
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn) sender;
        
        var addRoomType = new AddRoomType(btn.Tag.ToString(), this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this)
        };
        addRoomType.ShowDialog();
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

            var itemSourceList = new CollectionViewSource() { Source = (DataContext as RoomtypeList)!.List };

            ICollectionView itemlist = itemSourceList.View;

            var filter = new Predicate<object>(item => ComplexFilter(item, text));

            itemlist.Filter = filter;

            DataGrid1.ItemsSource = itemlist;
        }
    }
    
    private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var text = SearchBox.Text.ToLower();

        var itemSourceList = new CollectionViewSource() { Source = (DataContext as RoomtypeList)!.List };

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
            DataGrid1.ItemsSource = (DataContext as RoomtypeList)!.List;
        }
    }

    private bool ComplexFilter(object obj, string text)
    {
        var item = (RoomtypeList.RoomtypeVM)obj;
        return item.ID!.ToLower().Contains(text) ||
               item.RoomTypeName!.ToLower().Contains(text);
    }

    private void RestoreBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RestoreMenu.IsOpen = true;
    }
    
    private void OnOpened(object sender, RoutedEventArgs e)
    {
        var contextMenu = (ContextMenu)sender;
        ((MenuItem)contextMenu.Items[0]!).Command = DataContext is RoomtypeList roomTypeList ? roomTypeList.RestoreLast7DaysCommand : null;
        ((MenuItem)contextMenu.Items[1]!).Command = DataContext is RoomtypeList roomTypeList1 ? roomTypeList1.RestoreLast30DaysCommand : null;
        ((MenuItem)contextMenu.Items[2]!).Command = DataContext is RoomtypeList roomTypeList2 ? roomTypeList2.RestoreAllCommand : null;
    }
}