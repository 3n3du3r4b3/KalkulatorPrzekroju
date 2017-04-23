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

            double mRd1, MRd;

            double d, alfa1, alfa2, delta1, delta2;
            double fyk = currentSteel.fyk;          // w MPa
            double Es = currentSteel.Es;            // w MPa
            double Ecm = currentConcrete.Ecm;       // w MPa
            double fck = currentConcrete.fck;       // w MPa
            double fcd = fck / gammaC * alfaCC;     // w MPa
            double fyd = fyk / gammaS;              // w MPa
            d = h - a1;

            double S = fcd * b * d;                          // w megaNiutonach
            //Nmax = fcd * b * h / 1000 + (As1 + As2) * fyd / 1000;   // w kiloNiutonach!
            double nEd = NEd / S / 1000;                                   // kiloNiuton / megaNiuton!
            double eta, lambda;
            double epsilon_c2 = currentConcrete.epsilon_c2;         //w promilach
            double epsilon_cu2 = currentConcrete.epsilon_cu2;       //w promilach
            double epsilon_c3 = currentConcrete.epsilon_c3;         //w promilach
            double K = Math.Min(1.0, Es * epsilon_c3 / fyd);
            double C = Es * epsilon_cu2 / fyd;

            alfa1 = As1 * fyd / S;
            alfa2 = As2 * fyd / S;
            delta1 = a1 / d;
            delta2 = a2 / d;

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

            //SPRAWDZENIE CZY PRZYPADEK "CT" CZY "CC"
            if (nEd < (lambda * eta + alfa2))
            {
                //zachodzi przypadek CT
                // HIPOTEZA I:
                double ksi = (nEd - alfa2 + alfa1) / (lambda * eta);

                if ((C * (1 - ksi) / ksi) >= 1 && (C * (ksi - delta2) / ksi) >= 1)
                {
                    double k2 = 1.0;
                    mRd1 = lambda * eta * ksi * (1 - 0.5 * lambda * ksi) + k2 * alfa2 * (1 - delta2);
                    MRd = (mRd1 - 0.5 * nEd * (1 - delta1)) * S * d;
                }
                else
                {
                    double Bn = alfa2 + C * alfa1 - nEd;
                    ksi = (Math.Sqrt(Bn * Bn + 4 * lambda * eta * C * alfa1) - Bn) / (2 * lambda * eta);
                    double k2 = 1.0;
                    if ((C * (1 - ksi) / ksi) < 1 && (C * (ksi - delta2) / ksi) >= 1)
                    {
                        mRd1 = lambda * eta * ksi * (1 - 0.5 * lambda * ksi) + k2 * alfa2 * (1 - delta2);
                        MRd = (mRd1 - 0.5 * nEd * (1 - delta1)) * S * d;
                    }
                    else
                    {
                        Bn = C * alfa2 - alfa1 - nEd;
                        ksi = (Math.Sqrt(Bn * Bn + 4 * lambda * eta * C * alfa2 * delta2) - Bn) / (2 * lambda * eta);
                        if ((C * (1 - ksi) / ksi) >= 1 && (C * (ksi - delta2) / ksi) < 1)
                        {
                            k2 = C * (ksi - delta2) / ksi;
                            mRd1 = lambda * eta * ksi * (1 - 0.5 * lambda * ksi) + k2 * alfa2 * (1 - delta2);
                            MRd = (mRd1 - 0.5 * nEd * (1 - delta1)) * S * d;
                        }
                        else
                        {
                            // rownanie kwadratowe
                            Bn = C * alfa2 + C * alfa1 - nEd;
                            double ksi1 = (-Bn - Math.Sqrt(Bn * Bn - 4 * lambda * eta * (C * alfa2 * delta2 + C * alfa1))) / (2 * lambda * eta);
                            double ksi2 = (-Bn + Math.Sqrt(Bn * Bn - 4 * lambda * eta * (C * alfa2 * delta2 + C * alfa1))) / (2 * lambda * eta);
                            if (ksi1 < 0 && ksi2 < 0)
                            {
                                ksi = 0;
                            }
                            else if (ksi1 < 0)
                            {
                                ksi = ksi2;
                            }
                            else if (ksi2 < 0)
                            {
                                ksi = ksi1;
                            }
                            else
                            {
                                ksi = Math.Max(ksi1, ksi2);
                            }
                            mRd1 = lambda * eta * ksi * (1 - 0.5 * lambda * ksi) + k2 * alfa2 * (1 - delta2);
                            MRd = (mRd1 - 0.5 * nEd * (1 - delta1)) * S * d;
                        }
                    }
                }
            }
            else if (((lambda * eta + alfa2) < nEd) && (nEd < ((eta * (1 + delta1) + K * (alfa1 + alfa2)))))
            {
                //zachodzi przypadek CC
                double mrd1_1 = (lambda * eta * (1 - 0.5 * lambda) + alfa2 * (1 - delta2));
                double mrd1_2 = 0.5 * eta * (1 - delta1 * delta1) + K * alfa2 * (1 - delta2);
                mRd1 = Math.Min(mrd1_1, mrd1_2);
                MRd = (mRd1 - 0.5 * nEd * (1 - delta1)) * S * d;
            }
            else
            {
                mRd1 = 0;
                MRd = 0;
            }

            return MRd * 1000;      //w kiloNiutonoMetrach!
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

            /*
            return 1 * (((b * (h - c1) * currentConrete.fck * eta / gammaC * alfaCC) / 1000) +
                (As1 + As2) * currentSteel.fyk / gammaS / 1000);
                */
            double N1 = (((b * (h - c1) * currentConrete.fck * eta * lambda / gammaC * alfaCC) / 1000) +
                (As1 + As2) * currentSteel.fyk / gammaS / 1000);
            double N2 = 1.3*(((b * h * currentConrete.fck * eta / gammaC * alfaCC) / 1000) +
                (As1 + As2) * currentSteel.fyk / gammaS / 1000);
            double N = (N1 + N2) / 2;


            double Np = ULS.MomentKrytyczny(section, N, situation);
            double Nl = -ULS.MomentKrytyczny(section.reversedSection, N, situation);
            double delta = N1 - N2;

            while (Math.Abs(delta) > 0.001)
            {
                N = (N1 + N2) / 2;
                Np = ULS.MomentKrytyczny(section, N, situation);
                Nl = -ULS.MomentKrytyczny(section.reversedSection, N, situation);
                delta = N1 - N2;

                if (Np > Nl)
                {
                    N1 = N;
                }
                else
                {
                    N2 = N;
                }
            }

            return N;
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

            //return -((section.As1 + section.As2) * (section.currentSteel.fyk / gammaS) / 1000);
            double N1 = -((section.As1 + section.As2) * (section.currentSteel.fyk / gammaS) / 1000);
            double N2 = 1.2 * -((section.As1 + section.As2) * (section.currentSteel.fyk / gammaS) / 1000);
            double N = (N1 + N2) / 2;


            double Np = ULS.MomentKrytyczny(section, N, situation);
            double Nl = -ULS.MomentKrytyczny(section.reversedSection, N, situation);
            double delta = N1 - N2;

            while (Math.Abs(delta) > 0.001)
            {
                N = (N1 + N2) / 2;
                Np = ULS.MomentKrytyczny(section, N, situation);
                Nl = -ULS.MomentKrytyczny(section.reversedSection, N, situation);
                delta = N1 - N2;

                if (Np > Nl)
                {
                    N1 = N;
                }
                else
                {
                    N2 = N;
                }
            }

            return N;
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
                double Ned = min + (max - min) / NoOfPoints * i;
                if (Ned == 0)
                {
                    Ned = 0.01;
                }
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
