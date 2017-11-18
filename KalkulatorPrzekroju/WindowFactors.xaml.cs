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

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Interaction logic for WindowFactors.xaml
    /// </summary>
    public partial class WindowFactors : Window
    {
        Factors wspolczynniki;

        Factors factors;

        string format = "0.###";

        public void Show(Factors wsp)
        {
            this.wspolczynniki = wsp;
            this.factors = wspolczynniki;
            this.Show();
            
            ReadFactors();
        }

        public WindowFactors()
        {
            InitializeComponent();
        }

        private void ReadFactors()
        {
            textBox_gammaC_ACC.Text = factors.GammaC_Accidental.ToString(format);
            textBox_gammaC_PT.Text = factors.GammaC_PermAndTrans.ToString(format);
            textBox_gammaS_ACC.Text = factors.GammaS_Accidental.ToString(format);
            textBox_gammaS_PT.Text = factors.GammaS_PermAndTrans.ToString(format);
            textBox_alfaCC.Text = factors.AlfaCC.ToString(format);
            textBox_alfaCT.Text = factors.AlfaCT.ToString(format);

            textBox_crack_k1.Text = factors.Crack_k1.ToString(format);
            textBox_crack_k3.Text = factors.Crack_k3.ToString(format);
            textBox_crack_k4.Text = factors.Crack_k4.ToString(format);
            textBox_crack_limit.Text = factors.Crack_wklim.ToString(format);

            textBox_stress_k1.Text = factors.Stresses_k1.ToString(format);
            textBox_stress_k3.Text = factors.Stresses_k3.ToString(format);

            textBox_general_NoOfPoints.Text = factors.NoOfPoints.ToString();
        }
        

        // obsługa przycisków
        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            wspolczynniki.AlfaCC = Double.Parse(textBox_alfaCC.Text);
            wspolczynniki.AlfaCT = Double.Parse(textBox_alfaCT.Text);
            wspolczynniki.Crack_k1 = Double.Parse(textBox_crack_k1.Text);
            wspolczynniki.Crack_k3 = Double.Parse(textBox_crack_k3.Text);
            wspolczynniki.Crack_k4 = Double.Parse(textBox_crack_k4.Text);
            wspolczynniki.Crack_wklim = Double.Parse(textBox_crack_limit.Text);
            wspolczynniki.GammaC_Accidental = Double.Parse(textBox_gammaC_ACC.Text);
            wspolczynniki.GammaC_PermAndTrans = Double.Parse(textBox_gammaC_PT.Text);
            wspolczynniki.GammaS_Accidental = Double.Parse(textBox_gammaS_ACC.Text);
            wspolczynniki.GammaS_PermAndTrans = Double.Parse(textBox_gammaS_PT.Text);
            wspolczynniki.NoOfPoints = Int32.Parse(textBox_general_NoOfPoints.Text);
            wspolczynniki.Stresses_k1 = Double.Parse(textBox_stress_k1.Text);
            wspolczynniki.Stresses_k3 = Double.Parse(textBox_stress_k3.Text);

            wspolczynniki.SaveToFile();
        }

        private void buttonRestoreDef_Click(object sender, RoutedEventArgs e)
        {
            wspolczynniki.SetDefault();
            ReadFactors();
        }
        // koniec obsługi przycisków

        //kontrola wprowadzania danych przez użytkownika ->
        private void textBox_gammaC_PT_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_gammaC_PT;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_gammaS_PT_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_gammaS_PT;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_gammaC_ACC_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_gammaC_ACC;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_gammaS_ACC_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_gammaS_ACC;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_alfaCC_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_alfaCC;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_alfaCT_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_alfaCT;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_crack_k1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_crack_k1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_crack_k3_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_crack_k3;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_crack_k4_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_crack_k4;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_stress_k1_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_stress_k1;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_stress_k3_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_stress_k3;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_general_NoOfPoints_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_general_NoOfPoints;
            int input;
            Int32.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }

        private void textBox_crack_limit_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = textBox_crack_limit;
            double input;
            Double.TryParse(tb.Text, out input);
            tb.Text = input.ToString(format);
        }
        // koniec kontroli wprowadzania danych przez użytkownika
    }
}
