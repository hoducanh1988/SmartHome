using SmartHomeControlLibrary.__Common__;
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

namespace SmartHomeControlLibrary.ZIGBEE.PCBAFUNCTION {
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
                        Thread t = new Thread(new ThreadStart(() => {
                            _RunAll();
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
                default: break;
            }
        }


        //
        private void _RunAll() {
            myGlobal.myTesting.StartButtonContent = "STOP";
            timer_scrollsystemlog.Start();

            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;
            int idretry = myGlobal.mySetting.GetIDRetry;
            int comretry = myGlobal.mySetting.CommonRetry;

            //release device under test
            if (ProjectTestItem.DUT != null) ProjectTestItem.DUT = null;

            //init control 
            myGlobal.myTesting.initValidating();


            //1 - Validate connection vs usb dongle
            if (myGlobal.myTesting.IsCheckConnection) {
                myGlobal.myTesting.ValidateConnection = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateConnection = "Failed";
                        goto END;
                    }
                }
                //check connection between DUT (module zigbee) vs usb dongle
                string id = "";
                r = ProjectTestItem.Is_Module_ZigBee_Join_To_Network_D<TestingInformation>(myGlobal.myTesting, comretry, idretry, myGlobal.mySetting.DelayRetry, out id);
                myGlobal.myTesting.ValidateConnection = r == true ? "Passed" : "Failed";
                myGlobal.myTesting.ID = id;
                if (!r) goto END;
            }


            //2 - Validate data transmission vs usb dongle
            if (myGlobal.myTesting.IsCheckTransmission) {
                myGlobal.myTesting.ValidateTransmission = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateTransmission = "Failed";
                        goto END;
                    }
                }
                //check data transmission between DUT (module zigbee) vs usb dongle
                string cmd = string.Format("CHECK,{0},SMH_ZIGBEE,RF!", myGlobal.myTesting.ID);
                r = ProjectTestItem.Is_DUT_Transmitted_To_Node_RF_D<TestingInformation>(myGlobal.myTesting, comretry, myGlobal.mySetting.DelayRetry, cmd);
                myGlobal.myTesting.ValidateTransmission = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }


        END:
            st.Stop();
            timer_scrollsystemlog.Stop();

            myGlobal.myTesting.LogSystem += "\r\n+++ KẾT THÚC KIỂM TRA SẢN PHẨM +++\r\n";
            myGlobal.myTesting.LogSystem += string.Format(">>> Tổng thời gian kiểm tra: {0} ms\r\n", String.Format("{0:n0}", st.ElapsedMilliseconds));
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
            string pddir = string.Format("{0}\\zigbee", logdir);
            if (!Directory.Exists(pddir)) { Directory.CreateDirectory(pddir); Thread.Sleep(100); }
            string stdir = string.Format("{0}\\pcba", pddir);
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
            string _title = "DateTimeCreate,ID,ConRF,TransRF,Total";
            string _content = string.Format("{0},{1},{2},{3},{4}",
                                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                            myGlobal.myTesting.ID,
                                            myGlobal.myTesting.ValidateConnection,
                                            myGlobal.myTesting.ValidateTransmission,
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
        void initParameter() {
            LogSystem = "";
            TotalResult = "-";
            ID = "";
            TestMessage = "";

            ValidateConnection = "-";
            ValidateTransmission = "-";

            StartButtonContent = "START";
            StartButtonEnable = true;
        }

        public void initValidating() {
            LogSystem = "";
            TotalResult = "Waiting...";
            ID = "";
            TestMessage = "";

            ValidateConnection = "-";
            ValidateTransmission = "-";
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

        string _validateconnection;
        public string ValidateConnection {
            get { return _validateconnection; }
            set {
                _validateconnection = value;
                OnPropertyChanged(nameof(ValidateConnection));
            }
        }
        string _validatetransmission;
        public string ValidateTransmission {
            get { return _validatetransmission; }
            set {
                _validatetransmission = value;
                OnPropertyChanged(nameof(ValidateTransmission));
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
        bool _ischeckconnection;
        public bool IsCheckConnection {
            get { return _ischeckconnection; }
            set {
                _ischeckconnection = value;
                OnPropertyChanged(nameof(IsCheckConnection));
            }
        }
        bool _ischecktransmision;
        public bool IsCheckTransmission {
            get { return _ischecktransmision; }
            set {
                _ischecktransmision = value;
                OnPropertyChanged(nameof(IsCheckTransmission));
            }
        }
    }
}
