using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    static class Solver
    {

        static private double W3goStopnia(double a, double b, double c, double d, double x)
        {
            return a * x * x * x + b * x * x + c * x + d;
        }

        /// <summary>
        /// Funkcja rozwiązuje równanie trzeciego stopnia w postaci a*x^3 + b*x^2 + c*x + d = 0 w zadanym przedziale
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="d"></param>
        /// <param name="x1">Punkt początkowy przedziału</param>
        /// <param name="x2">Punkt końcowy przedziału</param>
        /// <returns></returns>
        static public double RozwRownanie3goStopnia(double a, double b, double c, double d, double x1, double x2)
        {
            // rozwiązuje równanie stylu a*x^3 + b*x^2 + c*x + d = 0
            double x = (x1 + x2) / 2;
            double eps = 0.0001;
            double y1 = W3goStopnia(a, b, c, d, x1);
            double y2 = W3goStopnia(a, b, c, d, x2);
            
            if (y1 * y2 <= 0)
            {
                while (Math.Abs(x2 - x1) > eps)
                {
                    x = (x1 + x2) / 2;
                    double y = W3goStopnia(a, b, c, d, x);
                    if (y == 0.0)
                    {
                        return x;
                    }

                    if (y1 * y < 0)
                    {
                        x2 = x;
                        y2 = W3goStopnia(a, b, c, d, x2);
                    }
                    else
                    {
                        x1 = x;
                        y1 = W3goStopnia(a, b, c, d, x1);
                    }
                }
            }
            else
                x = -1.0;
            return x;
        }
    }
}
