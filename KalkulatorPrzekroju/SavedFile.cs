using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalkulatorPrzekroju
{
    [Serializable]
    class SavedFile
    {
        public Section section1 { get; set; }
        public Section section2 { get; set; }

        public Stirrups stirrups1 { get; set; }
        public Stirrups stirrups2 { get; set; }

        public string section1_h { get; set; }
        public string section2_h { get; set; }
        public string section1_b { get; set; }
        public string section2_b { get; set; }
        public string section1_c1 { get; set; }
        public string section1_c2 { get; set; }
        public string section2_c1 { get; set; }
        public string section2_c2 { get; set; }


        public string section1_diameter { get; set; }
        public string section2_diameter { get; set; }
        public string section1_cover { get; set; }
        public string section2_cover { get; set; }

        public int section1_diameterBars { get; set; }
        public int section2_diameterBars { get; set; }
        public string section1_noOfBars { get; set; }
        public string section2_noOfBars { get; set; }

        public int diameter_As1_1 { get; set; }
        public int diameter_As2_1 { get; set; }
        public int diameter_As1_2 { get; set; }
        public int diameter_As2_2 { get; set; }
        public int section1_As1_noOfBars { get; set; }
        public int section1_As2_noOfBars { get; set; }
        public int section2_As1_noOfBars { get; set; }
        public int section2_As2_noOfBars { get; set; }
        public string spac_no_As1_1 { get; set; }
        public string spac_no_As2_1 { get; set; }
        public string spac_no_As1_2 { get; set; }
        public string spac_no_As2_2 { get; set; }

        public int concrete1 { get; set; }
        public int concrete2 { get; set; }

        public int steel1 { get; set; }
        public int steel2 { get; set; }

        public int section1DS { get; set; }
        public int section2DS { get; set; }

        public int diameter_stir_s1 { get; set; }
        public int diameter_stir_s2 { get; set; }
        public string legs_stir_s1 { get; set; }
        public string legs_stir_s2 { get; set; }
        public string spacing_stir_s1 { get; set; }
        public string spacing_stir_s2 { get; set; }
        public string angle_stir_s1 { get; set; }
        public string angle_stir_s2 { get; set; }

        public double[][] tabSLS_ConcreteStress { get; set; }
        public double[][] tabSLS_SteelStress { get; set; }
        public double[][] tabVRd1 { get; set; }
        public double[][] tabVRdc1 { get; set; }
        public double[][] tabSLS_NonCrack { get; set; }
        public double[][] tabSLS_Crack { get; set; }
        public double[][] tab2_ULS { get; set; }
        public double[][] tab1_ULS { get; set; }

        public List<CasePoint> points_MN { get; set; }
        public List<CasePoint> points_VN { get; set; }
        public List<CasePoint> points_SLS_QPR { get; set; }
        public List<CasePoint> points_SLS_CHR { get; set; }
        
        public string creep1 { get; set; }
        public string creep2 { get; set; }

        public bool consider4steel1 { get; set; }
        public bool consider4concrete1 { get; set; }
        public bool consider4crack1 { get; set; }
        public bool consider4steel2 { get; set; }
        public bool consider4concrete2 { get; set; }
        public bool consider4crack2 { get; set; }

    }
}
