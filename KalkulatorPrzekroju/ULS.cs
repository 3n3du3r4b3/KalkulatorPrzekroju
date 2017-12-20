using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    static class ULS
    {
        /// <summary>
        /// Określa sytuację obliczeniową oraz dobiera współcznniki obliczeniowe dla wybranej sytuacji.
        /// </summary>
        public enum DesignSituation { Accidental, PersistentAndTransient }

        private static double sigmaC(double fcd, double epsilon, double epsilonC2, double epsilonCU2, double n)
        {
            double sigma;

            if (epsilon >= 0 && epsilon < epsilonC2)
            {
                sigma = fcd * (1 - Math.Pow(1 - epsilon / epsilonC2, n));
            }
            else if (epsilonC2 <= epsilon && epsilon <= epsilonCU2)
            {
                sigma = fcd;
            }
            else
            {
                sigma = 0;
            }
            return sigma;
        }

        private static double sigmaS(double epsilon, double Es, double fyd)
        {
            if (Math.Abs(epsilon)/1000 < fyd/Es)
            {
                return epsilon * Es / 1000;
            }
            else
            {
                return Math.Abs(epsilon)/epsilon * fyd;
            }
            
        }

        private static double epsilonR(double epsilonCU2, double r, double x, double d)
        {
            return epsilonCU2 * (r + x - d) / x;
        }

        private static double circleWidthAtR(CircleSection section, double r)
        {
            double R = section.D / 2 / 1000;    //promień przekroju w m
            double alfa = 2 * Math.Acos((R - r) / (R));
            return 2 * R * Math.Sin(alfa / 2);
        }

        /// <summary>Funkcja zwraca wartość pozytywnego lub negatywnego momentu krytycznego dla siły NEd</summary>
        /// <param name="section"> obiekt typu section reprezentujący przekrój</param>
        /// <param name="NEd"> siła podłużna w kN </param>
        /// <param name="situation">określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param> 
        /// <returns>Zwraca moment krytyczny w kNm</returns>
        public static double MomentKrytyczny(Section section, double NEd, DesignSituation situation)
        {
            double gammaC, gammaS;
            if (situation == DesignSituation.Accidental)
            {
                gammaC = 1.2;
                gammaS = 1.0;
            }
            else
            {
                gammaC = 1.5;
                gammaS = 1.15;
            }
            double alfaCC = 0.85;

            Steel currentSteel = section.currentSteel;
            Concrete currentConcrete = section.currentConrete;
            double h = section.h / 1000;        // w metrach
            double b = section.b / 1000;        // w metrach
            double a1 = section.a1 / 1000;      // w metrach
            double a2 = section.a2 / 1000;      // w metrach
            double As1 = section.As1 / 1000000;     // w metrach kwadratowych
            double As2 = section.As2 / 1000000;     // w metrach kwadratowych

            double fyk = currentSteel.fyk;          // w MPa
            double Es = currentSteel.Es;            // w MPa
            //double Ecm = currentConcrete.Ecm;       // w MPa
            double fck = currentConcrete.fck;       // w MPa
            double fcd = fck / gammaC * alfaCC;     // w MPa
            double fyd = fyk / gammaS;
            double n = currentConcrete.n;

            double eps = 0.00001;
            double d = h - a1;
            double epsilonC2 = currentConcrete.epsilon_c2;
            double epsilonCU2 = currentConcrete.epsilon_cu2;
            double x1 = 0;
            double x2 = 100000 * h;

            double rAs2 = d - a2;
            double Pc = 0;
            double x, range;
            int k = 100;
            double NRd = 0;

            NEd = NEd / 1000;

            do
            {
                x = (x1 + x2) / 2;
                Pc = 0;
                range = Math.Min(x, h);
                for (int i = 0; i < k; i++)
                {
                    double ri = d - range / k * (i + 0.5);
                    Pc += sigmaC(fcd, epsilonR(epsilonCU2, ri, x, d), epsilonC2, epsilonCU2, n) * b * range / k;
                }

                double PAs1 = As1 * sigmaS(epsilonR(epsilonCU2, 0, x, d), Es, fyd);
                double PAs2 = As2 * sigmaS(epsilonR(epsilonCU2, d - a2, x, d), Es, fyd);
                NRd = Pc + PAs2 + PAs1;

                if (NRd >= NEd)
                {
                    x2 = x;
                }
                else
                {
                    x1 = x;
                }

            } while (Math.Abs(NEd - NRd) > eps);
            
            double Pcz = 0;
            for (int i = 0; i < k; i++)
            {
                double ri = d - range / k * (i + 0.5);
                double sC = sigmaC(fcd, epsilonR(epsilonCU2, ri, x, d), epsilonC2, epsilonCU2, n);
                Pcz += sC * b * (range / k) * ri;
            }
            
            double MAs2 = As2 * sigmaS(epsilonR(epsilonCU2, d - a2, x, d), Es, fyd) * (d - a2);
            double Ms1 = Pcz + MAs2;
            
            double MRd = Ms1 - NEd * (d - 0.5 * h);

            return MRd*1000;
        }

        /// <summary>Funkcja zwraca wartość pozytywnego lub negatywnego momentu krytycznego dla siły NEd</summary>
        /// <param name="section"> obiekt typu CircleSection reprezentujący przekrój okrągły</param>
        /// <param name="NEd"> siła podłużna w kN </param>
        /// <param name="situation">określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param> 
        /// <returns>Zwraca moment krytyczny w kNm</returns>
        public static double MomentKrytyczny(CircleSection section, double NEd, DesignSituation situation)
        {
            double gammaC, gammaS;
            if (situation == DesignSituation.Accidental)
            {
                gammaC = 1.2;
                gammaS = 1.0;
            }
            else
            {
                gammaC = 1.5;
                gammaS = 1.15;
            }
            double alfaCC = 0.85;

            Steel currentSteel = section.currentSteel;
            Concrete currentConcrete = section.currentConrete;
            double D = section.D / 1000;        // w metrach
            double a = section.a / 1000;      // w metrach
            double Ab = section.Ab / 1000000;     // w metrach kwadratowych
            double fiB = section.fiB / 1000;
            int noB = section.noB;

            double fyk = currentSteel.fyk;          // w MPa
            double Es = currentSteel.Es;            // w MPa
            //double Ecm = currentConcrete.Ecm;       // w MPa
            double fck = currentConcrete.fck;       // w MPa
            double fcd = fck / gammaC * alfaCC;     // w MPa
            double fyd = fyk / gammaS;
            double n = currentConcrete.n;

            double eps = 0.00001;
            double epsilonC2 = currentConcrete.epsilon_c2;
            double epsilonCU2 = currentConcrete.epsilon_cu2;
            double x1 = 0;
            double x2 = 100000 * D;

            double d; //wysokość użyteczna, zależna od wysokości strefy ściskanej x
            double As; //pole zbrojenia rozciąganego, zależne od wysokości strefy ściskanej x

            //obliczenia parametrów geometryczny dla przekroju okragłego
            double R = D / 2;     //promień przekroju
            double rAs = R - a;   //promień okręgu po którym rozmieszczone są pręty
            double[] beta = new double[noB]; //tablica z kątami w rad rozmieszczenia prętów w przekróju, założenie, że pierwszy pręt jest na kącie 0 jeśli nieparzysta ilość, lub są rozmieszczone symetrycznie
            double[] di = new double[noB]; //tablica z odległościami prętów od górnej krawędzi przekroju (ściskanej)
            bool[] ki = new bool[noB]; //tablica okreslajaca czy dany pret jest rozciagany (true), ściskany(false)
            for (int i = 0; i < noB; i++)
            {
                beta[i] = 2 * Math.PI / noB * ((double)(noB % 2 + 1) / 2 + i);
                di[i] = a + (1 - Math.Cos(beta[i])) * rAs;
            }

            double Pc = 0;
            double PAs1 = 0;
            double PAs2 = 0;
            double x, range;
            int k = 1000;
            double NRd = 0;

            NEd = NEd / 1000;

            do
            {
                x = (x1 + x2) / 2;
                Pc = 0;

                //określenie wysokości użytecznej d która jest zależna od x
                As = 0; // sumaryczne pole powierzchni zbrojenia rozciaganego
                double Asd = 0;   //moment statyczny zbrojenia rozciaganego wzgledem gornej krawedzi przekroju

                for (int i = 0; i < noB; i++)
                {
                    if (di[i] < x)
                    {
                        ki[i] = false;
                    }
                    else
                        ki[i] = true;
                    Asd += Ab * Convert.ToUInt32(ki[i]) * di[i];
                    As += Ab * Convert.ToUInt32(ki[i]);
                }

                if (As == 0)
                {
                    d = D;
                }
                else
                    d = Asd / As; //wysokość uzyteczna znana

                range = Math.Min(x, D);
                for (int i = 0; i < k; i++)
                {
                    double ri = d - range / k * (i + 0.5);
                    Pc += sigmaC(fcd, epsilonR(epsilonCU2, ri, x, d), epsilonC2, epsilonCU2, n) * circleWidthAtR(section, d - ri) * range / k;
                }

                double[] PAsi = new double[noB];
                PAs1 = 0;
                PAs2 = 0;
                for (int i = 0; i < noB; i++)
                {
                    PAsi[i] = Ab * sigmaS(epsilonR(epsilonCU2, d - di[i], x, d), Es, fyd);
                    if (!ki[i])
                    {
                        PAs2 += PAsi[i];
                    }
                    else
                    {
                        PAs1 += Ab * sigmaS(epsilonR(epsilonCU2, 0, x, d), Es, fyd);
                    }
                }

                NRd = Pc + PAs1 + PAs2;

                if (NRd >= NEd)
                {
                    x2 = x;
                }
                else
                {
                    x1 = x;
                }

            } while (Math.Abs(NEd - NRd) > eps);

            double Pcz = 0;
            for (int i = 0; i < k; i++)
            {
                double ri = d - range / k * (i + 0.5);
                double sC = sigmaC(fcd, epsilonR(epsilonCU2, ri, x, d), epsilonC2, epsilonCU2, n);
                Pcz += sC * circleWidthAtR(section, d - ri) * (range / k) * ri;
            }

            double[] MAs2i = new double[noB];
            double MAs2 = 0;
            for (int i = 0; i < noB; i++)
            {
                MAs2i[i] = Convert.ToUInt32(!ki[i]) * Ab * sigmaS(epsilonR(epsilonCU2, d - di[i], x, d), Es, fyd) * (d - di[i]);
                MAs2 += MAs2i[i];
            }

            double Ms1 = Pcz + MAs2;

            double MRd = Ms1 - NEd * (d - 0.5 * D);

            return MRd * 1000;
        }

        public static double SilaKrytycznaSciskajaca(Section section, DesignSituation situation)
        {
            double gammaC, gammaS;
            if (situation == DesignSituation.Accidental)
            {
                gammaC = 1.2;
                gammaS = 1.0;
            }
            else
            {
                gammaC = 1.5;
                gammaS = 1.15;
            }
            double alfaCC = 0.85;
            
            double b = section.b;
            double h = section.h;
            Concrete currentConrete = section.currentConrete;
            double As1 = section.As1;
            double As2 = section.As2;
            Steel currentSteel = section.currentSteel;
            double lambda, eta;
            double fck = currentConrete.fck;
            double c2 = section.c2;
            double c1 = section.c1;
            double epsilon = currentConrete.epsilon_cu2;
            double Es = currentSteel.Es;
            double fyd = currentSteel.fyk / gammaS;
            
            if (fck <= 50)
            {
                eta = 1;
                lambda = 0.8;
            }
            else
            {
                eta = 1.0 - ((fck - 50) / 200);
                lambda = 0.8 - ((fck - 50) / 400);
            }

            //return (((b * h * currentConrete.fck / gammaC * alfaCC) / 1000) + (As1 + As2) * fyd / 1000);
            return (((b * h * currentConrete.fck / gammaC * alfaCC) / 1000) + (As1 + As2) * sigmaS(epsilon, Es, fyd) / 1000);

        }

        public static double SilaKrytycznaRozciagajaca(Section section, DesignSituation situation)
        {
            double b = section.b;
            double h = section.h;
            Concrete currentConrete = section.currentConrete;
            double As1 = section.As1;
            double As2 = section.As2;
            Steel currentSteel = section.currentSteel;
            double gammaS;
            if (situation == DesignSituation.Accidental)
            {
                gammaS = 1.0;
            }
            else
            {
                gammaS = 1.15;
            }

            return -((section.As1 + section.As2) * (section.currentSteel.fyk / gammaS) / 1000);
        }

        /// <summary>
        /// Funkcja oblicza nośność przekroju betonowego na ścinanie przy podanej sile podłużnej
        /// </summary>
        /// <param name="section">Analizowany przekrój</param>
        /// <param name="NEd">Siła podłużna w przekroju w kN</param>
        /// <param name="situation">określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <returns>Zwraca wartość nośności betonu na ścinanie przekroju w kN</returns>
        public static double NosnoscBetonuNaScinanie(Section section, double NEd, DesignSituation situation)
        {
            double gammaC;
            if (situation == DesignSituation.Accidental)
            {
                gammaC = 1.2;
            }
            else
            {
                gammaC = 1.5;
            }
            double alfaCC = 0.85;

            double k1 = 0.15;

            double b = section.b / 1000;                                    // szerokość przekroju w metrach
            double bw = b;
            double h = section.h / 1000;                                    // wysokość przekroju w metrach
            double d = h - (section.a1 / 1000);                             // wysokosc uzyteczna przekroju w metrach

            double fck = section.currentConrete.fck;
            double fcd = fck / gammaC * alfaCC;                             //obliczeniowa nośność betonu na ściskanie
            double sigmaCP1 = NEd / (b * h) / 1000;
            double sigmaCP2 = 0.2 * fcd;
            double sigmaCP = Math.Max(sigmaCP1, sigmaCP2);

            double k = 1 + Math.Sqrt(200 / (d * 1000));

            double Asl = Math.Min(section.As1, section.As2) / 1000 / 1000;                // to trzeba jeszcze dobrze przeanalizowac jak dobierac to zbrojenie

            double rol = Math.Min(Asl / (bw * d), 0.02);

            double VRdc = (0.18 / gammaC * k * Math.Pow(100 * rol * fck, 1 / 3) + k1 * sigmaCP) * bw * d;
            double vmin = 0.035 * Math.Pow(k, 3 / 2) * Math.Pow(fck, 0.5);
            double VRdcMin = (vmin + k1 * sigmaCP) * bw * d;

            VRdc = Math.Max(VRdcMin, VRdc);

            //korekta ze wzgledu na punkt 6.2 EN
            double ni = 0.6 * (1 - fck / 250);
            double VEdmax = 0.5 * bw * d * ni * fcd;
            VRdc = Math.Min(VRdc, VEdmax);

            return VRdc * 1000;
        }

        /// <summary>
        /// Funkcja oblicza nośność przekroju ze zbrojeniem na ścinanie przy podanej sile podłużnej
        /// </summary>
        /// <param name="section">Analizowany przekrój</param>
        /// <param name="NEd">Siła podłużna w przekroju w kN</param>
        /// <param name="situation">określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <param name="stirrups">klasa określająca zastosowane strzemiona</param>
        /// <returns>Zwraca wartość nośności przekroju zbrojonego na ścinanie w kN</returns>
        public static double NosnoscCalkowitaNaScinanie(Section section, double NEd, DesignSituation situation, Stirrups stirrups)
        {
            double gammaC, gammaS;
            if (situation == DesignSituation.Accidental)
            {
                gammaC = 1.2;
                gammaS = 1.0;
            }
            else
            {
                gammaC = 1.5;
                gammaS = 1.15;
            }
            double alfaCC = 0.85;

            double VRdc = NosnoscBetonuNaScinanie(section, NEd, situation);

            double d = (section.h - section.a1) / 1000;         // wysokosc uzyteczna przekroju w metrach
            double Asw = stirrups.Asw / 1000 / 1000;
            double s = stirrups.Swd / 1000;
            double z = 0.9 * d;           // ramię sił wewnętrznych - trzeba sprawdzić jak to analizować, dla czystego zginania z=0.9d
            double bw = section.b / 1000;
            double fywd = 0.8 * stirrups.currentSteel.fyk / gammaS;
            double fcd = section.currentConrete.fck / gammaC * alfaCC;

            double cotQ;
            if (NEd >= 0)
            {
                cotQ = 2.0;
            }
            else
                cotQ = 1.25;
            
            double VRds = Asw / s * z * fywd * cotQ;            // w meganiotonach

            double alfaCW = 1;

            double alfa = stirrups.Alfa / (2 * Math.PI);

            double ni1;
            if (section.currentConrete.fck <= 60)
            {
                ni1 = 0.54 * (1 - 0.5 * Math.Cos(alfa));
            }
            else
            {
                ni1 = Math.Max((0.84 - section.currentConrete.fck / 200) * (1 - 0.5 * Math.Cos(alfa)), 0.5);
            }

            double VRdMax = alfaCW * bw * z * ni1 * fcd / (cotQ + 1/cotQ);      //w meganiotonach

            if (stirrups.Alfa != 90.0)
            {
                VRds = Asw / s * z * fywd * (cotQ + (1 / Math.Tan(alfa))) * Math.Sin(alfa);
                VRdMax = alfaCW * bw * z * ni1 * fcd * (cotQ + (1 / Math.Tan(alfa))) / (1 + Math.Pow(cotQ, 2.0));
            }

            double VRd = Math.Min(VRds, VRdMax);
            return VRd * 1000;
        }
        
        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interakcji.
        /// </summary>
        /// <param name="section">Przekrój analizowany</param>
        /// <param name="situation">Sytuacja projektowa</param>
        /// <param name="NoOfPoints">Liczba punktów tworzących krzywą (liczba punktów z jednej strony osi)</param>
        /// <returns></returns>
        public static double[][] GetULS_MN_Curve(Section section, DesignSituation situation, int NoOfPoints)
        {
            double max = SilaKrytycznaSciskajaca(section, situation);
            double min = SilaKrytycznaRozciagajaca(section, situation);
            double[][] results = new double[2 * NoOfPoints][];
            
            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / (NoOfPoints-1) * i;
                
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = ULS.MomentKrytyczny(section, Ned, situation);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ULS.MomentKrytyczny(section.reversedSection, Ned, situation);
            }

            return results;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji VRdC / NEd
        /// </summary>
        /// <param name="section">Przekrój obliczany</param>
        /// <param name="situation">Sytuacja obliczeniowa</param>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <returns></returns>
        public static double[][] GetULS_VRdcN_Curve(Section section, DesignSituation situation, int NoOfPoints)
        {
            double max = SilaKrytycznaSciskajaca(section, situation);
            double min = SilaKrytycznaRozciagajaca(section, situation);
            double[][] results = new double[2*NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / NoOfPoints * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = ULS.NosnoscBetonuNaScinanie(section, Ned, situation);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ULS.NosnoscBetonuNaScinanie(section.reversedSection, Ned, situation);
            }

            return results;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji VRd / NEd
        /// </summary>
        /// <param name="section">Przekrój obliczany</param>
        /// <param name="situation">Sytuacja obliczeniowa</param>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <param name="stirrups">Obiekty reprezentujący strzemiona</param>
        /// <returns></returns>
        public static double[][] GetULS_VRdN_Curve(Section section, DesignSituation situation, int NoOfPoints, Stirrups stirrups)
        {
            double max = SilaKrytycznaSciskajaca(section, situation);
            double min = SilaKrytycznaRozciagajaca(section, situation);
            double[][] results = new double[2*NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / NoOfPoints * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = ULS.NosnoscCalkowitaNaScinanie(section, Ned, situation, stirrups);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ULS.NosnoscCalkowitaNaScinanie(section.reversedSection, Ned, situation, stirrups);
            }

            return results;
        }
        
    }
}
