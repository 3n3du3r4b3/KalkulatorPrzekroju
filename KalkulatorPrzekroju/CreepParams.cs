using System;
using System.Runtime.Serialization.Formatters.Binary;
    
    namespace KalkulatorPrzekroju
{
    [Serializable]
    public class CreepParams
    {
        public double cemv { get; set; }
        public double RH { get; set; }
        public double u { get; set; }
        public double t0 { get; set; }
        public double tend{ get; set; }
        public bool isBridge;
        public bool isSilicafume;

        public CreepParams(double RH, double u, double t00, double tend0, double cem0, bool bridge, bool sfume)
        {
            this.RH = RH;
            this.u = u;
            this.cemv = cem0;
            this.t0 = t00;
            this.tend = tend0;
            this.isBridge = bridge;
            this.isSilicafume = sfume;
        }
        public CreepParams()
        {
        }
    }
}