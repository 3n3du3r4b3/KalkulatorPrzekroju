using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    static class BridgeCreepCoefficient
    {
        public static double BridgeCreepCoefficientCalc(double Acd, double fcmd, double fckd, double RH, double u, double t00, double tend, double cem, bool sfume)
        {
            bool sf = sfume;
            double fcm = fcmd;
            double fck = fckd;
            double t0 = t00;
            double te = tend;
            double fcmt0 = fcmt0f(cem, fcm, t0);
            double fi_b0 = fi_b0f(fcmt0, sf);
            double beta_bc = bbc(fcmt0, fck, sf);
            double fi_end = fiend(fi_b0, beta_bc, t0, te);
            return fi_end;
        }

        static double fcmt0f(double cem, double fcmd, double t0d)
        {
            double s;
            if (cem == -1) { s = 0.38; } else if (cem == 0) { s = 0.25; } else s = 0.2;
            return fcmd * Math.Exp(s * (1 - Math.Pow(28/t0d,0.5)));
        }

        static double fi_b0f(double fcmt0d, bool sf)
        {
            double fib0;
            if (sf)
            {
                fib0 = 3.6 / Math.Pow(fcmt0d, 0.37);
            }
            else fib0 = 1.4;
            return fib0;
        }

        static double bbc(double fcmt0d, double fckd, bool sf)
        {
            double bb;
            if(sf)
            {
                bb = 0.37 * Math.Exp(2.8 * fcmt0d / fckd);
            }
            else
            {
                bb = 0.4 * Math.Exp(3.1 * fcmt0d / fckd);
            }
            return bb;
        }

        static double fiend(double fib0, double bbc, double t00, double tend)
        {
            double fib;
            double timef = Math.Sqrt(tend - t00);
            fib = fib0 * timef / (timef + bbc);
            return fib;
        }
    }
}
