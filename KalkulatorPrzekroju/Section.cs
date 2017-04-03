using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KalkulatorPrzekroju
{
    class Section
    {
    	/// <summary>
    	/// Szerokość przekroju w mm
    	/// </summary>
        public double b { get; private set; }
        /// <summary>
    	/// Wysokość przekroju w mm
    	/// </summary>
        public double h { get; private set; }
        /// <summary>
    	/// Odległość środka ciężkości zbrojenia As1 od najbliższej krawędzi betonu w mm
    	/// </summary>
        public double a1 { get; private set; }
        /// <summary>
    	/// Odległość środka ciężkości zbrojenia As2 od najbliższej krawędzi betonu w mm
    	/// </summary>
        public double a2 { get; private set; }
        /// <summary>
    	/// Pole powierzchni zbrojenia As1 w mm2
    	/// </summary>
        public double As1 { get; private set; }
        /// <summary>
    	/// Pole powierzchni zbrojenia As2 w mm2
    	/// </summary>
        public double As2 { get; private set; }
        /// <summary>
    	/// Obiekt typu Concrete reprezentujący klasę betonu w przekroju
    	/// </summary>
        public Concrete currentConrete { get; private set; }
        /// <summary>
    	/// Obiekt typu Steel reprezentujący klasę stali w przekroju
    	/// </summary>
        public Steel currentSteel { get; private set; }
        /// <summary>
        /// Średnica zbrojenia As1 w mm
        /// </summary>
        public double fi1 {get; private set;}
        /// <summary>
        /// Średnica zbrojenia As2 w mm
        /// </summary>
        public double fi2 {get; private set;}
        /// <summary>
        /// Otulina zbrojenia As1 w mm
        /// </summary>
        public double c1 {get; private set;}
        /// <summary>
        /// Otulina zbrojenia As2 w mm
        /// </summary>
        public double c2 {get; private set;}
        /// <summary>
        /// Rozstaw prętów zbrojenia As1 w mm
        /// </summary>
        public double spacing1 {get; private set;}
        /// <summary>
        /// Rozstaw prętów zbrojenia As2 w mm
        /// </summary>
        public double spacing2 {get; private set;}
        
        /*
        /// <summary>
    	/// Współczynnik częściowy betonu
    	/// </summary>
        public double gammaC { get; private set; }
        /// <summary>
    	/// Współczynnik stosowany w celu uwzględnienia efektów dłutrwałych oraz niekorzystnych wpływów, 
    	/// wynikających ze sposobu przyłożenia obciążenia (dla BS-EN 0.85, dla PN-EN 1.00)
    	/// </summary>
        public double alfaCC { get; private set; }
        /// <summary>
    	/// Współczynnik częściowy stali
    	/// </summary>
        public double gammaS { get; private set; }
        */
       
        /// <summary>
    	/// Zwraca aktualny przekrój obrócony o 180 stopni
    	/// </summary>
        public Section reversedSection { get { return new Section(currentConrete, currentSteel, b, h, fi2, spacing2, c2, fi1,spacing1,c1); } }
       
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
        /// <param name="spacing2">Rozstaw prętów zbrojenia As1 w mm</param>
        /// <param name="c2">Otulina zbrojenia As2 w mm</param>
        public Section(Concrete concrete, Steel steel, double b, double h, double fi1, double spacing1, double c1, double fi2, double spacing2, double c2)
        {
            this.b = b;
            this.h = h;
            this.fi1 = fi1;
            this.c1 = c1;
            this.spacing1=spacing1;
            this.fi2 = fi2;
            this.c2 = c2;
            this.spacing2=spacing2;
            currentConrete = concrete;
            currentSteel = steel;
            As1 = (fi1/2)*(fi1/2)*Math.PI*b/spacing1;
            As2 = (fi2/2)*(fi2/2)*Math.PI*b/spacing2;
            a1 = c1+0.5*fi1;
            a2 = c2+0.5*fi2;
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
        public Section(Concrete concrete, Steel steel, double b, double h, double fi1, int noOfBars1, double c1, double fi2, int noOfBars2, double c2)
        {
            this.b = b;
            this.h = h;
            this.fi1 = fi1;
            this.c1 = c1;
            this.spacing1=b/(1+noOfBars1);
            this.spacing2=b/(1+noOfBars2);
            this.fi2 = fi2;
            this.c2 = c2;
            currentConrete = concrete;
            currentSteel = steel;
            As1 = (fi1/2)*(fi1/2)*Math.PI*noOfBars1;
            As2 = (fi2/2)*(fi2/2)*Math.PI*noOfBars2;
            a1 = c1+0.5*fi1;
            a2 = c2+0.5*fi2;
        }
    }
}
