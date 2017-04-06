using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    class CreepCoefficient
    {
        // zgodnie z EN 1992-1-1 Aneks B
        public double RH { get; private set; }
        public double Ac { get; private set; }
        public double u { get; private set; }
        public double h0 { get; private set; }
        public double fcm { get; private set; }
        public double t0 { get; private set; }
        public double fi0 { get; private set; }

        public CreepCoefficient(Concrete concrete, Section section, double RH, double u, double t0)
        {
            this.RH = RH;
            this.u = u;
            Ac = section.b * section.h;
            fcm = concrete.fcm;
            h0 = 2 * Ac / u; //B.6
            fi0 = fiRH(RH, h0, alpha1(fcm), alpha2(fcm), fcm); //B.2

        }

        private double alpha1(double fcm)
        {
            double a1 = Math.Pow((35.0 / fcm), 0.7);
            return a1;
        }

        private double alpha2(double fcm)
        {
            double a1 = Math.Pow((35.0 / fcm), 0.2);
            return a1;
        }

        private double alpha3(double fcm)
        {
            double a1 = Math.Pow((35.0 / fcm), 0.5);
            return a1;
        }

        private double fiRH(double RH, double h0, double alpha1, double alpha2, double fcm)
        {
            if (fcm <= 35)
            {
                return (1 + (1 - (RH / 100)) / (0.1 * Math.Pow(h0, 0.333))); //B.3a
            }
            else return ((1 + (1 - (RH / 100)) * alpha1) / (0.1 * Math.Pow(h0, 0.333))) * alpha2; //B.3b
        }

        private double betafcm(double fcm)
        {
            return 16.8 / Math.Sqrt(fcm); //B.4
        }

        private double betat0(double t0)
        {
            return 1 / (0.1 + Math.Pow(t0, 0.2)); //B.5
        }

        private double betaH(double RH, double fcm, double h0, double alpha3)
        {
            if (fcm <= 35)
            {
                return Math.Min(1.5 * (1 + Math.Pow((0.012 * RH), 18)) * h0 + 250, 1500); //B.8a
            }
            else return Math.Min(1.5 * (1 + Math.Pow((0.012 * RH), 18)) * h0 + 250 * alpha3, 1500 * alpha3); //B.8b
        }
    }
}
