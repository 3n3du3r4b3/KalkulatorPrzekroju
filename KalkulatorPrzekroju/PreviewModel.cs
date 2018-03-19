using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows;

namespace KalkulatorPrzekroju
{
    class PreviewModel
    {

        public DrawInfo[] sec = new DrawInfo[2];
        //public GeometryGroup[] dimlineh = new GeometryGroup[2];
        public int index;

        public PreviewModel(int ind, DrawInfo section1, DrawInfo section2)
        {
            index = ind;
            GeometryGroup sectionOutline = new GeometryGroup();
            GeometryGroup Rebar = new GeometryGroup();
            GeometryGroup dimlinev = new GeometryGroup();
            GeometryGroup dimlineh = new GeometryGroup();
            GeometryDrawing sectionDrawing = new GeometryDrawing();
            Label secName = new Label();
            secName.Content = String.Format("Section {0}", index + 1);
            sec[0] = section1;
            sec[1] = section2;

            //Outline.Data = sectionOutline;
        }
    }
}
