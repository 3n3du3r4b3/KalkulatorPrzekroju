using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Factors wspolczynniki;
        AllConcrete betony = new AllConcrete(AllConcrete.LoadType.DAT);
        Steel stal = new Steel(Steel.classes.B500B);
        Concrete beton1;
        Concrete beton2;
        string format = "0.##";
        public MainWindow()
        {
            InitializeComponent();

            wspolczynniki = new Factors(Factors.Settings.zachowane);
            SetControlls();
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
            }
            comboBox_diameter_As1_1.SelectedIndex = 0;
            comboBox_diameter_As2_1.SelectedIndex = 0;
            comboBox_diameter_As1_2.SelectedIndex = 0;
            comboBox_diameter_As2_2.SelectedIndex = 0;

            comboBox_As1_spac_no_1.Items.Add("spacing");
            comboBox_As1_spac_no_1.Items.Add("no of bars");
            comboBox_As2_spac_no_1.Items.Add("spacing");
            comboBox_As2_spac_no_1.Items.Add("no of bars");
            comboBox_As1_spac_no_2.Items.Add("spacing");
            comboBox_As1_spac_no_2.Items.Add("no of bars");
            comboBox_As2_spac_no_2.Items.Add("spacing");
            comboBox_As2_spac_no_2.Items.Add("no of bars");
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


        // OPROGRAMOWANIE KONTROLEK
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


        //PRZYCISKI
        private void button_UpdateGraph_Click(object sender, RoutedEventArgs e)
        {
            PlotModel MyModel = new PlotModel();
            IList<DataPoint> Points = new List<DataPoint>();
            DataPoint punkt = new DataPoint(1.25, 2.5);
            DataPointSeries punkty;

            //MyModel.Series.Add(Points);
            PlotView_ULS_MS.Model = MyModel;
        }



        // KONIEC OPROGRAMOWANIA KONTROLEK
    }
}
