using SmartHomeControlLibrary.__Common__;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartHomeControlLibrary.__Window__ {
    /// <summary>
    /// Interaction logic for MultiLED.xaml
    /// </summary>
    public partial class MultiLED : Window {

        string dut_id;
        DeviceType dut_type;
        public bool Led1Result = false;
        public bool Led2Result = false;
        public bool Led3Result = false;


        public MultiLED(string deviceid, DeviceType devicetype) {
            InitializeComponent();

            this.dut_id = deviceid;
            this.dut_type = devicetype;

            string cmd = string.Format("CHECK,{0},{1},Test_Led_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
            if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                ProjectTestItem.DUT.WriteLine(cmd);
                Thread.Sleep(100);
            }
        }


        private void Path_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Path p = sender as Path;
                switch (p.Tag) {
                    case "led1": {
                            if (lblledtext1.Content.ToString().Equals("???") || lblledtext1.Content.ToString().Equals("Failed")) {
                                lblledtext1.Content = "Passed";
                                p.Fill = Brushes.Lime;
                                Led1Result = true;
                            }
                            else {
                                lblledtext1.Content = "Failed";
                                p.Fill = Brushes.Red;
                                Led1Result = false;
                            }
                            break;
                        }
                    case "led2": {
                            if (lblledtext2.Content.ToString().Equals("???") || lblledtext2.Content.ToString().Equals("Failed")) {
                                lblledtext2.Content = "Passed";
                                p.Fill = Brushes.Lime;
                                Led2Result = true;
                            }
                            else {
                                lblledtext2.Content = "Failed";
                                p.Fill = Brushes.Red;
                                Led2Result = false;
                            }
                            break;
                        }
                    case "led3": {
                            if (lblledtext3.Content.ToString().Equals("???") || lblledtext3.Content.ToString().Equals("Failed")) {
                                lblledtext3.Content = "Passed";
                                p.Fill = Brushes.Lime;
                                Led3Result = true;
                            }
                            else {
                                lblledtext3.Content = "Failed";
                                p.Fill = Brushes.Red;
                                Led3Result = false;
                            }
                            break;
                        }
                }
            }

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Label l = sender as Label;
                switch (l.Tag) {
                    case "led1": {
                            if (lblledtext1.Content.ToString().Equals("???") || lblledtext1.Content.ToString().Equals("Failed")) {
                                lblledtext1.Content = "Passed";
                                path1.Fill = Brushes.Lime;
                                Led1Result = true;
                            }
                            else {
                                lblledtext1.Content = "Failed";
                                path1.Fill = Brushes.Red;
                                Led1Result = false;
                            }
                            break;
                        }
                    case "led2": {
                            if (lblledtext2.Content.ToString().Equals("???") || lblledtext2.Content.ToString().Equals("Failed")) {
                                lblledtext2.Content = "Passed";
                                path2.Fill = Brushes.Lime;
                                Led2Result = true;
                            }
                            else {
                                lblledtext2.Content = "Failed";
                                path2.Fill = Brushes.Red;
                                Led2Result = false;
                            }
                            break;
                        }
                    case "led3": {
                            if (lblledtext3.Content.ToString().Equals("???") || lblledtext3.Content.ToString().Equals("Failed")) {
                                lblledtext3.Content = "Passed";
                                path3.Fill = Brushes.Lime;
                                Led3Result = true;
                            }
                            else {
                                lblledtext3.Content = "Failed";
                                path3.Fill = Brushes.Red;
                                Led3Result = false;
                            }
                            break;
                        }
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Tag) {
                case "ok": {
                        string cmd = string.Format("CHECK,{0},{1},Test_Led_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }
                        this.Close();
                        break;
                    }
                case "ledon": {
                        string cmd = string.Format("CHECK,{0},{1},Test_Led_On!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }
                        break;
                    }
                case "ledoff": {
                        string cmd = string.Format("CHECK,{0},{1},Test_Led_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }
                        break;
                    }
            }

        }
    }
}
