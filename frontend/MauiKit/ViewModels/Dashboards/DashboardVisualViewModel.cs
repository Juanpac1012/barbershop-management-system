using MauiKit.Models;
using System;

using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.Measure;

using MauiKit.Controls.Charts;

namespace MauiKit.ViewModels;
public partial class DashboardVisualViewModel : ObservableObject
{
    public DashboardVisualViewModel()
    {

        // you could convert any IEnumerable to a pie series collection
        var data = new double[] { 2, 4, 1, 4, 3 };

        // Series = data.AsLiveChartsPieSeries(); this could be enough in some cases // mark
        // but you can customize the series properties using the following overload: // mark

        PieSeries = data.AsLiveChartsPieSeries((value, series) =>
        {
            // here you can configure the series assigned to each value.
            series.Name = $"Series for value {value}";
            series.DataLabelsPaint = new SolidColorPaint(new SKColor(30, 30, 30));
            series.DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer;
            series.DataLabelsFormatter = p => $"{p.PrimaryValue} / {p.StackedValue!.Total} ({p.StackedValue.Share:P2})";
        });

        // this is an equivalent and more verbose syntax. // mark
        // Series = new ISeries[]
        // {
        //     new PieSeries<double> { Values = new double[] { 2 }, Name = "Slice 1" },
        //     new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 2" },
        //     new PieSeries<double> { Values = new double[] { 1 }, Name = "Slice 3" },
        //     new PieSeries<double> { Values = new double[] { 4 }, Name = "Slice 4" },
        //     new PieSeries<double> { Values = new double[] { 3 }, Name = "Slice 5" }
        // };

        
    }

    //public IEnumerable<ChartEntry> ChartEntries { get; set; }
    public ChartEntry[] ChartEntries { get; set; } =
    {
        new ChartEntry
        {
            Value = 71,
            Color = Color.FromArgb("#6023FF"),
            Text = "Visual Studio Code"
        },
        new ChartEntry
        {
            Value = 33,
            Color = Color.FromArgb("#3059FE"),
            Text = "Visual Studio"
        },
        new ChartEntry
        {
            Value = 29,
            Color = Color.FromArgb("#2EF1D2"),
            Text = "Notepad++"
        },
        new ChartEntry
        {
            Value = 28,
            Color = Color.FromArgb("#F8426E"),
            Text = "IntelliJ"
        }
    };


    #region TotalUsers Chart
    public ISeries[] TotalUserSeries { get; set; } =
    {
        new ColumnSeries<double>
        {
            IsHoverable = false, // disables the series from the tooltips 
            Values = new double[] { 10, 10, 10, 10, 10, 10, 10 },
            Stroke = null,
            Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
            IgnoresBarPosition = true
        },
        new ColumnSeries<double>
        {
            Values = new double[] { 3, 10, 5, 3, 7, 3, 8 },
            Stroke = null,
            Fill = new SolidColorPaint(SKColors.CornflowerBlue),
            IgnoresBarPosition = true
        }
    };

    public Axis[] YAxes { get; set; } =
    {
        new Axis { MinLimit = 0, MaxLimit = 10 }
    };

    public DrawMarginFrame Frame { get; set; } =
    new()
    {
        Fill = new SolidColorPaint
        {
            Color = new SKColor(0, 0, 0, 30)
        },
        Stroke = new SolidColorPaint
        {
            Color = new SKColor(80, 80, 80),
            StrokeThickness = 2
        }
    };
    #endregion TotalUsers Chart


    #region LineSmoothies Chart
    public ISeries[] SmoothLineSeries { get; set; } =
    {
        new LineSeries<double>
        {
            Values = new double[] { 5, 0, 5, 0, 5, 0 },
            Fill = null,
            GeometrySize = 0,
            // use the line smoothness property to control the curve
            // it goes from 0 to 1
            // where 0 is a straight line and 1 the most curved
            LineSmoothness = 0
        },
        new LineSeries<double>
        {
            Values = new double[] { 7, 2, 7, 2, 7, 2 },
            Fill = null,
            GeometrySize = 0,
            LineSmoothness = 1
        }
    };
    #endregion LineSmoothies Chart

    #region Pie Chart
    public IEnumerable<ISeries> PieSeries { get; set; }
    #endregion Pie Chart
}
