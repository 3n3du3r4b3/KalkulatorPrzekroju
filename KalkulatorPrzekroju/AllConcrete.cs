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
    public class AllConcrete
    {
        public List<string> concreteNames = new List<string>();
        public List<double[]> concreteData = new List<double[]>();

        public AllConcrete(LoadType type)
        {
        	if (type == LoadType.CSV) {
        		readConcreteDataFromCSV();
        	}
        	else if (type == LoadType.DAT) {
        		readConcreteDataFromDAT();
        	}
        }
        
        public enum LoadType { CSV=1, DAT=2}

        private void readConcreteDataFromCSV()
        {
            using (StreamReader sr = new StreamReader(@"data/concrete.csv"))
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
                                concreteNames.Add(dataLine[0]);
                            }
                            else
                            {
                                dataLine2[i - 1] = Double.Parse(dataLine[i]);
                            }
                        }
                        concreteData.Add(dataLine2);
                    }
                    counter++;
                }
            }
        }

        private void readConcreteDataFromDAT()
        {
            using (Stream input = File.OpenRead(@"data/concrete.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                AllConcrete betony = (AllConcrete)formatter.Deserialize(input);
                concreteNames = betony.concreteNames;
                concreteData = betony.concreteData;
            }
        }
        
        public void saveToDatFile()
        {
            using (Stream output = File.Create(@"data/concrete.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(output, this);
            }
        }
    }
}
