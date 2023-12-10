using System.Windows.Controls;

namespace HotelManagement.View;

public partial class Settings : UserControl
{


    public Settings()
    {
        InitializeComponent();
        btnLight.IsChecked = true;
        cbboxLanguage.SelectedIndex = 0;
    }

    private void btnLight_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        btnDark.IsChecked = false;
    }

    private void btnDark_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        btnLight.IsChecked = false;
    }


}