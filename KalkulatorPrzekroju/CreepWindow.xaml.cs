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
using System.Windows.Shapes;
using OxyPlot;
using OxyPlot.Series;

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Interaction logic for CreepWindow.xaml
    /// </summary>
    public partial class CreepWindow : Window
    {
        double[] cr;
        double[] day;
        double Ac;
        double fcm;
        bool bridge;
        bool sfume;
        bool linear;
        int divide = 10;
        double cemcoeff;
        double crinput;
        public double CrCoeff;
        List<CreepAtDay> creepResults = new List<CreepAtDay>();

        string format = "0.###";

        Dictionary<string, double> cem = new Dictionary<string, double>()
        {
            {"S",-1},
            {"N",0},
            {"R",1}
        };

        private class CreepAtDay
        {
            public double day { get; set; }
            public double cr { get; set; }

            public CreepAtDay(double Ac, double fcm, double RH, double u, double cst, double day, double cemcoeff)
            {
                this.day = day;
                cr = CreepCoefficient.CreepCoefficientCalc(Ac, fcm, RH, u, cst, day, cemcoeff);
            }
        }

        public void Show(double Acd, double fcmd, double input)
        {
            Ac = Acd;
            fcm = fcmd;
            crinput = input;
            comboBox_Cement.ItemsSource = cem;
            ShowDialog();
        }

        public double Result()
        {
            return CrCoeff;
        }

        public CreepWindow()
        {
            InitializeComponent();
        }

        private double[] DayCalc(int div, bool lin)
        {
            double[] days = new double[div+1];
            double pow = Math.Pow((Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text)), (1/Convert.ToDouble(div)));
            if (lin)
            {
                for (double i = 0; i <= divide; i++)
                {
                    days[Convert.ToInt32(i)] = Double.Parse(textBox_Cst.Text) + (i / divide) * (Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text));
                }
            }
            else
            {
                for (double i = 0; i <= divide; i++)
                {
                    days[Convert.ToInt32(i)] = Math.Pow(pow,i) + Double.Parse(textBox_Cst.Text) - ((Convert.ToDouble(div) - i)/Convert.ToDouble(div));
                }
            }

            return days;
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

        private void textBox_Div_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_Div;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
            divide = Int32.Parse(textBox_Div.Text);
        }

        private void button_Creep_Click(object sender, RoutedEventArgs e)
        {
            creepResults.Clear();
            this.CreepResults.ItemsSource = null;
            double[] day = DayCalc(divide,linear);
            foreach (double i in day)
            {
                CreepAtDay temp = new CreepAtDay(Ac, fcm, Double.Parse(textBox_RH.Text), Double.Parse(textBox_u.Text), Double.Parse(textBox_Cst.Text), i, cemcoeff);
                creepResults.Add(temp);
            }
            this.CreepResults.ItemsSource = creepResults;
            CrCoeff = creepResults.Last().cr;
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            CrCoeff = crinput;
            Close();
        }

        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void radio_199211_Checked(object sender, RoutedEventArgs e)
        {
            bridge = false;
        }

        private void radio_19922_Checked(object sender, RoutedEventArgs e)
        {
            bridge = true;
        }

        private void radio_lin_Checked(object sender, RoutedEventArgs e)
        {
            linear = true;
        }

        private void radio_log_Checked(object sender, RoutedEventArgs e)
        {
            linear = false;
        }

        private void checkBox_sfume_Checked(object sender, RoutedEventArgs e)
        {
            sfume = true;
        }

        private void checkBox_sfume_Unchecked(object sender, RoutedEventArgs e)
        {
            sfume = false;
        }

        private void comboBox_Cement_SelectionChanged(object sender, RoutedEventArgs e)
        {
            cemcoeff = Convert.ToDouble(comboBox_Cement.SelectedValue);
        }
    }
}