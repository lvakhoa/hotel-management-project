using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;
using System.Windows.Media;
namespace HotelManagement.View;

public partial class Home : UserControl
{
    public SeriesCollection SeriesCollection { get; set; }
    public string[] Labels { get; set; }

    public Func<ChartPoint, string> Pointlabel { get; set; }
    public SeriesCollection PieSeriesCollection { get; set; }

    public Home()
    {
        Pointlabel = chartpoint => string.Format("{0}({1:p})", chartpoint.Y, chartpoint.Participation);
        InitializeComponent();
        //DataContext = new HomeVM();
        SeriesCollection = new SeriesCollection
        {
            new ColumnSeries
            {
                Title = "Checkin",
                Values = new ChartValues<int> { 10, 50, 39, 50, 12, 30, 20 },
                   Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#66CDAA"))
            },
            new ColumnSeries
            {
                Title = "Checkout",
                Values = new ChartValues<int> { 7, 6, 9, 10, 11, 5, 8 },

                Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFCCCC"))
            }
        };
        Labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        PieSeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Values = new ChartValues<int> { 3 },
                    Title = "Available",
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#E4FFE0")),
                    DataLabels = true,
                    LabelPoint = Pointlabel
                },
                new PieSeries
                {
                    Values = new ChartValues<int> { 3 },
                    Title = "Blocked",
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFE0FC")),
                    DataLabels = true,
                    LabelPoint = Pointlabel
                },
                new PieSeries
                {
                    Values = new ChartValues<int> { 3 },
                    Title = "Occupied",
                    Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom("#F8EFE2")),
                    DataLabels = true,
                    LabelPoint = Pointlabel
                }
            };



    }

    private void PieChart_DataClick_1(object sender, ChartPoint chartPoint)
    {
        var chart = (LiveCharts.Wpf.PieChart)chartPoint.ChartView;
        foreach (PieSeries series in chart.Series)
        {
            series.PushOut = 0;
        }
        var selectedSeries = (PieSeries)chartPoint.SeriesView;
        selectedSeries.PushOut = 8;
    }

}

