using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SmartHomeControlLibrary.__Common__;

namespace SmartHomeControlLibrary.__Window__ {
    /// <summary>
    /// Interaction logic for SingleLED.xaml
    /// </summary>
    public partial class SingleLED : Window {

        string dut_id;
        DeviceType dut_type;
        public string LedResult = "";

        public SingleLED(string deviceid, DeviceType devicetype) {
            InitializeComponent();

            this.dut_id = deviceid;
            this.dut_type = devicetype;

            string cmd = string.Format("CHECK,{0},{1},Test_Led_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
            if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                ProjectTestItem.DUT.WriteLine(cmd);
                Thread.Sleep(100);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();
            switch (tag) {
                case "ON": {
                        this.lbl_ledstatus.Content = "LED đang bật sáng !";
                        this.pg_LED.Fill = Brushes.Lime;
                        string cmd = string.Format("CHECK,{0},{1},Test_Led_On!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }
                        break;
                    }
                case "OFF": {
                        this.lbl_ledstatus.Content = "LED đang tắt !";
                        this.pg_LED.Fill = Brushes.White;
                        string cmd = string.Format("CHECK,{0},{1},Test_Led_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT!= null && ProjectTestItem.DUT.IsConnected == true) {
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }
                        
                        break;
                    }
                case "OK": {
                        if (th_1.IsChecked == false && th_2.IsChecked == false && th_3.IsChecked == false) {
                            MessageBox.Show("Vui chọn lòng đánh giá LED.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (th_1.IsChecked == true) LedResult = "1";
                        if (th_2.IsChecked == true) LedResult = "2";
                        if (th_3.IsChecked == true) LedResult = "3";
                        if (th_4.IsChecked == true) LedResult = "4";

                        string cmd = string.Format("CHECK,{0},{1},Test_Led_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }

                        this.Close();
                        break;
                    }
                default: break;
            }
        }
    }
}
