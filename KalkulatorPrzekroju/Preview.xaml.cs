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
    /// Interaction logic for Preview.xaml
    /// </summary>
    public partial class Preview : Window
    {
        public Preview(TabItem ind)
        {
            string currentTab = ind.Header.ToString();
            InitializeComponent();
            this.Show();
            if (currentTab == "Secton 1") { Console.WriteLine("Good"); }
            else Console.WriteLine("Not Good");
        }
    }
}
