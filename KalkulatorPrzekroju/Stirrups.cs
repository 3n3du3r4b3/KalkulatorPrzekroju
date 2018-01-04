using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{/// <summary>
/// Klasa reprezentująca strzemiona
/// </summary>
    [Serializable]
    class Stirrups
    {
        /// <summary>
        /// Ilość ramion strzemion
        /// </summary>
        public int Legs { private set; get; }
        /// <summary>
        /// Średnica prętów strzemion w mm
        /// </summary>
        public double Fi { private set; get; }
        /// <summary>
        /// Pole powierzchni strzemion w mm2
        /// </summary>
        public double Asw { private set; get; }
        /// <summary>
        /// Stal zbrojeniowa strzemion
        /// </summary>
        public Steel CurrentSteel { private set; get; }
        /// <summary>
        /// Rozstaw podłużny strzemion w mm
        /// </summary>
        public double Swd { private set; get; }
        /// <summary>
        /// Kąt strzemion w stosunku do osi elementu w stopniach
        /// </summary>
        public double Alfa { private set; get; }

        /// <summary>
        /// Konstruktor klasy reprezentującej strzemiona
        /// </summary>
        /// <param name="legs">Ilość ramion strzemion</param>
        /// <param name="fi">Średnica prętów strzemion</param>
        /// <param name="steel">Stal zbrojeniowa strzemion</param>
        /// <param name="s">Rozstaw podłużny strzemion w mm</param>
        /// <param name="alfa">Kąt strzemion w stosunku do osi elementu w stopniach</param>
        public Stirrups(int legs, double fi, Steel steel, double s, double alfa)
        {
            Legs = legs;
            Fi = fi;
            Asw = Math.PI * (Fi / 2) * (Fi / 2) * Legs;
            CurrentSteel = steel;
            Swd = s;
            Alfa = alfa;
        }

        public override bool Equals(Object obj)
        {
            Stirrups other = obj as Stirrups;
            if (this.Legs == other.Legs &&
                this.Fi == other.Fi &&
                Equals(this.CurrentSteel, other.CurrentSteel) &&
                this.Swd == other.Swd &&
                this.Alfa == other.Alfa)
            {
                return true;
            }
            else
                return false;
        }
    }
}
