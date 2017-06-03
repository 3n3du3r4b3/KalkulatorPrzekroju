using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    static class SLS
    {
        private static double dimfactor = 1000;         // scale factor do wymiarow: 1000 - jedn podst to mmm
        private static double forcefactor = 1000;       // scale factor dla sil: 1000 - jedn podst to kN

        /// <summary>Funkcja zwraca naprężenia w MPa</summary>
        /// <param name="section">klasa reprezentująca obliczany przekrój</param>
        /// <param name="NEd">siła osiowa w kN </param>
        /// <param name="MEd">moment zginający w kNm</param>
        /// <returns>Zwraca obiekt typu StressState który zawiera informacje m.in. o naprężeniach w stali i betonie</returns>
        public static StressState GetStresses(Section section, double NEd, double MEd)
        {
            Steel currentSteel = section.currentSteel;
            Concrete currentConcrete = section.currentConrete;
            //double dimfactor = 1000; // scale factor do wymiarow: 1000 - jedn podst to mmm
            //double forcefactor = 1000; // scale factor dla sil: 1000 - jedn podst to kN
            double h = section.h / dimfactor;
            double b = section.b / dimfactor;
            double a1 = section.a1 / dimfactor;
            double a2 = section.a2 / dimfactor;
            double As1 = section.As1 / (dimfactor * dimfactor);
            double As2 = section.As2 / (dimfactor * dimfactor);
            double fi = 0;
            double alfaE = section.currentSteel.Es * (1 + fi) / section.currentConrete.Ecm;

            //rezultaty
            double SigmaBetonTop;
            double SigmaBetonBottom;
            double SigmaStalAs1;
            double SigmaStalAs2;
            double x;
            int faza;

            double A, A_I, A_II, S, xc, Iy, MEd_c;

            //OBLICZENIE PRZEKROJU JAKO FAZA I:
            A = b * h;                                      // pole pow przekroju betonu w m2
            A_I = A + alfaE * (As1 + As2);                  // sprowadzone pole powierzchni przekroju w m2
                                                            //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
            S = b * h * h / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            xc = S / A_I;
            // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
            Iy = b * h * h * h / 12 + b * h * (0.5 * h - xc) * (0.5 * h - xc) +
            alfaE * (As1 * (h - xc - a1) * (h - xc - a1) + (As2 * (xc - a2) * (xc - a2)));

            // moment zginajacy w srodku ciezkosci przekroju
            MEd_c = MEd - NEd * (0.5 * h - xc);

            double zTop = xc;               //odleglosc gornej krawedzi przekroju od srodka ciezkosci
            double zBottom = xc - h;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci

            // naprezenia w betonie
            SigmaBetonTop = NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zTop;
            SigmaBetonBottom = NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zBottom;

            // naprezenia w stali
            SigmaStalAs1 = alfaE * (NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zBottom + a1));
            SigmaStalAs2 = alfaE * (NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zTop - a2));

            if (As1 == 0)
                SigmaStalAs1 = -10 * currentSteel.fyk;

            if (As2 == 0)
                SigmaStalAs2 = -10 * currentSteel.fyk;

            //określenie wysokości strefy ściskanej
            if (SigmaBetonBottom <= 0 && SigmaBetonTop <= 0)
            {
                x = 0;
            }
            else if (SigmaBetonBottom >= 0 && SigmaBetonTop >= 0)
            {
                x = h;
            }
            else
            {
                x = (SigmaBetonTop / (SigmaBetonTop + Math.Abs(SigmaBetonBottom))) * h;
            }

            faza = 1;

            if (SigmaBetonBottom >= -currentConcrete.fctm && SigmaBetonTop >= -currentConcrete.fctm)
            {
                return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs1, SigmaStalAs2, x, 0.5 * h - xc, faza);
            }
            else if (As1 == 0)
            {
                return new StressState(SigmaBetonTop, SigmaBetonBottom, 0, SigmaStalAs2, x, 0.5 * h - xc, 2);
            }
            else
            {
                A_II = A_I;
                //int licznik = 0;
                //int licznik_zwieksz = 0;
                //int licznik_zmniejsz = 0;
                double x1 = 0;
                double x2 = h;
                do
                {
                    x = (x1 + x2) / 2;

                    A_II = alfaE * (As1 + As2) + b * x;       // sprowadzone pole powierzchni przekroju w m2
                                                              //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
                    S = b * x * x / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
                    // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
                    xc = S / A_II;
                    // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
                    Iy = b * x * x * x / 12 + b * x * (xc - 0.5 * x) * (xc - 0.5 * x) +
                    alfaE * (As1 * (h - xc - a1) * (h - xc - a1) + (As2 * (xc - a2) * (xc - a2)));

                    // moment zginajacy w srodku ciezkosci przekroju
                    MEd_c = MEd - NEd * (0.5 * h - xc);

                    zTop = xc;                //odleglosc gornej krawedzi przekroju od srodka ciezkosci w mm
                    zBottom = xc - x;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci w mm

                    // naprezenia w betonie
                    SigmaBetonTop = Naprezenie(NEd, MEd_c, zTop, Iy, A_II);
                    SigmaBetonBottom = Naprezenie(NEd, MEd_c, zBottom, Iy, A_II);

                    // naprezenia w stali
                    SigmaStalAs1 = alfaE * Naprezenie(NEd, MEd_c, (xc - h + a1), Iy, A_II);
                    SigmaStalAs2 = alfaE * Naprezenie(NEd, MEd_c, (zTop - a2), Iy, A_II);


                    if (Naprezenie(NEd, MEd_c, (xc - x), Iy, A_II) < 0)
                    {
                        x2 = x;
                    }
                    else
                    {
                        x1 = x;
                    }

                    if (As1 == 0)
                        SigmaStalAs1 = 0;

                    if (As2 == 0)
                        SigmaStalAs2 = 0;

                    faza = 2;
                }
                while ((Math.Abs(SigmaBetonBottom) > 0.001) && (x != 0));

                if (x == 0)
                {
                    return new StressState(0, 0, SigmaStalAs1, SigmaStalAs2, x, 0.5 * h - xc, faza);
                }
                else
                    return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs1, SigmaStalAs2, x, 0.5 * h - xc, faza);
            }
        }

        /// <summary>
        /// Funkcja zwraca szerokość rozwarcia rysy dla zadanej siły podłużnej oraz momentu zginającego
        /// </summary>
        /// <param name="section">klasa reprezentująca obliczany przekrój</param>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="MEd">moment zginający w kNm</param>
        /// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC)</param>
        /// <param name="kt">współczynnik kt zależny od czasu trwania obciążenia (0.6 obc. krótkotrwałe, 0.4 obc. długotrwałe)</param>
        /// <returns>Zwraca szerokość rozwarcia rysy w mm</returns>
        public static double GetCrackWidth(Section section, double NEd, double MEd, double kt, double k1)
        {
            StressState naprezenia = SLS.GetStresses(section, NEd, MEd);
            double alfaE = section.currentSteel.Es / section.currentConrete.Ecm;
            double As = section.As1;                                    //w milimetrach kwadratowych!!
            double fi = section.fi1;
            double c = section.c1;
            double spacing1 = section.spacing1;
            double roPeff, deltaEpsilon, AcEff, hcEff, srMax;
            double sigmaS = Math.Abs(naprezenia.SteelAs1Stress);
            double Es = section.currentSteel.Es;
            double fctEff = section.currentConrete.fctm;
            double h = section.h;                                       //wysokosc w milimetrach!!
            double x = naprezenia.WysokośćStrefySciskanej * 1000;       //w milimetrach!!   
            double k2;
            double k3 = 3.4;
            double k4 = 0.425;

            //przypadek gdy nie dochodzi do zarysowania w ogóle
            if (naprezenia.Faza == 1)
            {
                return 0;
            }
            if (As == 0)
            {
                return 100;
            }

            double hcEff1 = 2.5 * section.a2;
            double hcEff2;
            if (x == 0)
            {
                hcEff2 = 0.5 * h;
            }
            else
            {
                hcEff2 = (h - x) / 3;
            }

            hcEff = Math.Min(hcEff1, hcEff2);
            AcEff = hcEff * section.b;      // w milimetrach kwadratowych!!  
            roPeff = As / AcEff;

            double deltaEpsilon1 = (sigmaS - kt * fctEff / roPeff * (1 + alfaE * roPeff)) / Es;
            double deltaEpsilon2 = 0.6 * sigmaS / Es;
            deltaEpsilon = Math.Max(deltaEpsilon1, deltaEpsilon2);

            //określenie współczynnika k2
            double epsilon1, epsilon2;

            if (naprezenia.SteelAs1Stress * naprezenia.SteelAs2Stress < 0)
            {
                k2 = 0.5;
            }
            else if (naprezenia.SteelAs1Stress == naprezenia.SteelAs2Stress && naprezenia.SteelAs1Stress < 0)
            {
                k2 = 1;
            }
            else if (naprezenia.SteelAs1Stress > 0)
            {
                k2 = 0;
            }
            else
            {
                double epsilonAs1 = naprezenia.SteelAs1Stress * Es;
                double epsilonAs2 = naprezenia.SteelAs2Stress * Es;
                double epsEdge1 = Math.Abs((epsilonAs1 - epsilonAs2) / (h - section.a1 - section.a2) * section.a1 - epsilonAs1);
                double epsEdge2 = Math.Abs((epsilonAs2 - epsilonAs1) / (h - section.a1 - section.a2) * section.a2 - epsilonAs2);
                epsilon1 = Math.Max(epsEdge1, epsEdge2);
                epsilon2 = Math.Min(epsEdge1, epsEdge2);
                k2 = (epsilon1 + epsilon2) / (2 * epsilon1);
            }

            double maxSpace = 5 * (c + 0.5 * fi);
            //określenie maksymalnego rozstawu rys
            if (As == 0 || spacing1 > maxSpace)
            {  // jesli nie ma zbrojenia lub rozstaw jest zbyt duży
                srMax = 1.3 * (h - x);      // w milimetrach
            }
            else
                srMax = k3 * c + k4 * (k1 * k2 * fi) / roPeff;      // w milimetrach

            double wk1 = srMax * deltaEpsilon;

            return wk1;        //w milimetrach
        }

        /// <summary>
        /// Funkcja zwraca moment krytyczny w kNm dla zadanej siły podłużnej, 
        /// dla którego naprężenia w stali osiągają dopuszczalny poziom
        /// </summary>
        /// <param name="section">klasa reprezentująca obliczany przekrój</param>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="wspRedukcji">współczynnik określający dopuszczalny stopień naprężenia w stali (wsp * fyk)</param>
        /// <returns>Zwraca moment krytyczny w kNm</returns>
        public static double GetMomentKrytycznyStal(Section section, double NEd, double wspRedukcji)
        {
            double minMoment = SLS.GetStresses(section, NEd, 0).Mimosrod * NEd;
            double maxMoment = minMoment + 100;
            double momentKryt = (maxMoment + minMoment) / 2;
            double eps = 0.001;
            StressState naprezenia = GetStresses(section, NEd, maxMoment);

            while (Math.Abs(naprezenia.SteelAs1Stress) < section.currentSteel.fyk * wspRedukcji &&
                Math.Abs(naprezenia.SteelAs2Stress) < section.currentSteel.fyk * wspRedukcji)
            {
                minMoment += 100;
                maxMoment += 100;
                naprezenia = GetStresses(section, NEd, maxMoment);
            }

            momentKryt = (maxMoment + minMoment) / 2;

            while (Math.Abs(maxMoment - minMoment) > eps)
            {
                naprezenia = GetStresses(section, NEd, momentKryt);
                if (Math.Abs(naprezenia.SteelAs1Stress) == wspRedukcji * section.currentSteel.fyk ||
                    Math.Abs(naprezenia.SteelAs2Stress) == wspRedukcji * section.currentSteel.fyk)
                {
                    return momentKryt;
                }
                else if (Math.Abs(naprezenia.SteelAs1Stress) > wspRedukcji * section.currentSteel.fyk)
                {
                    maxMoment = momentKryt;
                }
                else
                {
                    minMoment = momentKryt;
                }

                momentKryt = (maxMoment + minMoment) / 2;
            }
            return momentKryt;
        }

        /// <summary>
        /// Funkcja zwraca moment krytyczny w kNm dla zadanej siły podłużnej, 
        /// dla którego naprężenia w betonie osiągają dopuszczalny poziom
        /// </summary>
        /// <param name="section">klasa reprezentująca obliczany przekrój</param>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="wspRedukcji">współczynnik określający dopuszczalny stopień naprężenia w betonie (wsp * fck)</param>
        /// <returns>Zwraca moment krytyczny w kNm</returns>
        public static double GetMomentKrytycznyBeton(Section section, double NEd, double wspRedukcji)
        {
            double minMoment = -SLS.GetStresses(section, 0, 0).Mimosrod * SLS.GetSilaOsiowaKrytycznaBeton(section, wspRedukcji);
            double maxMoment = minMoment + 100;
            double momentKryt = (maxMoment + minMoment) / 2;
            double eps = 0.001;
            StressState naprezenia = GetStresses(section, NEd, maxMoment);

            while (naprezenia.ConcreteTopStress < wspRedukcji * section.currentConrete.fck)
            {
                minMoment += 100;
                maxMoment += 100;
                naprezenia = GetStresses(section, NEd, maxMoment);
            }

            momentKryt = (maxMoment + minMoment) / 2;

            while (Math.Abs(maxMoment - minMoment) > eps)
            {
                naprezenia = GetStresses(section, NEd, momentKryt);
                if (naprezenia.ConcreteTopStress == wspRedukcji * section.currentConrete.fck)
                {
                    return momentKryt;
                }
                else if (naprezenia.ConcreteTopStress > wspRedukcji * section.currentConrete.fck)
                {
                    maxMoment = momentKryt;
                }
                else
                {
                    minMoment = momentKryt;
                }

                momentKryt = (maxMoment + minMoment) / 2;
            }
            return momentKryt;
        }

        /// <summary>
        /// Funkcja zwraca moment krytyczny w kNm dla zadanej siły podłużnej,
        /// dla którego szerokość rozwarcia rysy osiąga dopuszczalną wielkość
        /// </summary>
        /// <param name="section">klasa reprezentująca obliczany przekrój</param>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="rysaGraniczna">graniczna szerokość rozwarcia rysy w mm</param>
        /// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC)</param>
        /// <param name="kt">współczynnik kt zależny od czasu trwania obciążenia (0.6 obc. krótkotrwałe, 0.4 obc. długotrwałe)</param>
        /// <returns>Zwraca moment krytycny w kNm</returns>
        public static double GetMomentKrytycznyRysa(Section section, double NEd, double rysaGraniczna, double kt, double k1)
        {
            double minMoment = 1.1 * SLS.GetStresses(section, NEd, 0).Mimosrod * NEd;
            double maxMoment = minMoment + 100;
            double momentKryt = (maxMoment + minMoment) / 2;
            double eps = 0.00001;
            double wk = SLS.GetCrackWidth(section, NEd, maxMoment, kt, k1);

            while (wk < rysaGraniczna)
            {
                minMoment += 100;
                maxMoment += 100;
                wk = SLS.GetCrackWidth(section, NEd, maxMoment, kt, k1);
            }

            momentKryt = (maxMoment + minMoment) / 2;

            while (Math.Abs(maxMoment - minMoment) > eps)
            {
                wk = SLS.GetCrackWidth(section, NEd, momentKryt, kt, k1);
                if (wk == rysaGraniczna)
                {
                    return momentKryt;
                }
                else if (wk > rysaGraniczna)
                {
                    maxMoment = momentKryt;
                }
                else
                {
                    minMoment = momentKryt;
                }

                momentKryt = (maxMoment + minMoment) / 2;
            }
            return maxMoment;
        }

        /// <summary>
        /// Funkcja zwraca maksymalną (ściskającą) siłę osiową dla której naprężenia w betonie nie przekraczają wsp*fck
        /// </summary>
        /// <param name="section">obiekt przekrój</param>
        /// <param name="wspRedukcji">współczynnik do którego ograniczamy naprężenia w betonie</param>
        /// <returns>Zwraca wartość siły osiowej w kN</returns>
        public static double GetSilaOsiowaKrytycznaBeton(Section section, double wspRedukcji)
        {
            double eps = 0.001;

            double minForce = 0;
            double maxForce = 100;
            double Force = (minForce + maxForce) / 2;

            double mimosrod = SLS.GetStresses(section, maxForce, 0).Mimosrod;
            StressState naprezenia = SLS.GetStresses(section, maxForce, maxForce * mimosrod);

            while (naprezenia.ConcreteTopStress < wspRedukcji * section.currentConrete.fck)// &&
                                                                                           //naprezenia.ConcreteBottomStress < wspRedukcji * section.currentConrete.fck)
            {
                minForce += 100;
                maxForce += 100;
                naprezenia = SLS.GetStresses(section, maxForce, maxForce * mimosrod);
            }

            Force = (minForce + maxForce) / 2;

            while (Math.Abs(maxForce - minForce) > eps)
            {
                naprezenia = SLS.GetStresses(section, maxForce, maxForce * mimosrod);
                if (naprezenia.ConcreteTopStress == wspRedukcji * section.currentConrete.fck) // ||
                                                                                              //naprezenia.ConcreteBottomStress == wspRedukcji * section.currentConrete.fck)
                {
                    return Force;
                }
                else if (naprezenia.ConcreteTopStress > wspRedukcji * section.currentConrete.fck) // ||
                                                                                                  //naprezenia.ConcreteBottomStress > wspRedukcji * section.currentConrete.fck)
                {
                    maxForce = Force;
                }
                else
                {
                    minForce = Force;
                }
                Force = (maxForce + minForce) / 2;
            }
            return Force;
        }

        /// <summary>
        /// Funkcja zwraca minimalna (rozciągającą) siłę osiową dla której naprężenia w stali nie przekraczają wsp*fyk
        /// </summary>
        /// <param name="section"></param>
        /// <param name="wspRedukcji"></param>
        /// <returns></returns>
        public static double GetSilaOsiowaKrytycznaStal(Section section, double wspRedukcji)
        {
            double eps = 0.001;

            double minForce = 0;
            double maxForce = -100;
            double Force = (minForce + maxForce) / 2;

            double mimosrod = SLS.GetStresses(section, maxForce, 0).Mimosrod;
            StressState naprezenia = SLS.GetStresses(section, maxForce, -maxForce * mimosrod);

            while (naprezenia.SteelAs1Stress > -section.currentSteel.fyk * wspRedukcji &&
                naprezenia.SteelAs2Stress > -section.currentSteel.fyk * wspRedukcji)
            {
                mimosrod = SLS.GetStresses(section, maxForce, 0).Mimosrod;
                minForce -= 100;
                maxForce -= 100;
                naprezenia = SLS.GetStresses(section, maxForce, -maxForce * mimosrod);
            }

            Force = (minForce + maxForce) / 2;

            while (Math.Abs(maxForce - minForce) > eps)
            {
                mimosrod = SLS.GetStresses(section, Force, 0).Mimosrod;
                naprezenia = SLS.GetStresses(section, Force, -Force * mimosrod);
                if (naprezenia.SteelAs1Stress == -wspRedukcji * section.currentSteel.fyk ||
                    naprezenia.SteelAs2Stress == -wspRedukcji * section.currentSteel.fyk)
                {
                    return Force;
                }
                else if (naprezenia.SteelAs1Stress < -wspRedukcji * section.currentSteel.fyk ||
                    naprezenia.SteelAs2Stress < -wspRedukcji * section.currentSteel.fyk)
                {
                    maxForce = Force;
                }
                else
                {
                    minForce = Force;
                }
                Force = (maxForce + minForce) / 2;
            }
            return 0.999 * Force;
        }

        /// <summary>
        /// Funkcja zwraca minimalna (rozciągającą) siłę osiową dla której szerokość rozwarcia rysy nie przekracza zadanej wartośći
        /// </summary>
        /// <param name="section">obiekt przekrój</param>
        /// <param name="rysaGraniczna">Rysa graniczna w mm</param>
		/// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC)</param>
		/// <param name="kt">współczynnik kt zależny od czasu trwania obciążenia (0.6 obc. krótkotrwałe, 0.4 obc. długotrwałe)</param>
        /// <returns>Zwraca wartość siły osiowej w kN</returns>
        public static double GetSilaOsiowaKrytycznaRysa(Section section, double rysaGraniczna, double kt, double k1)
        {
            double eps = 0.001;

            double minForce = 0;
            double maxForce = -100;
            double Force = (minForce + maxForce) / 2;

            double mimosrod = SLS.GetStresses(section, maxForce, 0).Mimosrod;
            double wk = SLS.GetCrackWidth(section, maxForce, maxForce * mimosrod, kt, k1);

            while (wk < rysaGraniczna)
            {
                minForce -= 100;
                maxForce -= 100;
                double wk1 = SLS.GetCrackWidth(
                    section,
                    maxForce,
                    maxForce * SLS.GetStresses(section, maxForce, 0).Mimosrod,
                    kt,
                    k1
                    );
                double wk2 = SLS.GetCrackWidth(
                    section.reversedSection,
                    maxForce,
                    maxForce * SLS.GetStresses(section.reversedSection, maxForce, 0).Mimosrod,
                    kt,
                    k1
                    );

                wk = Math.Max(wk1, wk2);
            }

            Force = (minForce + maxForce) / 2;

            while (Math.Abs(maxForce - minForce) > eps)
            {
                double wk1 = SLS.GetCrackWidth(
                    section,
                    maxForce,
                    maxForce * SLS.GetStresses(section, Force, 0).Mimosrod,
                    kt,
                    k1
                    );

                double wk2 = SLS.GetCrackWidth(
                    section.reversedSection,
                    maxForce,
                    maxForce * SLS.GetStresses(section.reversedSection, Force, 0).Mimosrod,
                    kt,
                    k1
                    );

                wk = Math.Max(wk1, wk2);

                if (wk == rysaGraniczna)
                {
                    return Force;
                }
                else if (wk > rysaGraniczna)
                {
                    maxForce = Force;
                }
                else
                {
                    minForce = Force;
                }
                Force = (maxForce + minForce) / 2;
            }
            return Force;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji MEk / NEk
        /// </summary>
        /// <param name="section">Przekrój obliczany</param>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <param name="wspRedukcjiBeton">Współczynnik do którego redukowane są naprężenie w betonie</param>
        /// <returns></returns>
        public static double[][] GetSLS_StressConcrete_Curve(Section section, int NoOfPoints, double wspRedukcjiBeton)
        {
            double max = GetSilaOsiowaKrytycznaBeton(section, wspRedukcjiBeton);
            double min = GetSilaOsiowaKrytycznaStal(section, 0.8);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / NoOfPoints * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = SLS.GetMomentKrytycznyBeton(section, Ned, wspRedukcjiBeton);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -SLS.GetMomentKrytycznyBeton(section.reversedSection, Ned, wspRedukcjiBeton);
            }

            return results;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji MEk / NEk
        /// </summary>
        /// <param name="section">Przekrój obliczany</param>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <param name="wspRedukcjiStal">Współczynnik do którego redukowane są naprężenia w stali</param>
        /// <returns></returns>
        public static double[][] GetSLS_StressSteel_Curve(Section section, int NoOfPoints, double wspRedukcjiStal)
        {
            double max = GetSilaOsiowaKrytycznaBeton(section, 0.6);
            double min = GetSilaOsiowaKrytycznaStal(section, wspRedukcjiStal);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / NoOfPoints * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = SLS.GetMomentKrytycznyStal(section, Ned, wspRedukcjiStal);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -SLS.GetMomentKrytycznyStal(section.reversedSection, Ned, wspRedukcjiStal);
            }

            return results;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji MEk / NEk
        /// </summary>
        /// <param name="section">Przekrój obliczany</param>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <param name="rysaGraniczna">Rysa graniczna w mm</param>
		/// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC)</param>
		/// <param name="kt">współczynnik kt zależny od czasu trwania obciążenia (0.6 obc. krótkotrwałe, 0.4 obc. długotrwałe)</param>
        /// <returns></returns>
        public static double[][] GetSLS_Crack_Curve(Section section, int NoOfPoints, double rysaGraniczna, double kt, double k1)
        {
            double max = GetSilaOsiowaKrytycznaBeton(section, 1.0);
            double min = GetSilaOsiowaKrytycznaRysa(section, rysaGraniczna, kt, k1);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / NoOfPoints * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = SLS.GetMomentKrytycznyRysa(section, Ned, rysaGraniczna, kt, k1);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -SLS.GetMomentKrytycznyRysa(section.reversedSection, Ned, rysaGraniczna, kt, k1);
            }

            return results;
        }

        /// <summary>
        /// Funkcja zwraca wartość naprężenia na poziomie z od środka cięzkości przekroju
        /// </summary>
        /// <param name="N">Siła osiowa w kN</param>
        /// <param name="M">Moment zginający w kNm</param>
        /// <param name="z">Odległoś od środka ciężkości przekroju</param>
        /// <param name="Iy">Moment bezwładności przekroju w m4</param>
        /// <param name="A">Pole powierzchni przekroju w m2</param>
        /// <returns>Wartość napręzenia na poziome z od środka ciężkości w MPa</returns>
        private static double Naprezenie(double N, double M, double z, double Iy, double A)
        {
            return N / A * (forcefactor / (dimfactor * dimfactor)) + M / Iy * (forcefactor / (dimfactor * dimfactor)) * z;
        }
    }
}
