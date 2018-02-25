using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : Window
    {

        public Preview(int ind, DrawInfo s1, DrawInfo s2)
        {
            InitializeComponent();
            this.Show();

            GeometryGroup sectionOutline = new GeometryGroup();
            GeometryGroup rebar = new GeometryGroup();
            GeometryGroup dimline = new GeometryGroup();
            //DrawingGroup sectgroup = new DrawingGroup();
            GeometryDrawing sectionDrawing = new GeometryDrawing();
            Label secName = new Label();

            SetBackground(prCanvas);

            Path outlinePath = new Path();
            outlinePath.Stroke = new SolidColorBrush(Colors.Black);


            if (ind == 0)
            {
                secName.Content = "Section 1";
                if (s1.isRectangle)
                {
                    double W = s1.B;
                    double H = s1.H;
                    RectangleGeometry rsec = DrawRectangle(H, W);
                    sectionOutline.Children.Add(rsec);
                }
                else
                {
                    double D = s1.D;
                    EllipseGeometry csec = DrawCircle(D);
                    sectionOutline.Children.Add(csec);
                }
                
            }
            else if (ind == 1)
            {
                secName.Content = "Section 2";
                if (s2.isRectangle)
                {
                    double W = s2.B;
                    double H = s2.H;
                    RectangleGeometry rsec = DrawRectangle(H, W);
                    sectionOutline.Children.Add(rsec);
                }
                else
                {
                    double D = s2.D;
                    EllipseGeometry csec = DrawCircle(D);
                    sectionOutline.Children.Add(csec);
                }
            }
            else Console.WriteLine("Not Good");

            Outline.Data = sectionOutline;
        }

        void Preview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //StretchVbox();
            ScaleTransform myScale = new ScaleTransform();
            TransformGroup myTransform = new TransformGroup();
            myTransform.Children.Add(myScale);
            myScale.ScaleX = this.ActualWidth/500;
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

        private void SetBackground(Canvas can)
    {
        LinearGradientBrush bkgr = new LinearGradientBrush();
        bkgr.StartPoint = new Point(0, 0);
        bkgr.EndPoint = new Point(0, 1);
        GradientStop whiteGS = new GradientStop();
        whiteGS.Color = Colors.White;
        whiteGS.Offset = 0.0;
        bkgr.GradientStops.Add(whiteGS);
        GradientStop grayGS = new GradientStop();
        grayGS.Color = Colors.LightGray;
        grayGS.Offset = 1.0;
        bkgr.GradientStops.Add(grayGS);

        can.Background = bkgr;
    }

        private RectangleGeometry DrawRectangle(double B, double H)
        {

            double aH = prCanvas.ActualHeight;
            double aB = prCanvas.ActualWidth;
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
            myRect1.X = prCanvas.ActualHeight/2 - trH/2;
            myRect1.Y = prCanvas.ActualWidth/2 - trB/2;
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
