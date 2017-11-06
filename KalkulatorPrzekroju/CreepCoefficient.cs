using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    static class CreepCoefficient
    {
        // zgodnie z EN 1992-1-1 Aneks B
        /*public double RH { get; private set; }
        public double Ac { get; private set; }
        public double u { get; private set; }
        public double h0 { get; private set; }
        public double fcm { get; private set; }
        public double t0 { get; private set; }
        public double fi0 { get; private set; }
        public double beta_H { get; private set; }
        public double beta_C { get; private set; }
        public double fi_end { get; private set; }
        public double tend { get; private set; }*/

        public static double CreepCoefficientCalc(Section section, double RH, double u, double t00, double tend, double cem)
        {
            double t0 = cemt(t00,cem);
            double Ac = section.b * section.h;
            double fcm = section.currentConrete.fcm;
            double h0 = 2 * Ac / (u); //B.6
            double fi0 = fiRH(RH, h0, alpha1(fcm), alpha2(fcm), fcm) * betafcm(fcm) * betat0(t0); //B.2
            double beta_H = betaH(RH, fcm, h0, alpha3(fcm));
            double beta_C = betaC(beta_H, tend, t0);
            double fi_end = fi0 * beta_C;
            return fi_end;
        }

        private static double cemt(double t0, double cem)
        {
            return t0 * Math.Max(Math.Pow(9/Math.Pow((2+t0),(1.2)),(cem)),0.5);
        }

        private static double alpha1(double fcm)
        {
            double a1 = Math.Pow((35.0 / fcm), 0.7);
            return a1;
        }

        private static double alpha2(double fcm)
        {
            double a1 = Math.Pow((35.0 / fcm), 0.2);
            return a1;
        }

        private static double alpha3(double fcm)
        {
            double a1 = Math.Pow((35.0 / fcm), 0.5);
            return a1;
        }

        private static double fiRH(double RH, double h0, double alpha1, double alpha2, double fcm)
        {
            if (fcm <= 35)
            {
                return (1 + (1 - (RH / 100)) / (0.1 * Math.Pow(h0, 0.333))); //B.3a
            }
            else return (1 + (((1 - (RH / 100)) / (0.1 * Math.Pow(h0, 0.333))) * alpha1)) * alpha2; //B.3b
        }

        private static double betafcm(double fcm)
        {
            return (16.8 / Math.Sqrt(fcm)); //B.4
        }

        private static double betat0(double t0)
        {
            return 1 / (0.1 + Math.Pow(t0, 0.2)); //B.5
        }

        private static double betaH(double RH, double fcm, double h0, double alpha3)
        {
            if (fcm <= 35)
            {
                return Math.Min(1.5 * (1 + Math.Pow((0.012 * RH), 18)) * h0 + 250, 1500); //B.8a
            }
            else return Math.Min(1.5 * (1 + Math.Pow((0.012 * RH), 18)) * h0 + 250 * alpha3, 1500 * alpha3); //B.8b
        }

        private static double betaC(double betaH, double tend, double t0)
        {
            double btt = (tend - t0) / (betaH + tend - t0);
            return Math.Pow(btt, 0.3);
        }
    }
}