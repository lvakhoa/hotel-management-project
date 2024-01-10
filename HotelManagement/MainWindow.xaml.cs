using System.Configuration;
using System.Windows;
using System.Windows.Input;
using HotelManagement.Model;
using HotelManagement.Themes;
using HotelManagement.View;
using Wpf.Ui.Common;
using Wpf.Ui.Controls;
using UIBtn = Wpf.Ui.Controls.Button;

namespace HotelManagement;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    public MainWindow()
    {
        InitializeComponent();
        this.PreviewKeyDown += MainWindowPreviewKeyDown;

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