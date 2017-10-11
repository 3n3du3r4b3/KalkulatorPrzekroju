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
    /// Interaction logic for CreepWindow.xaml
    /// </summary>
    public partial class CreepWindow : Window
    {
        double[] cr;

        string format = "0.###";

        public void Show(double[] ccr)
        {
            this.cr = ccr;
            this.Show();
            CoeffShow();
        }

        public CreepWindow()
        {
            InitializeComponent();
        }

        private void CoeffShow()
        {
            textBox_creep.Text = cr[1].ToString(format);
        }

    }
}