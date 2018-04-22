using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;

namespace KalkulatorPrzekroju
{
    class PreviewViewModel : INotifyPropertyChanged
    {
        public Brush GradientBackground
        {
            set
            {

            }
            get
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
                return bkgr;
            }
        }

        private StreamGeometry Arrow(Point p1, Point p2, Point p3)
        {
            StreamGeometry streamGeometry = new StreamGeometry();
            using (StreamGeometryContext geometryContext = streamGeometry.Open())
            {
                geometryContext.BeginFigure(p1, true, true);
                PointCollection points = new PointCollection { p2, p3 };
                geometryContext.PolyLineTo(points, true, true);
            }

            streamGeometry.Freeze();

            return streamGeometry;
        }

        public GeometryGroup DimLineV(double x1, double x2, double y1, double y2, double offset, double size)
        {

            {
                GeometryGroup dimv = new GeometryGroup();
                double extoffset = 30;
                double dimoffset = 0;
                Point ext11 = new Point(x1 + extoffset, y1);
                Point ext12 = new Point(x1 + offset + extoffset, y1);
                Point ext21 = new Point(x2 + extoffset, y2);
                Point ext22 = new Point(x2 + offset + extoffset, y2);
                Point dim1 = new Point(x1 + offset, y1 - dimoffset);
                Point dimp11 = new Point(x1 + offset + 6, y1 - dimoffset - 30);
                Point dimp12 = new Point(x1 + offset - 6, y1 - dimoffset - 30);
                Point dim2 = new Point(x2 + offset, y2 + dimoffset);
                Point dimp21 = new Point(x2 + offset + 6, y2 + dimoffset + 30);
                Point dimp22 = new Point(x2 + offset - 6, y2 + dimoffset + 30);
                StreamGeometry topArrow = Arrow(dim1, dimp11, dimp12);
                StreamGeometry botArrow = Arrow(dim2, dimp21, dimp22);
                LineGeometry extl1 = new LineGeometry(ext11, ext12);
                LineGeometry extl2 = new LineGeometry(ext21, ext22);
                LineGeometry dim = new LineGeometry(dim1, dim2);
                dimv.Children.Add(extl1);
                dimv.Children.Add(extl2);
                dimv.Children.Add(dim);
                dimv.Children.Add(topArrow);
                dimv.Children.Add(botArrow);
                return dimv;
            }
        }

        public GeometryGroup DimLineL(double x1, double x2, double y1, double y2, double offset, double size)
        {

            {
                GeometryGroup dimv = new GeometryGroup();
                double extoffset = 30;
                double dimoffset = 0;
                Point ext11 = new Point(x1, y1 + extoffset);
                Point ext12 = new Point(x1, y1 + offset + extoffset);
                Point ext21 = new Point(x2, y2 + extoffset);
                Point ext22 = new Point(x2, y2 + offset + extoffset);
                Point dim1 = new Point(x1 - dimoffset, y1  + offset);
                Point dimp11 = new Point(x1 - dimoffset + 30, y1 + offset + 6);
                Point dimp12 = new Point(x1 - dimoffset + 30, y1 + offset - 6);
                Point dim2 = new Point(x2 - dimoffset, y2 + offset);
                Point dimp21 = new Point(x2 - dimoffset - 30, y2 + offset + 6);
                Point dimp22 = new Point(x2 - dimoffset - 30, y2 + offset - 6);
                StreamGeometry topArrow = Arrow(dim1, dimp11, dimp12);
                StreamGeometry botArrow = Arrow(dim2, dimp21, dimp22);
                LineGeometry extl1 = new LineGeometry(ext11, ext12);
                LineGeometry extl2 = new LineGeometry(ext21, ext22);
                LineGeometry dim = new LineGeometry(dim1, dim2);
                dimv.Children.Add(extl1);
                dimv.Children.Add(extl2);
                dimv.Children.Add(dim);
                dimv.Children.Add(topArrow);
                dimv.Children.Add(botArrow);
                return dimv;
            }
        }

        private GeometryGroup _rebar;
        public GeometryGroup Rebar
        {
            set
            {
                _rebar = value;
                NotifyPropertyChanged();
            }
            get
            {
                return _rebar;
            }
        }

        private double _aH;
        public double AH
        {
            get { return _aH; }
            set
            {
                _aH = value;
                NotifyPropertyChanged();
            }
        }

        private double _aW;
        public double AW
        {
            get { return _aW; }
            set
            {
                _aW = value;
                NotifyPropertyChanged();
            }
        }

        private double _SCAH;
        public double SCAH
        {
            get { return _SCAH; }
            set
            {
                _SCAH = value;
                NotifyPropertyChanged();
            }
        }

        private double _SCAW;
        public double SCAW
        {
            get { return _SCAW; }
            set
            {
                _SCAW = value;
                NotifyPropertyChanged();
            }
        }

        private double _ldimvx;
        public double LDimvX
        {
            get { return _ldimvx; }
            set
            {
                _ldimvx = value;
                NotifyPropertyChanged();
            }
        }

        private double _ldimvy;
        public double LDimvY
        {
            get { return _ldimvy; }
            set
            {
                _ldimvy = value;
                NotifyPropertyChanged();
            }
        }

        private double _ldimlx;
        public double LDimlX
        {
            get { return _ldimlx; }
            set
            {
                _ldimlx = value;
                NotifyPropertyChanged();
            }
        }

        private double _ldimly;
        public double LDimlY
        {
            get { return _ldimly; }
            set
            {
                _ldimly = value;
                NotifyPropertyChanged();
            }
        }

        private GeometryGroup _outline;
        public GeometryGroup Outline
        {
            get { return _outline; }
            set
            {
                _outline = value;
                NotifyPropertyChanged();
            }
        }

        private GeometryGroup _dimv;
        public GeometryGroup DimV
        {
            get { return _dimv; }
            set
            {
                _dimv = value;
                NotifyPropertyChanged();
            }
        }

        private GeometryGroup _diml;
        public GeometryGroup DimL
        {
            get { return _diml; }
            set
            {
                _diml = value;
                NotifyPropertyChanged();
            }
        }

        private ScaleTransform _scale1;
        public ScaleTransform Scale1
        {
            get { return _scale1; }
            set
            {
                _scale1 = value;
                NotifyPropertyChanged();
            }
        }

        private TranslateTransform _offset1;
        public TranslateTransform Offset1
        {
            get { return _offset1; }
            set
            {
                _offset1 = value;
                NotifyPropertyChanged();
            }
        }

        private TransformGroup _shapeTransform1;
        public TransformGroup ShapeTransform1
        {
            get { return _shapeTransform1; }
            set
            {
                _shapeTransform1 = value;
                NotifyPropertyChanged();
            }
        }

        private Brush _windowBackground;
        public Brush WindowBackground
        {
            get { return _windowBackground; }
            set
            {
                _windowBackground = value;
                NotifyPropertyChanged();
            }
        }

        private String _ldimv;
        public String LDimv
        {
            get { return _ldimv; }
            set
            {
                _ldimv = value;
                NotifyPropertyChanged();
            }
        }

        private String _ldiml;
        public String LDiml
        {
            get { return _ldiml; }
            set
            {
                _ldiml = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public PreviewViewModel(PreviewModel drs)
        {
            double sc = 0.6;
            AH = 700;
            AW = 700;

            double scf = sc * Math.Max(AH, AW) / drs.sec[drs.index].size;
            double off = -50;// -0.15 * AH * sc * Math.Max(AH, AW) / drs.sec[drs.index].size;

            Scale1 = new ScaleTransform(scf, scf);
            Offset1 = new TranslateTransform(off, off);
            ShapeTransform1 = new TransformGroup();
            WindowBackground = GradientBackground;
            ShapeTransform1.Children.Add(Scale1);
            ShapeTransform1.Children.Add(Offset1);
            Outline = drs.sec[drs.index].Shape;
            Outline.Transform = ShapeTransform1;
            Rebar = drs.sec[drs.index].Reinforcement;
            
            Rebar.Transform = ShapeTransform1;
            DimV = DimLineV(scf * drs.sec[drs.index].bindtop1[0] + off, scf * drs.sec[drs.index].bindtop2[0] + off, scf * drs.sec[drs.index].bindtop1[1] + off, scf * drs.sec[drs.index].bindtop2[1] + off, 100, 1);  
            LDimv = drs.sec[drs.index].vert.ToString();
            LDimvX = scf * (drs.sec[drs.index].bindtop1[0] + drs.sec[drs.index].bindtop2[0]) / 2;
            LDimvY = scf * (drs.sec[drs.index].bindtop1[1] + drs.sec[drs.index].bindtop2[1]) / 2 + 2.25 * off;

            if (drs.sec[drs.index].isRectangle)
            {
                DimL = DimLineL(scf * drs.sec[drs.index].bindbot1[0] + off, scf * drs.sec[drs.index].bindbot2[0] + off, scf * drs.sec[drs.index].bindbot1[1] + off, scf * drs.sec[drs.index].bindbot2[1] + off, 100, 1);
                LDiml = drs.sec[drs.index].hor.ToString();
                LDimlX = scf * (drs.sec[drs.index].bindbot1[0] + drs.sec[drs.index].bindbot2[0]) / 2 + 2.25 * off;
                LDimlY = scf * (drs.sec[drs.index].bindbot1[1] + drs.sec[drs.index].bindbot2[1]) / 2;
            }
        }
    }
}
