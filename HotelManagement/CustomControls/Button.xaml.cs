using System.Windows;
using System.Windows.Controls;

namespace HotelManagement.CustomControls;

public partial class Button : UserControl
{
    public static readonly DependencyProperty HoveredColorProperty = DependencyProperty.Register(
        "HoveredColor", 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string HoveredColor
    {
        get { return (string) GetValue(HoveredColorProperty); }
        set { SetValue(HoveredColorProperty, value); }
    }
    
    public static readonly DependencyProperty ClickedColorProperty = DependencyProperty.Register(
        "ClickedColor", 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string ClickedColor
    {
        get { return (string) GetValue(ClickedColorProperty); }
        set { SetValue(ClickedColorProperty, value); }
    }
    
    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        "Icon", 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string Icon
    {
        get { return (string) GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    
    public static readonly DependencyProperty BorderRadiusProperty = DependencyProperty.Register(
        "BorderRadius", 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string BorderRadius
    {
        get { return (string) GetValue(BorderRadiusProperty); }
        set { SetValue(BorderRadiusProperty, value); }
    }
    
    public Button()
    {
        DataContext = this;
        InitializeComponent();
    }
}