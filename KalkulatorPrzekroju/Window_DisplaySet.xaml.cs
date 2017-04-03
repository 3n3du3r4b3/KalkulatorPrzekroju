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
    /// Interaction logic for Window_DisplaySet.xaml
    /// </summary>
    public partial class Window_DisplaySet : Window
    {
        public Window_DisplaySet()
        {
            InitializeComponent();
        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
