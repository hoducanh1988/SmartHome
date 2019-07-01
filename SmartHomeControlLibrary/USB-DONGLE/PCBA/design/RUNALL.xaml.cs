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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;

using SmartHomeControlLibrary.__Common__;
using System.IO;
using UtilityPack.IO;
using System.Windows.Threading;

namespace SmartHomeControlLibrary.USBDongle.PCBAFUNCTION {
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

            //release device under test
            if (ProjectTestItem.DUT != null) ProjectTestItem.DUT = null;

            //init control 
            myGlobal.myTesting.initValidating();
            

            //1 - Validate communication vs client PC
            if (myGlobal.myTesting.IsCheckComunication) {
                myGlobal.myTesting.ValidatePC = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidatePC = "Failed";
                        goto END;
                    }
                }
                //check transmission between DUT (usb dongle) vs PC
                string id = "";
                string cmd = "CHECK,0000000000000000,USB_DONGLE!";
                r = ProjectTestItem.Is_DUT_Transmitted_To_Client_PC<TestingInformation>(myGlobal.myTesting, 10, cmd , out id);
                myGlobal.myTesting.ID = id;
                myGlobal.myTesting.ValidatePC = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }


            //2 - Validate communication vs node RF
            if (myGlobal.myTesting.IsCheckFunction) {
                myGlobal.myTesting.ValidateNodeRF = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateNodeRF = "Failed";
                        goto END;
                    }
                }
                //check transmission between DUT (usb dongle) vs node RF
                string cmd = string.Format("CHECK,{0},SMH_{1},RF!", myGlobal.mySetting.NodeRFID, myGlobal.mySetting.NodeRFType);
                r = ProjectTestItem.Is_DUT_Transmitted_To_Node_RF<TestingInformation>(myGlobal.myTesting, 10, cmd, myGlobal.mySetting.NodeRFID, string.Format("SMH_{0}", myGlobal.mySetting.NodeRFType));
                myGlobal.myTesting.ValidateNodeRF = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }


            //3 - Print the product ID label
            if (myGlobal.myTesting.IsPrintLabel) {
                myGlobal.myTesting.PrintLabel = "Waiting...";

                //check device id
                r =! string.IsNullOrEmpty(myGlobal.myTesting.ID);
                if (!r) {
                    myGlobal.myTesting.PrintLabel = "Failed";
                    goto END;
                }
                //print label
                string id_table_name = "tb_ProductID";
                string log_table_name = "tb_DataLog";
                string report_name = "ProductID";

                TableProductID id_info = new TableProductID() { ProductID = myGlobal.myTesting.ID };
                TableDataLog ms_log_info = new TableDataLog() {
                    DateTimeCreated = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                    ErrorMessage = "",
                    Factory = "",
                    JigNumber = "",
                    LineCode = "",
                    ProductCode = "",
                    ProductColor = "",
                    ProductID = id_info.ProductID,
                    ProductName = "USB DONGLE",
                    ProductNumber = "",
                    Station = "PCBA-FUNCTION",
                    Worker = "",
                    WorkOrder = ""
                };

                r = ProjectTestItem.Print_DUT_ID_Labels<TestingInformation, TableProductID, TableDataLog>
                    (
                        myGlobal.myTesting, 
                        myGlobal.myTesting.ID,
                        string.Format("{0}access_db\\{1}", AppDomain.CurrentDomain.BaseDirectory, myGlobal.mySetting.MSAccessFile),
                        id_table_name,
                        id_info,
                        log_table_name,
                        ms_log_info,
                        report_name
                    );
                myGlobal.myTesting.PrintLabel = r == true ? "Passed" : "Failed";
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
            string pddir = string.Format("{0}\\usbdongle", logdir);
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
            string _title = "DateTimeCreate,ID,ConPC,TransRF,PrintLabel,Total";
            string _content = string.Format("{0},{1},{2},{3},{4},{5}",
                                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                            myGlobal.myTesting.ID,
                                            myGlobal.myTesting.ValidatePC,
                                            myGlobal.myTesting.ValidateNodeRF,
                                            myGlobal.myTesting.PrintLabel,
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

    #region sub_class

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

            ValidatePC = "-";
            ValidateNodeRF = "-";
            PrintLabel = "-";

            StartButtonContent = "START";
            StartButtonEnable = true;
        }

        public void initValidating() {
            LogSystem = "";
            TotalResult = "Waiting...";
            ID = "";
            TestMessage = "";

            ValidatePC = "-";
            ValidateNodeRF = "-";
            PrintLabel = "-";
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
        string _validatepc;
        public string ValidatePC {
            get { return _validatepc; }
            set {
                _validatepc = value;
                OnPropertyChanged(nameof(ValidatePC));
            }
        }
        string _validatenoderf;
        public string ValidateNodeRF {
            get { return _validatenoderf; }
            set {
                _validatenoderf = value;
                OnPropertyChanged(nameof(ValidateNodeRF));
            }
        }
        string _printlabel;
        public string PrintLabel {
            get { return _printlabel; }
            set {
                _printlabel = value;
                OnPropertyChanged(nameof(PrintLabel));
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
        bool _ischeckcomunication;
        public bool IsCheckComunication {
            get { return _ischeckcomunication; }
            set {
                _ischeckcomunication = value;
                OnPropertyChanged(nameof(IsCheckComunication));
            }
        }
        bool _ischeckfunction;
        public bool IsCheckFunction {
            get { return _ischeckfunction; }
            set {
                _ischeckfunction = value;
                OnPropertyChanged(nameof(IsCheckFunction));
            }
        }
        bool _isprintlabel;
        public bool IsPrintLabel {
            get { return _isprintlabel; }
            set {
                _isprintlabel = value;
                OnPropertyChanged(nameof(IsPrintLabel));
            }
        }
    }

    public class TableProductID {
        public string ProductID { get; set; }
    }

    public class TableDataLog {

        public string tb_ID { get; set; }
        public string DateTimeCreated { get; set; }
        public string ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductNumber { get; set; }
        public string ProductColor { get; set; }
        public string WorkOrder { get; set; }
        public string Factory { get; set; }
        public string LineCode { get; set; }
        public string Station { get; set; }
        public string JigNumber { get; set; }
        public string Worker { get; set; }
        public string TotalResult { get; set; }
        public string ErrorMessage { get; set; }
    }

    #endregion

}
