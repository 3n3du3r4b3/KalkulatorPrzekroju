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

        public MainPlotView()
        {

        }
    }
}
