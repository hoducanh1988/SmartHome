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
using SmartHomeControlLibrary.__Common__;

namespace SmartHomeControlLibrary.__Window__ {


    /// <summary>
    /// Interaction logic for SingleSpeaker.xaml
    /// </summary>
    public partial class SingleSpeaker : Window {

        string dut_id;
        DeviceType dut_type;
        public string SpeakerResult = "";
        Thread thr_bzz = null;

        public SingleSpeaker(string deviceid, DeviceType devicetype) {
            InitializeComponent();

            this.dut_id = deviceid;
            this.dut_type = devicetype;

            string cmd = string.Format("CHECK,{0},{1},Test_Buzzer_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
            if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {

                if (thr_bzz != null) thr_bzz.Abort();
                thr_bzz = new Thread(new ThreadStart(() => {
                    ProjectTestItem.DUT.WriteLine(cmd);
                    ProjectTestItem.DUT.WriteLine(cmd);
                    ProjectTestItem.DUT.WriteLine(cmd);
                    Thread.Sleep(250);
                }));
                thr_bzz.IsBackground = true;
                thr_bzz.Start();
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();
            switch (tag) {

                case "ON": {
                        this.z1.Visibility = Visibility.Visible;
                        this.z2.Visibility = Visibility.Visible;
                        this.z3.Visibility = Visibility.Visible;

                        this.lbl_speakerstatus.Content = "Loa đang kêu !";
                        this.pg_Speaker.Fill = Brushes.Lime;
                        string cmd = string.Format("CHECK,{0},{1},Test_Buzzer_On!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {

                            if (thr_bzz != null) thr_bzz.Abort();
                            thr_bzz = new Thread(new ThreadStart(() => {
                                ProjectTestItem.DUT.WriteLine(cmd);
                                ProjectTestItem.DUT.WriteLine(cmd);
                                ProjectTestItem.DUT.WriteLine(cmd);
                                Thread.Sleep(250);
                            }));
                            thr_bzz.IsBackground = true;
                            thr_bzz.Start();

                        }
                        break;
                    }

                case "OFF": {
                        this.z1.Visibility = Visibility.Collapsed;
                        this.z2.Visibility = Visibility.Collapsed;
                        this.z3.Visibility = Visibility.Collapsed;

                        this.lbl_speakerstatus.Content = "Loa đang tắt !";
                        this.pg_Speaker.Fill = Brushes.White;
                        string cmd = string.Format("CHECK,{0},{1},Test_Buzzer_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {

                            if (thr_bzz != null) thr_bzz.Abort();
                            thr_bzz = new Thread(new ThreadStart(() => {
                                ProjectTestItem.DUT.WriteLine(cmd);
                                ProjectTestItem.DUT.WriteLine(cmd);
                                ProjectTestItem.DUT.WriteLine(cmd);
                                Thread.Sleep(250);
                            }));
                            thr_bzz.IsBackground = true;
                            thr_bzz.Start();
                        }
                        break;
                    }

                case "OK": {
                        if (th_1.IsChecked == false && th_2.IsChecked == false) {
                            MessageBox.Show("Vui chọn lòng đánh giá loa.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        if (th_1.IsChecked == true) SpeakerResult = "1";
                        if (th_2.IsChecked == true) SpeakerResult = "2";

                        string cmd = string.Format("CHECK,{0},{1},Test_Buzzer_Off!", this.dut_id, this.dut_type.ToString().ToUpper());
                        if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {

                            if (thr_bzz != null) thr_bzz.Abort();
                            thr_bzz = new Thread(new ThreadStart(() => {
                                ProjectTestItem.DUT.WriteLine(cmd);
                                ProjectTestItem.DUT.WriteLine(cmd);
                                ProjectTestItem.DUT.WriteLine(cmd);
                                Thread.Sleep(250);
                            }));
                            thr_bzz.IsBackground = true;
                            thr_bzz.Start();
                        }

                        this.Close();
                        break;
                    }
                default: break;
            }
        }

    }
}
