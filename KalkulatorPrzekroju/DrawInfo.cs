using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Media;

namespace KalkulatorPrzekroju
{
    [Serializable]
    public class DrawInfo
    {
        public double H;
        public double B;
        public double D;
        public GeometryGroup Shape;
        public bool isRectangle;
        public bool bySpacing;


        public DrawInfo()
        {
            isRectangle = false;
            bySpacing = false;
        }
        public DrawInfo(double H, double B, bool spac)
        {
            this.H = H;
            this.B = B;
            isRectangle = true;
            bySpacing = spac;
            this.Shape = CreateShape(H, B, 0, true);
        }

        public DrawInfo(double D)
        {
            this.D = D;
            isRectangle = false;
            bySpacing = false;
            this.Shape = CreateShape(0, 0, D, false);
        }

        public GeometryGroup CreateShape(double H, double B, double D, bool isR)
        {
            GeometryGroup aaa = new GeometryGroup();
            if (isR)
            {
                RectangleGeometry aa = new RectangleGeometry();
                aa.Rect = new Rect(0, 0, H, B);
                aaa.Children.Add(aa);
            }
            else
            {
                EllipseGeometry bb = new EllipseGeometry();
                bb.Center = new Point(D/2, D / 2);
                bb.RadiusX = D / 2;
                bb.RadiusY = D / 2;
                aaa.Children.Add(bb);
            }
            return aaa;
        }
    }
}