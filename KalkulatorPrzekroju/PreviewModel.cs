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
        public int index;

        public PreviewModel(int ind, DrawInfo section1, DrawInfo section2)
        {
            index = ind;
            GeometryGroup sectionOutline = new GeometryGroup();
            GeometryGroup Rebar = new GeometryGroup();
            GeometryGroup dimline = new GeometryGroup();
            GeometryDrawing sectionDrawing = new GeometryDrawing();
            Label secName = new Label();
            secName.Content = String.Format("Section {0}", index + 1);
            sec[0] = section1;
            sec[1] = section2;

            //Outline.Data = sectionOutline;
        }
        /*
                void Preview_SizeChanged(object sender, SizeChangedEventArgs e)
                {
                    //StretchVbox();
                    ScaleTransform myScale = new ScaleTransform();
                    TransformGroup myTransform = new TransformGroup();
                    myTransform.Children.Add(myScale);
                    myScale.ScaleX = this.ActualWidth / 500;
                    myScale.ScaleY = myScale.ScaleX;
                    //myGrid.RenderTransform = myTransform;
                }

                private void StretchVbox()
                {
                    Viewbox Outline = new Viewbox();
                    Outline.StretchDirection = StretchDirection.Both;
                    Outline.Stretch = Stretch.UniformToFill;
                    Outline.MaxWidth = 1200;
                    Outline.MaxHeight = 1200;
                }

                */

        
            /*
        private RectangleGeometry DrawRectangle(double B, double H, double aH, double aB)
        {

            double scale = 0.8;

            double trH;
            double trB;

            if (H / aH > B / aB)
            {
                trH = scale * aH;
                trB = scale * (aH / H) * B;
            }
            else
            {
                trB = scale * aB;
                trH = scale * (aB / B) * H;
            }

            Rect myRect1 = new Rect();
            myRect1.X = aH / 2 - trH / 2;
            myRect1.Y = aB / 2 - trB / 2;
            myRect1.Width = trH;
            myRect1.Height = trB;
            RectangleGeometry rsec = new RectangleGeometry();
            rsec.Rect = myRect1;
            return rsec;
        }
        
        private EllipseGeometry DrawCircle(double D)
        {
            double aH = prCanvas.ActualHeight;
            double aB = prCanvas.ActualWidth;
            double scale = 0.8;

            double trH;
            double trB;

            if (D / aH > D / aB)
            {
                trH = scale * aH;
                trB = scale * (aH / D) * D;
            }
            else
            {
                trB = scale * aB;
                trH = scale * (aB / D) * D;
            }

            Rect myRect1 = new Rect();
            myRect1.X = prCanvas.ActualHeight / 2 - trH / 2;
            myRect1.Y = prCanvas.ActualWidth / 2 - trB / 2;
            myRect1.Width = trH;
            myRect1.Height = trB;
            EllipseGeometry csec = new EllipseGeometry(myRect1);
            return csec;
        }
        */

        private class DimLine
        {
            public DimLine(double H, double B)
            {
            }
        }

        private class Reinforcement
        {
            public Reinforcement(double H, double B)
            {
            }
        }
    }
}
