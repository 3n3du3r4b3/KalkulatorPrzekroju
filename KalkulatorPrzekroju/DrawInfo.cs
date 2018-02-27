using System;
using System.Runtime.Serialization.Formatters.Binary;

namespace KalkulatorPrzekroju
{
    [Serializable]
    public class DrawInfo
    {
        public double H;
        public double B;
        public double D;
        public bool isRectangle;
        public bool bySpacing;

        public DrawInfo(double H, double B, bool spac)
        {
            this.H = H;
            this.B = B;
            isRectangle = true;
            bySpacing = spac;
        }

        public DrawInfo(double D)
        {
            this.D = D;
            isRectangle = false;
            bySpacing = false;
        }
    }
}