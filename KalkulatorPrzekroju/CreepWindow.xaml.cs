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
        double cem = 0;
        //public Section section;

        string format = "0.###";
        string formatd = "0";

        public void Show(double Acd, double fcmd)
        {
            this.Ac = Acd;
            this.fcm = fcmd;
            this.Show();
            CoeffShow();
        }

        public CreepWindow()
        {
            InitializeComponent();

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

        private void button_Creep_Click(object sender, RoutedEventArgs e)
        {
            //double[] crCoeff = { 40, 1, 100, 10000 };
            double divide = 10;
            double[] cr = new double[Convert.ToInt32(divide)];
            double[] day = new double[Convert.ToInt32(divide)];
            for (double i = 1; i <= divide; i++)
            {
                cr[Convert.ToInt32(i - 1)] = CreepCoefficient.CreepCoefficientCalc(Ac, fcm, Double.Parse(textBox_RH.Text), Double.Parse(textBox_u.Text), Double.Parse(textBox_Cst.Text), Double.Parse(textBox_Cst.Text) + ((i - 1) / (divide - 1)) * (Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text)), cem);
                day[Convert.ToInt32(i - 1)] = Double.Parse(textBox_Cst.Text) + ((i - 1) / (divide - 1)) * (Double.Parse(textBox_Cse.Text) - Double.Parse(textBox_Cst.Text));
            }
        }

        private void CoeffShow()
        {
            /*textBox_creep1.Text = cr[0].ToString(format);
            textBox_creep2.Text = cr[1].ToString(format);
            textBox_creep3.Text = cr[2].ToString(format);
            textBox_creep4.Text = cr[3].ToString(format);
            textBox_creep5.Text = cr[4].ToString(format);
            textBox_creep6.Text = cr[5].ToString(format);
            textBox_creep7.Text = cr[6].ToString(format);
            textBox_creep8.Text = cr[7].ToString(format);
            textBox_creep9.Text = cr[8].ToString(format);
            textBox_creep10.Text = cr[9].ToString(format);

            textBox_day1.Text = day[0].ToString(formatd);
            textBox_day2.Text = day[1].ToString(formatd);
            textBox_day3.Text = day[2].ToString(formatd);
            textBox_day4.Text = day[3].ToString(formatd);
            textBox_day5.Text = day[4].ToString(formatd);
            textBox_day6.Text = day[5].ToString(formatd);
            textBox_day7.Text = day[6].ToString(formatd);
            textBox_day8.Text = day[7].ToString(formatd);
            textBox_day9.Text = day[8].ToString(formatd);
            textBox_day10.Text = day[9].ToString(formatd);*/
        }

        private void button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void radio_199211_Checked(object sender, RoutedEventArgs e)
        {
            bridge = false;
        }

        private void radio_19922_Checked(object sender, RoutedEventArgs e)
        {
            bridge = true;
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
            if (comboBox_Cement.SelectedItem == "N") { cem = 0; }
            else if (comboBox_Cement.SelectedItem == "R") { cem = 1; }
            else cem = -1;
        }
    }
}