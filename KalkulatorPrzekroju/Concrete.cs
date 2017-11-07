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
        /// Potęga krzywizny
        /// </summary>
        public double n { get; private set; }

        public enum classes { C12_15 = 0, C16_20 = 1, C20_25 = 2, C25_30 = 3, C30_37 = 4, C35_45 = 5, C40_50 = 6, C45_55 = 7, C50_60 = 8, C55_67 = 9, C60_75 = 10, C70_85 = 11, C80_95 = 12, C90_105 = 13, }

        public Concrete(classes klasa)
        {
            switch ((int)klasa)
            {
                case 0:
                    fck = 12;
                    Name = "C12/15";
                    break;
                case 1:
                    fck = 16;
                    Name = "C16/20";
                    break;
                case 2:
                    fck = 20;
                    Name = "C20/25";
                    break;
                case 3:
                    fck = 25;
                    Name = "C25/30";
                    break;
                case 4:
                    fck = 30;
                    Name = "C30/37";
                    break;
                case 5:
                    fck = 35;
                    Name = "C35/45";
                    break;
                case 6:
                    fck = 40;
                    Name = "C40/50";
                    break;
                case 7:
                    fck = 45;
                    Name = "C45/55";
                    break;
                case 8:
                    fck = 50;
                    Name = "C50/60";
                    break;
                case 9:
                    fck = 55;
                    Name = "C55/67";
                    break;
                case 10:
                    fck = 60;
                    Name = "C60/75";
                    break;
                case 11:
                    fck = 70;
                    Name = "C70/85";
                    break;
                case 12:
                    fck = 80;
                    Name = "C80/95";
                    break;
                case 13:
                    fck = 90;
                    Name = "C90/105";
                    break;
                default:
                    fck = 0;
                    Name = "error";
                    break;
            }

            SetFactors(fck);
        }

        public Concrete(double fck)
        {
            SetFactors(fck);
            Name = "fck = " + String.Format(fck.ToString(), "0.##");
        }

        private void SetFactors(double fck)
        {
            fcm = fck + 8;

            if (fck <= 50)
            {
                fctm = 0.30 * Math.Pow(fck, 2 / 3);
                epsilon_cu1 = 3.5;
                epsilon_c2 = 2.0;
                epsilon_cu2 = 3.5;
                epsilon_c3 = 1.75;
                epsilon_cu3 = 3.5;
                n = 2.0;
            }
            else
            {
                fctm = 2.12 * Math.Log(1 + 0.1 * fcm);
                epsilon_cu1 = 2.8 + 27 * Math.Pow(0.01 * (98 - fcm), 4);
                epsilon_c2 = 2.0 + 0.085 * Math.Pow(fck - 50, 0.53);
                epsilon_cu2 = 3.5 + 35 * Math.Pow(0.01 * (90 - fck), 4);
                epsilon_c3 = 1.75 + 0.01375 * (fck - 50);
                epsilon_cu3 = 2.6 + 35 * Math.Pow(0.01 * (90 - fck), 4);
                n = 1.4 + 23.4 * Math.Pow(0.01 * (90 - fck), 4);
            }

            fctk005 = 0.7 * fctm;
            fctk095 = 1.3 * fctm;
            Ecm = 22 * Math.Pow(0.1 * fcm, 0.3) * 1000;
            epsilon_c1 = Math.Min(0.7 * Math.Pow(fcm, 0.31), 2.8);
        }
    }
}
