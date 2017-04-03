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
    	/// Naprężenia w zbrojeniu dolnym w MPa
    	/// </summary>
        public double SteelAs1Stress { private set; get; }
        /// <summary>
    	/// Naprężenia w zbrojeniu górnym w MPa
    	/// </summary>
        public double SteelAs2Stress { private set; get; }
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
        public StressState(double sigmaConcreteTop, double sigmaConcreteBottom, double sigmaSteelAs1, double sigmaSteelAs2, double wysStrefySciskanej, double mimosrod, int faza)
        {
            ConcreteTopStress = sigmaConcreteTop;
            ConcreteBottomStress = sigmaConcreteBottom;
            SteelAs1Stress = sigmaSteelAs1;
            SteelAs2Stress = sigmaSteelAs2;
            WysokośćStrefySciskanej = wysStrefySciskanej;
            Mimosrod = mimosrod;
            Faza = faza;
        }
    }
}
