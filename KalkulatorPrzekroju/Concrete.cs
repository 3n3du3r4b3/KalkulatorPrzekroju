using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace KalkulatorPrzekroju
{
    public class Concrete
    {
    	/// <summary>
    	/// Nazwa klasy betonu
    	/// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Wytrzymałość charakterystyczna betonu na ściskanie w MPa
        /// </summary>
        public double fck { get; private set; }
        /// <summary>
        /// Średnia wytrzymałość betonu na ściskanie w MPa
        /// </summary>
        public double fcm { get; private set; }
        /// <summary>
        /// Średnia wytrzymałość betonu na rozciąganie w MPa
        /// </summary>
        public double fctm { get; private set; }
        /// <summary>
        /// Wytrzymałość charakterystyczna betonu na rozciąganie w MPa (kwantyl 5%)
        /// </summary>
        public double fctk005 { get; private set; }
        /// <summary>
        /// Wytrzymałość charakterystyczna betonu na rozciąganie w MPa (kwantyl 95%)
        /// </summary>
        public double fctk095 { get; private set; }
        /// <summary>
        /// Średni moduł sprężystości betonu w MPa
        /// </summary>
        public double Ecm { get; private set; }
        /// <summary>
        /// Odkształcenie betonu przy ściskaniu wywołane maksymalnym naprężeniem fc, w promilach
        /// </summary>
        public double epsilon_c1 { get; private set; }
        /// <summary>
        /// Odkształcenie graniczne betonu przy którym osiąga się wytrzymałość betonu, w promilach
        /// </summary>
        public double epsilon_cu1 { get; private set; }
        /// <summary>
        /// Odkształcenie betonu przy ściskaniu wywołane maksymalnym naprężeniem fc
        /// (dla wykresu sigma-epsilon parabola-prostokąt), w promilach
        /// </summary>
        public double epsilon_c2 { get; private set; }
        /// <summary>
        /// Odkształcenie graniczne betonu przy którym osiąga się wytrzymałość betonu 
        /// (dla wykresu sigma-epsilon parabola-prostokąt), w promilach
        /// </summary>
        public double epsilon_cu2 { get; private set; }
        /// <summary>
        /// Odkształcenie betonu przy ściskaniu wywołane maksymalnym naprężeniem fc
        /// (dla wykresu sigma-epsilon bilinear), w promilach
        /// </summary>
        public double epsilon_c3 { get; private set; }
        /// <summary>
        /// Odkształcenie graniczne betonu przy którym osiąga się wytrzymałość betonu 
        /// (dla wykresu sigma-epsilon bilinear), w promilach
        /// </summary>
        public double epsilon_cu3 { get; private set; }
        /// <summary>
        /// Współczynnik określający efektywną wysokość strefy ściskanej (dla prostokątnego rozkłądu naprężeń)
        /// </summary>
        public double eta { get; private set; }
        /// <summary>
        /// Współczynnik określający efektywną wytrzymałość betonu (dla prostokątnego rozkłądu naprężeń)
        /// </summary>
        public double chi { get; private set; }

        public enum classes {C12_15=0, C16_20=1, C20_25=2, C25_30=3, C30_37=4, C35_45=5, C40_50=6, C45_55=7, C50_60=8, C55_67=9, C60_75=10, C70_85=11, C80_95=12, C90_105=13, }
        
        public Concrete(classes klasa)
        {
        	int i = (int)klasa;
        	AllConcrete betony = new AllConcrete(AllConcrete.LoadType.DAT);
        	this.Name = betony.concreteNames[i];
            fck = betony.concreteData[i][0];
            fcm = betony.concreteData[i][1];
            fctm = betony.concreteData[i][2];
            fctk005 = betony.concreteData[i][3];
            fctk095 = betony.concreteData[i][4];
            Ecm = betony.concreteData[i][5] * 1000;
            epsilon_c1 = betony.concreteData[i][6];
            epsilon_cu1 = betony.concreteData[i][7];
            epsilon_c2 = betony.concreteData[i][8];
            epsilon_cu2 = betony.concreteData[i][9];
            epsilon_c3 = betony.concreteData[i][10];
            epsilon_cu3 = betony.concreteData[i][11];
            eta = betony.concreteData[i][12];
            chi = betony.concreteData[i][13];
        }
    }
}
