using System;
using System.Runtime.Serialization.Formatters.Binary;
    
    namespace KalkulatorPrzekroju
{
    [Serializable]
    public class CreepParams
    {
        public double Acd { get; set; }
        public double fcm { get; set; }
        public string cem { get; set; }
        public double RH { get; set; }
        public double u { get; set; }
        public double t0 { get; set; }
        public double tend{ get; set; }
        public double cemcoeff { get; set; }
        public bool isBridge;
        public bool isSilicafume;
        public bool filled;

        public CreepParams(double Ac, double fcm, double RH, double u, double t00, double tend0, double cemcoeff0, bool bridge, bool sfume, bool filled)
        {
            this.Acd = Ac;
            this.fcm = fcm;
            this.RH = RH;
            this.u = u;
            this.t0 = t00;
            this.tend = tend0;
            this.cemcoeff = cemcoeff0;
            this.isBridge = bridge;
            this.isSilicafume = sfume;
            this.filled = true;
        }
        public CreepParams()
        {
        }

        public CreepParams(bool fil)
        {
            this.filled = fil;
        }
    }
}