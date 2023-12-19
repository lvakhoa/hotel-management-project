using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using UIBtn = Wpf.Ui.Controls.Button;

namespace HotelManagement.CustomControls.Toast;

public partial class BaseToast : UserControl
{
    
    public BaseToast()
    {
        InitializeComponent();
    }

    private void CloseBtn_OnMouseEnter(object sender, MouseEventArgs e)
    {
        var btn = sender as UIBtn;
        var sb = new Storyboard();
        var animation = new DoubleAnimation
        {
            From = 0.7,
            To = 1.0,
            Duration = new Duration(TimeSpan.FromSeconds(0.3))
        };

        Storyboard.SetTarget(animation, btn);
        Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

        sb.Children.Add(animation);

        sb.Begin();
    }

    private void CloseBtn_OnMouseLeave(object sender, MouseEventArgs e)
    {
        var btn = sender as UIBtn;
        var sb = new Storyboard();
        var animation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.7,
            Duration = new Duration(TimeSpan.FromSeconds(0.3))
        };

        Storyboard.SetTarget(animation, btn);
        Storyboard.SetTargetProperty(animation, new PropertyPath(Control.OpacityProperty));

        sb.Children.Add(animation);

        sb.Begin();
    }

    private void CloseBtn_OnClick(object sender, RoutedEventArgs e)
    {
        Storyboard sb = new Storyboard();

        DoubleAnimation animation = new DoubleAnimation
        {
            From = -320,
            To = 0,
            Duration = new Duration(TimeSpan.FromSeconds(0.3))
        };

        CubicEase cubicEase = new CubicEase
        {
            EasingMode = EasingMode.EaseInOut
        };
        animation.EasingFunction = cubicEase;
        
        Storyboard.SetTarget(animation, Toast);
        Storyboard.SetTargetProperty(animation, new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)"));

        sb.Children.Add(animation);

        sb.Begin();

    }
    
    // Dependency Property
    // Background
    public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
        nameof(BackgroundColor), 
        typeof(string), 
        typeof(BaseToast), 
        new PropertyMetadata(default(string)));

    public string BackgroundColor
    {
        get { return (string) GetValue(BackgroundColorProperty); }
        set { SetValue(BackgroundColorProperty, value); }
    }
    
    // Foreground Text
    public static readonly DependencyProperty ForegroundTextProperty = DependencyProperty.Register(
        nameof(ForegroundText), 
        typeof(string), 
        typeof(BaseToast), 
        new PropertyMetadata(default(string)));

    public string ForegroundText
    {
        get { return (string) GetValue(ForegroundTextProperty); }
        set { SetValue(ForegroundTextProperty, value); }
    }
    
    // Text
    public static readonly DependencyProperty TextContentProperty = DependencyProperty.Register(
        nameof(TextContent), 
        typeof(string), 
        typeof(BaseToast), 
        new PropertyMetadata(default(string)));

    public string TextContent
    {
        get { return (string) GetValue(TextContentProperty); }
        set { SetValue(TextContentProperty, value); }
    }
    
    // Is Loading
    public static readonly DependencyProperty IsLoadingProperty = 
        DependencyProperty.Register("IsLoading", typeof(bool), typeof(BaseToast), new PropertyMetadata(false));

    public bool IsLoading
    {
        get { return (bool) GetValue(IsLoadingProperty); }
        set { SetValue(IsLoadingProperty, value); }
    } 
    
    // Icon
    public static readonly DependencyProperty IconProperty = 
        DependencyProperty.Register("Icon", typeof(string), typeof(BaseToast), new PropertyMetadata(default(string)));
    
    public string Icon
    {
        get { return (string) GetValue(IconProperty); }
        set { SetValue(IconProperty, value); }
    }
    
    // Is Opened
    public static readonly DependencyProperty IsOpenedProperty = 
        DependencyProperty.Register("IsOpened", typeof(bool), typeof(BaseToast), new PropertyMetadata(false));

    public bool IsOpened
    {
        get { return (bool) GetValue(IsOpenedProperty); }
        set { SetValue(IsOpenedProperty, value); }
    }
}