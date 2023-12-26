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
    public Home()
    {
        InitializeComponent();
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

        Pointlabel = chartpoint => string.Format("{0}({1:p})", chartpoint.Y, chartpoint.Participation);

        DataContext = this;

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

