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
        double[] day;

        string format = "0.###";
        string formatd = "0";

        public void Show(double[] ccr, double[] dday)
        {
            this.cr = ccr;
            this.day = dday;
            this.Show();
            CoeffShow();
        }

        public CreepWindow()
        {
            InitializeComponent();
        }

        private void CoeffShow()
        {
            textBox_creep1.Text = cr[0].ToString(format);
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
            textBox_day10.Text = day[9].ToString(formatd);
        }

    }
}