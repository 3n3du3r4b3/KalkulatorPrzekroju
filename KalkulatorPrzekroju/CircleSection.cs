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
        public double fiB { get; private set; }
        /// <summary>
        /// Otulina zbrojenia w mm
        /// </summary>
        public double c { get; private set; }
        /// <summary>
        /// Odległość środka ciężkości zbrojenia od krawędzi w mm
        /// </summary>
        public double a { get; private set; }
        /// <summary>
        /// Ilość prętów zbrojenia w przekroju
        /// </summary>
        public int noB { get; private set; }
        /// <summary>
    	/// Całkowite pole powierzchni przekroju betonowego w mm2
    	/// </summary>
        public new double AcTotal { get; private set; }

        /// <summary>
        /// Całkowite pole powierzchni zbrojenia w mm2
        /// </summary>
        public new double AsTotal { get; private set; }

        /// <summary>
        /// Obiekt typu Concrete reprezentujący klasę betonu w przekroju
        /// </summary>
        public new Concrete currentConrete { get; private set; }

        /// <summary>
        /// Obiekt typu Steel reprezentujący klasę stali w przekroju
        /// </summary>
        public new Steel currentSteel { get; private set; }

        /// <summary>
        /// Współczynnik pełzania
        /// </summary>
        public new double fi { get; set; }

        /// <summary>
        /// Zwraca aktualny przekrój obrócony o 180 stopni
        /// </summary>
        public new CircleSection reversedSection { get { return new CircleSection(currentConrete, currentSteel, D, fiB, c, noB); } }


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
            this.fiB = fiB;
            this.c = c;
            
            if (noB < 4)
            	this.noB = 4;
            else
            	this.noB = noB;
            
            this.currentConrete = concrete;
            this.currentSteel = steel;
            this.Ab = (fiB/2)*(fiB/2)*Math.PI;
            this.a = c+fiB/2;
            fi = 0;
            this.AcTotal = (D/2)*(D/2)*Math.PI;
            this.AsTotal = Ab * noB;
        }

        public override bool Equals(object obj)
        {
            CircleSection s2 = obj as CircleSection;

            if (this.D == s2.D &&
                this.fiB == s2.fiB &&
                this.c == s2.c &&
                this.noB == s2.noB &&
                Equals(this.currentConrete, s2.currentConrete) &&
                Equals(this.currentSteel, s2.currentSteel) &&
                this.fi == s2.fi &&
                this.Ab == s2.Ab &&
                this.a == s2.a &&
                this.fi == s2.fi)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
