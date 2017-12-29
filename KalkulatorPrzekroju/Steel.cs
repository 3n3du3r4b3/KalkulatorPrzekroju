using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


namespace KalkulatorPrzekroju
{
    [Serializable]
    class Steel:IComparable<Steel>
    {
        public string Name { get; private set; }
        public double fyk { get; private set; }
        public double Es { get; private set; }
        public double epsilon_uk { get; private set; }

        public List<string> steelNames = new List<string>();
        public List<double[]> steelData = new List<double[]>();

        public enum classes { B500A = 0, B500B = 1, B500C = 2 }

        public Steel(classes klasa)
        {
            readFromDAT();
            //readFromCSV();
            //saveToDatFile();
            int i = (int)klasa;
            this.Name = steelNames[i];
            this.fyk = steelData[i][0];
            this.Es = steelData[i][1] * 1000;
            this.epsilon_uk = steelData[i][2];
        }

        private void readFromCSV()
        {
            using (StreamReader sr = new StreamReader(@"data/steel.csv"))
            {
                string line;
                int counter = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (counter != 0)
                    {
                        string[] dataLine;
                        dataLine = line.Split(new char[] { ';' });
                        double[] dataLine2 = new double[dataLine.Length - 1];
                        for (int i = 0; i < dataLine.Length; i++)
                        {
                            if (i == 0)
                            {
                                steelNames.Add(dataLine[0]);
                            }
                            else
                            {
                                dataLine2[i - 1] = Double.Parse(dataLine[i]);
                            }
                        }
                        steelData.Add(dataLine2);
                    }
                    counter++;
                }
            }
        }

        private void readFromDAT()
        {
            try
            {
                using (Stream input = File.OpenRead(@"data/steel.dat"))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    Steel stale = (Steel)formatter.Deserialize(input);
                    steelNames = stale.steelNames;
                    steelData = stale.steelData;
                }
            }
            catch (Exception)
            {

            }
            
        }

        public void saveToDatFile()
        {
            using (Stream output = File.Create(@"data/steel.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(output, this);
            }
        }

        public int CompareTo(Steel other)
        {
            if (this.fyk == other.fyk)
            {
                return 0;
            }
            else if (this.fyk < other.fyk)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }

        public override bool Equals(object obj)
        {
            Steel other = obj as Steel;
            if (this.fyk == other.fyk)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        public double SigmaS(double epsilon, double fyd)
        {
            if (Math.Abs(epsilon) / 1000 < fyd / Es)
            {
                return epsilon * Es / 1000;
            }
            else
            {
                return Math.Abs(epsilon) / epsilon * fyd;
            }

        }

    }
}
