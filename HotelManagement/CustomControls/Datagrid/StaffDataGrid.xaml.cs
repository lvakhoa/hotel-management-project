using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HotelManagement.View.AddView;
using HotelManagement.ViewModel.ManagementList;
using UIBtn = Wpf.Ui.Controls.Button;
using TextBox = System.Windows.Controls.TextBox;

namespace HotelManagement.CustomControls.Datagrid;

public partial class StaffDataGrid : UserControl
{
    public StaffDataGrid()
    {
        InitializeComponent();
    }

    private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var addStaff = new AddStaff(this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this)
        };
        addStaff.ShowDialog();
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn)sender;

        var addStaff = new AddStaff(btn.Tag.ToString(), this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this)
        };
        addStaff.ShowDialog();
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

            var itemSourceList = new CollectionViewSource() { Source = (DataContext as StaffList)!.List };

            ICollectionView itemlist = itemSourceList.View;

            var filter = new Predicate<object>(item => ComplexFilter(item, text));

            itemlist.Filter = filter;

            DataGrid1.ItemsSource = itemlist;
        }
    }

    private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var text = SearchBox.Text.ToLower();

        var itemSourceList = new CollectionViewSource() { Source = (DataContext as StaffList)!.List };

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
            DataGrid1.ItemsSource = (DataContext as StaffList)!.List;
        }
    }

    private bool ComplexFilter(object obj, string text)
    {
        var item = (StaffList.StaffVM)obj;
        return item.ID!.ToLower().Contains(text) ||
               item.FullName!.ToLower().Contains(text) ||
               item.Position!.ToLower().Contains(text) ||
               item.ContactNumber!.ToLower().Contains(text) ||
               item.Email!.ToLower().Contains(text) ||
               item.Address!.ToLower().Contains(text) ||
               item.Gender!.ToLower().Contains(text);
    }
}