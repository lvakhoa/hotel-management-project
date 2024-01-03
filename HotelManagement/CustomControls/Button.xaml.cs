using System.Windows;
using System.Windows.Controls;

namespace HotelManagement.CustomControls;

public partial class Button : UserControl
{
    public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register(
        nameof(ForegroundColor), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string ForegroundColor
    {
        get { return (string) GetValue(ForegroundColorProperty); }
        set { SetValue(ForegroundColorProperty, value); }
    }
    
    public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
        nameof(BackgroundColor), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string BackgroundColor
    {
        get { return (string) GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }
    
    public static readonly DependencyProperty HoveredColorProperty = DependencyProperty.Register(
        nameof(HoveredColor), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string HoveredColor
    {
        get { return (string) GetValue(HoveredColorProperty); }
        set { SetValue(HoveredColorProperty, value); }
    }
    
    public static readonly DependencyProperty ClickedColorProperty = DependencyProperty.Register(
        nameof(ClickedColor), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string ClickedColor
    {
        get { return (string) GetValue(ClickedColorProperty); }
        set { SetValue(ClickedColorProperty, value); }
    }
    
    public static readonly DependencyProperty ButtonContentProperty = DependencyProperty.Register(
        nameof(ButtonContent), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string ButtonContent
    {
        get { return (string) GetValue(ButtonContentProperty); }
        set { SetValue(ButtonContentProperty, value); }
    }
    
    public static readonly DependencyProperty TypeProperty = DependencyProperty.Register(
        nameof(Type), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string Type
    {
        get { return (string) GetValue(TypeProperty); }
        set { SetValue(TypeProperty, value); }
    }
    
    public static readonly DependencyProperty BorderRadiusProperty = DependencyProperty.Register(
        nameof(BorderRadius), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string BorderRadius
    {
        get { return (string) GetValue(BorderRadiusProperty); }
        set { SetValue(BorderRadiusProperty, value); }
    }
    
    public static readonly DependencyProperty WidthButtonProperty = DependencyProperty.Register(
        nameof(WidthButton), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string WidthButton
    {
        get { return (string) GetValue(WidthButtonProperty); }
        set { SetValue(WidthButtonProperty, value); }
    }
    
    public static readonly DependencyProperty HeightButtonProperty = DependencyProperty.Register(
        nameof(HeightButton), 
        typeof(string), 
        typeof(Button), 
        new PropertyMetadata(default(string)));

    public string HeightButton
    {
        get { return (string) GetValue(HeightButtonProperty); }
        set { SetValue(HeightButtonProperty, value); }
    }
    
    public Button()
    {
        DataContext = this;
        InitializeComponent();
    }
}