using System.Windows;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using HotelManagement.Model;
using HotelManagement.ViewModel.ManagementList;
using MaterialDesignThemes.Wpf;
using Wpf.Ui.Controls;

namespace HotelManagement.View;

public partial class PrintInvoice : Window
{
    public PrintInvoice(string? id, object dataContext)
    {
        InitializeComponent();
        
        DataContext = dataContext;

        if (id != null)
            (DataContext as InvoiceList).GetInvoiceById(id);
        
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void Print_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void PrintButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrintDialog printDlg = new PrintDialog();
        if (printDlg.ShowDialog() == true)
        {
            PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);
            double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth,
                capabilities.PageImageableArea.ExtentHeight / this.ActualHeight);
            this.LayoutTransform = new ScaleTransform(scale, scale);
            Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            this.Measure(sz);
            this.Arrange(new Rect(
                new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight),
                sz));
            this.Close();
            printDlg.PrintVisual(InvoiceViewer, "Print");
        }
    }
}