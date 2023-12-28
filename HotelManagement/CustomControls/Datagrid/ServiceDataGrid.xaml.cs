using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using HotelManagement.View.AddView;
using HotelManagement.ViewModel.ManagementList;
using UIBtn = Wpf.Ui.Controls.Button;

namespace HotelManagement.CustomControls.Datagrid;

public partial class ServiceDataGrid : UserControl
{
    public ServiceDataGrid()
    {
        InitializeComponent();
    }
    
    private void ButtonAdd_OnClick(object sender, RoutedEventArgs e)
    {
        var addService = new AddService(this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this)
        };
        addService.ShowDialog();
    }

    private void ButtonEdit_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = (UIBtn) sender;
        
        var addService = new AddService(btn.Tag.ToString(), this.DataContext)
        {
            ShowInTaskbar = false,
            Owner = Window.GetWindow(this)
        };
        addService.ShowDialog();
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

            var itemSourceList = new CollectionViewSource() { Source = (DataContext as ServiceList)!.List };

            ICollectionView itemlist = itemSourceList.View;

            var filter = new Predicate<object>(item => ComplexFilter(item, text));

            itemlist.Filter = filter;

            DataGrid1.ItemsSource = itemlist;
        }
    }
    
    private void SearchBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var text = SearchBox.Text.ToLower();

        var itemSourceList = new CollectionViewSource() { Source = (DataContext as ServiceList)!.List };

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
            DataGrid1.ItemsSource = (DataContext as ServiceList)!.List;
        }
    }

    private bool ComplexFilter(object obj, string text)
    {
        var item = (ServiceList.ServiceVM)obj;
        return item.ID!.ToLower().Contains(text) ||
               item.ServiceName!.ToLower().Contains(text) ||
               item.ServiceType!.ToLower().Contains(text);
    }
}