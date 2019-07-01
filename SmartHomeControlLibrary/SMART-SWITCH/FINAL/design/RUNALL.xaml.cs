using SmartHomeControlLibrary.__Common__;
using SmartHomeControlLibrary.__Window__;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UtilityPack.IO;

namespace SmartHomeControlLibrary.SMARTSWITCH.FINALFUNCTION {

    /// <summary>
    /// Interaction logic for RUNALL.xaml
    /// </summary>
    public partial class RUNALL : UserControl {

        DispatcherTimer timer_scrollsystemlog = null;

        //constructor
        public RUNALL() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(myGlobal.SettingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.SettingFileFullName);

            //binding data
            this.DataContext = myGlobal.myTesting;

            //control scroll view to end
            timer_scrollsystemlog = new DispatcherTimer();
            timer_scrollsystemlog.Interval = TimeSpan.FromMilliseconds(500);
            timer_scrollsystemlog.Tick += (sender, e) => {
                this.Scr_LogSystem.ScrollToEnd();
            };
        }

        //start button
        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                case "runall": {
                        InputDeviceID window = new InputDeviceID();
                        window.ShowDialog();
                        string id = window.DeviceID;
                        bool r = ProjectUtility.IsSmartHomeDeviceID(id);
                        if (!r) return;

                        Thread t = new Thread(new ThreadStart(() => {
                            _RunAll(id);
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
                default: break;
            }
        }

        //
        private void _RunAll(string _id) {
            myGlobal.myTesting.StartButtonContent = "STOP";
            timer_scrollsystemlog.Start();

            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;

            //release device under test
            if (ProjectTestItem.DUT != null) ProjectTestItem.DUT = null;

            //init control 
            myGlobal.myTesting.initValidating();
            myGlobal.myTesting.ID = _id;

            ////1 - Validate LED
            if (myGlobal.myTesting.IsCheckLED) {
                myGlobal.myTesting.ValidateLED = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateLED = "Failed";
                        goto END;
                    }
                }
                //check led
                Dispatcher.Invoke(new Action(() => {
                    this.Opacity = 0.5;
                    myGlobal.myTesting.LogSystem += string.Format("\r\n+++ KIỂM TRA LED +++\r\n");
                    MultiLED window = new MultiLED(myGlobal.myTesting.ID, DeviceType.SMH_SW3);
                    window.ShowDialog();
                    r = window.Led1Result && window.Led2Result && window.Led3Result;
                    myGlobal.myTesting.ValidateLED = r ? "Passed" : "Failed";
                    string msg = string.Format(".........LED1 : {0}\r\n", window.Led1Result);
                    msg += string.Format(".........LED2 : {0}\r\n", window.Led2Result);
                    msg += string.Format(".........LED3 : {0}\r\n", window.Led3Result);
                    myGlobal.myTesting.LogSystem += msg;
                    myGlobal.myTesting.LogSystem += r ? ".........Kết quả: Passed\r\n" : ".........Kết quả: Failed\r\n";
                    this.Opacity = 1;
                }));

                if (!r) { goto END; }
            }

            //2 - Validate touch button
            if (myGlobal.myTesting.IsCheckTouchButton) {
                myGlobal.myTesting.ValidateTouchButton = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateTouchButton = "Failed";
                        goto END;
                    }
                }
                //check touch button
                Dispatcher.Invoke(new Action(() => {
                    this.Opacity = 0.5;
                    myGlobal.myTesting.LogSystem += string.Format("\r\n+++ KIỂM TRA NÚT CẢM ỨNG +++\r\n");
                    MultiTouchButton window = new MultiTouchButton(myGlobal.myTesting.ID);
                    window.ShowDialog();
                    r = window.TouchResult;
                    myGlobal.myTesting.ValidateTouchButton = r ? "Passed" : "Failed";
                    string msg = window.Message;
                    myGlobal.myTesting.LogSystem += msg;
                    myGlobal.myTesting.LogSystem += r ? ".........Kết quả: Passed\r\n" : ".........Kết quả: Failed\r\n";
                    this.Opacity = 1;
                }));

                if (!r) { goto END; }
            }


            //3 - Validate power switch
            if (myGlobal.myTesting.IsCheckPowerSwitch) {
                myGlobal.myTesting.ValidatePowerSwitch = "Waiting...";

                //check power switch
                Dispatcher.Invoke(new Action(() => {
                    this.Opacity = 0.5;
                    myGlobal.myTesting.LogSystem += string.Format("\r\n+++ KIỂM TRA POWER SWITCH +++\r\n");
                    PowerSwitch window = new PowerSwitch();
                    window.ShowDialog();
                    r = window.Lamp1Result && window.Lamp2Result && window.Lamp3Result;
                    string msg = string.Format(".........Power Switch1 : {0}\r\n", window.Lamp1Result);
                    msg += string.Format(".........Power Switch2 : {0}\r\n", window.Lamp2Result);
                    msg += string.Format(".........Power Switch3 : {0}\r\n", window.Lamp3Result);
                    myGlobal.myTesting.LogSystem += msg;
                    myGlobal.myTesting.ValidatePowerSwitch = r ? "Passed" : "Failed";
                    myGlobal.myTesting.LogSystem += r ? ".........Kết quả: Passed\r\n" : ".........Kết quả: Failed\r\n";
                    this.Opacity = 1;
                }));
                
                if (!r) goto END;
            }

        END:
            st.Stop();
            timer_scrollsystemlog.Stop();

            myGlobal.myTesting.LogSystem += "\r\n+++ KẾT THÚC KIỂM TRA SẢN PHẨM +++\r\n";
            myGlobal.myTesting.LogSystem += string.Format(">>> Tổng thời gian sản phẩm: {0} mili giây\r\n", String.Format("{0:n0}", st.ElapsedMilliseconds));
            myGlobal.myTesting.LogSystem += string.Format(">>> Phán định sản phẩm: {0}\r\n", r == true ? "Passed" : "Failed");
            bool ___ = r == true ? myGlobal.myTesting.initPassed() : myGlobal.myTesting.initFailed();

            myGlobal.myTesting.StartButtonContent = "START";

            //close device under test
            if (ProjectTestItem.DUT != null && ProjectTestItem.DUT.IsConnected == true) {
                ProjectTestItem.DUT.Close();
            }

            //save log
            string logdir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
            if (!Directory.Exists(logdir)) { Directory.CreateDirectory(logdir); Thread.Sleep(100); }
            string pddir = string.Format("{0}\\smartswitch", logdir);
            if (!Directory.Exists(pddir)) { Directory.CreateDirectory(pddir); Thread.Sleep(100); }
            string stdir = string.Format("{0}\\asm", pddir);
            if (!Directory.Exists(stdir)) { Directory.CreateDirectory(stdir); Thread.Sleep(100); }

            string lgsingle = string.Format("{0}\\logsingle", stdir);
            if (!Directory.Exists(lgsingle)) { Directory.CreateDirectory(lgsingle); Thread.Sleep(100); }
            string lgtotal = string.Format("{0}\\logtotal", stdir);
            if (!Directory.Exists(lgtotal)) { Directory.CreateDirectory(lgtotal); Thread.Sleep(100); }

            //log single
            string dedir = string.Format("{0}\\{1}", lgsingle, DateTime.Now.ToString("yyyyMMdd"));
            if (!Directory.Exists(dedir)) { Directory.CreateDirectory(dedir); Thread.Sleep(100); }
            string file = string.Format("{0}\\{1}_{2}_{3}_{4}.txt", dedir, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), string.IsNullOrEmpty(myGlobal.myTesting.ID) ? "NULL" : myGlobal.myTesting.ID, myGlobal.myTesting.TotalResult);

            StreamWriter sw = new StreamWriter(file, true, Encoding.Unicode);
            sw.WriteLine(myGlobal.myTesting.LogSystem);
            sw.Close();

            //log total
            string _title = "DateTimeCreate,ID,LED,TouchButton,PowerSwitch,Total";
            string _content = string.Format("{0},{1},{2},{3},{4},{5}",
                                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                            myGlobal.myTesting.ID,
                                            myGlobal.myTesting.ValidateLED,
                                            myGlobal.myTesting.ValidateTouchButton,
                                            myGlobal.myTesting.ValidatePowerSwitch,
                                            myGlobal.myTesting.TotalResult);

            string ft = string.Format("{0}\\{1}.csv", lgtotal, DateTime.Now.ToString("yyyyMMdd"));
            if (!File.Exists(ft)) {
                sw = new StreamWriter(ft, true, Encoding.Unicode);
                sw.WriteLine(_title);
            }
            else sw = new StreamWriter(ft, true, Encoding.Unicode);
            sw.WriteLine(_content);
            sw.Close();

        }

    }


    public class TestingInformation : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //constructor
        public TestingInformation() {
            initParameter();
        }

        //method
        public void initParameter() {
            LogSystem = "";
            TotalResult = "-";
            ID = "";
            TestMessage = "";

            ValidateLED = "-";
            ValidateTouchButton = "-";
            ValidatePowerSwitch = "-";

            StartButtonContent = "START";
            StartButtonEnable = true;
        }

        public void initValidating() {
            LogSystem = "";
            TotalResult = "Waiting...";
            ID = "";
            TestMessage = "";

            ValidateLED = "-";
            ValidateTouchButton = "-";
            ValidatePowerSwitch = "-";
        }

        public bool initPassed() {
            TotalResult = "Passed";
            return true;
        }

        public bool initFailed() {
            TotalResult = "Failed";
            return true;
        }

        //property
        string _logsystem;
        public string LogSystem {
            get { return _logsystem; }
            set {
                _logsystem = value;
                OnPropertyChanged(nameof(LogSystem));
            }
        }
        string _totalresult;
        public string TotalResult {
            get { return _totalresult; }
            set {
                _totalresult = value;
                OnPropertyChanged(nameof(TotalResult));
            }
        }
        string _id;
        public string ID {
            get { return _id; }
            set {
                _id = value;
                OnPropertyChanged(nameof(ID));
            }
        }
        string _testmessage;
        public string TestMessage {
            get { return _testmessage; }
            set {
                _testmessage = value;
                OnPropertyChanged(nameof(TestMessage));
            }
        }

        string _validateled;
        public string ValidateLED {
            get { return _validateled; }
            set {
                _validateled = value;
                OnPropertyChanged(nameof(ValidateLED));
            }
        }
        string _validatetouchbutton;
        public string ValidateTouchButton {
            get { return _validatetouchbutton; }
            set {
                _validatetouchbutton = value;
                OnPropertyChanged(nameof(ValidateTouchButton));
            }
        }
        string _validatepowerswitch;
        public string ValidatePowerSwitch {
            get { return _validatepowerswitch; }
            set {
                _validatepowerswitch = value;
                OnPropertyChanged(nameof(ValidatePowerSwitch));
            }
        }


        string _startbuttoncontent;
        public string StartButtonContent {
            get { return _startbuttoncontent; }
            set {
                _startbuttoncontent = value;
                OnPropertyChanged(nameof(StartButtonContent));
            }
        }
        bool _startbuttonenable;
        public bool StartButtonEnable {
            get { return _startbuttonenable; }
            set {
                _startbuttonenable = value;
                OnPropertyChanged(nameof(StartButtonEnable));
            }
        }

        //is validate
        bool _ischeckled;
        public bool IsCheckLED {
            get { return _ischeckled; }
            set {
                _ischeckled = value;
                OnPropertyChanged(nameof(IsCheckLED));
            }
        }
        bool _ischecktouchbutton;
        public bool IsCheckTouchButton {
            get { return _ischecktouchbutton; }
            set {
                _ischecktouchbutton = value;
                OnPropertyChanged(nameof(IsCheckTouchButton));
            }
        }
        bool _ischeckpowerswitch;
        public bool IsCheckPowerSwitch {
            get { return _ischeckpowerswitch; }
            set {
                _ischeckpowerswitch = value;
                OnPropertyChanged(nameof(IsCheckPowerSwitch));
            }
        }
    }
}
