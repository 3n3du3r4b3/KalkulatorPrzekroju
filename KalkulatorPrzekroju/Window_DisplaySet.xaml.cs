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
using Microsoft.Win32;
using Xceed.Wpf.Toolkit;

namespace KalkulatorPrzekroju
{
    /// <summary>
    /// Interaction logic for Window_DisplaySet.xaml
    /// </summary>
    public partial class Window_DisplaySet : Window
    {
        MySettings mySettings;
        public Window_DisplaySet(MySettings set)
        {
            InitializeComponent();
            mySettings = set;
            /*Settings ustawienia = new Settings();
            ustawienia.SaveToFile();
            ustawienia.ReadFromFile();
            MyColor c = ustawienia.ULSMN_Section1LineColor;
            colorPicker_ULS_MN_Line1.SelectedColor = Color.FromArgb(c.A,c.R,c.G,c.B);*/
        }

        //  obsługa przycisków
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {   
            this.Close();
        }
    }
}
