using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OxyPlot;
using System.Windows.Media;

namespace KalkulatorPrzekroju
{
    [Serializable]
    public class MyColor
    {
        public byte R { get; private set; }
        public byte G { get; private set; }
        public byte B { get; private set; }
        public byte A { get; private set; }

        public MyColor(OxyColor oxyColor)
        {
            this.A = oxyColor.A;
            this.B = oxyColor.B;
            this.G = oxyColor.G;
            this.R = oxyColor.R;
        }

        public MyColor(Color color)
        {
            this.A = color.A;
            this.B = color.B;
            this.G = color.G;
            this.R = color.R;
        }

        public OxyColor GetOxy()
        {
            return OxyColor.FromArgb(A, R, G, B);
        }

        public Color GetMedia()
        {
            return Color.FromArgb(A, R, G, B);
        }
    }
}
