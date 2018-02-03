using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
	[Serializable]
    class RectangleSection : Section
    {
        /// <summary>
        /// Szerokość przekroju w mm
        /// </summary>
        public double B { get; private set; }
        /// <summary>
    	/// Wysokość przekroju w mm
    	/// </summary>
        public double H { get; private set; }
        /// <summary>
    	/// Odległość środka ciężkości zbrojenia As1 od najbliższej krawędzi betonu w mm
    	/// </summary>
        public double A1 { get; private set; }
        /// <summary>
    	/// Odległość środka ciężkości zbrojenia As2 od najbliższej krawędzi betonu w mm
    	/// </summary>
        public double A2 { get; private set; }
        /// <summary>
    	/// Pole powierzchni zbrojenia As1 w mm2
    	/// </summary>
        public double As1 { get; private set; }
        /// <summary>
    	/// Pole powierzchni zbrojenia As2 w mm2
    	/// </summary>
        public double As2 { get; private set; }
        /// <summary>
        /// Średnica zbrojenia As1 w mm
        /// </summary>
        public double Fi1 { get; private set; }
        /// <summary>
        /// Średnica zbrojenia As2 w mm
        /// </summary>
        public double Fi2 { get; private set; }
        /// <summary>
        /// Otulina zbrojenia As1 w mm
        /// </summary>
        public double C1 { get; private set; }
        /// <summary>
        /// Otulina zbrojenia As2 w mm
        /// </summary>
        public double C2 { get; private set; }
        /// <summary>
        /// Rozstaw prętów zbrojenia As1 w mm
        /// </summary>
        public double Spacing1 { get; private set; }
        /// <summary>
        /// Rozstaw prętów zbrojenia As2 w mm
        /// </summary>
        public double Spacing2 { get; private set; }
        
        public override double HTotal { get {return H;}}

        public override int NoB { get {return 2;} protected set { } }

        public override double[] Asi { get { return new double[] { As2 / Dimfactor / Dimfactor, As1 / Dimfactor / Dimfactor }; } }

        public override double AcTotal { get { return B * H; } }

        public override double AsTotal { get { return As1 + As2; } }

        public override Section ReversedSection { get { Section sec = new RectangleSection(CurrentConcrete, CurrentSteel, B, H, Fi2, Spacing2, C2, As2, Fi1, Spacing1, C1, As1, MyStirrups, Fi); return sec; } }

        RectangleSection(Concrete concrete, Steel steel, double b, double h, double fi1, double spacing1, double c1, double As1, double fi2, double spacing2, double c2, double As2, Stirrups stirrups, double fi)
        {
            this.B = b;
            this.H = h;
            this.Fi1 = fi1;
            this.C1 = c1;
            this.Spacing1 = spacing1;
            this.Fi2 = fi2;
            this.C2 = c2;
            this.Spacing2 = spacing2;
            CurrentConcrete = concrete;
            CurrentSteel = steel;
            this.As1 = As1;
            this.As2 = As2;
            if (As1 == 0)
            {
                c1 = 0;
                A1 = 0;
            }
            else
                A1 = c1 + 0.5 * fi1;
            if (As2 == 0)
            {
                A2 = 0;
                c2 = 0;
            }
            else
                A2 = c2 + 0.5 * fi2;

            SetCreepFactor(fi);
            MyStirrups = stirrups;
        }

        /// <summary>
        /// Konstruktor przekroju na podstawie rozstawu zbrojenia
        /// </summary>
        /// <param name="concrete">Obiekt reprezentujący klasę betonu dla przekroju</param>
        /// <param name="steel">Obiekt reprezentujący klasę stali zbrojeniowej w przekroju</param>
        /// <param name="b">Szerokość przekroju w milimetrach</param>
        /// <param name="h">Wysokość przekroju w milimetrach</param>
        /// <param name="fi1">Średnica zbrojenia As1 w mm</param>
        /// <param name="spacing1">Rozstaw prętów zbrojenia As1 w mm</param>
        /// <param name="c1">Otulina zbrojenia As1 w mm</param>
        /// <param name="fi2">Średnica zbrojenia As2 w mm</param>
        /// <param name="spacing2">Rozstaw prętów zbrojenia As2 w mm</param>
        /// <param name="c2">Otulina zbrojenia As2 w mm</param>
        public RectangleSection(Concrete concrete, Steel steel, double b, double h, double fi1, double spacing1, double c1, double fi2, double spacing2, double c2, Stirrups stirrups)
        {
            this.B = b;
            this.H = h;
            this.Fi1 = fi1;
            this.C1 = c1;
            this.Spacing1 = spacing1;
            this.Fi2 = fi2;
            this.C2 = c2;
            this.Spacing2 = spacing2;
            CurrentConcrete = concrete;
            CurrentSteel = steel;

            if (fi1 == 0)
            {
                c1 = 0;
                A1 = 0;
                As1 = 0.000000001;
            }
            else
            {
                As1 = (fi1 / 2) * (fi1 / 2) * Math.PI * b / spacing1;
                A1 = c1 + 0.5 * fi1;
            }
            
            if (fi2 == 0)
            {
                A2 = 0;
                c2 = 0;
                As2 = 0.000000001;
            }
            else
            {
                As2 = (fi2 / 2) * (fi2 / 2) * Math.PI * b / spacing2;
                A2 = c2 + 0.5 * fi2;
            }
            
            SetCreepFactor(0);
            MyStirrups = stirrups;
        }

        /// <summary>
        /// Konstruktor przekroju na podstawie ilości prętów zbrojenia w przekroju
        /// </summary>
        /// <param name="concrete">Obiekt reprezentujący klasę betonu dla przekroju</param>
        /// <param name="steel">Obiekt reprezentujący klasę stali zbrojeniowej w przekroju</param>
        /// <param name="b">Szerokość przekroju w milimetrach</param>
        /// <param name="h">Wysokość przekroju w milimetrach</param>
        /// <param name="fi1">Średnica zbrojenia As1 w mm</param>
        /// <param name="noOfBars1">Ilość prętów zbrojenia As1 w sztukach (liczba całkowita)</param>
        /// <param name="c1">Otulina zbrojenia As1 w mm</param>
        /// <param name="fi2">Średnica zbrojenia As2 w mm</param>
        /// <param name="noOfBars2">Ilość prętów zbrojenia As2 w sztukach (liczba całkowita)</param>
        /// <param name="c2">Otulina zbrojenia As2 w mm</param>
        public RectangleSection(Concrete concrete, Steel steel, double b, double h, double fi1, int noOfBars1, double c1, double fi2, int noOfBars2, double c2, Stirrups stirrups)
        {
            this.B = b;
            this.H = h;
            this.Fi1 = fi1;
            this.C1 = c1;
            this.Fi2 = fi2;
            this.C2 = c2;
            this.Spacing1 = (b - 2 * c1 - fi1) / (noOfBars1 - 1);
            this.Spacing2 = (b - 2 * c2 - fi2) / (noOfBars2 - 1);
            CurrentConcrete = concrete;
            CurrentSteel = steel;
            A1 = c1 + 0.5 * fi1;
            A2 = c2 + 0.5 * fi2;

            if (fi1 == 0 || noOfBars1 == 0)
            {
                c1 = 0;
                A1 = 0;
                As1 = 0.000000001;
            }
            else
            {
                As1 = (fi1 / 2) * (fi1 / 2) * Math.PI * noOfBars1;
                A1 = c1 + 0.5 * fi1;
            }

            if (fi2 == 0 || noOfBars2 == 0)
            {
                A2 = 0;
                c2 = 0;
                As2 = 0.000000001;
            }
            else
            {
                As2 = (fi2 / 2) * (fi2 / 2) * Math.PI * noOfBars2;
                A2 = c2 + 0.5 * fi2;
            }

            SetCreepFactor(0);
            MyStirrups = stirrups;
        }

        public override bool Equals(object obj)
        {
            RectangleSection s2 = obj as RectangleSection;

            if (this.H == s2.H &&
                this.B == s2.B &&
                this.A1 == s2.A1 &&
                this.A2 == s2.A2 &&
                this.As1 == s2.As1 &&
                this.As2 == s2.As2 &&
                this.C1 == s2.C1 &&
                this.C2 == s2.C2 &&
                Equals(this.CurrentConcrete, s2.CurrentConcrete) &&
                Equals(this.CurrentSteel, s2.CurrentSteel) &&
                this.Fi == s2.Fi &&
                this.Fi1 == s2.Fi1 &&
                this.Fi2 == s2.Fi2 &&
                this.Spacing1 == s2.Spacing1 &&
                this.Spacing2 == s2.Spacing2 &&
                this.considerFi4concrete == s2.considerFi4concrete &&
                this.considerFi4crack == s2.considerFi4crack &&
                this.considerFi4steel == s2.considerFi4steel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override double ULS_MomentKrytyczny(double NEd, ULS_Set factors)
        {
            double alfaCC, gammaC, gammaS;
            alfaCC = factors.alfaCC;
            gammaS = factors.gammaS;
            gammaC = factors.gammaC;

            double h = H / 1000;        // w metrach
            double b = B / 1000;        // w metrach
            double a1 = A1 / 1000;      // w metrach
            double a2 = A2 / 1000;      // w metrach
            double As1 = this.As1 / 1000000;     // w metrach kwadratowych
            double As2 = this.As2 / 1000000;     // w metrach kwadratowych

            double fyk = CurrentSteel.fyk;          // w MPa
            double Es = CurrentSteel.Es;            // w MPa
                                                    //double Ecm = currentConcrete.Ecm;       // w MPa
            double fck = CurrentConcrete.fck;       // w MPa
            double fcd = fck / gammaC * alfaCC;     // w MPa
            double fyd = fyk / gammaS;
            double n = CurrentConcrete.n;

            double eps = 0.00001;
            double d = h - a1;
            double epsilonC2 = CurrentConcrete.epsilon_c2;
            double epsilonCU2 = CurrentConcrete.epsilon_cu2;
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
                    Pc += CurrentConcrete.SigmaC(fcd, EpsilonR(ri, x, d)) * b * range / k;
                }

                double PAs1 = As1 * CurrentSteel.SigmaS(EpsilonR(0, x, d), fyd);
                double PAs2 = As2 * CurrentSteel.SigmaS(EpsilonR(d - a2, x, d), fyd);
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
                double sC = CurrentConcrete.SigmaC(fcd, EpsilonR(ri, x, d));
                Pcz += sC * b * (range / k) * ri;
            }

            double MAs2 = As2 * CurrentSteel.SigmaS(EpsilonR(d - a2, x, d), fyd) * (d - a2);
            double Ms1 = Pcz + MAs2;

            double MRd = Ms1 - NEd * (d - 0.5 * h);

            return MRd * 1000;
        }
        
        public override double ULS_ScinanieBeton(double NEd, ULS_Set factors)
        {
            double alfaCC, gammaC, gammaS;
            alfaCC = factors.alfaCC;
            gammaS = factors.gammaS;
            gammaC = factors.gammaC;

            double k1 = 0.15;

            double b = this.B / 1000;                                    // szerokość przekroju w metrach
            double bw = b;
            double h = this.H / 1000;                                    // wysokość przekroju w metrach
            double d = h - (this.A1 / 1000);                             // wysokosc uzyteczna przekroju w metrach

            double fck = this.CurrentConcrete.fck;
            double fcd = fck / gammaC * alfaCC;                             //obliczeniowa nośność betonu na ściskanie
            double sigmaCP1 = NEd / (b * h) / 1000;
            double sigmaCP2 = 0.2 * fcd;
            double sigmaCP = Math.Max(sigmaCP1, sigmaCP2);

            double k = 1 + Math.Sqrt(200 / (d * 1000));

            double Asl = Math.Min(this.As1, this.As2) / 1000 / 1000;                // to trzeba jeszcze dobrze przeanalizowac jak dobierac to zbrojenie

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
        
        public override double ULS_ScinanieTotal(double NEd, ULS_Set factors)
        {
            double alfaCC, gammaC, gammaS;
            alfaCC = factors.alfaCC;
            gammaS = factors.gammaS;
            gammaC = factors.gammaC;

            double VRdc = ULS_ScinanieBeton(NEd, factors);

            double d = (H - A1) / 1000;         // wysokosc uzyteczna przekroju w metrach
            double Asw = this.MyStirrups.Asw / 1000 / 1000;
            double s = this.MyStirrups.Swd / 1000;
            double z = 0.9 * d;           // ramię sił wewnętrznych - trzeba sprawdzić jak to analizować, dla czystego zginania z=0.9d
            double bw = B / 1000;
            double fywd = 0.8 * CurrentSteel.fyk / gammaS;
            double fcd = CurrentConcrete.fck / gammaC * alfaCC;

            double cotQ;
            if (NEd >= 0)
            {
                cotQ = 2.0;
            }
            else
                cotQ = 1.25;
            
            double VRds = Asw / s * z * fywd * cotQ;            // w meganiotonach

            double alfaCW = 1;

            double alfa = this.MyStirrups.Alfa / (2 * Math.PI);

            double ni1;
            if (CurrentConcrete.fck <= 60)
            {
                ni1 = 0.54 * (1 - 0.5 * Math.Cos(alfa));
            }
            else
            {
                ni1 = Math.Max((0.84 - CurrentConcrete.fck / 200) * (1 - 0.5 * Math.Cos(alfa)), 0.5);
            }

            double VRdMax = alfaCW * bw * z * ni1 * fcd / (cotQ + 1/cotQ);      //w meganiotonach

            if (this.MyStirrups.Alfa != 90.0)
            {
                VRds = Asw / s * z * fywd * (cotQ + (1 / Math.Tan(alfa))) * Math.Sin(alfa);
                VRdMax = alfaCW * bw * z * ni1 * fcd * (cotQ + (1 / Math.Tan(alfa))) / (1 + Math.Pow(cotQ, 2.0));
            }

            double VRd = Math.Min(VRds, VRdMax);
            return VRd * 1000;
        }
        
		protected override double SrCiezkPrzekr(double x)
		{
            double B = this.B / Dimfactor;
            double H = this.H / Dimfactor;
			double As1 = this.As1/Dimfactor/Dimfactor;
			double As2 = this.As2/Dimfactor/Dimfactor;
			double A1 = this.A1/Dimfactor;
			double A2 = this.A2/Dimfactor;
			
			double A_II = AlfaE * (As1 + As2) + B * x;       // sprowadzone pole powierzchni przekroju w m2
			//sprowadzony moment statyczny przekroju względem górnej krawędzi przekroju w m3
			double S = B * x * x / 2 + AlfaE * (As1 * (H - A1) + As2 * A2);
			// wysokosc srodka ciezkosci przekroju od gornej krawedzi przekroju w m
			double xc = S / A_II;
			return xc;
		}
		
		protected override double MomBezwPrzekr(double x)
		{
            double B = this.B / Dimfactor;
            double H = this.H / Dimfactor;
			double As1 = this.As1/Dimfactor/Dimfactor;
			double As2 = this.As2/Dimfactor/Dimfactor;
			double A1 = this.A1/Dimfactor;
			double A2 = this.A2/Dimfactor;
			
			double xc=SrCiezkPrzekr(x);
			// sprowadzony moment bezwladnosci przekroju wzgledem srodka ciezkosci! w m4
			double Iy = B * x * x * x / 12 + B * x * (xc - 0.5 * x) * (xc - 0.5 * x) +
			AlfaE * (As1 * (H - xc - A1) * (H - xc - A1) + (As2 * (xc - A2) * (xc - A2)));
			return Iy;
		}
		
		protected override double SprowPolePrzekr(double x)
		{
			double B = this.B/Dimfactor;
			double PoleBet = B*x;
			double PoleZbr = AsTotal/Dimfactor/Dimfactor;
			double PoleSpr = PoleBet+AlfaE*PoleZbr;
			return PoleSpr;
		}
			
		protected override double[] RzednePretowUpEdge()
		{
			double[] di = {A2/Dimfactor, (HTotal-A1)/Dimfactor};
			return di;
		}

        protected override double Crack_AcEff(double x)
        {
            double hcEff1 = 2.5 * A1;

            double hcEff2;
            if (x == 0)
            {
                hcEff2 = 0.5 * HTotal;
            }
            else
            {
                hcEff2 = (HTotal - x) / 3;
            }

            double hcEff = Math.Min(hcEff1, hcEff2);
            return hcEff * B;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
