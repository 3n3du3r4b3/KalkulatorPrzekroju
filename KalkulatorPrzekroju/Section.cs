using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Określa sytuację obliczeniową oraz dobiera współcznniki obliczeniowe dla wybranej sytuacji.
    /// </summary>
    public enum DesignSituation { Accidental, PersistentAndTransient }

    [Serializable]
    abstract class Section
    {
        /// <summary>
        /// Całkowita wysokość przekroju betonowego w mm
        /// </summary>
        public abstract double HTotal { get; }
        /// <summary>
        /// Całkowita ilość prętów zbrojeniowych w przekroju (dla przekroju prostokątnego to ilość rzędów zbrojenia)
        /// </summary>
        public abstract int NoB { get; protected set; }
        /// <summary>
        /// Odległości kolejnych prętów (lub rzędów dla przekroju prostokątnego) od górnej krawędzi przekroju w m
        /// </summary>
        public double[] DiE { get { return RzednePretowUpEdge(); } }
        /// <summary>
        /// Pole powierzchni zbrojenia w każdym z rzędów
        /// </summary>
        public abstract double[] Asi { get; }
        /// <summary>
        /// Całkowite pole powierzchni przekroju betonowego w mm2
        /// </summary>
        public abstract double AcTotal { get; }
        /// <summary>
    	/// Całkowite pole powierzchni zbrojenia w mm2
    	/// </summary>
        public abstract double AsTotal { get; }
        /// <summary>
    	/// Obiekt typu Concrete reprezentujący klasę betonu w przekroju
    	/// </summary>
        public Concrete CurrentConcrete { get; protected set; }
        /// <summary>
    	/// Obiekt typu Steel reprezentujący klasę stali w przekroju
    	/// </summary>
        public Steel CurrentSteel { get; protected set; }
        /// <summary>
        /// Współczynnik pełzania
        /// </summary>
        public double Fi { get; protected set; }
        /// <summary>
        /// Uwzględnij współczynnik pełzania dla stali
        /// </summary>
        public bool considerFi4steel { get; protected set; }
        /// <summary>
        /// Uwzględnij współczynnik pełzania dla betonu
        /// </summary>
        public bool considerFi4concrete { get; protected set; }
        /// <summary>
        /// Uwzględnij współczynnik pełzania dla zarysowania
        /// </summary>
        public bool considerFi4crack { get; protected set; }
        /// <summary>
        /// Stosunek modułów sprężystości stali do betonu z uwzględnieniem współczynnika pełzania
        /// </summary>
        protected double AlfaE { get { return (1 + Fi) * CurrentSteel.Es / CurrentConcrete.Ecm; } }
        /// <summary>
        /// Klasa określająca strzemiona w przekroju
        /// </summary>
        public Stirrups MyStirrups { get; set; }
        /// <summary>
        /// Klasa dająca informacje do rysowania
        /// </summary>
        public abstract DrawInfo draw();
        /// <summary>
        /// Klasa przechowująca dane do liczenia creepa
        /// </summary>

        protected double Dimfactor { get { return 1000.0; } }         // scale factor do wymiarow: 1000 - jedn podst to mmm
        protected double Forcefactor { get { return 1000.0; } }       // scale factor dla sil: 1000 - jedn podst to kN

        /// <summary>
        /// Funkcja zwraca wartość odkształcenia w płaskim stanie odkształcenia w przekroju przy założeniu odkształcenia granicznego epsilonCU2 na górnej krawędzi przekroju
        /// </summary>
        /// <param name="r">Odległość od środka ciężkości zbrojenia rozciąganego do zadanego punktu</param>
        /// <param name="x">Wysokość strefy ściskanej</param>
        /// <param name="d">Wysokość użyteczna przekroju</param>
        /// <returns>Wartość odkształcenia w promilach na poziomie r od środka ciężkości zbrojenia rozciąganego</returns>
        protected double EpsilonR(double r, double x, double d)
        {
            return CurrentConcrete.epsilon_cu2 * (r + x - d) / x;
        }

        /// <summary>
        /// Ustawia wartość współczynnika pełzania dla przekroju, jeśli podana wartość będzie mniejsza od zera, współczynnik przyjmie wartość zero.
        /// </summary>
        /// <param name="fi">Współczynnik pełzania</param>
        /// <param name="forConcrete">Uwzględnij dla betonu</param>
        /// <param name="forSteel">Uwzględnij dla stali</param>
        /// <param name="forCrack">Uwzględnij dla zarysowania</param>
        public void SetCreepFactor(double fi, bool forConcrete, bool forSteel, bool forCrack)
        {
            SetCreepFactor(fi);
            ReversedSection.SetCreepFactor(fi);

            this.considerFi4concrete = forConcrete;
            this.considerFi4steel = forSteel;
            this.considerFi4crack = forCrack;
            ReversedSection.considerFi4concrete = forConcrete;
            ReversedSection.considerFi4steel = forSteel;
            ReversedSection.considerFi4crack = forCrack;
        }

        /// <summary>
        /// Ustawia wartość współczynnika pełzania dla przekroju, jeśli podana wartość będzie mniejsza od zera, współczynnik przyjmie wartość zero.
        /// </summary>
        /// <param name="fi">Współczynnik pełzania</param>
        protected void SetCreepFactor(double fi)
        {
            if (fi >= 0)
            {
                Fi = fi;
            }
            else
                Fi = 0;
        }

        /// <summary>
        /// Zwraca aktualny przekrój obrócony o 180 stopni
        /// </summary>
        public abstract Section ReversedSection { get; }

        /// <summary>
        /// Zwraca tablicę z rzędnymi prętów od górnej krawędzi przekroju w m
        /// </summary>
        /// <returns>Tablica z rzędnymi prętów od górnej krawędzi przekroju w m</returns>
        protected abstract double[] RzednePretowUpEdge();

        /// <summary>
        /// Zwraca tablicę z informacją czy pręt jest ściskany czy rozciągany (true - rozciągany/ false - ściskany)
        /// </summary>
        /// <param name="x">Wysokość odcinka koła (wysokość strefy ściskanej) w m</param>
        /// <returns>Tablica z informacją czy pręt jest ściskany czy rozciągany (true - rozciągany/ false - ściskany)</returns>
        protected bool[] CzyPretRozciagany(double x)
        {
            double[] di = RzednePretowUpEdge();
            bool[] ki = new bool[NoB];
            for (int i = 0; i < NoB; i++)
            {
                if ((di[i]) < x)
                {
                    ki[i] = false;
                }
                else
                    ki[i] = true;
            }
            return ki;
        }

        /// <summary>
        /// Zwraca pole powierzchni zbrojenia rozciąganego w przekroju w m2
        /// </summary>
        /// <param name="x">Wysokość strefy ściskanej w m</param>
        /// <returns></returns>
        protected double ZbrRozcPole(double x)
        {
            double[] Asi = this.Asi;
            double As1 = 0;
            bool[] ki = CzyPretRozciagany(x);
            for (int i = 0; i < NoB; i++)
            {
                As1 += Convert.ToInt32(ki[i]) * Asi[i];
            }
            return As1;
        }

        /// <summary>
        /// Zwraca pole powierzchni zbrojenia ściskanego w przekroju w m2
        /// </summary>
        /// <param name="x">Wysokość strefy ściskanej w m</param>
        /// <returns></returns>
        protected double ZbrSciskPole(double x)
        {
            double[] Asi = this.Asi;
            double As2 = 0;
            bool[] ki = CzyPretRozciagany(x);
            for (int i = 0; i < NoB; i++)
            {
                As2 += Convert.ToInt32(!ki[i]) * Asi[i];
            }
            return As2;
        }

        /// <summary>
        /// Funkcja zwraca wartość krytycznej osiowej siły rozciągającej dla przekroju
        /// </summary>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <returns>Wartość krytycznej osiowej siły rozciągającej dla przekroju w kN</returns>
        public double ULS_SilaKrytycznaRozciagajaca(ULS_Set factors)
        {
            double gammaS = factors.gammaS;
            return -((AsTotal) * (CurrentSteel.fyk / gammaS) / Forcefactor);
        }

        /// <summary>
        /// Funkcja zwraca wartość krytycznej osiowej siły ściskającej dla przekroju
        /// </summary>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <returns>Wartość krytycznej osiowej siły ściskającej dla przekroju w kN</returns>
        public double ULS_SilaKrytycznaSciskajaca(ULS_Set factors)
        {
            double alfaCC, gammaC, gammaS;
            alfaCC = factors.alfaCC;
            gammaS = factors.gammaS;
            gammaC = factors.gammaC;

            double epsilon = CurrentConcrete.epsilon_cu2;
            double fyd = CurrentSteel.fyk / gammaS;

            return (((AcTotal * CurrentConcrete.fck / gammaC * alfaCC) / Forcefactor) + (AsTotal * CurrentSteel.SigmaS(epsilon, fyd)) / Forcefactor);
        }

        /// <summary>Funkcja zwraca wartość momentu krytycznego dla siły NEd</summary>
        /// <param name="NEd"> Siła podłużna w kN </param>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param> 
        /// <returns>Zwraca moment krytyczny w kNm</returns>
        public abstract double ULS_MomentKrytyczny(double NEd, ULS_Set factors);

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interakcji.
        /// </summary>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <param name="NoOfPoints">Liczba punktów tworzących krzywą (liczba punktów z jednej strony osi)</param>
        /// <returns></returns>
        public double[][] ULS_MN_Curve(ULS_Set factors, int NoOfPoints)
        {
            double max = ULS_SilaKrytycznaSciskajaca(factors);
            double min = ULS_SilaKrytycznaRozciagajaca(factors);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / (NoOfPoints - 1) * i;

                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = ULS_MomentKrytyczny(Ned, factors);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ReversedSection.ULS_MomentKrytyczny(Ned, factors);
            }

            return results;
        }

        /// <summary>
        /// Funkcja oblicza nośność przekroju betonowego na ścinanie przy podanej sile podłużnej
        /// </summary>
        /// <param name="NEd">Siła podłużna w przekroju w kN</param>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <returns>Zwraca wartość nośności betonu na ścinanie przekroju w kN</returns>
        public abstract double ULS_ScinanieBeton(double NEd, ULS_Set factors);

        /// <summary>
        /// Funkcja oblicza nośność przekroju ze zbrojeniem na ścinanie przy podanej sile podłużnej
        /// </summary>
        /// <param name="NEd">Siła podłużna w przekroju w kN</param>
        /// <param name="part">Współczynnik w zakresie od 0 do 1 wskzujący dla jakiej części MRd należy wyznaczyć nośność na ścinanie</param>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <returns>Zwraca wartość nośności przekroju zbrojonego na ścinanie w kN</returns>
        public abstract double ULS_ScinanieTotal(double NEd, double part, ULS_Set factors);

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji VRdC / NEd
        /// </summary>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <returns></returns>
        public double[][] ULS_VRdcN_Curve(ULS_Set factors, int NoOfPoints)
        {
            double max = ULS_SilaKrytycznaSciskajaca(factors);
            double min = ULS_SilaKrytycznaRozciagajaca(factors);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / (NoOfPoints - 1) * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = ULS_ScinanieBeton(Ned, factors);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ReversedSection.ULS_ScinanieBeton(Ned, factors);
            }

            return results;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji VRd / NEd
        /// </summary>
        /// <param name="situation">Określa sytuację obliczeniową dla której przeprowadzone zostaną obliczenia</param>
        /// <param name="part">Współczynnik w zakresie od 0 do 1 wskzujący dla jakiej części MRd należy wyznaczyć nośność na ścinanie</param>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <returns></returns>
        public double[][] ULS_VRdN_Curve(ULS_Set factors, double part, int NoOfPoints)
        {
            double max = 0.99 * CurrentConcrete.fck * factors.alfaCC / factors.gammaC * AcTotal / Dimfactor / Dimfactor * Forcefactor;
            double min = ULS_SilaKrytycznaRozciagajaca(factors);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / (NoOfPoints - 1) * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = ULS_ScinanieTotal(Ned, part, factors);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ReversedSection.ULS_ScinanieTotal(Ned, part, factors);
            }

            return results;
        }

        /// <summary>
        /// Zwraca odległość środka ciężkości przekroju od górnej krawędzi przekroju dla zadanej wysokości strefy ściskanej x w m
        /// </summary>
        /// <param name="x">Wysokość strefy ściskanej w m</param>
        /// <returns>Odległość środka ciężkości przekroju od górnej krawędzi przekroju dla zadanej wysokości strefy ściskanej x w m</returns>
        protected abstract double SrCiezkPrzekr(double x);

        /// <summary>
        /// Zwraca moment bezwładności przekroju względem jego środka ciężkości
        /// </summary>
        /// <param name="x">Wysokość strefy ściskanej w m</param>
        /// <returns>Moment bezwładności przekroju względem jego środka ciężkości w m4</returns>
        protected abstract double MomBezwPrzekr(double x);

        /// <summary>
        /// Zwraca sprowadzone pole przekroju
        /// </summary>
        /// <param name="x">Wysokość strefy ściskanej w m</param>
        /// <returns>Sprowadzone pole przekroju w m2</returns>
        protected abstract double SprowPolePrzekr(double x);

        /// <summary>
        /// Efektywne pole powierzchni współpracującego przekroju roziąganego dla zarysowania
        /// </summary>
        /// <param name="x">Wysokość strefy ściskanej w mm</param>
        /// <param name="lowerFace">prawda - funkcja liczy dla dolnej (prawej) powierzchni przekroju, fałsz - dla górnej</param>
        /// <returns>Pole powierzchni w mm2</returns>
        protected abstract double Crack_AcEff(double x, bool lowerFace);

        /// <summary>
        /// Funkcja zwraca wartość naprężenia na poziomie z od środka cięzkości przekroju
        /// </summary>
        /// <param name="N">Siła osiowa w kN</param>
        /// <param name="M">Moment zginający w kNm</param>
        /// <param name="z">Odległoś od środka ciężkości przekroju w m</param>
        /// <param name="Iy">Moment bezwładności przekroju w m4</param>
        /// <param name="A">Pole powierzchni przekroju w m2</param>
        /// <returns>Wartość napręzenia na poziome z od środka ciężkości w MPa</returns>
        protected double SLS_Naprezenie(double N, double M, double z, double Iy, double A)
        {
            return N / A * (Forcefactor / (Dimfactor * Dimfactor)) + M / Iy * (Forcefactor / (Dimfactor * Dimfactor)) * z;
        }

        private double[] GetReversedSteelStresses(double[] SigmaStalAs)
        {
            double[] SigmaStalAsRev = new double[SigmaStalAs.Length];

            if (this is RectangleSection)
            {
                for (int i = 0; i < NoB / 2; i++)
                {
                    SigmaStalAsRev[i] = SigmaStalAs[NoB - i - 1];
                }
            }
            else if (this is CircleSection)
            {
                if (NoB % 2 == 0)
                {
                    for (int i = 0; i < NoB / 2; i++)
                    {
                        SigmaStalAsRev[i] = SigmaStalAs[NoB / 2 - i];
                        SigmaStalAsRev[NoB - 1] = SigmaStalAs[NoB / 2 - i];
                    }
                }
                else
                {
                    SigmaStalAsRev[NoB / 2] = SigmaStalAs[NoB - 1];
                    SigmaStalAsRev[NoB / 2 - 1] = SigmaStalAs[NoB - 1];
                    SigmaStalAsRev[NoB - 1] = SigmaStalAs[NoB / 2];

                    for (int i = NoB - 2; i > NoB / 2; i--)
                    {
                        SigmaStalAsRev[i] = SigmaStalAs[i - (NoB / 2 + 1)];
                        SigmaStalAsRev[i - (NoB / 2 + 1)] = SigmaStalAs[i];
                    }
                    
                }
            }
            else
                SigmaStalAsRev = null;

            return SigmaStalAsRev;
        }

        /// <summary>Funkcja zwraca naprężenia w MPa</summary>
        /// <param name="NEd">siła osiowa w kN </param>
        /// <param name="MEd">moment zginający w kNm</param>
        /// <returns>Zwraca obiekt typu StressState który zawiera informacje m.in. o naprężeniach w stali i betonie</returns>
        public StressState SLS_GetStresses(double NEd, double MEd, bool IsCracked)
        {
            //rezultaty
            double SigmaBetonTop;
            double SigmaBetonBottom;
            double[] SigmaStalAs = new double[NoB];
            double x;
            int faza;
            double[] di = RzednePretowUpEdge();
            double HTotal = this.HTotal / Dimfactor;

            double A_I, A_II, xc, Iy, MEd_c;

            //OBLICZENIE PRZEKROJU JAKO FAZA I:
            // sprowadzone pole powierzchni przekroju w m2
            A_I = SprowPolePrzekr(HTotal);
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            xc = SrCiezkPrzekr(HTotal);
            // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
            Iy = MomBezwPrzekr(HTotal);
            // moment zginajacy w srodku ciezkosci przekroju
            MEd_c = MEd - NEd * (0.5 * HTotal - xc);

            double zTop = xc;               //odleglosc gornej krawedzi przekroju od srodka ciezkosci
            double zBottom = xc - HTotal;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci

            double fctm;
            if (IsCracked)
            {
                fctm = 0;
            }
            else
            {
                fctm = CurrentConcrete.fctm;
            }


            // naprezenia w betonie
            SigmaBetonTop = SLS_Naprezenie(NEd, MEd_c, zTop, Iy, A_I);
            SigmaBetonBottom = SLS_Naprezenie(NEd, MEd_c, zBottom, Iy, A_I);

            // naprezenia w stali
            for (int i = 0; i < NoB; i++)
            {
                SigmaStalAs[i] = AlfaE * SLS_Naprezenie(NEd, MEd_c, xc - di[i], Iy, A_I);
            }

            //określenie wysokości strefy ściskanej
            if (SigmaBetonBottom <= 0 && SigmaBetonTop <= 0)
            {
                x = 0;
            }
            else if (SigmaBetonBottom >= 0 && SigmaBetonTop >= 0)
            {
                x = HTotal;
            }
            else
            {
                if (SigmaBetonTop > 0)
                {
                    x = (SigmaBetonTop / (SigmaBetonTop + Math.Abs(SigmaBetonBottom))) * HTotal;
                }
                else
                {
                    x = (SigmaBetonBottom / (SigmaBetonBottom + Math.Abs(SigmaBetonTop))) * HTotal;
                }
            }

            faza = 1;

            if (SigmaBetonBottom >= -fctm && SigmaBetonTop >= -fctm)
            {
                return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs, x, 0.5 * HTotal - xc, faza);
            }
            else
            {
                StressState naprezenia;
                bool reversed = false;
                StressState reversedStresses;

                A_II = A_I;
                double x1 = 0;
                double x2 = HTotal;
                do
                {
                    x = (x1 + x2) / 2;

                    if (x < 0.00001 * HTotal)
                    {
                        x = 0;
                    }
                    // sprowadzone pole powierzchni przekroju w m2
                    A_II = SprowPolePrzekr(x);
                    // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
                    xc = SrCiezkPrzekr(x);
                    // sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
                    Iy = MomBezwPrzekr(x);

                    // moment zginajacy w srodku ciezkosci przekroju
                    MEd_c = MEd - NEd * (0.5 * HTotal - xc);

                    zTop = xc;                //odleglosc gornej krawedzi przekroju od srodka ciezkosci w mm
                    zBottom = xc - x;        //odleglosc dolnej krawedzi strefy ściskanej od srodka ciezkosci w mm

                    // naprezenia w betonie
                    if (x < 0.00001 * HTotal)
                    {
                        SigmaBetonBottom = 0;
                        SigmaBetonTop = 0;
                    }
                    else
                    {
                        SigmaBetonTop = SLS_Naprezenie(NEd, MEd_c, zTop, Iy, A_II);
                        SigmaBetonBottom = SLS_Naprezenie(NEd, MEd_c, zBottom, Iy, A_II);
                    }

                    // naprezenia w stali
                    for (int i = 0; i < NoB; i++)
                    {
                        SigmaStalAs[i] = AlfaE * SLS_Naprezenie(NEd, MEd_c, xc - di[i], Iy, A_II);
                    }

                    if (SigmaBetonBottom < 0)
                    {
                        x2 = x;
                    }
                    else
                    {
                        x1 = x;
                    }

                    for (int i = 0; i < NoB; i++)
                    {
                        if (Math.Abs(Asi[i]) < 0.0001)
                        {
                            SigmaStalAs[i] = 0;
                        }
                    }

                    faza = 2;
                    naprezenia = new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs, x, 0.5 * HTotal - xc, faza);

                }
                while (Math.Abs(x1 - x2) >= 0.000001 * HTotal && x != 0);

                if (faza == 2 && Math.Abs(x1 - x2) < 0.0001 * HTotal && MEd - NEd * (0.5 * HTotal - xc) < 0 && naprezenia.BottomSteelStress(this) > 0)
                {
                    reversed = !reversed;
                    reversedStresses = ReversedSection.SLS_GetStresses(NEd, -MEd, IsCracked);
                    SigmaStalAs = reversedStresses.SteelStress;
                    x = reversedStresses.WysokośćStrefySciskanej;
                    SigmaBetonTop = reversedStresses.ConcreteTopStress;
                    SigmaBetonBottom = reversedStresses.ConcreteBottomStress;
                    xc = reversedStresses.Mimosrod;
                }

                if (x == 0)
                {
                    if (!reversed)
                        return new StressState(0, 0, SigmaStalAs, x, 0.5 * HTotal - xc, faza);
                    else
                    {
                        double[] SigmaStalAsRev = GetReversedSteelStresses(SigmaStalAs);
                        return new StressState(0, 0, SigmaStalAsRev, x, -(0.5 * HTotal - xc), faza);
                    }
                }
                else
                {
                    if (!reversed)
                        return new StressState(SigmaBetonTop, SigmaBetonBottom, SigmaStalAs, x, 0.5 * HTotal - xc, faza);
                    else
                    {
                        double[] SigmaStalAsRev = GetReversedSteelStresses(SigmaStalAs);
                        return new StressState(SigmaBetonBottom, SigmaBetonTop, SigmaStalAsRev, x, -(0.5 * HTotal - xc), faza);
                    }
                }
            }
        }

        /// <summary>
        /// Funkcja zwraca szerokość rozwarcia rysy dla zadanej siły podłużnej oraz momentu zginającego
        /// </summary>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="MEd">moment zginający w kNm</param>
        /// <param name="factors">klasa ze współczynnikami</param>
        /// <param name="lowerFace">prawda - funkcja liczy szerokość rysy na dolnej (prawej) powierzchni przekroju, fałsz - na górnej</param>
        /// <returns>Zwraca szerokość rozwarcia rysy w mm</returns>
        public double SLS_CrackWidth(double NEd, double MEd, Factors factors, bool lowerFace, bool cracked)
        {
            double kt = factors.Crack_kt;
            double k1 = factors.Crack_k1;
            StressState naprezenia = SLS_GetStresses(NEd, MEd, true);
            double alfaE = CurrentSteel.Es / CurrentConcrete.Ecm;
            double As;    //w milimetrach kwadratowych!!
            double fi, c, spacing;

            if (this is RectangleSection)
            {
                RectangleSection sec = this as RectangleSection;
                if (lowerFace)
                {
                    fi = sec.Fi1;
                    c = sec.C1;
                    spacing = sec.Spacing1;
                    As = Asi[1] * Dimfactor * Dimfactor;
                }
                else
                {
                    fi = sec.Fi2;
                    c = sec.C2;
                    spacing = sec.Spacing2;
                    As = Asi[0] * Dimfactor * Dimfactor;
                }
            }
            else if (this is CircleSection)
            {
                CircleSection sec = this as CircleSection;
                fi = sec.FiB;
                c = sec.C;
                spacing = sec.Spacing;
                As = ZbrRozcPole(naprezenia.WysokośćStrefySciskanej) * Dimfactor * Dimfactor;
            }
            else
            {
                fi = 0;
                c = 0;
                spacing = 0;
                As = 0;
            }
            double roPeff, deltaEpsilon, AcEff, srMax;
            double sigmaS;

            if (lowerFace)
            {
                sigmaS = naprezenia.BottomSteelStress(this);
            }
            else
            {
                sigmaS = naprezenia.TopSteelStress(this);
            }

            double Es = this.CurrentSteel.Es;
            double fctEff = this.CurrentConcrete.fctm;
            double h = HTotal;                                       //wysokosc w milimetrach!!
            double x = naprezenia.WysokośćStrefySciskanej * Dimfactor;       //w milimetrach!!   
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
                As = 0.0000001;
            }

            if (sigmaS > 0)
                return 0;
            else
                sigmaS = Math.Abs(sigmaS);
            /*
            if (As == 0)
            {
                //return 100;
                As = 0.000001;
            }
            */

            AcEff = Crack_AcEff(x, lowerFace);      // w milimetrach kwadratowych!!  
            roPeff = As / AcEff;

            double deltaEpsilon1 = (sigmaS - kt * fctEff / roPeff * (1 + alfaE * roPeff)) / Es;
            double deltaEpsilon2 = 0.6 * sigmaS / Es;
            deltaEpsilon = Math.Max(deltaEpsilon1, deltaEpsilon2);

            //określenie współczynnika k2
            double epsilon1, epsilon2;

            if (naprezenia.BottomSteelStress(this) * naprezenia.TopSteelStress(this) < 0)
            {
                k2 = 0.5;
            }/*
            else if (naprezenia.SteelAs1Stress < 0 && naprezenia.SteelAs1Stress < 0)
            {
                k2 = 1;
            }*/
            else if (naprezenia.BottomSteelStress(this) > 0)
            {
                k2 = 0;
            }
            else
            {
                double epsilonAs1 = naprezenia.BottomSteelStress(this) / Es;
                double epsilonAs2 = naprezenia.TopSteelStress(this) / Es;
                double epsEdge1 = Math.Abs((epsilonAs1 - epsilonAs2) / (DiE.Max() - DiE.Min()) * DiE.Min() - epsilonAs1);
                double epsEdge2 = Math.Abs((epsilonAs1 - epsilonAs2) / (DiE.Max() - DiE.Min()) * (HTotal / Dimfactor - DiE.Max()) + epsilonAs2);
                epsilon1 = Math.Max(epsEdge1, epsEdge2);
                epsilon2 = Math.Min(epsEdge1, epsEdge2);
                k2 = (epsilon1 + epsilon2) / (2 * epsilon1);
            }

            double maxSpace = 5 * (c + 0.5 * fi);
            //określenie maksymalnego rozstawu rys
            if (As == 0 || spacing > maxSpace)
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
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="wspRedukcji">współczynnik określający dopuszczalny stopień naprężenia w stali (wsp * fyk)</param>
        /// <returns>Zwraca moment krytyczny w kNm</returns>
        public double SLS_MomentKrytycznyStal(double NEd, Factors factors)
        {   /*
            double tempFi = this.Fi;
            if (considerFi4steel == false)
            {
                SetCreepFactor(0.0);
            }*/
            double wspRedukcji = factors.Stresses_k3;
            double minMoment = SLS_MimosrodOsiowegoRozciagania(NEd) * NEd;
            double maxMoment = minMoment + 100;
            double momentKryt = (maxMoment + minMoment) / 2;
            double eps = 0.0001;
            StressState naprezenia = SLS_GetStresses(NEd, maxMoment, true);

            while (Math.Abs(naprezenia.BottomSteelStress(this)) < CurrentSteel.fyk * wspRedukcji)
            {
                maxMoment += 100;
                naprezenia = SLS_GetStresses(NEd, maxMoment, true);
            }

            momentKryt = (maxMoment + minMoment) / 2;

            while (Math.Abs(maxMoment - minMoment) > eps)
            {
                naprezenia = SLS_GetStresses(NEd, momentKryt, true);
                if (Math.Abs(naprezenia.BottomSteelStress(this)) == wspRedukcji * CurrentSteel.fyk)
                {
                    return momentKryt;
                }
                else if (Math.Abs(naprezenia.BottomSteelStress(this)) > wspRedukcji * CurrentSteel.fyk)
                {
                    maxMoment = momentKryt;
                }
                else
                {
                    minMoment = momentKryt;
                }

                momentKryt = (maxMoment + minMoment) / 2;
            }/*
            if (considerFi4steel == false)
            {
                SetCreepFactor(tempFi);
            }*/
            return momentKryt;
        }

        /// <summary>
        /// Funkcja zwraca moment krytyczny w kNm dla zadanej siły podłużnej, 
        /// dla którego naprężenia w betonie osiągają dopuszczalny poziom
        /// </summary>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="wspRedukcji">współczynnik określający dopuszczalny stopień naprężenia w betonie (wsp * fck)</param>
        /// <returns>Zwraca moment krytyczny w kNm</returns>
        public double SLS_MomentKrytycznyBeton(double NEd, Factors factors)
        {/*
            double tempFi = this.Fi;
            if (considerFi4concrete == false)
            {
                SetCreepFactor(0.0);
            }*/
            double wspRedukcji = factors.Stresses_k1;
            double minMoment = SLS_MimosrodOsiowegoRozciagania(NEd) * NEd;
            double maxMoment = minMoment + 100;
            double momentKryt;
            double eps = 0.001;
            StressState naprezenia = SLS_GetStresses(NEd, maxMoment, true);

            while (naprezenia.ConcreteTopStress < wspRedukcji * CurrentConcrete.fck)
            {
                maxMoment += 100;
                naprezenia = SLS_GetStresses(NEd, maxMoment, true);
            }

            momentKryt = (maxMoment + minMoment) / 2;

            while (Math.Abs(maxMoment - minMoment) > eps)
            {
                naprezenia = SLS_GetStresses(NEd, momentKryt, true);
                if (naprezenia.ConcreteTopStress == wspRedukcji * CurrentConcrete.fck)
                {
                    return momentKryt;
                }
                else if (naprezenia.ConcreteTopStress > wspRedukcji * CurrentConcrete.fck)
                {
                    maxMoment = momentKryt;
                }
                else
                {
                    minMoment = momentKryt;
                }

                momentKryt = (maxMoment + minMoment) / 2;
            } /*
            if (considerFi4concrete == false)
            {
                SetCreepFactor(tempFi);
            }*/
            return momentKryt;
        }

        /// <summary>
        /// Funkcja zwraca moment krytyczny w kNm dla zadanej siły podłużnej,
        /// dla którego szerokość rozwarcia rysy osiąga dopuszczalną wielkość
        /// </summary>
        /// <param name="NEd">siła osiowa w kN</param>
        /// <param name="factors">Klasa ze współczynnikami do obliczeń</param>
        /// <param name="cracked">Prawda jeśli analizujesz przekrój zarysowany bez względy na wartość naprężeń rozciągających w przekroju</param>
        /// <returns>Zwraca moment krytycny w kNm</returns>
        public double SLS_MomentKrytycznyRysa(double NEd, Factors factors, bool cracked)
        {
            if (!cracked)
            {
                double xc1 = SrCiezkPrzekr(HTotal / Dimfactor);
                double A = SprowPolePrzekr(HTotal / Dimfactor);
                double Iy = MomBezwPrzekr(HTotal / Dimfactor);
                double Mkr = -(-CurrentConcrete.fctm - NEd / A / Dimfactor) * Iy / (HTotal / Dimfactor - xc1);
                return Mkr * Forcefactor;
            }
            double wklim = factors.Crack_wklim;

            double momP = 0;// 100*SLS_MomentKrytycznyStal(NEd, factors);
            double momL = 0;// -100*ReversedSection.SLS_MomentKrytycznyStal(NEd, factors);
            double momC;

            do
            {
                momP += 100;
            } while (SLS_CrackWidth(NEd, momP, factors, true, cracked) < wklim);

            do
            {
                momL -= 100;
            } while (SLS_CrackWidth(NEd, momL, factors, true, cracked) > wklim);

            double wkP = SLS_CrackWidth(NEd, momP, factors, true, true);
            double wkL = SLS_CrackWidth(NEd, momL, factors, true, true);
            double wkC;

            do
            {
                momC = (momL + momP) / 2;

                wkC = SLS_CrackWidth(NEd, momC, factors, true, true);

                if (wkC >= wklim)
                {
                    momP = momC;
                    wkP = SLS_CrackWidth(NEd, momP, factors, true, true);
                }
                else if (wkC <= wklim)
                {
                    momL = momC;
                    wkL = SLS_CrackWidth(NEd, momL, factors, true, true);
                }
            } while (Math.Abs(wkP - wkL) > 0.00001 && Math.Abs(momP - momL) > 0.001);


            return momC;

            /*
            



            double Force1 = SLS_SilaRysujacaOsiowa();
            double Moment1 = Force1 * (HTotal / Dimfactor / 2 - xc1);

            double xc2 = SrCiezkPrzekr(0.0); // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double Force2 = -AsTotal * CurrentSteel.fyk * Forcefactor;
            double Moment2 = Force2 * (HTotal / Dimfactor / 2 - xc2);

            double Moment0;

            if (NEd < Force1)
            {
                double wkP, wkL;
                double moment = NEd * (HTotal / Dimfactor / 2 - xc2);
                wkP = SLS_CrackWidth(NEd, moment, factors, true, cracked);
                wkL = SLS_CrackWidth(NEd, moment, factors, false, cracked);

                double mom11, mom22, momLL, momPP, momCC;
                mom11 = moment;
                mom22 = moment;

                if (wkP > wkL)
                {
                    while (wkP > wkL)
                    {
                        mom11 -= 0.01 * Math.Abs(NEd);
                        wkP = SLS_CrackWidth(NEd, mom11, factors, true, cracked);
                        wkL = SLS_CrackWidth(NEd, mom11, factors, false, cracked);
                    }
                }
                else if (wkP < wkL)
                {
                    while (wkP < wkL)
                    {
                        mom11 += 0.01 * Math.Abs(NEd);
                        wkP = SLS_CrackWidth(NEd, mom11, factors, true, cracked);
                        wkL = SLS_CrackWidth(NEd, mom11, factors, false, cracked);
                    }
                }
                else
                    moment = mom11;

                momLL = Math.Min(mom11, mom22);
                momPP = Math.Max(mom11, mom22);
                momCC = (momPP + momLL) / 2;

                while (Math.Abs(wkP - wkL) > 0.000001 || wkL == 0 || wkP == 0)
                {
                    wkP = SLS_CrackWidth(NEd, momCC, factors, true, cracked);
                    wkL = SLS_CrackWidth(NEd, momCC, factors, false, cracked);

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
                Moment0 = NEd * (HTotal / Dimfactor / 2 - xc1);
            }

            double mom1 = Moment0;
            double mom2 = Moment0;
            double wk0 = SLS_CrackWidth(NEd, Moment0, factors, true, cracked);
            double delta;
            bool warunek;

            if (wk0 > rysaGraniczna)
                delta = Math.Min(-0.01 * Math.Abs(NEd), -1);
            else
                delta = Math.Max(0.01 * Math.Abs(NEd), 1);

            do
            {
                mom1 += delta;
                double wkS = SLS_CrackWidth(NEd, mom1, factors, true, cracked);

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
                double wk = SLS_CrackWidth(NEd, momC, factors, true, cracked);
                if (wk > rysaGraniczna)
                {
                    momP = momC;
                }
                else
                {
                    momL = momC;
                }

                momC = (momP + momL) / 2;
            }*/

        }

        /// <summary>
        /// Funkcja zwraca maksymalną (ściskającą) siłę osiową dla której naprężenia w betonie nie przekraczają wsp*fck
        /// </summary>
        /// <param name="wspRedukcji">współczynnik do którego ograniczamy naprężenia w betonie</param>
        /// <returns>Zwraca wartość siły osiowej w kN</returns>
        public double SLS_SilaOsiowaKrytycznaBeton(Factors factors)
        {/*
            double tempFi = this.Fi;
            if (considerFi4concrete == false)
            {
                SetCreepFactor(0.0);
            }*/
            double wspRedukcji = factors.Stresses_k1;
            double A_I = SprowPolePrzekr(HTotal / Dimfactor);                  // sprowadzone pole powierzchni przekroju w m2
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double Force = wspRedukcji * CurrentConcrete.fck * A_I * Forcefactor;
            /*
            if (considerFi4concrete == false)
            {
                SetCreepFactor(tempFi);
            }
            */
            return Force;
        }

        /// <summary>
        /// Funkcja zwraca minimalna (rozciągającą) siłę osiową dla której naprężenia w stali nie przekraczają wsp*fyk
        /// </summary>
        /// <param name="wspRedukcji"></param>
        /// <returns>Siła w kN</returns>
        public double SLS_SilaOsiowaKrytycznaStal(Factors factors)
        {
            double wspRedukcji = factors.Stresses_k3;
            double AsTotal = this.AsTotal / Dimfactor / Dimfactor;

            double A_I = SprowPolePrzekr(HTotal / Dimfactor);                  // sprowadzone pole powierzchni przekroju w m2

            double Force1 = A_I * CurrentConcrete.fctm;
            double Force2 = AsTotal * wspRedukcji * CurrentSteel.fyk;

            return -Math.Max(Force1, Force2) * Forcefactor;
        }

        /// <summary>
        /// Funkcja zwraca minimalna (rozciągającą) siłę osiową dla której szerokość rozwarcia rysy nie przekracza zadanej wartośći
        /// </summary>
        /// <param name="rysaGraniczna">Rysa graniczna w mm</param>
        /// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC)</param>
        /// <param name="kt">współczynnik kt zależny od czasu trwania obciążenia (0.6 obc. krótkotrwałe, 0.4 obc. długotrwałe)</param>
        /// <returns>Zwraca wartość siły osiowej w kN</returns>
        public double SLS_SilaOsiowaKrytycznaRysa(Factors factors, bool cracked)
        {/*
            double tempFi = this.Fi;
            if (considerFi4crack == false)
            {
                SetCreepFactor(0.0);
            }
            */
         /*        double NEd = -627.56908;
                 double MEd1 = 1152.05704;
                 double MEd2 = 127.56908;
                 StressState nap1a = SLS_GetStresses(NEd, MEd1, true);
                 StressState nap1b = ReversedSection.SLS_GetStresses(NEd, -MEd1, true);
                 StressState nap2a = SLS_GetStresses(NEd, MEd2, true);
                 StressState nap2b = ReversedSection.SLS_GetStresses(NEd, -MEd2, true);
                 double wk1P = SLS_CrackWidth(NEd, MEd1, factors, true, true);
                 double wk1L = SLS_CrackWidth(NEd, MEd1, factors, false, true);
                 double wk2P = ReversedSection.SLS_CrackWidth(NEd, -MEd1, factors, false, true);
                 double wk2L = ReversedSection.SLS_CrackWidth(NEd, -MEd1, factors, true, true);
           */
            if (!cracked)
            {
                return SLS_SilaRysujacaOsiowa();
            }

            double wklim = factors.Crack_wklim;
            //double wkP, wkL;

            double Nmax = SLS_SilaOsiowaKrytycznaStal(factors);
            // ALGORYTM 3
            double Nmin = 0;
            double momL, momP, Nc;
            do
            {
                Nc = (Nmax + Nmin) / 2;

                momL = SLS_MomentKrytycznyRysa(Nc, factors, cracked);
                momP = -ReversedSection.SLS_MomentKrytycznyRysa(Nc, factors, cracked);

                if (momP > momL)
                {
                    Nmax = Nc;
                }
                else if (momP < momL)
                {
                    Nmin = Nc;
                }
                else
                {
                    return Nc;
                }

            } while (Math.Abs(Nmax - Nmin) > 0.0001);

            return Nc;

            //ALGORYTM 3 - KONIEC

            //ALGORYTM 2 - krótszy lepsze ale niedoskonały
            /*            double momC;

                        do
                        {
                            double Mom = SLS_MomentKrytycznyStal(Nmax, factors);

                            wkP = SLS_CrackWidth(Nmax, Mom, factors, true, true);
                            wkL = SLS_CrackWidth(Nmax, Mom, factors, false, true);

                            double momL = Mom;
                            double momP = Mom;

                            bool prawaStrona;
                            if (wkP >= wkL) {
                                prawaStrona = true;
                            }
                            else {
                                prawaStrona = false;
                            }

                            // pętla szukająca drugiego momentu aby znaleźć GRANICE ZAKRESU w którym będziemy szukać takiego momentu
                            // dla którego rysy po obu stronach przekroju będą równe
                            bool warunek;
                            do
                            {
                                warunek = true;
                                if (prawaStrona) {
                                    momL -= 100;
                                    wkP = SLS_CrackWidth(Nmax, momL, factors, true, true);
                                    wkL = SLS_CrackWidth(Nmax, momL, factors, false, true);
                                    if (wkP <= wkL) {
                                        warunek = false;
                                    }
                                } else {
                                    momP += 100;
                                    wkP = SLS_CrackWidth(Nmax, momP, factors, true, true);
                                    wkL = SLS_CrackWidth(Nmax, momP, factors, false, true);
                                    if (wkL <= wkP) {
                                        warunek = false;
                                    }
                                }
                            } while (warunek);

                            //pętla szukająca momentu dla którego RYSY PO OBU STRONACH przekroju będą RÓWNE

                            do
                            {
                                momC = (momL + momP) / 2;

                                wkP = SLS_CrackWidth(Nmax, momC, factors, true, true);
                                wkL = SLS_CrackWidth(Nmax, momC, factors, false, true);

                                if (wkP >= wkL)
                                {
                                    momP = momC;
                                }
                                else
                                {
                                    momL = momC;
                                }
                            } while (Math.Abs(wkP - wkL) > 0.00001);

                            double wk = (wkP+wkL)/2;
                            Nmax = wklim/wk * Nmax;

                        } while (Math.Abs(Math.Max(wkP,wkL)-wklim) > 0.00001);

                        double Nm = -1804.7586602836398;
                        double ML = -1082.6318409146525;
                        double MP = -663.10169633309511;
                        double wk1 = SLS_CrackWidth(Nm, ML, factors, true, true);
                        double wk2 = SLS_CrackWidth(Nm, ML, factors, false, true);

                        double wk3 = SLS_CrackWidth(Nm, MP, factors, true, true);
                        double wk4 = SLS_CrackWidth(Nm, MP, factors, false, true);

                        return Nmax;    */
            // ALGORYTM 2 - koniec

            // ALGORYTM 1 - POCZĄTKOWY
            //double alfaE = CurrentSteel.Es / CurrentConcrete.Ecm;
            /*
            double A_I = SprowPolePrzekr(HTotal / Dimfactor);                   // sprowadzone pole powierzchni przekroju w m2
            double xc1 = SrCiezkPrzekr(HTotal / Dimfactor);                                 // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double A_II = SprowPolePrzekr(0.0);
            double xc2 = SrCiezkPrzekr(0.0); // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double Force1 = 0; // 0.999 * SLS_SilaRysujacaOsiowa();
            double Moment1 = Force1 * (HTotal / Dimfactor / 2 - xc1);
            //StressState str1 = GetStresses(section, Force1, Moment1);

            double Force2 = -CurrentSteel.fyk * AsTotal / Dimfactor / Dimfactor * Forcefactor;
            double Moment2 = Force2 * (HTotal / Dimfactor / 2 - xc2);
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
                momentForce = Force2 * (HTotal / Dimfactor / 2 - xc2);
            }

            double wk, wkP, wkL; // = SLS.GetCrackWidth(section, minForce, momentForce, kt, k1);
            double Force = (Force1 + Force2) / 2;
            //StressState str = GetStresses(section, Force, momentForce);
            double moment = Force * (HTotal / Dimfactor / 2 - xc2);
            while (Math.Abs(maxForce - minForce) > eps)
            {
                moment = Force * (HTotal / Dimfactor / 2 - xc2);
                wkP = SLS_CrackWidth(Force, moment, factors, true, cracked);
                wkL = SLS_CrackWidth(Force, moment, factors, false, cracked);

                double mom1, mom2, momL, momP, momC;
                mom1 = moment;
                mom2 = moment;

                if (wkP > wkL)
                {
                    while (wkP >= wkL)
                    {
                        mom1 -= 0.01 * Math.Abs(Force);
                        wkP = SLS_CrackWidth(Force, mom1, factors, true, cracked);
                        wkL = SLS_CrackWidth(Force, mom1, factors, false, cracked);
                    }
                }
                else if (wkP <= wkL)
                {
                    while (wkP < wkL)
                    {
                        mom1 += 0.01 * Math.Abs(Force);
                        wkP = SLS_CrackWidth(Force, mom1, factors, true, cracked);
                        wkL = SLS_CrackWidth(Force, mom1, factors, false, cracked);
                    }
                }
                else
                    moment = mom1;

                momL = Math.Min(mom1, mom2);
                momP = Math.Max(mom1, mom2);
                momC = (momP + momL) / 2;

                if (momL != momP)
                {
                    while (Math.Abs(wkP - wkL) > 0.000001 && Math.Abs(momL - momP) > 0.000001) // || wkL == 0 || wkP == 0)
                    {
                        wkP = SLS_CrackWidth(Force, momC, factors, true, cracked);
                        wkL = SLS_CrackWidth(Force, momC, factors, false, cracked);

                        if (wkP > wkL)
                            momP = momC;
                        else
                            momL = momC;

                        momC = (momP + momL) / 2;
                        moment = momC;
                    }
                }

                wk = Math.Max(wkP, wkL);

                if (wk > wklim)
                {
                    minForce = Force;
                }
                else
                {
                    maxForce = Force;
                }
                Force = (maxForce + minForce) / 2;
            }
            wkP = SLS_CrackWidth(Force, moment, factors, true, cracked);
            wkL = SLS_CrackWidth(Force, moment, factors, false, cracked);
            /*
            if (considerFi4crack == false)
            {
                SetCreepFactor(tempFi);
            }
            */
            //return maxForce;
            //ALGORYTM 1 - KONIEC
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji MEk / NEk
        /// </summary>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <param name="wspRedukcjiBeton">Współczynnik do którego redukowane są naprężenie w betonie</param>
        /// <returns></returns>
        public double[][] SLS_StressConcrete_Curve(Factors factors)
        {
            double tempFi = this.Fi;
            if (!considerFi4concrete)
            {
                SetCreepFactor(0.0);
            }
            int NoOfPoints = factors.NoOfPoints;
            double wspRedukcjiBeton = factors.Stresses_k1;

            double max = SLS_SilaOsiowaKrytycznaBeton(factors);
            double min = 0.5 * SLS_SilaOsiowaKrytycznaStal(factors);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = min + (max - min) / (NoOfPoints - 1) * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = SLS_MomentKrytycznyBeton(Ned, factors);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ReversedSection.SLS_MomentKrytycznyBeton(Ned, factors);
            }

            SetCreepFactor(tempFi);
            return results;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji MEk / NEk
        /// </summary>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <param name="wspRedukcjiStal">Współczynnik do którego redukowane są naprężenia w stali</param>
        /// <returns></returns>
        public double[][] SLS_StressSteel_Curve(Factors factors)
        {
            double tempFi = this.Fi;
            if (!considerFi4steel)
            {
                SetCreepFactor(0.0);
            }
            int NoOfPoints = factors.NoOfPoints;
            double wspRedukcjiStal = factors.Stresses_k3;
            double max = 0.25 * SLS_SilaOsiowaKrytycznaBeton(factors);
            double min = SLS_SilaOsiowaKrytycznaStal(factors);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / (NoOfPoints - 1) * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[i][1] = SLS_MomentKrytycznyStal(Ned, factors);
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[results.Length - i - 1][1] = -ReversedSection.SLS_MomentKrytycznyStal(Ned, factors);
            }

            SetCreepFactor(tempFi);

            return results;
        }

        /// <summary>
        /// Funkcja zwraca wskaźnik macierzy złożonej z punktów tworzących krzywą interkacji MEk / NEk
        /// </summary>
        /// <param name="NoOfPoints">Liczba punktów towrzących krzywą</param>
        /// <param name="rysaGraniczna">Rysa graniczna w mm</param>
		/// <param name="k1">współczynnik k1 zależny od przyczepności zbrojenia (0.8 lub 1.6 patrz EC)</param>
		/// <param name="kt">współczynnik kt zależny od czasu trwania obciążenia (0.6 obc. krótkotrwałe, 0.4 obc. długotrwałe)</param>
        /// <returns></returns>
        public double[][] SLS_Crack_Curve(Factors factors, bool cracked)
        {
            double tempFi = this.Fi;
            if (!considerFi4crack)
            {
                SetCreepFactor(0.0);
            }
            int NoOfPoints = factors.NoOfPoints;
            double rysaGraniczna = factors.Crack_wklim;
            double max = 0.25 * SLS_SilaOsiowaKrytycznaBeton(factors);
            double min = SLS_SilaOsiowaKrytycznaRysa(factors, cracked);
            double[][] results = new double[2 * NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / (NoOfPoints - 1) * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                results[results.Length - i - 1] = new double[2];
                results[results.Length - i - 1][0] = Ned;
                results[i][1] = SLS_MomentKrytycznyRysa(Ned, factors, cracked);
                results[results.Length - i - 1][1] = -ReversedSection.SLS_MomentKrytycznyRysa(Ned, factors, cracked);
            }

            SetCreepFactor(tempFi);
            return results;
        }

        public double[][] SLS_Crack_OneSide_Curve(Factors factors, bool cracked, bool positiveSide)
        {
            double tempFi = this.Fi;
            if (!considerFi4crack)
            {
                SetCreepFactor(0.0);
            }
            int NoOfPoints = factors.NoOfPoints;
            double rysaGraniczna = factors.Crack_wklim;
            double max = 0.25 * SLS_SilaOsiowaKrytycznaBeton(factors);
            double min = 1.01 * SLS_SilaOsiowaKrytycznaRysa(factors, true);
            double[][] results = new double[NoOfPoints][];

            for (int i = 0; i < NoOfPoints; i++)
            {
                double Ned = max - (max - min) / (NoOfPoints - 1) * i;
                results[i] = new double[2];
                results[i][0] = Ned;
                if (positiveSide)
                {
                    results[i][1] = SLS_MomentKrytycznyRysa(Ned, factors, cracked);
                }
                else
                {
                    results[i][1] = -ReversedSection.SLS_MomentKrytycznyRysa(Ned, factors, cracked);
                }
            }

            SetCreepFactor(tempFi);
            return results;
        }

        /// <summary>
        /// Funkcja zwraca wartość osiowej siły rysującej przekrój, w kN.
        /// </summary>
        /// <returns>Siła osiowa rysująca przekrój, w kN</returns>
        private double SLS_SilaRysujacaOsiowa()
        {
            double fctm = CurrentConcrete.fctm;
            double fyk = CurrentSteel.fyk;
            double AcTotal = this.AcTotal / Dimfactor / Dimfactor;
            double AsTotal = this.AsTotal / Dimfactor / Dimfactor;
            double alfaE = AlfaE;
            double A_I = AcTotal + alfaE * AsTotal;
            return -A_I * fctm * Forcefactor;
        }

        /// <summary>
        /// Funkcja zwraca wartość mimośrodu dla osiowego rozciągania/ściskania w metrach [m]
        /// </summary>
        /// <param name="N">Siła osiowa rozciągająca w kN</param>
        /// <returns>Zwraca wartość mimośrodu dla osiowego rozciągania w metrach [m]</returns>
        private double SLS_MimosrodOsiowegoRozciagania(double N)
        {
            // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
            double xc1 = SrCiezkPrzekr(HTotal / Dimfactor);

            double xc2 = SrCiezkPrzekr(0.0); // wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m

            double Force1 = SLS_SilaRysujacaOsiowa();
            //double Moment1 = Force1 * (h / 2 - xc1);
            //StressState str1 = GetStresses(section, Force1, Moment1);

            double Force2 = -CurrentSteel.fyk * SprowPolePrzekr(0.0) * Forcefactor;
            //double Moment2 = Force2 * (h / 2 - xc2);
            //StressState str2 = GetStresses(section, Force2, Moment2);

            if (N > Force1)
            {
                return HTotal / Dimfactor / 2 - xc1;
            }
            else
            {
                return HTotal / Dimfactor / 2 - xc2;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
