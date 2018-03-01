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
            ReadFactors();
            this.ShowDialog();
            
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

            textBox_ULS_Shear_teta.Text = factors.Shear_Teta.ToString(format);
            label_Cot_teta.Content = "Current cot(θ) = " + Math.Tan(Math.PI / 2 - factors.Shear_Teta / 360 * 2 * Math.PI).ToString("0.##");

            textBox_crack_k1.Text = factors.Crack_k1.ToString(format);
            textBox_crack_k3.Text = factors.Crack_k3.ToString(format);
            textBox_crack_k4.Text = factors.Crack_k4.ToString(format);
            textBox_crack_limit.Text = factors.Crack_wklim.ToString(format);

            if (factors.Crack_kt == 0.4)
                radioButton_long.IsChecked = true;
            else
                radioButton_short.IsChecked = true;

            textBox_stress_k1.Text = factors.Stresses_k1.ToString(format);
            textBox_stress_k3.Text = factors.Stresses_k3.ToString(format);

            textBox_general_NoOfPoints.Text = factors.NoOfPoints.ToString();

            if (factors.Annex == NationalAnnex.BS)
            {
                RadioButton_NA_BS.IsChecked = true;
            }
            else if (factors.Annex == NationalAnnex.EN)
            {
                RadioButton_NA_EN.IsChecked = true;
            }
            else if (factors.Annex == NationalAnnex.PN)
            {
                RadioButton_NA_PN.IsChecked = true;
            }
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
            wspolczynniki.Shear_Teta = Double.Parse(textBox_ULS_Shear_teta.Text);
            wspolczynniki.NoOfPoints = Int32.Parse(textBox_general_NoOfPoints.Text);
            wspolczynniki.Stresses_k1 = Double.Parse(textBox_stress_k1.Text);
            wspolczynniki.Stresses_k3 = Double.Parse(textBox_stress_k3.Text);

            if ((bool)radioButton_long.IsChecked)
                wspolczynniki.Crack_kt = 0.4;
            else
                wspolczynniki.Crack_kt = 0.6;

            if (RadioButton_NA_EN.IsChecked == true)
            {
                wspolczynniki.Annex = NationalAnnex.EN;
            }
            else if (RadioButton_NA_BS.IsChecked == true)
            {
                wspolczynniki.Annex = NationalAnnex.BS;
            }
            else if (RadioButton_NA_PN.IsChecked == true)
            {
                wspolczynniki.Annex = NationalAnnex.PN;
            }


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

        private void textBox_ULS_Shear_teta_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = sender as TextBox; //textBox_ULS_Shear_teta;
            double input;
            double cotMax = 2.5;

            if (RadioButton_NA_EN.IsChecked == true)
            {
                cotMax = 2.5;
            }
            else if (RadioButton_NA_BS.IsChecked == true)
            {
                cotMax = 2.5;
            }
            else if (RadioButton_NA_PN.IsChecked == true)
            {
                cotMax = 2.0;
            }
            
            Double.TryParse(tb.Text, out input);
            if (Math.Tan(Math.PI / 2 - input / 360 * 2 * Math.PI) < 1.0)
            {
                input = (Math.PI / 2 - Math.Atan(1.0)) / (2 * Math.PI) * 360;
            }
            if (Math.Tan(Math.PI / 2 - input / 360 * 2 * Math.PI) > 2.5)
            {
                input = (Math.PI / 2 - Math.Atan(cotMax)) / (2 * Math.PI) * 360;
            }
            tb.Text = input.ToString(format);
            label_Cot_teta.Content = "Current cot(θ) = " + Math.Tan(Math.PI / 2 - input / 360 * 2 * Math.PI).ToString("0.##");
        }

        private void RadioButton_NA_Checked(object sender, RoutedEventArgs e)
        {
            textBox_ULS_Shear_teta_LostFocus(textBox_ULS_Shear_teta, e);
        }
        // koniec kontroli wprowadzania danych przez użytkownika
    }
}
