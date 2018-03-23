using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace KalkulatorPrzekroju
{
    [Serializable]
    public class CasePoint : INotifyPropertyChanged
    {
        private double? x, y;
        private int? row;
        public double? X { get { return x; } set { x = value; OnPropertyChanged("X"); } }
        public double? Y { get { return y; } set { y = value; OnPropertyChanged("Y"); } }
        public int? Row { get { return row; } set { row = value; OnPropertyChanged("Row"); } }

        public CasePoint(int? Row, double? X, double? Y)
        {
            this.row = Row;
            this.x = X;
            this.y = Y;
        }

        public CasePoint()
        {
            this.row = null;
            this.x = null;
            this.y = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string nameProperty)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameProperty));
            }
        }

        public override bool Equals(object obj)
        {
            CasePoint CS = obj as CasePoint;
            if (CS != null && this != null)
            {
                if (CS.X == this.x &&
                    CS.Y == this.y)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
