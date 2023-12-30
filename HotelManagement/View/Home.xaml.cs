using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Controls;
namespace HotelManagement.View;

public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
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