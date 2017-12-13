using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    [Serializable]
    public class CasePoint
    {
        public int row { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public CasePoint(int row, double X, double Y)
        {
            this.row = row;
            this.X = X;
            this.Y = Y;
        }

        public override bool Equals(object obj)
        {
            CasePoint CS = obj as CasePoint;
            if (CS.row == this.row &&
                CS.X == this.X &&
                CS.Y == this.Y)
            {
                return true;
            }
            else
                return false;
        }
    }
}
