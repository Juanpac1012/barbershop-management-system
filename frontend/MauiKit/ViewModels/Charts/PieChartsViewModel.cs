using MauiKit.Models;
using System;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.Measure;

using MauiKit.Controls.Charts;
using LiveChartsCore.SkiaSharpView.Extensions;

namespace MauiKit.ViewModels;
public partial class PieChartsViewModel : ObservableObject
{

    public PieChartsViewModel()
    {
        LoadData();
    }

    //Basic Pie
    private static int _index = 0;
    private static string[] _names = new[] { "Maria", "Susan", "Charles", "Fiona", "George" };
    
    public void LoadData()
    {
        //Basic Pie
        BasicPieSeries = new[] { 8, 6, 5, 3, 3 }.AsPieSeries();

        //Donut
        DonutPieSeries = new ISeries[]
        {
            new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 80 },
            new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 80 }
        };

        //Custom
        CustomPieSeries = new ISeries[]
        {
            new PieSeries<double> { Values = new List<double> { 4 }, OuterRadiusOffset = 0.60 },
            new PieSeries<double> { Values = new List<double> { 5 }, OuterRadiusOffset = 0.65 },
            new PieSeries<double> { Values = new List<double> { 3 }, OuterRadiusOffset = 0.70 },
            new PieSeries<double> { Values = new List<double> { 5 }, OuterRadiusOffset = 0.85 },
            new PieSeries<double> { Values = new List<double> { 7 }, OuterRadiusOffset = 1.00 },
        };

        //Nightingale Rose
        NightingaleRosePieSeries = new ISeries[]
        {
            new PieSeries<double> { Values = new List<double> { 2 }, InnerRadius = 50, OuterRadiusOffset = 1.0 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, OuterRadiusOffset = 0.9 },
            new PieSeries<double> { Values = new List<double> { 1 }, InnerRadius = 50, OuterRadiusOffset = 0.8 },
            new PieSeries<double> { Values = new List<double> { 4 }, InnerRadius = 50, OuterRadiusOffset = 0.7 },
            new PieSeries<double> { Values = new List<double> { 3 }, InnerRadius = 50, OuterRadiusOffset = 0.6 }
        };

        //Basic Gauge
        BasicGaugePieSeries = new GaugeBuilder()
        .WithMaxColumnWidth(30)
        .AddValue(30)
        .BuildSeries();

        //270 Degrees Gauge
        DegreesGaugePieSeries = new GaugeBuilder()
        .WithLabelsSize(50)
        .WithInnerRadius(100)
        .WithBackgroundInnerRadius(100)
        .WithBackground(new SolidColorPaint(new SKColor(100, 181, 246, 90)))
        .WithLabelsPosition(PolarLabelsPosition.ChartCenter)
        .AddValue(30, "gauge value", SKColors.Green, SKColors.Red) // defines the value and the color 
        .BuildSeries();

        //Multiple Values Gauge
        MultipleValuesGaugePieSeries = new GaugeBuilder()
        .WithLabelsSize(20)
        .WithLabelsPosition(PolarLabelsPosition.Start)
        .WithLabelFormatter(point => $"{point.PrimaryValue} {point.Context.Series.Name}")
        .WithInnerRadius(20)
        .WithOffsetRadius(8)
        .WithBackgroundInnerRadius(20)

        .AddValue(30, "Vanessa")
        .AddValue(50, "Charles")
        .AddValue(70, "Ana")

        .BuildSeries();

        //Slim Gauge
        SlimGaugePieSeries = new GaugeBuilder()
        .WithLabelsSize(20)
        .WithLabelsPosition(PolarLabelsPosition.End)
        .WithLabelFormatter(point => point.PrimaryValue.ToString())
        .WithInnerRadius(20)
        .WithMaxColumnWidth(5)
        .WithBackground(null)

        .AddValue(50, "Vanessa")
        .AddValue(80, "Charles")
        .AddValue(95, "Ana")

        .BuildSeries();

    }

    #region Pie Chart
    public IEnumerable<ISeries> BasicPieSeries { get; set; }
    public ISeries[] DonutPieSeries { get; set; }
    public ISeries[] CustomPieSeries { get; set; }
    public ISeries[] NightingaleRosePieSeries { get; set; }
    public IEnumerable<ISeries> BasicGaugePieSeries { get; set; }
    public IEnumerable<ISeries> DegreesGaugePieSeries { get; set; }
    public IEnumerable<ISeries> MultipleValuesGaugePieSeries { get; set; }
    public IEnumerable<ISeries> SlimGaugePieSeries { get; set; }

    #endregion Pie Chart
}
