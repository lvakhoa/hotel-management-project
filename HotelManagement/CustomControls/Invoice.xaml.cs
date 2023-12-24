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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotelManagement.Model;

namespace HotelManagement.CustomControls
{
    /// <summary>
    /// Interaction logic for Invoice.xaml
    /// </summary>
    public partial class Invoice : UserControl
    {
        public Invoice()
        {
            InitializeComponent();
        }
        public Customer CustomerInfo
        {
            get { return (Customer)GetValue(CustomerInfoProperty); }
            set { SetValue(CustomerInfoProperty, value); }
        }
        
        public static readonly DependencyProperty CustomerInfoProperty =
            DependencyProperty.Register(nameof(CustomerInfo), typeof(Customer), typeof(Invoice));
        
        
    }
}
