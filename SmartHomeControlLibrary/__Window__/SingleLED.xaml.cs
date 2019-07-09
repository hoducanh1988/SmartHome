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

        Thread thrd = null;

        void controlLED(string command_line) {
            //if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
            if (thrd != null) thrd.Abort();
            thrd = new Thread(new ThreadStart(() => {
                ProjectTestItem.DUT.WriteLine(command_line);
                Thread.Sleep(500);
            }));
            thrd.IsBackground = true;
            thrd.Start();
            //}
        }


        public SingleLED(string deviceid, DeviceType devicetype) {
            InitializeComponent();
            
            this.dut_id = deviceid;
            this.dut_type = devicetype;

            string cmd = string.Format("CHECK,{0},{1},Led_Off_All!", this.dut_id, this.dut_type.ToString().ToUpper());
            controlLED(cmd);
        }

        

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();
            switch (tag) {
                case "GREEN_ON": {
                        this.lbl_ledstatus.Content = "bật LED xanh !";
                        this.pg_LED.Fill = Brushes.Lime;
                        string cmd = string.Format("CHECK,{0},{1},Led_Green_On!", this.dut_id, this.dut_type.ToString().ToUpper());
                        controlLED(cmd);
                        break;
                    }
                case "RED_ON": {
                        this.lbl_ledstatus.Content = "bật LED đỏ !";
                        this.pg_LED.Fill = Brushes.Red;
                        string cmd = string.Format("CHECK,{0},{1},Led_Red_On!", this.dut_id, this.dut_type.ToString().ToUpper());
                        controlLED(cmd);
                        break;
                    }
                case "OK": {
                        if (th_1.IsChecked == false && th_2.IsChecked == false && th_3.IsChecked == false && th_4.IsChecked == false) {
                            MessageBox.Show("Vui chọn lòng đánh giá LED.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (th_1.IsChecked == true) LedResult = "1";
                        if (th_2.IsChecked == true) LedResult = "2";
                        if (th_3.IsChecked == true) LedResult = "3";
                        if (th_4.IsChecked == true) LedResult = "4";

                        string cmd = string.Format("CHECK,{0},{1},Led_Off_All!", this.dut_id, this.dut_type.ToString().ToUpper());
                        controlLED(cmd);
                        this.Close();
                        break;
                    }
                default: break;
            }
        }
    }
}
