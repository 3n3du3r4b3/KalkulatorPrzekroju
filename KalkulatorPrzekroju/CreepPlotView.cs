using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;
using System.Windows.Media;

namespace KalkulatorPrzekroju
{
    class CreepPlotView
    {
        public OxyColor lineColor1 { get; set; }
        public OxyColor pointsColor { get; set; }
        public double lineThickness1 { get; set; }
        public double lineThickness2 { get; set; }
        public double pointSize { get; set; }
        public double[][] dataPoints { private get; set; }

        public PlotModel wykres { get; private set; }

        public CreepPlotView()
        {
            wykres = new PlotModel();
            wykres.LegendPosition = LegendPosition.BottomCenter;
            wykres.Axes.Clear();
            wykres.Axes.Add(new OxyPlot.Axes.LinearAxis());
            wykres.Axes.Add(new OxyPlot.Axes.LinearAxis());
            wykres.Axes[0].Position = OxyPlot.Axes.AxisPosition.Bottom;
            wykres.Axes[1].Position = OxyPlot.Axes.AxisPosition.Left;
            wykres.Axes[0].PositionAtZeroCrossing = true;
            wykres.Axes[1].PositionAtZeroCrossing = true;
            wykres.Axes[0].AxislineStyle = LineStyle.Automatic;
            wykres.Axes[1].AxislineStyle = LineStyle.Automatic;
        }

        public void AddLineSerie(double[][] dataPoints, string name,Color color, double size)
        {
            LineSeries punkty = new LineSeries();
            punkty.Title = name;
            
            for (int i = 0; i < dataPoints.Length; i++)
            {
                punkty.Points.Add(new DataPoint(dataPoints[i][1], dataPoints[i][0]));
            }
            punkty.Color = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            punkty.StrokeThickness = size;
            wykres.Series.Add(punkty);
        }

        public void AddPointSerie(List<CasePoint> dataPoints, string name, Color color, double size)
        {
            ScatterSeries punkty = new ScatterSeries();
            punkty.Title = name;

            for (int i = 0; i < dataPoints.Count; i++)
            {
                punkty.Points.Add(new ScatterPoint(dataPoints[i].X, dataPoints[i].Y));
            }
            punkty.MarkerStroke = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            punkty.MarkerSize = size;
            punkty.MarkerType = MarkerType.Circle;
            punkty.MarkerFill = OxyColor.FromArgb(color.A, color.R, color.G, color.B);
            wykres.Series.Add(punkty);
        }

        public void RemoveSerie(string name)
        {
            Series seriaToDelete = null;
            if (wykres.Series.Count > 0)
            {
                foreach (Series seria in wykres.Series)
                {
                    if (seria.Title == name)
                    {
                        seriaToDelete = seria;
                        wykres.Series.Remove(seriaToDelete);
                        return;
                    }
                }
            }
        }
    }
}
