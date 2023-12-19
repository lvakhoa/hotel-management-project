using System.Windows;
using System.Windows.Controls;

namespace HotelManagement.CustomControls.Toast;

public partial class SuccessToast : UserControl
{
    public SuccessToast()
    {
        InitializeComponent();
    }
    
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), 
        typeof(string), 
        typeof(BaseToast), 
        new PropertyMetadata(default(string)));

    public string Text
    {
        get { return (string) GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }
}