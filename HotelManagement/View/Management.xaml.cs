using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HotelManagement.ViewModel;
using HotelManagement.ViewModel.ManagementList;
using DefaultDatagrid = HotelManagement.CustomControls.DataGrid;
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
        ButtonUI btn = (NavStack.Children[0] as ButtonUI)!;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";
    }

    private void RemoveBackgroundBtn()
    {
        var e = NavStack.Children.GetEnumerator();
        while (e.MoveNext())
        {
            var currentBtn = (e.Current as ButtonUI);
            if (currentBtn!.Name == "ActiveBtn")
            {
                currentBtn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#C0C0C0")!;
                currentBtn!.Name = "";
                break;
            }
        }
    }

    private void StaffBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";   
    }

    private void CustomerBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";   
    }

    private void RoomBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";   
    }

    private void ServiceBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";   
    }

    private void InvoiceBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";   
    }

    private void BookingBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";   
    }

    private void RoomTypeBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";   
    }

    private void ServiceUseBtn_OnClick(object sender, RoutedEventArgs e)
    {
        RemoveBackgroundBtn();
        ButtonUI? btn = sender as ButtonUI;
        btn!.Background = (Brush)(new BrushConverter()).ConvertFrom("#4F4A4A")!;
        btn.Name = "ActiveBtn";
    }

}