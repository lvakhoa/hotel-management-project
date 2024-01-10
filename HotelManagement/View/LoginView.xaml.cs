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
using Wpf.Ui.Common;
using UIBtn = Wpf.Ui.Controls.Button;

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
            WindowState = App.WinState;
            
            if (WindowState == WindowState.Normal)
            {
                CloseBtn.CornerRadius = new CornerRadius(0, 10, 0, 0);
                WindowBorder.CornerRadius = new CornerRadius(10);
            }
            else
            {
                CloseBtn.CornerRadius = new CornerRadius(0);
                WindowBorder.CornerRadius = new CornerRadius(0);
            }
        }

        private void Box_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                PasswordBox.passbox.Clear();
                PasswordBox.showpasstxtbox.Clear();
                FocusManager.SetIsFocusScope(this, true);
            }
        }
        
        private void MinimizeBtn_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
            App.WinState = WindowState.Minimized;
        }

        private void MaximizeBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var btn = sender as UIBtn;
            if (WindowState == WindowState.Normal)
            {
                App.WinState = WindowState.Maximized;
                WindowState = WindowState.Maximized;
                btn!.Icon = SymbolRegular.SquareMultiple24;
                CloseBtn.CornerRadius = new CornerRadius(0);
                WindowBorder.CornerRadius = new CornerRadius(0);
            }
            else
            {
                App.WinState = WindowState.Normal;
                WindowState = WindowState.Normal;
                btn!.Icon = SymbolRegular.Square24;
                CloseBtn.CornerRadius = new CornerRadius(0, 10, 0, 0);
                WindowBorder.CornerRadius = new CornerRadius(10);
            }
        }

        private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
