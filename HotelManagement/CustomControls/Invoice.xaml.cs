using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using HotelManagement.Model;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.CustomControls
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class Invoice : UserControl
    {
        public DateTime InvoiceDate { get; set; }
        public Invoice()
        {
            InitializeComponent();
            InvoiceDate = DateTime.Today;
        }
        public ObservableCollection<InvoiceList.BookingInvoice> BookingList
        {
            get { return (ObservableCollection<InvoiceList.BookingInvoice>)GetValue(BookingListProperty); }
            set { SetValue(BookingListProperty, value); }
        }
        private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGridRow dgr) dgr.IsSelected = true;
        }
        public static readonly DependencyProperty BookingListProperty =
            DependencyProperty.Register(nameof(BookingList), typeof(ObservableCollection<InvoiceList.BookingInvoice>), typeof(Invoice));
        
        public ObservableCollection<InvoiceList.ServiceInvoice> ServiceList
        {
            get { return (ObservableCollection<InvoiceList.ServiceInvoice>)GetValue(ServiceListProperty); }
            set { SetValue(ServiceListProperty, value); }
        }
        
        public static readonly DependencyProperty ServiceListProperty =
            DependencyProperty.Register(nameof(ServiceList), typeof(ObservableCollection<InvoiceList.ServiceInvoice>), typeof(Invoice));

        public CustomerList.CustomerVM CustomerInfo
        {
            get { return (CustomerList.CustomerVM)GetValue(CustomerInfoProperty); }
            set { SetValue(CustomerInfoProperty, value); }
        }
        
        public static readonly DependencyProperty CustomerInfoProperty =
            DependencyProperty.Register(nameof(CustomerInfo), typeof(CustomerList.CustomerVM), typeof(Invoice));
        
        public InvoiceList.InvoiceVM InvoiceInfo
        {
            get { return (InvoiceList.InvoiceVM)GetValue(InvoiceInfoProperty); }
            set { SetValue(InvoiceInfoProperty, value); }
        }
        
        public static readonly DependencyProperty InvoiceInfoProperty =
            DependencyProperty.Register(nameof(InvoiceInfo), typeof(InvoiceList.InvoiceVM), typeof(Invoice));

    }
}
