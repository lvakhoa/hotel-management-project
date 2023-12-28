using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HotelManagement.View;

namespace HotelManagement;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.PreviewKeyDown += MainWindowPreviewKeyDown;
    }
    
    static void MainWindowPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if(e.Key == Key.Tab)
        {
            e.Handled = true;
        }
    }

    private void LogoutBtn_OnChecked(object sender, RoutedEventArgs e)
    {
        var loginView = new LoginView();
        App.ActivatedWindow = loginView;
        loginView.Show();
        loginView.IsVisibleChanged += (s, ev) =>
        {
            if (loginView.IsVisible == false && loginView.IsLoaded)
            {
                var mainView = new MainWindow();
                App.ActivatedWindow = mainView;
                mainView.Show();
                loginView.Close();
            }
        };
        this.Close();
    }
}