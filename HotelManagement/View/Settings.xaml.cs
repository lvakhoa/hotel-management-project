using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using HotelManagement.Model;
using HotelManagement.Themes;
using Wpf.Ui.Appearance;

namespace HotelManagement.View;

public partial class Settings : UserControl
{
    private Configuration AppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
    private UISettings UISettingSection;
    public Settings()
    {
        InitializeComponent();

        if (AppConfig.Sections["UISettings"] is null)
        {
            AppConfig.Sections.Add("UISettings", new UISettings());
        }
        UISettingSection = (UISettings)AppConfig.GetSection("UISettings");
        if (UISettingSection.Theme == "Light")
        {
            btnLight.IsChecked = true;
        }
        else
        {
            btnDark.IsChecked = true;
        }
    }

    private void btnLight_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        // Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources.MergedDictionaries[^1]!);

        ResourceDictionary resource = new ResourceDictionary();
        resource.Source = new Uri("pack://application:,,,/Wpf.Ui;component/Styles/Theme/light.xaml");
        Application.Current.Resources.MergedDictionaries.Add(resource);
        
        ThemesController.SetTheme(ThemesController.ThemeTypes.Light);
        UISettingSection.Theme = "Light";
        btnDark.IsChecked = false;
        AppConfig.Save();
    }

    private void btnDark_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        // Application.Current.Resources.MergedDictionaries.Remove(Application.Current.Resources.MergedDictionaries[^1]!);
        
        ResourceDictionary resource = new ResourceDictionary();
        resource.Source = new Uri("pack://application:,,,/Wpf.Ui;component/Styles/Theme/dark.xaml");
        Application.Current.Resources.MergedDictionaries.Add(resource);
        
        ThemesController.SetTheme(ThemesController.ThemeTypes.Dark);
        UISettingSection.Theme = "Dark";
        btnLight.IsChecked = false;
        AppConfig.Save();
    }
}