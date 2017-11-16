using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows;
using OxyPlot;

namespace KalkulatorPrzekroju
{
    public enum Source { domyslne, zapisane}

    [Serializable]
    public class MySettings
    {
        public MyColor ULSMN_Section1LineColor {get; set; }
        public int ULSMN_Section1LineWeight { get; set; }
        public MyColor ULSMN_Section2LineColor { get; set; }
        public int ULSMN_Section2LineWeight { get; set; }
        public MyColor ULSMN_DataPointColor { get; set; }
        public int ULSMN_DataPointWeight { get; set; }

        public MyColor ULSVN_VrdcLineColor { get; set; }
        public int ULSVN_VrdcLineWeight { get; set; }
        public MyColor ULSVN_VrdLineColor { get; set; }
        public int ULSVN_VrdLineWeight { get; set; }
        public MyColor ULSVN_DataPointColor { get; set; }
        public int ULSVN_DataPointWeight { get; set; }

        public MyColor SLS_Crack_LineColor { get; set; }
        public int SLS_Crack_LineWeight { get; set; }
        public MyColor SLS_Crack_DataPointColor { get; set; }
        public int SLS_Crack_DataPointWeight { get; set; }

        public MyColor SLS_SteelStress_LineColor { get; set; }
        public int SLS_SteelStress_LineWeight { get; set; }
        public MyColor SLS_ConcreteStress_LineColor { get; set; }
        public int SLS_ConcreteStress_LineWeight { get; set; }
        public MyColor SLS_Stress_DataPointColor { get; set; }
        public int SLS_Stress_DataPointWeight { get; set; }

        public MySettings(Source s)
        {
            if (s == Source.domyslne)
            {
                SetDefault();
            }
            else if (s == Source.zapisane)
            {
                ReadFromFile();
            }
            
        }

        public void SaveToFile()
        {
            using (Stream output = File.Create(@"data/settings.dat"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(output, this);
            }
        }

        public void ReadFromFile()
        {
            MySettings zapisane;
            try
            {
                using (Stream input = File.OpenRead(@"data/settings.dat"))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    zapisane = (MySettings)formatter.Deserialize(input);
                }
                this.SLS_ConcreteStress_LineColor = zapisane.SLS_ConcreteStress_LineColor;
                this.SLS_ConcreteStress_LineWeight = zapisane.SLS_ConcreteStress_LineWeight;
                this.SLS_Crack_DataPointColor = zapisane.SLS_Crack_DataPointColor;
                this.SLS_Crack_DataPointWeight = zapisane.SLS_Crack_DataPointWeight;
                this.SLS_Crack_LineColor = zapisane.SLS_Crack_LineColor;
                this.SLS_Crack_LineWeight = zapisane.SLS_Crack_LineWeight;
                this.SLS_SteelStress_LineColor = zapisane.SLS_SteelStress_LineColor;
                this.SLS_SteelStress_LineWeight = zapisane.SLS_SteelStress_LineWeight;
                this.SLS_Stress_DataPointColor = zapisane.SLS_Stress_DataPointColor;
                this.SLS_Stress_DataPointWeight = zapisane.SLS_Stress_DataPointWeight;
                this.ULSMN_DataPointColor = zapisane.ULSMN_DataPointColor;
                this.ULSMN_DataPointWeight = zapisane.ULSMN_DataPointWeight;
                this.ULSMN_Section1LineColor = zapisane.ULSMN_Section1LineColor;
                this.ULSMN_Section1LineWeight = zapisane.ULSMN_Section1LineWeight;
                this.ULSMN_Section2LineColor = zapisane.ULSMN_Section2LineColor;
                this.ULSMN_Section2LineWeight = zapisane.ULSMN_Section2LineWeight;
                this.ULSVN_DataPointColor = zapisane.ULSVN_DataPointColor;
                this.ULSVN_DataPointWeight = zapisane.ULSVN_DataPointWeight;
                this.ULSVN_VrdcLineColor = zapisane.ULSVN_VrdcLineColor;
                this.ULSVN_VrdcLineWeight = zapisane.ULSVN_VrdcLineWeight;
                this.ULSVN_VrdLineColor = zapisane.ULSVN_VrdLineColor;
                this.ULSVN_VrdLineWeight = zapisane.ULSVN_VrdLineWeight;
            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się wczytać pliku.", "Loading failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }
        }

        public void SetDefault()
        {
            ULSMN_Section1LineColor = new MyColor(OxyColors.Green);
            ULSMN_Section1LineWeight = 2;
            ULSMN_Section2LineColor = new MyColor(OxyColors.Red);
            ULSMN_Section2LineWeight = 2;
            ULSMN_DataPointColor = new MyColor(OxyColors.YellowGreen);
            ULSMN_DataPointWeight = 1;

            ULSVN_VrdcLineColor = new MyColor(OxyColors.DarkBlue);
            ULSVN_VrdcLineWeight = 2;
            ULSVN_VrdLineColor = new MyColor(OxyColors.LightBlue);
            ULSVN_VrdLineWeight = 2;
            ULSVN_DataPointColor = new MyColor(OxyColors.YellowGreen);
            ULSVN_DataPointWeight = 1;

            SLS_Crack_LineColor = new MyColor(OxyColors.DarkCyan);
            SLS_Crack_LineWeight = 2;
            SLS_Crack_DataPointColor = new MyColor(OxyColors.YellowGreen);
            SLS_Crack_DataPointWeight = 1;

            SLS_SteelStress_LineColor = new MyColor(OxyColors.DarkOrange);
            SLS_SteelStress_LineWeight = 2;
            SLS_ConcreteStress_LineColor = new MyColor(OxyColors.DarkGray);
            SLS_ConcreteStress_LineWeight = 2;
            SLS_Stress_DataPointColor = new MyColor(OxyColors.YellowGreen);
            SLS_Stress_DataPointWeight = 1;
        }
    }
}
