using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using OxyPlot.Series;

namespace KalkulatorPrzekroju
{
    public class Model_ULS_MN
    {
        public Model_ULS_MN()
        {
            this.MyModel = new PlotModel();
            
            //this.MyModel.Series.Add();
        }

        public PlotModel MyModel { get; private set; }
    }
}
