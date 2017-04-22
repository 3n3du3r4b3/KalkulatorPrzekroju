using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace KalkulatorPrzekroju
{
    class MainPlotView
    {
        public OxyColor lineColor1 { get; set; }
        public OxyColor lineColor2 { get; set; }
        public OxyColor pointsColor { get; set; }
        public double lineThickness1 { get; set; }
        public double lineThickness2 { get; set; }
        public double pointSize { get; set; }
        public double[][] dataPoints { private get; set; }

        public PlotModel wykres { get; private set; }

        public MainPlotView()
        {
            wykres = new PlotModel();
            wykres.LegendPosition = LegendPosition.BottomLeft;
        }

        public void AddLineSerie(double[][] dataPoints, string name)
        {
            LineSeries punkty = new LineSeries();
            punkty.Title = name;

            for (int i = 0; i < dataPoints.Length; i++)
            {
                punkty.Points.Add(new DataPoint(dataPoints[i][1], dataPoints[i][0]));
            }

            wykres.Series.Add(punkty);
        }

        public void AddPointSerie(double[][] dataPoints, string name)
        {
            ScatterSeries punkty = new ScatterSeries();
            punkty.Title = name;

            for (int i = 0; i < dataPoints.Length; i++)
            {
                punkty.Points.Add(new ScatterPoint(dataPoints[i][1], dataPoints[i][0]));
            }

            wykres.Series.Add(punkty);
        }

        public void RemoveSerie(string name)
        {
            Series seriaToDelete = null;
            foreach (Series seria in wykres.Series)
            {
                if (seria.Title == name)
                {
                    seriaToDelete = seria;
                }
            }
            wykres.Series.Remove(seriaToDelete);
        }

        public void SetGraph()
        {
            //wykres.Axes.Add(new OxyPlot.Axes.LinearAxis());
            //wykres.Axes.Add(new OxyPlot.Axes.LinearAxis());
            wykres.Axes[0].PositionAtZeroCrossing = true;
            wykres.Axes[1].PositionAtZeroCrossing = true;
            wykres.Axes[0].AxislineStyle = LineStyle.Solid;
            wykres.Axes[1].AxislineStyle = LineStyle.Solid;
            wykres.Axes[0].Layer = OxyPlot.Axes.AxisLayer.AboveSeries;
            wykres.Axes[1].Layer = OxyPlot.Axes.AxisLayer.AboveSeries;
            }
    }
}
