using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KalkulatorPrzekroju
{
    [Serializable]
    public class Factors
    {
        public enum Settings { domyslne, zachowane }

        /// <summary>
        /// Factor defining the effective height of the compression zone [p. 3.1.7 EN-1992-1-1]
        /// </summary>
        public double Lambda { get; set; }
        /// <summary>
        /// Factor defining the effective strength [p. 3.1.7 EN-1992-1-1]
        /// </summary>
        public double Eta { get; set; }

        public double GammaC_PermAndTrans { get; set; }

        public double GammaC_Accidental { get; set; }

        public double GammaS_PermAndTrans { get; set; }

        public double GammaS_Accidental { get; set; }

        public double AlfaCC { get; set; }

        public double AlfaCT { get; set; }

        public double Stresses_k1 { get; set; }

        public double Stresses_k3 { get; set; }

        public double Crack_k1 { get; set; }

        public double Crack_k3 { get; set; }

        public double Crack_k4 { get; set; }

        public double Crack_wklim { get; set; }

        public double Crack_kt { get; set; }

        public int NoOfPoints { get; set; }

        public Factors(Settings settings)
        {
            if (settings == Settings.domyslne)
            {
                SetDefault();
            }
            else if (settings == Settings.zachowane)
            {
                Factors zapisane;
                try
                {
                    using (Stream input = File.OpenRead(@"data/factors.dat"))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        zapisane = (Factors)formatter.Deserialize(input);
                    }

                    NoOfPoints = zapisane.NoOfPoints;
                    Lambda = zapisane.Lambda;
                    Eta = zapisane.Eta;
                    GammaC_Accidental = zapisane.GammaC_Accidental;
                    GammaC_PermAndTrans = zapisane.GammaC_PermAndTrans;
                    GammaS_Accidental = zapisane.GammaS_Accidental;
                    GammaS_PermAndTrans = zapisane.GammaS_PermAndTrans;
                    AlfaCC = zapisane.AlfaCC;
                    AlfaCT = zapisane.AlfaCT;
                    Stresses_k1 = zapisane.Stresses_k1;
                    Stresses_k3 = zapisane.Stresses_k3;
                    Crack_k1 = zapisane.Crack_k1;
                    Crack_k3 = zapisane.Crack_k3;
                    Crack_k4 = zapisane.Crack_k4;
                    Crack_wklim = zapisane.Crack_wklim;
                    Crack_kt = zapisane.Crack_kt;
                }
                catch (Exception)
                {
                    MessageBox.Show("Nie udało się wczytać pliku.", "Loading failed", 
                        MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
                }
            }
        }

        public void SaveToFile()
        {
            using (Stream output = File.Create(@"data/factors.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(output, this);
            }
        }

        public void SetDefault()
        {
            this.NoOfPoints = 20;
            this.Lambda = 0.8;
            this.Eta = 1.0;
            this.GammaC_Accidental = 1.2;
            this.GammaC_PermAndTrans = 1.5;
            this.GammaS_Accidental = 1.0;
            this.GammaS_PermAndTrans = 1.15;
            this.AlfaCC = 0.85;
            this.AlfaCT = 1.0;
            this.Stresses_k1 = 0.6;
            this.Stresses_k3 = 0.8;
            this.Crack_k1 = 0.8;
            this.Crack_k3 = 3.4;
            this.Crack_k4 = 0.425;
            this.Crack_wklim = 0.3;
            this.Crack_kt = 0.4;    
        }
    }
}
