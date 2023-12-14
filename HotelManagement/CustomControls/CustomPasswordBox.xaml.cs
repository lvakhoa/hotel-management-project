using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace HotelManagement.CustomControls
{
    /// <summary>
    /// Interaction logic for PasswordBox.xaml
    /// </summary>
    public partial class CustomPasswordBox : UserControl
    {
        private bool PasswordChanging;

        public CustomPasswordBox()
        {
            InitializeComponent();
        }

        public string Password
        {
            get { return (string)GetValue(PasswordProperty); }
            set { SetValue(PasswordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Password.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.Register("Password", typeof(string), typeof(CustomPasswordBox),
                new PropertyMetadata(string.Empty, PasswordPropertyChanged));

        private static void PasswordPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CustomPasswordBox passbox)
            {
                passbox.UpdatePassword();
            }
        }

        private void UpdatePassword()
        {
            if (!PasswordChanging)
            {
                passbox.Password = Password;
            }
        }

        private void passbox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordChanging = true;
            Password = passbox.Password;
            PasswordChanging = false;
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(passwordBox, new object[] { start, length });
        }

        private void Showpass_OnClicked(object sender, RoutedEventArgs e)
        {
            showpasstxtbox.Focus();
            showpasstxtbox.SelectionStart = showpasstxtbox.Text.Length;
            passbox.Focus();
            SetSelection(passbox, passbox.Password.Length, 0);
        }
    }
}