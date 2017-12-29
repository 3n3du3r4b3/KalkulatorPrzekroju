using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// klasa reprezentująca stan naprężeń w przekroju oraz wysokość strefy ściskanej
    /// </summary>
    class StressState
    {
    	/// <summary>
    	/// Naprężenia na górnej krawędzi betonu w MPa
    	/// </summary>
        public double ConcreteTopStress { private set; get; }
        /// <summary>
    	/// Naprężenia na dolnej krawędzi betonu dla fazy 1, lub naprężenia na dolnej krawędzi strefy ściskanej, w MPa
    	/// </summary>
        public double ConcreteBottomStress { private set; get; }
        /// <summary>
    	/// Naprężenia w zbrojeniu w MPa
    	/// </summary>
        public double[] SteelStress { private set; get; }
        /// <summary>
    	/// Wysokość strefy ściskanej przekroju w metrach
    	/// </summary>
        public double WysokośćStrefySciskanej { private set; get; }
        /// <summary>
        /// Mimośród przekroju względem geometrycznego środka przekroju w metrach. 
        /// Wartość dodatnia w kierunku zbrojenia As2 (powiększa dodatni moment przekroju)
        /// </summary>
        public double Mimosrod { private set; get; }
        /// <summary>
    	/// Faza w któtrej znajduje się przekrój
    	/// </summary>
        public int Faza { private set; get; }

        /// <summary>
        /// stan naprężeń w przekroju oraz wysokość strefy ściskanej
        /// </summary>
        /// <param name="sigmaConcreteTop">naprężenia na górnej krawędzi betonu w MPa</param>
        /// <param name="sigmaConcreteBottom">naprężenia na dolnej krawędzi betonu ściskanego lub dolnej krawędzi strefy ściskanej</param>
        /// <param name="sigmaSteelAs1">naprężenia w zbrojeniu As1</param>
        /// <param name="sigmaSteelAs2">naprężenia w zbrojeniu As2</param>
        public StressState(double sigmaConcreteTop, double sigmaConcreteBottom, double[] sigmaSteel, double wysStrefySciskanej, double mimosrod, int faza)
        {
            ConcreteTopStress = sigmaConcreteTop;
            ConcreteBottomStress = sigmaConcreteBottom;
            SteelStress = sigmaSteel;
            WysokośćStrefySciskanej = wysStrefySciskanej;
            Mimosrod = mimosrod;
            Faza = faza;
        }
        
        public double TopSteelStress(Section sec)
        {
            if (sec is RectangleSection)
            {
                return SteelStress[0];
            }
            else if (sec is CircleSection)
            {
                return SteelStress[0];
            }
            else
                return 0;
        }
        
        public double BottomSteelStress(Section sec)
        {
            if (sec is RectangleSection)
            {
                return SteelStress[1];
            }
            else if (sec is CircleSection)
            {
                return SteelStress[sec.NoB/2];
            }
            else
                return 0;
        }

    }
}
