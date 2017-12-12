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
            double fi = section.fi;
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
                if (SigmaBetonTop > 0)
                {
                    x = (SigmaBetonTop / (SigmaBetonTop + Math.Abs(SigmaBetonBottom))) * h;
                }
                else
                {
                    x = (SigmaBetonBottom / (SigmaBetonBottom + Math.Abs(SigmaBetonTop))) * h;
                }
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
                bool reversed = false;

                A_II = A_I;
                //int licznik = 0;
                //int licznik_zwieksz = 0;
                //int licznik_zmniejsz = 0;
                double x1 = 0;
                double x2 = h;
                do
                {
                    x = (x1 + x2) / 2;

                    if (x < 0.000001 * h)
                    {
                        x = 0;
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
                    MEd_c = MEd - NEd * (0.5 * h - xc);

                    zTop = xc;                //odleglosc gornej krawedzi przekroju od srodka ciezkosci w mm
                    zBottom = xc - x;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci w mm

                    // naprezenia w betonie
                    if (x < 0.000001 * h)
                    {
                        SigmaBetonBottom = 0;
                        SigmaBetonTop = 0;
                    }
                    else
                    {
                        SigmaBetonTop = Naprezenie(NEd, MEd_c, zTop, Iy, A_II);
                        SigmaBetonBottom = Naprezenie(NEd, MEd_c, zBottom, Iy, A_II);
                    }

                    // naprezenia w stali
                    SigmaStalAs1 = alfaE * Naprezenie(NEd, MEd_c, (xc - h + a1), Iy, A_II);
                    SigmaStalAs2 = alfaE * Naprezenie(NEd, MEd_c, (zTop - a2), Iy, A_II);


                    if (SigmaBetonBottom < 0)
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

                    if (Math.Abs(x1 - x2) < 0.0001 * h && MEd_c < 0)
                    {
                        reversed = true;
                        section = section.reversedSection;
                        a1 = section.a1 / dimfactor;
                        a2 = section.a2 / dimfactor;
                        As1 = section.As1 / (dimfactor * dimfactor);
                        As2 = section.As2 / (dimfactor * dimfactor);
                        x1 = 0;
                        x2 = h;
                        MEd = -MEd;
                    }
                }
                while ((Math.Abs(SigmaBetonBottom) > 0.001) && (x > 0.000001 * h));

                if (x == 0)
                {
                    if (!reversed)
                        return new StressState(0, 0, SigmaStalAs1, SigmaStalAs2, x, 0.5 * h - xc, faza);
                    else
                        return new StressState(0, 0, SigmaStalAs2, SigmaStalAs1, x, -(0.5 * h - xc), faza);
                }
                else
                {
                    if (!reversed)
                        return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs1, SigmaStalAs2, x, 0.5 * h - xc, faza);
                    else
                        return new StressState(SigmaBetonBottom, SigmaBetonTop, SigmaStalAs2, SigmaStalAs1, x, -(0.5 * h - xc), faza);
                }
            }
        }

        /// <summary>
        /// Funkcja zwraca szerokość rozwarcia rysy dla zadanej siły podłużnej oraz momentu zginającego
        /// </summary>
        /// <param name="section">klasa reprezentująca obliczany przekrój</param>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="MEd">moment zginający w kNm</param>
        /// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC / 0.8 dla prętów żebrowanych)</param>
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
            double sigmaS = naprezenia.SteelAs1Stress;
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

            if (sigmaS > 0)
                return 0;
            else
                sigmaS = Math.Abs(sigmaS);

            if (As == 0)
            {
                //return 100;
                As = 0.000001;
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
            }/*
            else if (naprezenia.SteelAs1Stress < 0 && naprezenia.SteelAs1Stress < 0)
            {
                k2 = 1;
            }*/
            else if (naprezenia.SteelAs1Stress > 0)
            {
                k2 = 0;
            }
            else
            {
                double epsilonAs1 = naprezenia.SteelAs1Stress * Es;
                double epsilonAs2 = naprezenia.SteelAs2Stress * Es;
                double epsEdge1 = Math.Abs((epsilonAs1 - epsilonAs2) / (h - section.a1 - section.a2) * section.a1 - epsilonAs1);
                double epsEdge2 = Math.Abs((epsilonAs1 - epsilonAs2) / (h - section.a1 - section.a2) * section.a2 + epsilonAs2);
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
            double minMoment = MimosrodOsiowegoRozciagania(section, NEd) * NEd;
            double maxMoment = minMoment + 100;
            double momentKryt = (maxMoment + minMoment) / 2;
            double eps = 0.0001;
            StressState naprezenia = GetStresses(section, NEd, maxMoment);

            while (Math.Abs(naprezenia.SteelAs1Stress) < section.currentSteel.fyk * wspRedukcji)
            {
                maxMoment += 100;
                naprezenia = GetStresses(section, NEd, maxMoment);
            }

            momentKryt = (maxMoment + minMoment) / 2;

            while (Math.Abs(maxMoment - minMoment) > eps)
            {
                naprezenia = GetStresses(section, NEd, momentKryt);
                if (Math.Abs(naprezenia.SteelAs1Stress) == wspRedukcji * section.currentSteel.fyk)
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
            double minMoment = MimosrodOsiowegoRozciagania(section, NEd) * NEd;
            double maxMoment = minMoment + 100;
            double momentKryt;
            double eps = 0.001;
            StressState naprezenia = GetStresses(section, NEd, maxMoment);

            while (naprezenia.ConcreteTopStress < wspRedukcji * section.currentConrete.fck)
            {
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
        /// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC / 0.8 dla prętów żebrowanych)</param>
        /// <param name="kt">współczynnik kt zależny od czasu trwania obciążenia (0.6 obc. krótkotrwałe, 0.4 obc. długotrwałe)</param>
        /// <returns>Zwraca moment krytycny w kNm</returns>
        public static double GetMomentKrytycznyRysa(Section section, double NEd, double rysaGraniczna, double kt, double k1)
        {
            double b = section.b / 1000;
            double h = section.h / 1000;
            double a1 = section.a1 / 1000;
            double a2 = section.a2 / 1000;
            double As1 = section.As1 / 1000 / 1000;
            double As2 = section.As2 / 1000 / 1000;
            double alfaE = section.currentSteel.Es / section.currentConrete.Ecm;

            double A_I = b * h + alfaE * (As1 + As2);                  // sprowadzone pole powierzchni przekroju w m2
                                                                       //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
            double S1 = b * h * h / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            double xc1 = S1 / A_I;

            double Force1 = -A_I * section.currentConrete.fctm * 1000;
            double Moment1 = Force1 * (h / 2 - xc1);

            double A_II = As1 + As2;
            double S2 = (As1 * (h - a1) + As2 * a2);
            double xc2 = S2 / A_II; // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double Force2 = -(As1 + As2) * section.currentSteel.fyk * 1000;
            double Moment2 = Force2 * (h / 2 - xc2);

            double Moment0;

            if (NEd < Force1)
            {
                double wkP, wkL;
                double moment = NEd * (h / 2 - xc2);
                wkP = SLS.GetCrackWidth(section, NEd, moment, kt, k1);
                wkL = SLS.GetCrackWidth(section.reversedSection, NEd, -moment, kt, k1);

                double mom11, mom22, momLL, momPP, momCC;
                mom11 = moment;
                mom22 = moment;

                if (wkP > wkL)
                {
                    while (wkP > wkL)
                    {
                        mom11 -= 0.01 * Math.Abs(NEd);
                        wkP = SLS.GetCrackWidth(section, NEd, mom11, kt, k1);
                        wkL = SLS.GetCrackWidth(section.reversedSection, NEd, -mom11, kt, k1);
                    }
                }
                else if (wkP < wkL)
                {
                    while (wkP < wkL)
                    {
                        mom11 += 0.01 * Math.Abs(NEd);
                        wkP = SLS.GetCrackWidth(section, NEd, mom11, kt, k1);
                        wkL = SLS.GetCrackWidth(section.reversedSection, NEd, -mom11, kt, k1);
                    }
                }
                else
                    moment = mom11;

                momLL = Math.Min(mom11, mom22);
                momPP = Math.Max(mom11, mom22);
                momCC = (momPP + momLL) / 2;

                while (Math.Abs(wkP - wkL) > 0.000001 || wkL == 0 || wkP == 0)
                {
                    wkP = SLS.GetCrackWidth(section, NEd, momCC, kt, k1);
                    wkL = SLS.GetCrackWidth(section.reversedSection, NEd, -momCC, kt, k1);

                    if (wkP > wkL)
                        momPP = momCC;
                    else
                        momLL = momCC;

                    momCC = (momPP + momLL) / 2;
                    moment = momCC;
                }

                Moment0 = moment;
            }
            else
            {
                Moment0 = NEd * (h / 2 - xc1);
            }

            double mom1 = Moment0;
            double mom2 = Moment0;
            double wk0 = GetCrackWidth(section, NEd, Moment0, kt, k1);
            double delta;
            bool warunek;

            if (wk0 > rysaGraniczna)
                delta = Math.Min(-0.01 * Math.Abs(NEd), -1);
            else
                delta = Math.Max(0.01 * Math.Abs(NEd), 1);

            do
            {
                mom1 += delta;
                double wkS = GetCrackWidth(section, NEd, mom1, kt, k1);

                if (delta > 0 && wkS <= rysaGraniczna)
                    warunek = true;
                else if (delta < 0 && wkS > rysaGraniczna)
                    warunek = true;
                else
                    warunek = false;

            } while (warunek);

            double momP = Math.Max(mom1, mom2);
            double momL = Math.Min(mom1, mom2);

            double momC = (momL + momP) / 2;
            double eps = 0.00001;
            while (Math.Abs(momP - momL) > eps)
            {
                double wk = SLS.GetCrackWidth(section, NEd, momC, kt, k1);
                if (wk > rysaGraniczna)
                {
                    momP = momC;
                }
                else
                {
                    momL = momC;
                }

                momC = (momP + momL) / 2;
            }
            return momL;
        }

        /// <summary>
        /// Funkcja zwraca maksymalną (ściskającą) siłę osiową dla której naprężenia w betonie nie przekraczają wsp*fck
        /// </summary>
        /// <param name="section">obiekt przekrój</param>
        /// <param name="wspRedukcji">współczynnik do którego ograniczamy naprężenia w betonie</param>
        /// <returns>Zwraca wartość siły osiowej w kN</returns>
        public static double GetSilaOsiowaKrytycznaBeton(Section section, double wspRedukcji)
        {
            double b = section.b / 1000;
            double h = section.h / 1000;
            double a1 = section.a1 / 1000;
            double a2 = section.a2 / 1000;
            double As1 = section.As1 / 1000 / 1000;
            double As2 = section.As2 / 1000 / 1000;
            double alfaE = section.currentSteel.Es / section.currentConrete.Ecm;

            double A = b * h;                                      // pole pow przekroju betonu w m2
            double A_I = A + alfaE * (As1 + As2);                  // sprowadzone pole powierzchni przekroju w m2
                                                                   //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
            double S = b * h * h / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            double xc = S / A_I;
            // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
            double Iy = b * h * h * h / 12 + b * h * (0.5 * h - xc) * (0.5 * h - xc) +
                alfaE * (As1 * (h - xc - a1) * (h - xc - a1) + (As2 * (xc - a2) * (xc - a2)));

            double Force = wspRedukcji * section.currentConrete.fck * A_I * 1000;

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
            double b = section.b / 1000;
            double h = section.h / 1000;
            double a1 = section.a1 / 1000;
            double a2 = section.a2 / 1000;
            double As1 = section.As1 / 1000 / 1000;
            double As2 = section.As2 / 1000 / 1000;
            double alfaE = section.currentSteel.Es / section.currentConrete.Ecm;

            double A_I = b * h + alfaE * (As1 + As2);                  // sprowadzone pole powierzchni przekroju w m2
                                                                       //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
            double S1 = b * h * h / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            double xc1 = S1 / A_I;

            double Force1 = A_I * section.currentConrete.fctm;
            double Force2 = (As1 + As2) * wspRedukcji * section.currentSteel.fyk;

            double A_II = As1 + As2;
            double S2 = (As1 * (h - a1) + As2 * a2);
            double xc2 = S2 / A_II; // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            return -Math.Max(Force1, Force2) * 1000;
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
            double b = section.b / 1000;
            double h = section.h / 1000;
            double a1 = section.a1 / 1000;
            double a2 = section.a2 / 1000;
            double As1 = section.As1 / 1000 / 1000;
            double As2 = section.As2 / 1000 / 1000;
            double alfaE = section.currentSteel.Es / section.currentConrete.Ecm;

            double A_I = b * h + alfaE * (As1 + As2);                  // sprowadzone pole powierzchni przekroju w m2
                                                                       //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
            double S1 = b * h * h / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            double xc1 = S1 / A_I;

            double A_II = As1 + As2;
            double S2 = (As1 * (h - a1) + As2 * a2);
            double xc2 = S2 / A_II; // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double Force1 = 0.999*SilaRysujacaOsiowa(section);
            double Moment1 = Force1 * (h / 2 - xc1);
            //StressState str1 = GetStresses(section, Force1, Moment1);

            double Force2 = -section.currentSteel.fyk * A_II * 1000;
            double Moment2 = Force2 * (h / 2 - xc2);
            //StressState str2 = GetStresses(section, Force2, Moment2);

            double eps = 0.001;
            double minForce, maxForce, momentForce;

            if (Force1 < Force2)
            {
                return Force1;
            }
            else
            {
                minForce = Force2;
                maxForce = Force1;
                momentForce = Force2 * (h / 2 - xc2);
            }

            double wk, wkP, wkL; // = SLS.GetCrackWidth(section, minForce, momentForce, kt, k1);
            double Force = (Force1 + Force2) / 2;
            //StressState str = GetStresses(section, Force, momentForce);
            double moment = Force * (h / 2 - xc2);
            while (Math.Abs(maxForce - minForce) > eps)
            {
                moment = Force * (h / 2 - xc2);
                wkP = SLS.GetCrackWidth(section, Force, moment, kt, k1);
                wkL = SLS.GetCrackWidth(section.reversedSection, Force, -moment, kt, k1);

                double mom1, mom2, momL, momP, momC;
                mom1 = moment;
                mom2 = moment;

                if (wkP > wkL)
                {
                    while (wkP >= wkL)
                    {
                        mom1 -= 0.01 * Math.Abs(Force);
                        wkP = SLS.GetCrackWidth(section, Force, mom1, kt, k1);
                        wkL = SLS.GetCrackWidth(section.reversedSection, Force, -mom1, kt, k1);
                    }
                }
                else if (wkP <= wkL)
                {
                    while (wkP < wkL)
                    {
                        mom1 += 0.01 * Math.Abs(Force);
                        wkP = SLS.GetCrackWidth(section, Force, mom1, kt, k1);
                        wkL = SLS.GetCrackWidth(section.reversedSection, Force, -mom1, kt, k1);
                    }
                }
                else
                    moment = mom1;

                momL = Math.Min(mom1, mom2);
                momP = Math.Max(mom1, mom2);
                momC = (momP + momL) / 2;

                if (momL != momP)
                {
                    while (Math.Abs(wkP - wkL) > 0.000001 && Math.Abs(momL-momP) > 0.000001) // || wkL == 0 || wkP == 0)
                    {
                        wkP = SLS.GetCrackWidth(section, Force, momC, kt, k1);
                        wkL = SLS.GetCrackWidth(section.reversedSection, Force, -momC, kt, k1);

                        if (wkP > wkL)
                            momP = momC;
                        else
                            momL = momC;

                        momC = (momP + momL) / 2;
                        moment = momC;
                    }
                }

                wk = Math.Max(wkP, wkL);

                if (wk > rysaGraniczna)
                {
                    minForce = Force;
                }
                else
                {
                    maxForce = Force;
                }
                Force = (maxForce + minForce) / 2;
            }
            wkP = GetCrackWidth(section, Force, moment, kt, k1);
            wkL = GetCrackWidth(section.reversedSection, Force, -moment, kt, k1);
            return maxForce;
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
            double min = 0.5 * GetSilaOsiowaKrytycznaStal(section, 0.8);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / (NoOfPoints - 1) * i;
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
            double max = 0.25 * GetSilaOsiowaKrytycznaBeton(section, 0.6);
            double min = GetSilaOsiowaKrytycznaStal(section, wspRedukcjiStal);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / (NoOfPoints - 1) * i;
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
            double max = 0.25 * GetSilaOsiowaKrytycznaBeton(section, 1.0);
            double min = GetSilaOsiowaKrytycznaRysa(section, rysaGraniczna, kt, k1);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / (NoOfPoints - 1) * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[i][1] = SLS.GetMomentKrytycznyRysa(section, Ned, rysaGraniczna, kt, k1);
                results[results.Length - i - 1][1] = -SLS.GetMomentKrytycznyRysa(section.reversedSection, Ned, rysaGraniczna, kt, k1);
                
                if (i == 98)
                {
                    //bool nic = true;
                }
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

        /// <summary>
        /// Funkcja zwraca wartość osiowej siły rysującej przekrój, w kN.
        /// </summary>
        /// <param name="section"></param>
        /// <returns>Siła osiowa rysująca przekrój, w kN</returns>
        private static double SilaRysujacaOsiowa(Section section)
        {
            double As1 = section.As1 / 1000 / 1000;
            double As2 = section.As2 / 1000 / 1000;
            double fctm = section.currentConrete.fctm;
            double fyk = section.currentSteel.fyk;
            double b = section.b / 1000;
            double h = section.h / 1000;
            double alfaE = section.currentSteel.Es / section.currentConrete.Ecm;
            double A_I = b * h + alfaE * (As1 + As2);
            return -A_I * fctm * 1000;
        }

        /// <summary>
        /// Funkcja zwraca wartość mimośrodu dla osiowego rozciągania/ściskania w metrach [m]
        /// </summary>
        /// <param name="section"></param>
        /// <param name="N">Siła osiowa rozciągająca w kN</param>
        /// <returns>Zwraca wartość mimośrodu dla osiowego rozciągania w metrach [m]</returns>
        private static double MimosrodOsiowegoRozciagania(Section section, double N)
        {
            double b = section.b / 1000;
            double h = section.h / 1000;
            double a1 = section.a1 / 1000;
            double a2 = section.a2 / 1000;
            double As1 = section.As1 / 1000 / 1000;
            double As2 = section.As2 / 1000 / 1000;
            double alfaE = section.currentSteel.Es / section.currentConrete.Ecm;

            double A_I = b * h + alfaE * (As1 + As2);                  // sprowadzone pole powierzchni przekroju w m2
                                                                       //sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
            double S1 = b * h * h / 2 + alfaE * (As1 * (h - a1) + As2 * a2);
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            double xc1 = S1 / A_I;

            double A_II = As1 + As2;
            double S2 = (As1 * (h - a1) + As2 * a2);
            double xc2 = S2 / A_II; // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double Force1 = SilaRysujacaOsiowa(section);
            //double Moment1 = Force1 * (h / 2 - xc1);
            //StressState str1 = GetStresses(section, Force1, Moment1);

            double Force2 = -section.currentSteel.fyk * A_II * 1000;
            //double Moment2 = Force2 * (h / 2 - xc2);
            //StressState str2 = GetStresses(section, Force2, Moment2);

            if (N > Force1)
            {
                return h / 2 - xc1;
            }
            else
            {
                return h / 2 - xc2;
            }
        }
    }
}
