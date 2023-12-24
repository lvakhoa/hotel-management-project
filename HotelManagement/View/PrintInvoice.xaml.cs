using System.Windows;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using HotelManagement.Model;
using MaterialDesignThemes.Wpf;
using Wpf.Ui.Controls;

namespace HotelManagement.View;

public partial class PrintInvoice : Window
{
    public PrintInvoice()
    {
        InitializeComponent();
    }

    private void CancelButton_OnClick(object sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void PrintButton_OnClick(object sender, RoutedEventArgs e)
    {
        PrintDialog printDlg = new System.Windows.Controls.PrintDialog();
        if (printDlg.ShowDialog() == true)
        {
            System.Printing.PrintCapabilities capabilities = printDlg.PrintQueue.GetPrintCapabilities(printDlg.PrintTicket);
            double scale = Math.Min(capabilities.PageImageableArea.ExtentWidth / this.ActualWidth, capabilities.PageImageableArea.ExtentHeight / this.ActualHeight);
            this.LayoutTransform = new ScaleTransform(scale, scale);
            Size sz = new Size(capabilities.PageImageableArea.ExtentWidth, capabilities.PageImageableArea.ExtentHeight);
            this.Measure(sz);
            this.Arrange(new Rect(new Point(capabilities.PageImageableArea.OriginWidth, capabilities.PageImageableArea.OriginHeight), sz));
            printDlg.PrintVisual(InvoiceViewer, "First Fit to Page WPF Print");
        }
    }
}