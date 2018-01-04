using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KalkulatorPrzekroju
{
    [Serializable]
    class CircleSection:Section
    {
        /// <summary>
        /// Średnica przekroju w mm
        /// </summary>
        public double D { get; private set; }
        /// <summary>
    	/// Pole powierzchni przekroju pojedynczego pręta Ab w mm2
    	/// </summary>
        public double Ab { get; private set; }
        /// <summary>
        /// Średnica prętów zbrojeniowych w mm
        /// </summary>
        public double FiB { get; private set; }
        /// <summary>
        /// Otulina zbrojenia w mm
        /// </summary>
        public double C { get; private set; }
        /// <summary>
        /// Odległość środka ciężkości zbrojenia od krawędzi w mm
        /// </summary>
        public double A { get; private set; }
        /// <summary>
        /// Odległość pomiędzy prętami zbrojenia w linii prostej w mm
        /// </summary>
        public double Spacing { get { return D * Math.Sin(2 * Math.PI / NoB); } }

        public override double HTotal { get {return D;}}

        public override int NoB { get; protected set; }

        public override double[] Asi
        {
            get
            {
                double[] Asi = new double[NoB];
                for (int i = 0; i < NoB; i++)
                {
                    Asi[i] = Ab / Dimfactor / Dimfactor;
                }
                return Asi;
            }
        }

        public override double AcTotal { get { return Math.PI * Math.Pow(D / 2, 2); } }

        public override double AsTotal { get { return NoB * Ab; } }

        public override Section ReversedSection { get { return new CircleSection(CurrentConcrete, CurrentSteel, D, FiB, C, NoB); } }

        /// <summary>
        /// Konstruktor przekroju
        /// </summary>
        /// <param name="concrete">Obiekt reprezentujący klasę betonu dla przekroju</param>
        /// <param name="steel">Obiekt reprezentujący klasę stali zbrojeniowej w przekroju</param>
        /// <param name="d">Średnica przekroju w milimetrach</param>
        /// <param name="fiB">Średnica prętów zbrojenia w mm</param>
        /// <param name="c">Otulina zbrojenia w mm</param>
        /// <param name="noB">Ilość prętów zbrojenia w przekroju - MINIMUM 4 sztuki!</param>
        public CircleSection(Concrete concrete, Steel steel, double d, double fiB, double c, int noB)
        {
            this.D = d;
            this.FiB = fiB;
            this.C = c;
            
            this.NoB = noB;
            
            CurrentConcrete = concrete;
            CurrentSteel = steel;
            this.Ab = (fiB/2)*(fiB/2)*Math.PI;
            this.A = c+fiB/2;
            SetCreepFactor(0);
        }

        public override bool Equals(object obj)
        {
            CircleSection s2 = obj as CircleSection;

            if (this.D == s2.D &&
                this.FiB == s2.FiB &&
                this.C == s2.C &&
                this.NoB == s2.NoB &&
                Equals(this.CurrentConcrete, s2.CurrentConcrete) &&
                Equals(this.CurrentSteel, s2.CurrentSteel) &&
                this.Fi == s2.Fi &&
                this.Ab == s2.Ab &&
                this.A == s2.A &&
                this.Fi == s2.Fi)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Zwraca pole powierzchni odcinka koła
        /// </summary>
        /// <param name="x">Wysokość odcinka koła (wysokość strefy ściskanej) w m</param>
        /// <returns>Pole powierzchni odcinka koła w m2</returns>
        double PoleOK(double x){
        	double R = D/2/1000;
        	double alfa = 2.0 * Math.Acos((R-x)/R);
        	double pole = R*R/2*(alfa - Math.Sin(alfa));
        	return pole;
        }

        /// <summary>
        /// Zwraca długość cięciwy na wysokości x od góry przekroju
        /// </summary>
        /// <param name="x">Wysokość odcinka koła (wysokość strefy ściskanej) w m</param>
        /// <returns>Długość cięciwy na wysokości x od góry przekroju w m</returns>
        double DlugoscOK(double x)
        {
            double R = D / 2 / 1000;
            double alfa = 2.0 * Math.Acos((R - x) / R);
            double dlugosc = 2 * R * Math.Sin(alfa / 2);
            return dlugosc;
        }

        /// <summary>
        /// Zwraca odległość środka ciężkosci odcinka koła od środka koła
        /// </summary>
        /// <param name="x">Wysokość odcinka koła (wysokość strefy ściskanej) w m</param>
        /// <returns>Odległość środka ciężkosci odcinka koła od środka koła w m</returns>
        double SrCzOK(double x){
        	double R = D/2/1000;
        	if (x<=0) {
        		return R;
        	}
        	double alfa = 2 * Math.Acos((R-x)/R);
        	double e = 4.0/3*R*Math.Pow(Math.Sin(alfa/2),3)/(alfa-Math.Sin(alfa));
        	return e;
        }
        
        /// <summary>
        /// Zwraca moment bezwładności odcinka koła względem głownej osi y
        /// </summary>
        /// <param name="x">Wysokość odcinka koła (wysokość strefy ściskanej) w m</param>
        /// <returns>Moment bezwładności odcinka koła względem głownej osi y w m4</returns>
        double MomBezwOK(double x){
        	if (x<=0) {
        		return 0;
        	}
        	double R = D/2/1000;
        	double alfa = 2 * Math.Acos((R-x)/R);
        	double Iy = R*R*R*R/16*(2*alfa - Math.Sin(2*alfa)) - R*R*R*R/9*Math.Pow(1-Math.Cos(alfa),3)/(alfa-Math.Sin(alfa));
        	return Iy;
        }
        
        /// <summary>
        /// Zwraca tablicę z rzędnymi prętów od środka koła w m
        /// </summary>
        /// <returns>Tablica z rzędnymi prętów od środka koła w m</returns>
        double[] RzednePretowCent(){
        	double R = D/2/1000;
        	double rAs = R-A/1000;
        	double[] di = new double[NoB];
        	for (int i = 0; i < NoB; i++) {
            	double beta = 2.0 * Math.PI/NoB * ((double)(NoB%2+1)/2 + i);
            	di[i] = Math.Cos(beta) * rAs;
            }
        	return di;
        }
        
        protected override double[] RzednePretowUpEdge(){
        	double R = D/2/1000;
        	double rAs = R-A/1000;
        	double[] di = new double[NoB];
        	for (int i = 0; i < NoB; i++) {
            	double beta = 2.0 * Math.PI/NoB * ((double)(NoB%2+1)/2 + i);
            	di[i] = A/1000 + (1-Math.Cos(beta)) * rAs;
            }
        	return di;
        }
        
        /// <summary>
        /// Zwraca tablicę z momentami bezwładności prętów względnem środka koła w m4
        /// </summary>
        /// <returns>Tablica z momentami bezwładności prętów względnem środka koła w m4</returns>
        double[] MomBezwPretow(){
        	double[] di = RzednePretowCent();
        	double fi = FiB/1000;
        	double A = Ab / 1000/1000;
        	double R = D/2/1000;
        	double rAs = R- this.A/1000;
        	double[] Iyi = new double[NoB];
        	for (int i = 0; i < NoB; i++) {
        		Iyi[i] = Math.PI*Math.Pow(fi/2,4)/4 + A*Math.Pow(di[i],2);
            }
        	return Iyi;
        }
        
		protected override double SrCiezkPrzekr(double x){
			double Ec = CurrentConcrete.Ecm;
			double Es = CurrentSteel.Es;
			double fi = this.Fi;
			double alfa = (1+fi)*Es/Ec;
			double R = D/2/1000;
			double Ap = Ab/1000/1000;
			double PoleBet = PoleOK(x);
			double e = SrCzOK(x);
			double MSbet = PoleBet * (R - e);
			double[] di = RzednePretowCent();
			double PoleZbr=0;
			double MSzbr=0;
			for (int i = 0; i < NoB; i++) {
				MSzbr += Ap * (R-di[i]);
				PoleZbr += Ap;
			}
			double srC = (MSbet+alfa*MSzbr)/(PoleBet+alfa*PoleZbr);
			return srC;
		}
		
		protected override double MomBezwPrzekr(double x){
			double Ec = CurrentConcrete.Ecm;
			double Es = CurrentSteel.Es;
			double fi = this.Fi;
			double alfa = (1+fi)*Es/Ec;
			double sc = SrCiezkPrzekr(x);
			double R = D/2/1000;
			double Ap = Ab/1000/1000;
			double MomBet = MomBezwOK(x) + PoleOK(x)*Math.Pow(sc-(R-SrCzOK(x)),2);
			double MomZbr=0;
			double MomPret = Math.PI* Math.Pow(FiB/1000/2,4)/4;
			double[] di = RzednePretowCent();
			for (int i = 0; i < NoB; i++) {
				MomZbr += MomPret + Ap*Math.Pow(sc-(R+di[i]),2);
			}
			double MomC = MomBet + alfa*MomZbr;
			return MomC;
		}
		
		protected override double SprowPolePrzekr(double x)
		{
			double R = D/2/Dimfactor;
			double PoleBet = PoleOK(x);
			double PoleZbr = AsTotal/Dimfactor/Dimfactor;
			double PoleSpr = PoleBet+AlfaE*PoleZbr;
			return PoleSpr;
		}

        protected override double Crack_AcEff(double x)
        {
            double hcEff1 = 2.5 * A;

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
            return PoleOK(hcEff);
        }
        
        public override double ULS_MomentKrytyczny(double NEd, DesignSituation situation)
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
            
            double D = this.D / 1000;        // w metrach
            double a = A / 1000;      // w metrach
            double Ab = this.Ab / 1000000;     // w metrach kwadratowych
            double fiB = FiB / 1000;
            int noB = NoB;

            double fyk = CurrentSteel.fyk;          // w MPa
            double Es = CurrentSteel.Es;            // w MPa
            //double Ecm = currentConcrete.Ecm;       // w MPa
            double fck = CurrentConcrete.fck;       // w MPa
            double fcd = fck / gammaC * alfaCC;     // w MPa
            double fyd = fyk / gammaS;
            double n = CurrentConcrete.n;

            double eps = 0.00001;
            double epsilonC2 = CurrentConcrete.epsilon_c2;
            double epsilonCU2 = CurrentConcrete.epsilon_cu2;
            double x1 = 0;
            double x2 = 100000 * D;

            double d; //wysokość użyteczna, zależna od wysokości strefy ściskanej x
            double As; //pole zbrojenia rozciąganego, zależne od wysokości strefy ściskanej x

            //obliczenia parametrów geometryczny dla przekroju okragłego
            double R = D / 2;		//promień przekroju
            double rAs = R - a;	//promień okręgu po którym rozmieszczone są pręty
            double[] di; //tablica z odległościami prętów od górnej krawędzi przekroju (ściskanej)
            bool[] ki; //tablica okreslajaca czy dany pret jest rozciagany (true), ściskany(false)

            di = RzednePretowUpEdge();

            double Pc = 0;
            double PAs1 = 0;
            double PAs2 = 0;
            double x, range;
            int k = 100;
            double NRd = 0;
            //d = di.Max();

            NEd = NEd / 1000;

            do
            {
                x = (x1 + x2) / 2;
                Pc = 0;

                //określenie wysokości użytecznej d która jest zależna od x
                As = 0; // sumaryczne pole powierzchni zbrojenia rozciaganego
                double Asd = 0; //moment statyczny zbrojenia rozciaganego wzgledem gornej krawedzi przekroju

                ki = CzyPretRozciagany(x);
                
                for (int i = 0; i < noB; i++)
                {
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
                    double riT = d - range / k * (i);
                    double riB = d - range / k * (i + 1);
                    Pc += CurrentConcrete.SigmaC(fcd, EpsilonR(ri, x, d)) * (DlugoscOK(d - riT) + DlugoscOK(d - riB)) / 2 * range / k;
                }

                double[] PAsi = new double[noB];
                PAs1 = 0;
                PAs2 = 0;
                for (int i = 0; i < noB; i++)
                {
                    PAsi[i] = Ab * CurrentSteel.SigmaS(EpsilonR(d - di[i], x, d), fyd);
                    if (!ki[i])
                    {
                        PAs2 += PAsi[i];
                    }
                    else
                    {
                        PAs1 += Ab * CurrentSteel.SigmaS(EpsilonR(0, x, d), fyd);
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

            } while (Math.Abs(x1 - x2) > eps);

            double Pcz = 0;
            for (int i = 0; i < k; i++)
            {
                double ri = d - range / k * (i + 0.5);
                double sC = CurrentConcrete.SigmaC(fcd, EpsilonR(ri, x, d));
                Pcz += sC * DlugoscOK(d - ri) * (range / k) * ri;
            }

            double[] MAs2i = new double[noB];
            double MAs2 = 0;
            for (int i = 0; i < noB; i++)
            {
                MAs2i[i] = Convert.ToUInt32(!ki[i]) * Ab * CurrentSteel.SigmaS(EpsilonR(d - di[i], x, d), fyd) * (d - di[i]);
                MAs2 += MAs2i[i];
            }

            double Ms1 = Pcz + MAs2;

            double MRd = Ms1 - NEd * (d - 0.5 * D);

            return MRd * 1000;
        }
        
        public override double ULS_ScinanieBeton(double NEd, DesignSituation situation)
        {
            return 0;
        }
        
        public override double ULS_ScinanieTotal(double NEd, DesignSituation situation)
        {
        	return 0;
        }
        
    }
}
