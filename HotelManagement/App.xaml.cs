using System.Configuration;
using System.Data;
using System.Windows;
using HotelManagement.View;

namespace HotelManagement;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // public static LoginView LoginView { get; set; }
    // public static MainWindow MainView { get; set; }
    private void ApplicationStart(object sender, StartupEventArgs e)
    {
        var loginView = new LoginView();
        loginView.Show();
        loginView.IsVisibleChanged += (s, ev) =>
        {
            if (loginView.IsVisible == false && loginView.IsLoaded)
            {
                var mainView = new MainWindow();
                mainView.Show();
                loginView.Close();
            }
        };
    }
}