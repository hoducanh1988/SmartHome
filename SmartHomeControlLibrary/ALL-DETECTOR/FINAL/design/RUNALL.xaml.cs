using SmartHomeControlLibrary.__Window__;
using SmartHomeControlLibrary.__Userctrl__;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UtilityPack.IO;
using SmartHomeControlLibrary.__Common__;
using UtilityPack.Protocol;
using System.IO.Ports;

namespace SmartHomeControlLibrary.ALLDETECTOR.USERFUNCTION {

    /// <summary>
    /// Interaction logic for RUNALL.xaml
    /// </summary>
    public partial class RUNALL : UserControl {

        List<string> listDevice = new List<string>();
        TestInfo testinfo = new TestInfo();
        SerialPort device = new SerialPort();
        string tempReadingString = "";
        Thread t = null;
        Thread ct = null;
        List<string> listResp = new List<string>();
        private static Object obj = new Object();

        //constructor
        public RUNALL() {
            //init control
            InitializeComponent();

            //load setting
            if (File.Exists(myGlobal.SettingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.SettingFileFullName);

            //
            this.DataContext = testinfo;
        }

        //save log
        void save_log() {
            string logdir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
            if (!Directory.Exists(logdir)) { Directory.CreateDirectory(logdir); Thread.Sleep(100); }
            string pddir = string.Format("{0}\\alldetector", logdir);
            if (!Directory.Exists(pddir)) { Directory.CreateDirectory(pddir); Thread.Sleep(100); }
            string stdir = string.Format("{0}\\asm", pddir);
            if (!Directory.Exists(stdir)) { Directory.CreateDirectory(stdir); Thread.Sleep(100); }
            string lgtotal = string.Format("{0}\\logtotal", stdir);
            if (!Directory.Exists(lgtotal)) { Directory.CreateDirectory(lgtotal); Thread.Sleep(100); }
            string lgdetail = string.Format("{0}\\logdetail", stdir);
            if (!Directory.Exists(lgdetail)) { Directory.CreateDirectory(lgdetail); Thread.Sleep(100); }

            string file = string.Format("{0}\\{1}.csv", lgtotal, DateTime.Now.ToString("yyyyMMdd"));
            string _title = "DateTimeCreate,DeviceType,DeviceID,TimeElapsed,TotalResult";
            StreamWriter sw = null;
            if (!File.Exists(file)) {
                sw = new StreamWriter(file, true, Encoding.Unicode);
                sw.WriteLine(_title);
            }
            else sw = new StreamWriter(file, true, Encoding.Unicode);

            foreach (ucUserFinal c in this.stackpanel_content.Children) {
                string _content = string.Format("{0},{1},{2},{3},{4}", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                                                       c.myTesting.DeviceType,
                                                                       c.myTesting.DeviceID,
                                                                       c.myTesting.ElapsedTime,
                                                                       c.myTesting.TotalResult);
                sw.WriteLine(_content);
            }
            sw.Dispose();

            //
            string _detail = string.Format("{0}\\{1}.txt", lgdetail, DateTime.Now.ToString("yyyyMMdd"));
            sw = new StreamWriter(_detail, true, Encoding.Unicode);
            sw.WriteLine(this.testinfo.LogUART);
            sw.Dispose();

        }

        void check_thread() {
            ct = new Thread(new ThreadStart(() => {
                while (t.IsAlive) {
                    if (listResp.Count > 0) {
                        List<string> tmpList = new List<string>();
                        tmpList.AddRange(listResp);
                        listResp.Clear();
                        
                        foreach (var item in tmpList) {
                            if (item.Contains(":1!")) {
                                Dispatcher.Invoke(new Action(() => {
                                    foreach (ucUserFinal c in this.stackpanel_content.Children) {
                                        string id = c.myTesting.DeviceID.ToLower();
                                        if (item.ToLower().Contains(id)) {
                                            c.myTesting.TotalResult = "Passed";
                                            string[] buffer = item.Split(',');
                                            for (int i = 0; i < buffer.Length; i++) {
                                                if (buffer[i].ToLower().Contains(id.ToLower())) {
                                                    c.myTesting.DeviceType = buffer[i+1];
                                                    break;
                                                }
                                            }
                                            c.StopThread();
                                        }
                                    }
                                }));
                            }
                        }
                    }
                    Thread.Sleep(250);
                }
            }));
            ct.IsBackground = true;
            ct.Start();
        }

        //start thread
        void start_thread() {
            t = new Thread(new ThreadStart(() => {
                bool _flag = false;
                while (!_flag) {
                    try {
                        if (listDevice.Count > 0) {
                            Dispatcher.Invoke(new Action(() => {
                                int p = 0, f = 0;
                                foreach (var c in this.stackpanel_content.Children) {
                                    string result = (c as ucUserFinal).myTesting.TotalResult;
                                    if (result == "Passed") p++;
                                    if (result == "Failed") f++;
                                }
                                testinfo.PassedDevice = p;
                                testinfo.FailedDevice = f;
                                int sum = p + f;
                                if (sum > 0 && sum == testinfo.TotalDevice) {
                                    //close device
                                    if (device != null && device.IsOpen == true) device.Close();

                                    //save log
                                    save_log();

                                    //stop display
                                    Dispatcher.Invoke(new Action(() => {
                                        btn_start.Opacity = 1;
                                        btn_stop.Opacity = 0.5;
                                        btn_start.IsEnabled = true;
                                        btn_stop.IsEnabled = false;
                                        btn_add.IsEnabled = true;
                                        btn_remove.IsEnabled = true;
                                    }));

                                    //exit thread
                                    _flag = true;
                                }

                            }));
                        }
                    }
                    catch { }
                    Thread.Sleep(500);
                }
            }));
            t.IsBackground = true;
            t.Start();
        }

        //start button
        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                case "add": {
                        InputDeviceID window = new InputDeviceID();
                        window.ShowDialog();
                        string id = window.DeviceID;
                        if (id != "" && listDevice.Contains(id) == false) {
                            var c = new ucUserFinal();
                            c.UserSetBindingData((testinfo.TotalDevice + 1).ToString(), id, int.Parse(myGlobal.mySetting.TimeOut));
                            this.stackpanel_content.Children.Add(c);
                            testinfo.TotalDevice++;
                            listDevice.Add(id);
                        }
                        break;
                    }
                case "clear": {
                        this.stackpanel_content.Children.Clear();
                        testinfo.TotalDevice = 0;
                        testinfo.PassedDevice = 0;
                        testinfo.FailedDevice = 0;
                        listDevice.Clear();
                        break;
                    }
                case "start": {
                        if (listDevice.Count > 0) {
                            try {
                                device.PortName = myGlobal.mySetting.SerialPortName;
                                device.DataBits = int.Parse(myGlobal.mySetting.SerialDataBits);
                                device.BaudRate = int.Parse(myGlobal.mySetting.SerialBaudRate);
                                device.Parity = Parity.None;
                                device.StopBits = StopBits.One;
                                device.Open();
                            }
                            catch { }

                            if (!device.IsOpen == true) {
                                MessageBox.Show("Can't connect to usb dongle.\r\nPlease check again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                break;
                            }

                            this.listResp.Clear();
                            this.testinfo.LogUART = "";
                            device.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                            start_thread();
                            Thread.Sleep(100);
                            check_thread();

                            foreach (ucUserFinal c in this.stackpanel_content.Children) {
                                c.StartThread();
                            }

                            btn_start.Opacity = 0.5;
                            btn_stop.Opacity = 1;
                            btn_start.IsEnabled = false;
                            btn_stop.IsEnabled = true;
                            btn_add.IsEnabled = false;
                            btn_remove.IsEnabled = false;
                        }

                        break;
                    }
                case "stop": {
                        if (listDevice.Count > 0) {
                            foreach (ucUserFinal c in this.stackpanel_content.Children) {
                                if (c.myTesting.TotalResult.ToLower().Equals("waiting...")) {
                                    c.myTesting.TotalResult = "Failed";
                                }
                                c.StopThread();
                            }
                            btn_start.Opacity = 1;
                            btn_stop.Opacity = 0.5;
                            btn_start.IsEnabled = true;
                            btn_stop.IsEnabled = false;
                            btn_add.IsEnabled = true;
                            btn_remove.IsEnabled = true;

                            //close device
                            if (device != null && device.IsOpen == true) device.Close();

                        }
                        break;
                    }
                default: break;
            }
        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e) {
            var sp = (SerialPort)sender;
            string s = sp.ReadExisting();
            tempReadingString += s;

            if (s.Contains("!")) {
                string[] buffer = tempReadingString.Split('!');
                for (int i = 0; i < buffer.Length - 1; i++) {
                    string _txt = string.Format("{0}!", buffer[i]);
                    listResp.Add(_txt);
                    this.testinfo.LogUART += _txt + "\r\n";
                }
                tempReadingString = "";
            }
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            CheckBox box = sender as CheckBox;
            if (box.IsChecked == true) {
                LogWindow window = new LogWindow("", "", this.testinfo.LogUART);
                window.Show();
                box.IsChecked = false;
            }
        }
    }


    public class TestInfo : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public TestInfo() {
            TotalDevice = 0;
            PassedDevice = 0;
            FailedDevice = 0;
            LogUART = "";
        }

        int _totaldevice;
        public int TotalDevice {
            get { return _totaldevice; }
            set {
                _totaldevice = value;
                TestProgress = string.Format("{0}/{1}", PassedDevice, TotalDevice);
                OnPropertyChanged(nameof(TotalDevice));
            }
        }
        int _passeddevice;
        public int PassedDevice {
            get { return _passeddevice; }
            set {
                _passeddevice = value;
                TestProgress = string.Format("{0}/{1}", PassedDevice, TotalDevice);
                OnPropertyChanged(nameof(PassedDevice));
            }
        }
        int _faileddevice;
        public int FailedDevice {
            get { return _faileddevice; }
            set {
                _faileddevice = value;
                OnPropertyChanged(nameof(FailedDevice));
            }
        }
        string _testprogress;
        public string TestProgress {
            get { return _testprogress; }
            set {
                _testprogress = value;
                OnPropertyChanged(nameof(TestProgress));
            }
        }
        string _loguart;
        public string LogUART {
            get { return _loguart; }
            set {
                _loguart = value;
                OnPropertyChanged(nameof(LogUART));
            }
        }
    }

}
