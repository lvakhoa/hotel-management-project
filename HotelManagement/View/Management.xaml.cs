using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HotelManagement.ViewModel;
using HotelManagement.ViewModel.ManagementList;
using ButtonUI = Wpf.Ui.Controls.Button;

namespace HotelManagement.View;

public partial class Management : UserControl
{
    public Management()
    {
        InitializeComponent();
    }
    
    private void Management_OnLoaded(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        ButtonUI btn = (NavStack.Children[0] as ButtonUI)!;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";
    }

    private void RemoveBackgroundBtn()
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("SecondaryButtonBackground");
        var e = NavStack.Children.GetEnumerator();
        while (e.MoveNext())
        {
            var currentBtn = (e.Current as ButtonUI);
            if (currentBtn!.Name == "ActiveBtn")
            {
                currentBtn!.Background = dynamicBrush;
                currentBtn!.Name = "";
                break;
            }
        }
    }

    private void StaffBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";   
    }

    private void CustomerBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";   
    }

    private void RoomBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";   
    }

    private void ServiceBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";   
    }

    private void InvoiceBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";   
    }

    private void BookingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";   
    }

    private void RoomTypeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";   
    }

    private void ServiceUseBtn_OnClick(object sender, RoutedEventArgs e)
    {
        SolidColorBrush dynamicBrush = (SolidColorBrush)FindResource("ButtonBackground");
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = dynamicBrush;
        btn.Name = "ActiveBtn";
    }

}