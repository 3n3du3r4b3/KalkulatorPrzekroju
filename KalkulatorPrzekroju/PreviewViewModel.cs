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
        public GeometryGroup Dimv
        {
            get { return _dimv; }
            set
            {
                _dimv = value;
                NotifyPropertyChanged();
            }
        }

        private GeometryGroup _dimh;
        public GeometryGroup Dimh
        {
            get { return _dimh; }
            set
            {
                _dimh = value;
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
            double sc = 0.8;
            AH = 500;
            AW = 500;
            Scale1 = new ScaleTransform(sc * Math.Max(AH, AW) / drs.sec[drs.index].size, sc * Math.Max(AH, AW) / drs.sec[drs.index].size);
            WindowBackground = GradientBackground;
            Outline = drs.sec[drs.index].Shape;
            Outline.Transform = Scale1;
            Rebar = drs.sec[drs.index].Reinforcement;
            Rebar.Transform = Scale1;
            Dimv = drs.sec[drs.index].Dimv;
            Dimv.Transform = Scale1;
            LDimv = drs.sec[drs.index].size.ToString();               
        }
    }
}
