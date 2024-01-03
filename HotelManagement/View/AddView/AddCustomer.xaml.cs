using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using HotelManagement.ViewModel.ManagementList;

namespace HotelManagement.View.AddView
{
    /// <summary>
    /// Interaction logic for Addcustomer.xaml
    /// </summary>
    public partial class AddCustomer : Window
    {
        public AddCustomer(object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;

            (DataContext as CustomerList).GenerateCustomerId();
        }
        
        public AddCustomer(string? id, object dataContext)
        {
            InitializeComponent();
            
            DataContext = dataContext;
            
            if(id != null)
                (DataContext as CustomerList).GetCustomerById(id);

        }

        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
        private void AddCustomer_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        
        private void SaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
