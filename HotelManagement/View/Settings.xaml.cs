using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HotelManagement.View;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
        btnLight.IsChecked = true;
    }

    private void btnLight_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        btnDark.IsChecked = false;
    }

    private void btnDark_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        btnLight.IsChecked = false;
    }

    private void EditTb_OnMouseEnter(object sender, MouseEventArgs e)
    {
            TextDecoration myUnderline = new TextDecoration
            {
                Pen = new Pen(new BrushConverter().ConvertFrom("#373434") as SolidColorBrush, 1),
                PenThicknessUnit = TextDecorationUnit.FontRecommended,
                PenOffset = 3
            };

            TextDecorationCollection myCollection = new TextDecorationCollection { myUnderline };
            EditTb.TextDecorations = myCollection;
    }

    private void EditTb_OnMouseLeave(object sender, MouseEventArgs e)
    {
        EditTb.TextDecorations = null;
    }

    private void ButtonEdit_Click(object sender, RoutedEventArgs e)
    {
        var edit = new UserInfo()
        {
            ShowInTaskbar = false,
            Topmost = true
        };
        edit.ShowDialog();
    }
}