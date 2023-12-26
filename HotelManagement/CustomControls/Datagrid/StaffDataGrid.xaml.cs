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
            Topmost = true
        };
        addStaff.ShowDialog();
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn)sender;

        var addStaff = new AddStaff(btn.Tag.ToString(), this.DataContext)
        {
            ShowInTaskbar = false,
            Topmost = true
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


    private void FilterByNumber(string text)
    {
        if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text)) return;

        var itemSourceList = new CollectionViewSource() { Source = (DataContext as StaffList)!.List };

        ICollectionView itemlist = itemSourceList.View;

        var filter =
            new Predicate<object>(item => decimal.Parse((item as StaffList.StaffVM)!.Salary!) >= decimal.Parse(text));

        itemlist.Filter = filter;

        DataGrid1.ItemsSource = itemlist;
    }

    private void MenuItem_OnClick(object sender, RoutedEventArgs e)
    {
        string result = Microsoft.VisualBasic.Interaction.InputBox("Enter some text:", "Input Dialog");

        FilterByNumber(result);
    }

    private void FilterBtn_OnClick(object sender, RoutedEventArgs e)
    {
        FilterMenu.IsOpen = true;
    }

    private void StaffDataGrid_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataGrid1 != null)
        {
            var columns = DataGrid1.Columns;

            FilterMenu.ItemsSource = columns.Where(x => x.Header != null)
                .Select(x => x.Header.ToString() );
        }
    }
}