using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for MultiTouchButton.xaml
    /// </summary>
    public partial class MultiTouchButton : Window {

        TouchButtonInfo sw1 = new TouchButtonInfo();
        TouchButtonInfo sw2 = new TouchButtonInfo();
        TouchButtonInfo sw3 = new TouchButtonInfo();

        string dut_ID = "";
        public bool TouchResult = false;
        public string Message = "";

        public MultiTouchButton(string _id) {
            InitializeComponent();
            this.dut_ID = _id;

            //CTRL,49B7E618004B1200,SMH_SW3,SW3:000!
            //string cmd = string.Format("CTRL,{0},SMH_SW3,SW3:000!", this.dut_ID);
            //if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
            //    ProjectTestItem.DUT.Write(cmd);
            //    Message += "......send: " + cmd + "\r\n";
            //    Thread.Sleep(3000);
            //    Message += "......wait 3000ms\r\n";
            //}

            this.button_1.DataContext = sw1;
            this.button_2.DataContext = sw2;
            this.button_3.DataContext = sw3;

            //start check
            Thread t = new Thread(new ThreadStart(() => {
                TouchResult = Test();
                Dispatcher.Invoke(new Action(() => {
                    this.Close();
                }));
            }));
            t.IsBackground = true;
            t.Start();
            
        }

        bool Test() {
            //sw1
            Dispatcher.Invoke(new Action(() => { this.tb_legend.Text = "Vui lòng nhấn nút cảm ứng SW1!"; }));
            bool r1 = false;
            sw1.TimeRemain = 60;
            sw1.Result = "Waiting...";
            string[] buffer;

            string val_sw1 = "", val_sw2 = "", val_sw3 = "";

        RE_SW1:
            sw1.TimeRemain--;
            string data = "";
            try {
                data = ProjectTestItem.DUT.Read();
            } catch { }
            Message += "......" + data + "\r\n";
            buffer = data.Split(new string[] { "SW3:" }, StringSplitOptions.None);
            if (buffer.Length >= 2) {
                string s = buffer[buffer.Length - 1].Split('!')[0];
                if (s.Length == 3) {
                    if (val_sw1 == "") {
                        val_sw1 = s.Substring(2, 1);
                    }
                    else {
                        r1 = s.Substring(2, 1) != val_sw1;
                        if (r1) val_sw2 = s.Substring(1, 1);
                    }
                }
            }
            //r1 = data.Contains("001");
            if (!r1) {
                if (sw1.TimeRemain > 0) { Thread.Sleep(500); goto RE_SW1; }
                else {
                    sw1.Result = "Failed";
                    Message += "...SW1 Failed\r\n";
                }
            }
            else {
                sw1.Result = "Passed";
                Message += "...SW1 Passed\r\n";
            }

            //sw2
            Dispatcher.Invoke(new Action(() => { this.tb_legend.Text = "Vui lòng nhấn nút cảm ứng SW2!"; }));
            bool r2 = false;
            sw2.TimeRemain = 60;
            sw2.Result = "Waiting...";
        RE_SW2:
            sw2.TimeRemain--;
            string data2 = "";
            try {
                data2 = ProjectTestItem.DUT.Read();
            }
            catch { }
            Message += "......" + data2 + "\r\n";
            buffer = data2.Split(new string[] { "SW3:" }, StringSplitOptions.None);
            if (buffer.Length >= 2) {
                string s = buffer[buffer.Length - 1].Split('!')[0];
                if (s.Length == 3) {
                    r2 = s.Substring(1, 1) != val_sw2;
                    if (r2) val_sw3 = s.Substring(0, 1);
                }
            }
            //r2 = data2.Contains("010");
            if (!r2) {
                if (sw2.TimeRemain > 0) { Thread.Sleep(500); goto RE_SW2; }
                else {
                    Message += "...SW2 Failed\r\n";
                    sw2.Result = "Failed";
                }
            }
            else {
                Message += "...SW2 Passed\r\n";
                sw2.Result = "Passed";
            }

            //sw3
            Dispatcher.Invoke(new Action(() => { this.tb_legend.Text = "Vui lòng nhấn nút cảm ứng SW3!"; }));
            bool r3 = false;
            sw3.TimeRemain = 60;
            sw3.Result = "Waiting...";
        RE_SW3:
            sw3.TimeRemain--;
            string data3 = "";
            try {
                data3 = ProjectTestItem.DUT.Read();
            }
            catch { }
            Message += "......" + data3 + "\r\n";
            buffer = data3.Split(new string[] { "SW3:" }, StringSplitOptions.None);
            if (buffer.Length >= 2) {
                string s = buffer[buffer.Length - 1].Split('!')[0];
                if (s.Length == 3) {
                    r3 = s.Substring(0, 1) != val_sw3;
                }
            }
            //r3 = data3.Contains("100");
            if (!r3) {
                if (sw3.TimeRemain > 0) { Thread.Sleep(500); goto RE_SW3; }
                else {
                    Message += "...SW3 Failed\r\n";
                    sw3.Result = "Failed";
                }
            }
            else {
                Message += "...SW3 Passed\r\n";
                sw3.Result = "Passed";
            }

            //return value
            return r1 && r2 && r3;
        }
        
    }

    public class TouchButtonInfo : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        string _result;
        public string Result {
            get { return _result; }
            set {
                _result = value;
                OnPropertyChanged(nameof(Result));
            }
        }
        int _timeremain;
        public int TimeRemain {
            get { return _timeremain; }
            set {
                _timeremain = value;
                OnPropertyChanged(nameof(TimeRemain));
            }
        }


    }
}
