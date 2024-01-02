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
        var uISettingSection = (UISettings)AppConfig.GetSection("UISettings");
        if (uISettingSection.Theme == "Light")
        {
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri("pack://application:,,,/Wpf.Ui;component/Styles/Theme/light.xaml");
            Application.Current.Resources.MergedDictionaries.Add(resource);
            
            // Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Light);
        }
        else
        {
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri("pack://application:,,,/Wpf.Ui;component/Styles/Theme/dark.xaml");
            Application.Current.Resources.MergedDictionaries.Add(resource);
            
            // Wpf.Ui.Appearance.Theme.Apply(Wpf.Ui.Appearance.ThemeType.Dark);
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
    }

    private void MaximizeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        var btn = sender as UIBtn;
        if (WindowState == WindowState.Normal)
        {
            WindowState = WindowState.Maximized;
            btn!.Icon = SymbolRegular.SquareMultiple24;
            CloseBtn.CornerRadius = new CornerRadius(0);
            WindowBorder.CornerRadius = new CornerRadius(0);
        }
        else
        {
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