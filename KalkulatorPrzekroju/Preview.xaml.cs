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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : Window
    {
        public Preview(int ind/*, Section s1, Section s2*/)
        {
            InitializeComponent();
            this.Show();
            if (ind == 0) { Console.WriteLine("Section 1"); }
            else if (ind == 1) { Console.WriteLine("Section 2"); }
            else Console.WriteLine("Not Good");

            SetBackground();
            double W = 6000;
            double H = 4400;
            DrawRectangle(H, W);

            //DrawReinforcement()
            this.SizeChanged += new SizeChangedEventHandler(Preview_SizeChanged);

        }

        private void SetBackground()
        {
            LinearGradientBrush bkground = new LinearGradientBrush();
            bkground.StartPoint = new Point(0,0);
            bkground.EndPoint = new Point(0, 1);
            GradientStop whiteGS = new GradientStop();
            whiteGS.Color = Colors.White;
            whiteGS.Offset = 0.0;
            bkground.GradientStops.Add(whiteGS);
            GradientStop grayGS = new GradientStop();
            grayGS.Color = Colors.LightGray;
            grayGS.Offset = 1.0;
            bkground.GradientStops.Add(grayGS);

            PreviewWindow.Background = bkground;
        }

        void Preview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            {
                double hDelta = e.NewSize.Width / e.PreviousSize.Width;
                double vDelta = e.NewSize.Height / e.PreviousSize.Height;
                double Delta;
                if (hDelta < 1 || vDelta <1)
                {
                    Delta = Math.Min(hDelta, vDelta);
                }
                else
                {
                    Delta = Math.Max(hDelta, vDelta);
                }

                if (double.IsInfinity(hDelta) || double.IsInfinity(vDelta)) return;

                foreach (FrameworkElement child in PreviewWindow.Children)
                {
                    child.Height *= Delta;
                    child.Width *= Delta;
                }
            }

        }


        private void DrawRectangle(double H, double B)
        {
            double aH = PreviewWindow.ActualHeight;
            double aB = PreviewWindow.ActualWidth;

            double trH;
            double trB;

            if (H/aH > B/aB)
            {
                trH = 0.8 * aH;
                trB = 0.8 * (aH / H) * B;
            }
            else
            {
                trB = 0.8 * aB;
                trH = 0.8 * (aB / B) * H;
            }

            Rectangle rsec = new Rectangle();
            rsec.Height = trH;
            rsec.Width = trB;
            rsec.Stroke = new SolidColorBrush(Colors.Black);
            rsec.StrokeThickness = 1;
            rsec.Fill = new SolidColorBrush(Colors.LightGoldenrodYellow);
            PreviewWindow.Children.Add(rsec);
            Canvas.SetTop(rsec, 50);
            Canvas.SetLeft(rsec, 50);
        }
    }
}
