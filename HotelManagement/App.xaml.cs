using System.Configuration;
using System.Data;
using System.Windows;
using HotelManagement.Model;
using HotelManagement.Themes;
using HotelManagement.View;

namespace HotelManagement;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static WindowState WinState { get; set; } = WindowState.Normal;
    public static Window ActivatedWindow {get;set;}
    private Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    private void ApplicationStart(object sender, StartupEventArgs e)
    {
        //Check theme in config before StartUp
        if (AppConfig.Sections["UISettings"] is null)
        {
            AppConfig.Sections.Add("UISettings", new UISettings());
        }
        var UISettingSection = (UISettings)AppConfig.GetSection("UISettings");

        var theme = UISettingSection.Theme;
        if (theme == "Light")
        {
            ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri("pack://application:,,,/Wpf.Ui;component/Styles/Theme/light.xaml");
            Application.Current.Resources.MergedDictionaries.Add(resource);
        }
        else
        {
            ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
            ResourceDictionary resource = new ResourceDictionary();
            resource.Source = new Uri("pack://application:,,,/Wpf.Ui;component/Styles/Theme/dark.xaml");
            Application.Current.Resources.MergedDictionaries.Add(resource);
        }
        
        var loginView = new LoginView();
        ActivatedWindow = loginView;
        loginView.Show();
        loginView.IsVisibleChanged += (s, ev) =>
        {
            if (loginView.IsVisible == false && loginView.IsLoaded)
            {
                var mainView = new MainWindow();
                ActivatedWindow = mainView;
                mainView.Show();
                loginView.Close();
            }
        };
    }
}