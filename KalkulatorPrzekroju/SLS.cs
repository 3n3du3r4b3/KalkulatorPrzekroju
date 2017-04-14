using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    static class SLS
    {
        /// <summary>Funkcja zwraca naprężenia w MPa</summary>
        /// <param name="section">klasa reprezentująca obliczany przekrój</param>
        /// <param name="NEd">siła osiowa w kN </param>
        /// <param name="MEd">moment zginający w kNm</param>
        /// <returns>Zwraca obiekt typu StressState który zawiera informacje m.in. o naprężeniach w stali i betonie</returns>
        public static StressState GetStresses(Section section, double NEd, double MEd)
        {
            Steel currentSteel = section.currentSteel;
            Concrete currentConcrete = section.currentConrete;
            double dimfactor = 1000; // scale factor do wymiarow: 1000 - jedn podst to mmm
            double forcefactor = 1000; // scale factor dla sil: 1000 - jedn podst to kN
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
            MEd_c = MEd + NEd * (0.5 * h - xc);

            double zTop = xc;               //odleglosc gornej krawedzi przekroju od srodka ciezkosci
            double zBottom = xc - h;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci

            // naprezenia w betonie
            SigmaBetonTop = NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zTop;
            SigmaBetonBottom = NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zBottom;

            // naprezenia w stali
            SigmaStalAs1 = alfaE * NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zBottom - a1);
            SigmaStalAs2 = alfaE * NEd / A_I * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zTop - a2);

            if (As1 == 0)
                SigmaStalAs1 = 0;

            if (As2 == 0)
                SigmaStalAs2 = 0;

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
                x = SigmaBetonTop / (SigmaBetonTop + Math.Abs(SigmaBetonBottom)) * h;
            }

            faza = 1;

            if (SigmaBetonBottom >= -currentConcrete.fctm && SigmaBetonTop >= -currentConcrete.fctm)
            {
                return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs1, SigmaStalAs2, x, xc - 0.5 * h, faza);
            }
            // SPRAWDZAMY ZAŁOŻENIA FAZY I ORAZ ROZPATRUJEMY PRZYPADKI FAZY II
            else if (SigmaBetonBottom <= 0 && SigmaBetonTop <= 0)
            {
                //  PRZYPADEK GDY CAŁY PRZEKRÓJ JEST ROZCIĄGANY I PRZEKROCZONE JEST FCTM:
                A_II = (As1 + As2);                // sprowadzone pole powierzchni przekroju w m2
                                                   //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
                S = (As1 * (h - a1) + As2 * a2);
                // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
                xc = S / A_II;
                // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
                Iy = (As1 * (h - xc - a1) * (h - xc - a1) + (As2 * (xc - a2) * (xc - a2)));

                // moment zginajacy w srodku ciezkosci przekroju
                MEd_c = MEd + NEd * (0.5 * h - xc);

                zTop = xc;                  //odleglosc gornej krawedzi przekroju od srodka ciezkosci w mm
                zBottom = 0.5 * h - xc;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci w mm

                // naprezenia w stali
                SigmaStalAs1 = NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zBottom - a1);
                SigmaStalAs2 = NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zTop - a2);
                SigmaBetonTop = 0;
                SigmaBetonBottom = 0;
                x = 0;
                faza = 2;

                return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs1, SigmaStalAs2, x, xc - 0.5 * h, faza);
            }

            if (NEd == 0)
            {
                // PRZYPADEK GDY SIŁA PODŁUŻNA JEST ZEROWA - CZYSTE ZGINANIE
                do
                {
                    x = xc;
                    A_II = alfaE * (As1 + As2) + b * x;       // sprowadzone pole powierzchni przekroju w m2
                                                              //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
                    S = b * x * x / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
                    // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
                    xc = S / A_II;
                    // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
                    Iy = b * x * x * x / 12 + b * x * (0.5 * x - xc) * (0.5 * x - xc) +
                    alfaE * (As1 * (h - xc - a1) * (h - xc - a1) + (As2 * (xc - a2) * (xc - a2)));

                    // moment zginajacy w srodku ciezkosci przekroju
                    MEd_c = MEd + NEd * (0.5 * h - xc);

                    zTop = xc;                //odleglosc gornej krawedzi przekroju od srodka ciezkosci w mm
                    zBottom = xc - x;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci w mm

                    // naprezenia w betonie
                    SigmaBetonTop = NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zTop;
                    SigmaBetonBottom = NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zBottom;
                } while (Math.Abs(SigmaBetonBottom) > 0.001);

                // naprezenia w stali
                SigmaStalAs1 = alfaE * (NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (xc - h + a1));
                SigmaStalAs2 = alfaE * (NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zTop - a2));

                if (As1 == 0)
                    SigmaStalAs1 = 0;

                if (As2 == 0)
                    SigmaStalAs2 = 0;

                faza = 2;
            }
            else
            {
                double es1 = MEd / NEd + (h - a1 - 0.5 * h);     //mimośród całkowity od środka ciezkosci zbrojenia As1
                                                                 // trzeba obliczyc rownanie 3go stopnia oraz wyliczyc wysokosc strefy sciskanej x
                                                                 // b*x*x*x + 3*b*x*x*(es1-(h-a1))+6*alfaE*x*(As1*es1+As2*(es1-(h-a1-a2)))-6*alfaE*(As1*es1*(h-a1)+As2*(es1-(h-a1-a2))*a2)=0
                double wa = b;
                double wb = 3 * b * (es1 - (h - a1));
                double wc = 6 * alfaE * (As1 * es1 + As2 * (es1 - (h - a1 - a2)));
                double wd = -6 * alfaE * (As1 * es1 * (h - a1) + As2 * (es1 - (h - a1 - a2)) * a2);

                x = Solver.RozwRownanie3goStopnia(wa, wb, wc, wd, 0, h);

                if (x < 0)
                {
                    x = 0; //błąd, nie ma rozwiązania w tym zakresie lub są dwa... ?!
                }

                A_II = alfaE * (As1 + As2) + b * x;       // sprowadzone pole powierzchni przekroju w m2
                                                          //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
                S = b * x * x / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
                // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
                xc = S / A_II;
                // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
                Iy = b * x * x * x / 12 + b * x * (xc - 0.5 * x) * (xc - 0.5 * x) +
                alfaE * (As1 * (h - xc - a1) * (h - xc - a1) + (As2 * (xc - a2) * (xc - a2)));

                // moment zginajacy w srodku ciezkosci przekroju
                MEd_c = MEd + NEd * (0.5 * h - xc);

                zTop = xc;                //odleglosc gornej krawedzi przekroju od srodka ciezkosci w mm
                zBottom = xc - x;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci w mm

                // naprezenia w betonie
                SigmaBetonTop = NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zTop;
                SigmaBetonBottom = NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * zBottom;

                // naprezenia w stali
                SigmaStalAs1 = alfaE * (NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (xc - h + a1));
                SigmaStalAs2 = alfaE * (NEd / A_II * (forcefactor / (dimfactor * dimfactor)) + MEd_c / Iy * (forcefactor / (dimfactor * dimfactor)) * (zTop - a2));

                if (As1 == 0)
                    SigmaStalAs1 = 0;

                if (As2 == 0)
                    SigmaStalAs2 = 0;

                faza = 2;
            }
            return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs1, SigmaStalAs2, x, xc - 0.5 * h, faza);
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
            if (NEd == 0)
            {
                k2 = 0.5;
            }
            else if (naprezenia.ConcreteBottomStress == naprezenia.ConcreteTopStress && naprezenia.ConcreteBottomStress <= 0)
            {
                k2 = 1.0;
            }
            else
            {
                double epsilon1, epsilon2;

                if (naprezenia.Faza == 1)
                {
                    epsilon1 = naprezenia.ConcreteBottomStress * section.currentConrete.Ecm;
                    epsilon2 = naprezenia.ConcreteTopStress * section.currentConrete.Ecm;
                    if (epsilon2 > 0)
                    {
                        epsilon2 = 0;
                    }
                    k2 = (epsilon1 + epsilon2) / (2 * epsilon1);
                }
                else
                {
                    if (As != 0)
                    {
                        double epsilonAs1 = naprezenia.SteelAs1Stress * Es;
                        double epsilonAs2 = naprezenia.SteelAs2Stress * Es;
                        epsilon1 = epsilonAs1 * (h - x) / (h - x - section.a1);
                        epsilon2 = epsilonAs2 * (x) / (x - section.a2);
                        if (epsilon2 > 0)
                        {
                            epsilon2 = 0;
                        }
                        k2 = (epsilon1 + epsilon2) / (2 * epsilon1);
                    }
                    else
                    {
                        epsilon1 = 0;
                        epsilon2 = 0;
                        k2 = 0;
                    }
                }
            }

            double maxSpace = 5 * (c + 0.5 * fi);
            //określenie maksymalnego rozstawu rys
            if (As == 0 || spacing1 > maxSpace)
            {  // jesli nie ma zbrojenia lub rozstaw jest zbyt duży
                srMax = 1.3 * (h - x);      // w milimetrach
            }
            else
                srMax = k3 * c + k4 * (k1 * k2 * fi) / roPeff;      // w milimetrach

            return srMax * deltaEpsilon;        //w milimetrach

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
            double minMoment = SLS.GetStresses(section, 0, 0).Mimosrod * SLS.GetSilaOsiowaKrytycznaBeton(section, wspRedukcji);
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
            double minMoment = SLS.GetStresses(section, NEd, 0).Mimosrod * NEd;
            double maxMoment = minMoment + 100;
            double momentKryt = (maxMoment + minMoment) / 2;
            double eps = 0.001;
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
            return momentKryt;
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
            StressState naprezenia = SLS.GetStresses(section, maxForce, maxForce * mimosrod);

            while (Math.Abs(naprezenia.SteelAs1Stress) < section.currentSteel.fyk * wspRedukcji &&
                Math.Abs(naprezenia.SteelAs2Stress) < section.currentSteel.fyk * wspRedukcji)
            {
                mimosrod = SLS.GetStresses(section, maxForce, 0).Mimosrod;
                minForce -= 100;
                maxForce -= 100;
                naprezenia = SLS.GetStresses(section, maxForce, maxForce * mimosrod);
            }

            Force = (minForce + maxForce) / 2;

            while (Math.Abs(maxForce - minForce) > eps)
            {
                mimosrod = SLS.GetStresses(section, Force, 0).Mimosrod;
                naprezenia = SLS.GetStresses(section, Force, Force * mimosrod);
                if (Math.Abs(naprezenia.SteelAs1Stress) == wspRedukcji * section.currentSteel.fyk ||
                    Math.Abs(naprezenia.SteelAs2Stress) == wspRedukcji * section.currentSteel.fyk)
                {
                    return Force;
                }
                else if (Math.Abs(naprezenia.SteelAs1Stress) > wspRedukcji * section.currentSteel.fyk ||
                    Math.Abs(naprezenia.SteelAs2Stress) > wspRedukcji * section.currentSteel.fyk)
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
                mimosrod = SLS.GetStresses(section, maxForce, 0).Mimosrod;
                wk = SLS.GetCrackWidth(section, maxForce, maxForce * mimosrod, kt, k1);
            }

            Force = (minForce + maxForce) / 2;

            while (Math.Abs(maxForce - minForce) > eps)
            {
                mimosrod = SLS.GetStresses(section, maxForce, 0).Mimosrod;
                wk = SLS.GetCrackWidth(section, Force, Force * mimosrod, kt, k1);
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
            double max = GetSilaOsiowaKrytycznaBeton(section,wspRedukcjiBeton);
            double min = GetSilaOsiowaKrytycznaStal(section, 1.0);
            double[][] results = new double[NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / NoOfPoints * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = SLS.GetSilaOsiowaKrytycznaBeton(section, Ned);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -SLS.GetSilaOsiowaKrytycznaBeton(section.reversedSection, Ned);
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
            double max = GetSilaOsiowaKrytycznaBeton(section, 1.0);
            double min = GetSilaOsiowaKrytycznaStal(section, wspRedukcjiStal);
            double[][] results = new double[NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / NoOfPoints * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = SLS.GetSilaOsiowaKrytycznaStal(section, Ned);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -SLS.GetSilaOsiowaKrytycznaStal(section.reversedSection, Ned);
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
            double[][] results = new double[2*NoOfPoints+1][];

            for (int i = 0; i <= NoOfPoints; i++)
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
        
    }
}
