using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using OxyPlot;
using OxyPlot.Series;
using Microsoft.Win32;

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : Window
    {
        Factors wspolczynniki;
        AllConcrete betony = new AllConcrete(AllConcrete.LoadType.DAT);
        Steel stal = new Steel(Steel.classes.B500B);
        Section section1;
        Section section2;

        string format = "0.##";

        public MainWindow()
        {
            InitializeComponent();
            //betony.saveToDatFile();
            wspolczynniki = new Factors(Factors.Settings.zachowane);
            SetControlls(); 
            Concrete bet = new Concrete(Concrete.classes.C12_15);
            Steel stl = new Steel(Steel.classes.B500A);
            Section sec = new Section(bet, stl, 1000, 1000, 40, 50, 40, 10, 100.0, 40);
            double nmax = SLS.GetSilaOsiowaKrytycznaRysa(sec, 0.2, 0.4, 0.8);
            //double mmax = SLS.GetMomentKrytycznyRysa(sec,-853.8253884230121, 0.2, 0.4, 0.8);
            StressState str = SLS.GetStresses(sec, -853.8253884230121, -337.59354350134885);
            double wkm1 = SLS.GetCrackWidth(sec, nmax, nmax, 0.4, 0.8);
            double wkm2 = SLS.GetCrackWidth(sec, nmax, 921, 0.4, 0.8);
        }

        private void SetControlls()
        {
            List<double> lista_srednic = LoadBarDiameters();
            foreach (var item in lista_srednic)
            {
                comboBox_diameter_As1_1.Items.Add(item);
                comboBox_diameter_As2_1.Items.Add(item);
                comboBox_diameter_As1_2.Items.Add(item);
                comboBox_diameter_As2_2.Items.Add(item);
                comboBox_diameter_AsStir_1.Items.Add(item);
                comboBox_diameter_AsStir_2.Items.Add(item);
            }
            comboBox_diameter_As1_1.SelectedIndex = 4;
            comboBox_diameter_As2_1.SelectedIndex = 4;
            comboBox_diameter_As1_2.SelectedIndex = 4;
            comboBox_diameter_As2_2.SelectedIndex = 4;
            comboBox_diameter_AsStir_1.SelectedIndex = 4;
            comboBox_diameter_AsStir_2.SelectedIndex = 4;

            comboBox_As1_spac_no_1.Items.Add("spacing");
            comboBox_As1_spac_no_1.Items.Add("no of bars");
            comboBox_As2_spac_no_1.ItemsSource = comboBox_As1_spac_no_1.Items;
            comboBox_As1_spac_no_2.ItemsSource = comboBox_As1_spac_no_1.Items;
            comboBox_As2_spac_no_2.ItemsSource = comboBox_As1_spac_no_1.Items;

            comboBox_As1_spac_no_1.SelectedIndex = 0;
            comboBox_As2_spac_no_1.SelectedIndex = 0;
            comboBox_As1_spac_no_2.SelectedIndex = 0;
            comboBox_As2_spac_no_2.SelectedIndex = 0;

            foreach (var item in betony.concreteNames)
            {
                comboBox_Concrete_1.Items.Add(item);
                comboBox_Concrete_2.Items.Add(item);
            }
            comboBox_Concrete_1.SelectedIndex = 0;
            comboBox_Concrete_2.SelectedIndex = 0;

            foreach (var item in stal.steelNames)
            {
                comboBox_Steel_1.Items.Add(item);
                comboBox_Steel_2.Items.Add(item);
            }
            comboBox_Steel_1.SelectedIndex = 0;
            comboBox_Steel_2.SelectedIndex = 0;

            comboBox_DesignSituation_1.Items.Add("Accidental");
            comboBox_DesignSituation_1.Items.Add("Persistent & Transient");
            comboBox_DesignSituation_2.ItemsSource = comboBox_DesignSituation_1.Items;
            comboBox_DesignSituation_1.SelectedIndex = 1;
            comboBox_DesignSituation_2.SelectedIndex = 1;
        }
        // załadowanie średnic pretow z pliku
        private List<double> LoadBarDiameters()
        {
            List<double> lista = new List<double>();
            try
            {
                using (StreamReader input = new StreamReader(@"data/bar_diameters.txt"))
                {
                    string line;
                    while ((line = input.ReadLine()) != null)
                    {
                        double diameter;
                        Double.TryParse(line, out diameter);
                        lista.Add(diameter);
                    }
                }
                lista.Sort();

            }
            catch (Exception)
            {
                MessageBox.Show("Nie udało się wczytać pliku.", "Loading failed",
                    MessageBoxButton.OK, MessageBoxImage.Warning, MessageBoxResult.OK);
            }
            return lista;
        }


        /// OPROGRAMOWANIE KONTROLEK
        private void comboBox_As1_spac_no_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_As1_spac_no_1.SelectedIndex == 0)
            {
                label_spac_no_As1_1.Visibility = Visibility.Visible;
            }
            else if (comboBox_As1_spac_no_1.SelectedIndex == 1)
            {
                label_spac_no_As1_1.Visibility = Visibility.Hidden;
            }
        }

        private void comboBox_As2_spac_no_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_As2_spac_no_1.SelectedIndex == 0)
            {
                label_spac_no_As2_1.Visibility = Visibility.Visible;
            }
            else if (comboBox_As2_spac_no_1.SelectedIndex == 1)
            {
                label_spac_no_As2_1.Visibility = Visibility.Hidden;
            }
        }

        private void comboBox_As1_spac_no_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_As1_spac_no_2.SelectedIndex == 0)
            {
                label_spac_no_As1_2.Visibility = Visibility.Visible;
            }
            else if (comboBox_As1_spac_no_2.SelectedIndex == 1)
            {
                label_spac_no_As1_2.Visibility = Visibility.Hidden;
            }
        }

        private void comboBox_As2_spac_no_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox_As2_spac_no_2.SelectedIndex == 0)
            {
                label_spac_no_As2_2.Visibility = Visibility.Visible;
            }
            else if (comboBox_As2_spac_no_2.SelectedIndex == 1)
            {
                label_spac_no_As2_2.Visibility = Visibility.Hidden;
            }
        }
        
        //oprogramowanie menu
        private void MenuItemSettingsFactors_Click(object sender, RoutedEventArgs e)
        {
            WindowFactors settingsWindow = new WindowFactors();
            settingsWindow.Show(wspolczynniki);
        }

        private void MenuItemSettingsDisplay_Click(object sender, RoutedEventArgs e)
        {
            Window_DisplaySet displaySettingsWindow = new Window_DisplaySet();
            displaySettingsWindow.Show();
        }

        private void menuItem_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuItem_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == true)
            {
                using (Stream output = File.Create(saveFileDialog1.FileName))
                {
                    //BinaryFormatter formatter = new BinaryFormatter();
                    //formatter.Serialize(output, this);
                }
            }
        }

        //kontrola wprowadzania danych przez uzytkownika
        private void textBox_width_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_width_1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_height_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_height_1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_cover_As1_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_cover_As1_1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_cover_As2_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_cover_As2_1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_spac_no_As1_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_spac_no_As1_1;
            if (comboBox_As1_spac_no_1.SelectedIndex == 0)
            {
                double input;
                Double.TryParse(tb.Text, out input);
                tb.Text = input.ToString(format);
            }
            else if (comboBox_As1_spac_no_1.SelectedIndex == 1)
            {
                int input;
                Int32.TryParse(tb.Text, out input);
                tb.Text = input.ToString(format);
            }

        }

        private void textBox_spac_no_As2_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_spac_no_As2_1;
            if (comboBox_As2_spac_no_1.SelectedIndex == 0)
            {
                double input;
                Double.TryParse(tb.Text, out input);
                tb.Text = input.ToString(format);
            }
            else if (comboBox_As2_spac_no_1.SelectedIndex == 1)
            {
                int input;
                Int32.TryParse(tb.Text, out input);
                tb.Text = input.ToString(format);
            }
        }

        private void textBox_width_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_width_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_height_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_height_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_cover_As1_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_cover_As1_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_cover_As2_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_cover_As2_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_spac_no_As1_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_spac_no_As1_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_spac_no_As2_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_spac_no_As2_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_legs_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_legs_2;
            int input;
            Int32.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_stir_spacing_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_stir_spacing_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_stir_angle_2_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_stir_angle_2;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_legs_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_legs_1;
            int input;
            Int32.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_stir_spacing_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_stir_spacing_1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_stir_angle_1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_stir_angle_1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_RH_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_RH;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_u_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_u;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_Cst_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_Cst;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_Cse_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_Cse;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        //PRZYCISKI
        private void button_UpdateGraph_Click(object sender, RoutedEventArgs e)
        {
            if (comboBox_As2_spac_no_1.Text == "spacing")
            {
                section1 = new Section(
                   new Concrete((Concrete.classes)comboBox_Concrete_1.SelectedIndex),
                   new Steel((Steel.classes)comboBox_Steel_1.SelectedIndex),
                   Double.Parse(textBox_width_1.Text),
                   Double.Parse(textBox_height_1.Text),
                   Double.Parse(comboBox_diameter_As1_1.Text),
                   Double.Parse(textBox_spac_no_As1_1.Text),
                   Double.Parse(textBox_cover_As1_1.Text),
                   Double.Parse(comboBox_diameter_As2_1.Text),
                   Double.Parse(textBox_spac_no_As2_1.Text),
                   Double.Parse(textBox_cover_As2_1.Text)
                   );
            }
            else if (comboBox_As2_spac_no_1.Text == "no of bars")
            {
                section1 = new Section(
                   new Concrete((Concrete.classes)comboBox_Concrete_1.SelectedIndex),
                   new Steel((Steel.classes)comboBox_Steel_1.SelectedIndex),
                   Double.Parse(textBox_width_1.Text),
                   Double.Parse(textBox_height_1.Text),
                   Double.Parse(comboBox_diameter_As1_1.Text),
                   Int32.Parse(textBox_spac_no_As1_1.Text),
                   Double.Parse(textBox_cover_As1_1.Text),
                   Double.Parse(comboBox_diameter_As2_1.Text),
                   Int32.Parse(textBox_spac_no_As2_1.Text),
                   Double.Parse(textBox_cover_As2_1.Text)
                   );
            }
            else
                section1 = null;

            if (comboBox_As2_spac_no_2.Text == "spacing")
            {
                section2 = new Section(
                   new Concrete((Concrete.classes)comboBox_Concrete_2.SelectedIndex),
                   new Steel((Steel.classes)comboBox_Steel_2.SelectedIndex),
                   Double.Parse(textBox_width_2.Text),
                   Double.Parse(textBox_height_2.Text),
                   Double.Parse(comboBox_diameter_As1_2.Text),
                   Double.Parse(textBox_spac_no_As1_2.Text),
                   Double.Parse(textBox_cover_As1_2.Text),
                   Double.Parse(comboBox_diameter_As2_2.Text),
                   Double.Parse(textBox_spac_no_As2_2.Text),
                   Double.Parse(textBox_cover_As2_2.Text)
                   );
            }
            else if (comboBox_As2_spac_no_2.Text == "no of bars")
            {
                section2 = new Section(
                   new Concrete((Concrete.classes)comboBox_Concrete_2.SelectedIndex),
                   new Steel((Steel.classes)comboBox_Steel_2.SelectedIndex),
                   Double.Parse(textBox_width_2.Text),
                   Double.Parse(textBox_height_2.Text),
                   Double.Parse(comboBox_diameter_As1_2.Text),
                   Int32.Parse(textBox_spac_no_As1_2.Text),
                   Double.Parse(textBox_cover_As1_2.Text),
                   Double.Parse(comboBox_diameter_As2_2.Text),
                   Int32.Parse(textBox_spac_no_As2_2.Text),
                   Double.Parse(textBox_cover_As2_2.Text)
                   );
            }
            else
                section1 = null;

            Stirrups stirrups1 = new Stirrups(
                Int32.Parse(textBox_legs_1.Text),
                Double.Parse(comboBox_diameter_AsStir_1.Text),
                section1.currentSteel,
                Double.Parse(textBox_stir_spacing_1.Text),
                Double.Parse(textBox_stir_angle_1.Text)
                );

            Stirrups stirrups2 = new Stirrups(
                Int32.Parse(textBox_legs_2.Text),
                Double.Parse(comboBox_diameter_AsStir_2.Text),
                section2.currentSteel,
                Double.Parse(textBox_stir_spacing_2.Text),
                Double.Parse(textBox_stir_angle_2.Text)
                );

            double[][] tab1_ULS = ULS.GetULS_MN_Curve(
                section1,
                (ULS.DesignSituation)comboBox_DesignSituation_1.SelectedIndex,
                wspolczynniki.NoOfPoints
                );

            double[][] tab2_ULS = ULS.GetULS_MN_Curve(
                section2,
                (ULS.DesignSituation)comboBox_DesignSituation_2.SelectedIndex,
                wspolczynniki.NoOfPoints
                );

            double[][] tabSLS_Crack = SLS.GetSLS_Crack_Curve(
                section1,
                wspolczynniki.NoOfPoints,
                0.2,
                0.4,
                wspolczynniki.Crack_k1
                );

            double[][] tabSLS_NonCrack = SLS.GetSLS_Crack_Curve(
                section1,
                wspolczynniki.NoOfPoints,
                0,
                0.4,
                wspolczynniki.Crack_k1
                );

            double[][] tabVRdc1 = ULS.GetULS_VRdcN_Curve(
                section1,
                (ULS.DesignSituation)comboBox_DesignSituation_1.SelectedIndex,
                wspolczynniki.NoOfPoints
                );

            double[][] tabVRd1 = ULS.GetULS_VRdN_Curve(
                section1,
                (ULS.DesignSituation)comboBox_DesignSituation_1.SelectedIndex,
                wspolczynniki.NoOfPoints,
                stirrups1
                );

            double[][] tabSLS_SteelStress = SLS.GetSLS_StressSteel_Curve(
                     section1,
                     wspolczynniki.NoOfPoints,
                     0.8
                     );
            
            double[][] tabSLS_ConcreteStress = SLS.GetSLS_StressConcrete_Curve(
                     section1,
                     wspolczynniki.NoOfPoints,
                     0.6
                     );
                   
            MainPlotView diagram1 = new MainPlotView();
            diagram1.AddLineSerie(tab1_ULS, "Section 1");
            diagram1.AddLineSerie(tab2_ULS, "Section 2");
            MainPlotView diagram2 = new MainPlotView();
            diagram2.AddLineSerie(tabSLS_Crack, "Section 1 - wk = 0.2mm");
            diagram2.AddLineSerie(tabSLS_NonCrack, "Section 1 - non-cracked");
            MainPlotView diagram3 = new MainPlotView();
            diagram3.AddLineSerie(tabSLS_ConcreteStress, "Section 1 - Concrete stress");
            diagram3.AddLineSerie(tabSLS_SteelStress, "Section 1 - Steel stress");
            MainPlotView diagramVN = new MainPlotView();
            diagramVN.AddLineSerie(tabVRdc1, "Section 1 - VRd.c");
            diagramVN.AddLineSerie(tabVRd1, "Section 1 - VRd.s");

            /* MyModel.Axes.Add(new OxyPlot.Axes.LinearAxis
                { Position=OxyPlot.Axes.AxisPosition.None, Minimum = -2000, Maximum = 2000 });
            MyModel.Axes.Add(new OxyPlot.Axes.LinearAxis
                { Position = OxyPlot.Axes.AxisPosition.None, Minimum = -1000, Maximum = 8000 });    */
            /*
        MyModel.Series.Add(punkty);
        MyModel.Series.Add(punkty2);
        punkty.Color = OxyColors.Red;
        punkty.StrokeThickness = 1;
        punkty2.MarkerSize = 2; */
            PlotView_ULS_MN.Model = diagram1.wykres;
            PlotView_SLS_Crack.Model = diagram2.wykres;
            PlotView_ULS_VN.Model = diagramVN.wykres;
            PlotView_SLS_Stresess.Model = diagram3.wykres;
            diagram1.SetGraph();
        }

        private void button_Creep1_Click(object sender, RoutedEventArgs e)
        {
            //double[] crCoeff = { 40, 1, 100, 10000 };
            double divide = 10;
            double[] cr = new double[Convert.ToInt32(divide)];
            double[] day = new double[Convert.ToInt32(divide)];
            for (double i = 1; i <= divide; i++)
            {
                cr[Convert.ToInt32(i-1)] = CreepCoefficient.CreepCoefficientCalc(section1, Double.Parse(textBox_RH.Text), Double.Parse(textBox_u.Text), Double.Parse(textBox_Cst.Text), Double.Parse(textBox_Cst.Text) + ((i - 1) / (divide - 1)) * (Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text)),0);
                day[Convert.ToInt32(i - 1)] = Double.Parse(textBox_Cst.Text) + ((i - 1) / (divide - 1)) * (Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text));
            }
            CreepWindow crWindow = new CreepWindow();
            crWindow.Show(cr,day);
        }

        private void button_Creep2_Click(object sender, RoutedEventArgs e)
        {
            //double[] crCoeff = { 40, 1, 100, 10000 };
            double divide = 10;
            double[] cr = new double[Convert.ToInt32(divide)];
            double[] day = new double[Convert.ToInt32(divide)];
            for (double i = 1; i <= divide; i++)
            {
                cr[Convert.ToInt32(i-1)] = CreepCoefficient.CreepCoefficientCalc(section2, Double.Parse(textBox_RH.Text), Double.Parse(textBox_u.Text), Double.Parse(textBox_Cst.Text), Double.Parse(textBox_Cst.Text) + ((i -1)/ (divide-1)) * (Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text)),0);
                day[Convert.ToInt32(i - 1)] = Double.Parse(textBox_Cst.Text) + ((i - 1) / (divide - 1)) * (Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text));
            }
            CreepWindow crWindow = new CreepWindow();
            crWindow.Show(cr,day);
        }

        // KONIEC OPROGRAMOWANIA KONTROLEK
    }
}
