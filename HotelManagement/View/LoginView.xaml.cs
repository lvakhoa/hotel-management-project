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

namespace HotelManagement.View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Login_OnClicked(object sender, RoutedEventArgs e)
        {
            LoginWindow.Focus();
        }

        private void Box_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
                LoginWindow.Focus();
        }
    }
}
