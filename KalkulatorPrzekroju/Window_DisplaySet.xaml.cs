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
            mySettings = new MySettings(Source.zapisane);
            mySettings = set;
            /*Settings ustawienia = new Settings();
            ustawienia.SaveToFile();
            ustawienia.ReadFromFile();
            MyColor c = ustawienia.ULSMN_Section1LineColor;*/
            SetControls(mySettings);
        }

        //  obsługa przycisków
        private void button_Close_Click(object sender, RoutedEventArgs e)
        {   

            this.Close();
        }

        private void button_RestoreDef_Click(object sender, RoutedEventArgs e)
        {
            MySettings mySet = new MySettings(Source.domyslne);
            SetControls(mySet);
        }

        private void SetControls(MySettings mySettings)
        {
            colorPicker_ULS_MN_Line1.SelectedColor = mySettings.ULSMN_Section1LineColor.GetMedia();
            colorPicker_ULS_MN_Line2.SelectedColor = mySettings.ULSMN_Section2LineColor.GetMedia();
            colorPicker_ULS_MN_Points.SelectedColor = mySettings.ULSMN_DataPointColor.GetMedia();
            slider_sec1_ULS_MN.Value = mySettings.ULSMN_Section1LineWeight;
            slider_sec2_ULS_MN.Value = mySettings.ULSMN_Section2LineWeight;
            slider_points_ULS_MN.Value = mySettings.ULSMN_DataPointWeight;

            colorPicker_ULS_VN_Line1.SelectedColor = mySettings.ULSVN_VrdcLineColor.GetMedia();
            colorPicker_ULS_VN_Line2.SelectedColor = mySettings.ULSVN_VrdLineColor.GetMedia();
            colorPicker_ULS_VN_Points.SelectedColor = mySettings.ULSVN_DataPointColor.GetMedia();
            slider_line1_ULS_VN.Value = mySettings.ULSVN_VrdcLineWeight;
            slider_line2_ULS_VN.Value = mySettings.ULSVN_VrdLineWeight;
            slider_points_ULS_VN.Value = mySettings.ULSVN_DataPointWeight;

            colorPicker_SLS_Crack_Cracked_Line.SelectedColor = mySettings.SLS_Crack_Cracked_LineColor.GetMedia();
            colorPicker_SLS_Crack_NonCracked_Line.SelectedColor = mySettings.SLS_Crack_NonCracked_LineColor.GetMedia();
            colorPicker_SLS_Crack_Points.SelectedColor = mySettings.SLS_Crack_DataPointColor.GetMedia();
            slider_SLS_Crack_Cracked_Line.Value = mySettings.SLS_Crack_Cracked_LineWeight;
            slider_SLS_Crack_NonCracked_Line.Value = mySettings.SLS_Crack_NonCracked_LineWeight;
            slider_SLS_Crack_Points.Value = mySettings.SLS_Crack_DataPointWeight;

            colorPicker_SLS_ConcreteStress_Line.SelectedColor = mySettings.SLS_ConcreteStress_LineColor.GetMedia();
            colorPicker_SLS_SteelStress_Line.SelectedColor = mySettings.SLS_SteelStress_LineColor.GetMedia();
            colorPicker_SLS_Stress_DataPoint.SelectedColor = mySettings.SLS_Stress_DataPointColor.GetMedia();
            slider_SLS_ConcreteStress_Line.Value = mySettings.SLS_Crack_Cracked_LineWeight;
            slider_SLS_SteelStress_Line.Value = mySettings.SLS_Crack_NonCracked_LineWeight;
            slider_SLS_Stress_DataPoint.Value = mySettings.SLS_Crack_DataPointWeight;
        }

        private void ReadFromControls(MySettings mySettings)
        {
            mySettings.ULSMN_Section1LineColor = new MyColor((Color)colorPicker_ULS_MN_Line1.SelectedColor);
            mySettings.ULSMN_Section2LineColor = new MyColor((Color)colorPicker_ULS_MN_Line2.SelectedColor);
            mySettings.ULSMN_DataPointColor = new MyColor((Color)colorPicker_ULS_MN_Points.SelectedColor);
            mySettings.ULSMN_Section1LineWeight = slider_sec1_ULS_MN.Value;
            mySettings.ULSMN_Section2LineWeight = slider_sec2_ULS_MN.Value;
            mySettings.ULSMN_DataPointWeight = slider_points_ULS_MN.Value;

            mySettings.ULSVN_VrdcLineColor = new MyColor((Color)colorPicker_ULS_VN_Line1.SelectedColor);
            mySettings.ULSVN_VrdLineColor = new MyColor((Color)colorPicker_ULS_VN_Line2.SelectedColor);
            mySettings.ULSVN_DataPointColor = new MyColor((Color)colorPicker_ULS_VN_Points.SelectedColor);
            mySettings.ULSVN_VrdcLineWeight = slider_line1_ULS_VN.Value;
            mySettings.ULSVN_VrdLineWeight = slider_line2_ULS_VN.Value;
            mySettings.ULSVN_DataPointWeight = slider_points_ULS_VN.Value;

            mySettings.SLS_Crack_Cracked_LineColor = new MyColor((Color)colorPicker_SLS_Crack_Cracked_Line.SelectedColor);
            mySettings.SLS_Crack_NonCracked_LineColor = new MyColor((Color)colorPicker_SLS_Crack_NonCracked_Line.SelectedColor);
            mySettings.SLS_Crack_DataPointColor = new MyColor((Color)colorPicker_SLS_Crack_Points.SelectedColor);
            mySettings.SLS_Crack_Cracked_LineWeight = slider_SLS_Crack_Cracked_Line.Value;
            mySettings.SLS_Crack_NonCracked_LineWeight = slider_SLS_Crack_NonCracked_Line.Value;
            mySettings.SLS_Crack_DataPointWeight = slider_SLS_Crack_Points.Value;

            mySettings.SLS_ConcreteStress_LineColor = new MyColor((Color)colorPicker_SLS_ConcreteStress_Line.SelectedColor);
            mySettings.SLS_SteelStress_LineColor = new MyColor((Color)colorPicker_SLS_SteelStress_Line.SelectedColor);
            mySettings.SLS_Stress_DataPointColor = new MyColor((Color)colorPicker_SLS_Stress_DataPoint.SelectedColor);
            mySettings.SLS_Crack_Cracked_LineWeight = slider_SLS_ConcreteStress_Line.Value;
            mySettings.SLS_Crack_NonCracked_LineWeight = slider_SLS_SteelStress_Line.Value;
            mySettings.SLS_Crack_DataPointWeight = slider_SLS_Stress_DataPoint.Value;
        }

        private void button_Save_Click(object sender, RoutedEventArgs e)
        {
            ReadFromControls(mySettings);
            mySettings.SaveToFile();
        }
    }
}
