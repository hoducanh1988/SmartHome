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

namespace SmartHomeControlLibrary.SMOKEDETECTOR.PCBAFUNCTION {
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


            //1 - Validate connection vs usb dongle
            if (myGlobal.myTesting.IsCheckConnection) {
                myGlobal.myTesting.ValidateConnection = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateConnection = "Failed";
                        goto END;
                    }
                }
                //check connection between DUT (module zigbee) vs usb dongle
                string id = "";
                r = ProjectTestItem.Is_Module_ZigBee_Join_To_Network<TestingInformation>(myGlobal.myTesting, 10, 1, out id);
                myGlobal.myTesting.ValidateConnection = r == true ? "Passed" : "Failed";
                myGlobal.myTesting.ID = id;
                if (!r) goto END;
            }

            //2 - Validate data transmission vs usb dongle
            if (myGlobal.myTesting.IsCheckTransmission) {
                myGlobal.myTesting.ValidateTransmission = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateTransmission = "Failed";
                        goto END;
                    }
                }
                //check data transmission between DUT (module zigbee) vs usb dongle
                string cmd = string.Format("CHECK,{0},SMH_ZIGBEE,RF!", myGlobal.myTesting.ID);
                r = ProjectTestItem.Is_DUT_Transmitted_To_Node_RF<TestingInformation>(myGlobal.myTesting, 10, cmd);
                myGlobal.myTesting.ValidateTransmission = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //3 - Validate temperature sensor
            if (myGlobal.myTesting.IsCheckTemperature) {
                myGlobal.myTesting.ValidateTemperature = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateTemperature = "Failed";
                        goto END;
                    }
                }
                //check temperature sensor
                r = ProjectTestItem.Is_Sensor_Valid<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_SMOKE, SensorType.Temperature, myGlobal.mySetting.TemperatureValue, myGlobal.mySetting.TemperatureAccuracy, 10);
                myGlobal.myTesting.ValidateTemperature = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //4 - Validate humidity sensor
            if (myGlobal.myTesting.IsCheckHumidity) {
                myGlobal.myTesting.ValidateHumidity = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateHumidity = "Failed";
                        goto END;
                    }
                }
                //check temperature sensor
                r = ProjectTestItem.Is_Sensor_Valid<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_SMOKE, SensorType.Humidity, myGlobal.mySetting.HumidityValue, myGlobal.mySetting.HumidityAccuracy, 10);
                myGlobal.myTesting.ValidateHumidity = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //5 - Validate smoke sensor
            if (myGlobal.myTesting.IsCheckSmokeSensor) {
                myGlobal.myTesting.ValidateSmokeSensor = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateSmokeSensor = "Failed";
                        goto END;
                    }
                }
                //check smoke sensor
                r = ProjectTestItem.Is_Sensor_Valid<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_SMOKE, SensorType.Smoke, myGlobal.mySetting.SmokeValue, myGlobal.mySetting.SmokeAccuracy, 10);
                myGlobal.myTesting.ValidateSmokeSensor = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //6 - Validate LED
            if (myGlobal.myTesting.IsCheckLED) {
                myGlobal.myTesting.ValidateLED = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateLED = "Failed";
                        goto END;
                    }
                }
                //check led
                Dispatcher.Invoke(new Action(() => {
                    this.Opacity = 0.5;
                    myGlobal.myTesting.LogSystem += string.Format("\r\n+++ KIỂM TRA LED +++\r\n");
                    SingleLED window = new SingleLED(myGlobal.myTesting.ID, DeviceType.SMH_SMOKE);
                    window.ShowDialog();
                    r = window.LedResult == "1";
                    myGlobal.myTesting.ValidateLED = window.LedResult == "1" ? "Passed" : "Failed";
                    myGlobal.myTesting.LogSystem += window.LedResult == "1" ? ".........Kết quả: Passed\r\n" : ".........Kết quả: Failed\r\n";

                    switch (window.LedResult) {
                        case "": { myGlobal.myTesting.LogSystem += ".........Thông tin lỗi: Người thao tác chưa click chọn phán định trạng thái LED.\r\n"; break; }
                        case "1": { break; }
                        case "2": { myGlobal.myTesting.LogSystem += ".........Thông tin lỗi: LED xanh không sáng.\r\n"; break; }
                        case "3": { myGlobal.myTesting.LogSystem += ".........Thông tin lỗi: LED đỏ không sáng.\r\n"; break; }
                        case "4": { myGlobal.myTesting.LogSystem += ".........Thông tin lỗi: Cả 2 LED đều không sáng.\r\n"; break; }
                    }
                    this.Opacity = 1;
                }));

                if (!r) { goto END; }
            }

            //7 - Validate speaker
            if (myGlobal.myTesting.IsCheckSpeaker) {
                myGlobal.myTesting.ValidateSpeaker = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.ValidateSpeaker = "Failed";
                        goto END;
                    }
                }
                //check speaker
                Dispatcher.Invoke(new Action(() => {
                    this.Opacity = 0.5;
                    myGlobal.myTesting.LogSystem += string.Format("\r\n+++ KIỂM TRA CÒI BÁO ĐỘNG +++\r\n");
                    SingleSpeaker window = new SingleSpeaker(myGlobal.myTesting.ID, DeviceType.SMH_SMOKE);
                    window.ShowDialog();
                    r = window.SpeakerResult == "1";
                    myGlobal.myTesting.ValidateSpeaker = window.SpeakerResult == "1" ? "Passed" : "Failed";
                    myGlobal.myTesting.LogSystem += window.SpeakerResult == "1" ? ".........Kết quả: Passed\r\n" : ".........Kết quả: Failed\r\n";

                    switch (window.SpeakerResult) {
                        case "": { myGlobal.myTesting.LogSystem += ".........Thông tin lỗi: người thao tác chưa click chọn phán định trạng thái còi báo động.\r\n"; break; }
                        case "1": { break; }
                        case "2": { myGlobal.myTesting.LogSystem += ".........Thông tin lỗi: còi báo động không kêu.\r\n"; break; }
                    }
                    this.Opacity = 1;
                }));

                if (!r) { goto END; }
            }

            //8 - Save device info (id,type) to sql server
            if (myGlobal.myTesting.IsStorageInfoToSql) {
                myGlobal.myTesting.SaveInfoToSql = "Waiting...";

                myGlobal.myTesting.LogSystem += string.Format("\r\n+++ LƯU THÔNG TIN CỦA THIẾT BỊ (ID,TYPE) LÊN SQL SERVER +++\r\n");

                //check id valid or not
                r = !string.IsNullOrEmpty(myGlobal.myTesting.ID);
                if (!r) {
                    myGlobal.myTesting.SaveInfoToSql = "Failed";
                    myGlobal.myTesting.LogSystem += ".........Kết quả: Failed\r\n";
                    myGlobal.myTesting.LogSystem += string.Format(".........Thông tin lỗi: ID thiết bị \"{0}\" không đúng định dạng.\r\n", myGlobal.myTesting.ID);
                    goto END;
                }
                //save device info to sql server
                string table_name = "ProductManagement";
                var sqlServer = new ProjectSqlServer(myGlobal.mySetting.SqlServerName, myGlobal.mySetting.SqlDatabase, myGlobal.mySetting.SqlUser, myGlobal.mySetting.SqlPassword);
                TableProductManagement deviceinfo = new TableProductManagement() { DeviceID = myGlobal.myTesting.ID, DeviceType = DeviceType.SMH_SMOKE.ToString().ToUpper() };
                r = sqlServer.Insert_NewRow_To_SqlTable<TableProductManagement>(table_name, deviceinfo, "tb_ID");
                if (!r) {
                    myGlobal.myTesting.SaveInfoToSql = "Failed";
                    myGlobal.myTesting.LogSystem += ".........Kết quả: Failed\r\n";
                    myGlobal.myTesting.LogSystem += ".........Thông tin lỗi: không thể ghi thông tin thiết bị lên sql server.\r\n";
                    goto END;
                }

                myGlobal.myTesting.SaveInfoToSql = "Passed";
                myGlobal.myTesting.LogSystem += ".........Kết quả: Passed\r\n";
            }

            //9 - Print the product id label
            if (myGlobal.myTesting.IsPrintLabel) {
                myGlobal.myTesting.PrintLabel = "Waiting...";

                //check id valid or not
                r = !string.IsNullOrEmpty(myGlobal.myTesting.ID);
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
                    ProductName = "SMOKE DETECTOR",
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

            //10. Switch firmware to user mode
            if (myGlobal.myTesting.IsSwitchFirmwareMode) {
                myGlobal.myTesting.SwitchFirmwareMode = "Waiting...";

                //check connection between DUT (module zigbee) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Is_DUT_Connected_To_Client_PC<TestingInformation, SettingInformation>(myGlobal.myTesting, myGlobal.mySetting, 10);
                    if (!r) {
                        myGlobal.myTesting.SwitchFirmwareMode = "Failed";
                        goto END;
                    }
                }
                //switch firmware to user mode
                r = ProjectTestItem.Switch_Firmware_To_User_Mode<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_SMOKE, "111", 3);
                myGlobal.myTesting.SwitchFirmwareMode = r == true ? "Passed" : "Failed";
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
            string pddir = string.Format("{0}\\smokedetector", logdir);
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
            string _title = "DateTimeCreate,ID,ConRF,TransRF,Temperature,Humidity,Smoke,LED,Speaker,PrintLabel,SwitchFW,Total";
            string _content = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                            myGlobal.myTesting.ID,
                                            myGlobal.myTesting.ValidateConnection,
                                            myGlobal.myTesting.ValidateTransmission,
                                            myGlobal.myTesting.ValidateTemperature,
                                            myGlobal.myTesting.ValidateHumidity,
                                            myGlobal.myTesting.ValidateSmokeSensor,
                                            myGlobal.myTesting.ValidateLED,
                                            myGlobal.myTesting.ValidateSpeaker,
                                            myGlobal.myTesting.PrintLabel,
                                            myGlobal.myTesting.SwitchFirmwareMode,
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

            ValidateConnection = "-";
            ValidateTransmission = "-";
            ValidateSmokeSensor = "-";
            ValidateHumidity = "-";
            ValidateLED = "-";
            ValidateSpeaker = "-";
            ValidateTemperature = "-";
            SaveInfoToSql = "-";
            PrintLabel = "-";
            SwitchFirmwareMode = "-";

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
            ValidateSmokeSensor = "-";
            ValidateHumidity = "-";
            ValidateLED = "-";
            ValidateSpeaker = "-";
            ValidateTemperature = "-";
            SaveInfoToSql = "-";
            PrintLabel = "-";
            SwitchFirmwareMode = "-";
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
        string _validatetemperature;
        public string ValidateTemperature {
            get { return _validatetemperature; }
            set {
                _validatetemperature = value;
                OnPropertyChanged(nameof(ValidateTemperature));
            }
        }
        string _validatehumidity;
        public string ValidateHumidity {
            get { return _validatehumidity; }
            set {
                _validatehumidity = value;
                OnPropertyChanged(nameof(ValidateHumidity));
            }
        }
        string _validatesmokesensor;
        public string ValidateSmokeSensor {
            get { return _validatesmokesensor; }
            set {
                _validatesmokesensor = value;
                OnPropertyChanged(nameof(ValidateSmokeSensor));
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
        string _validatespeaker;
        public string ValidateSpeaker {
            get { return _validatespeaker; }
            set {
                _validatespeaker = value;
                OnPropertyChanged(nameof(ValidateSpeaker));
            }
        }
        string _saveinfotosql;
        public string SaveInfoToSql {
            get { return _saveinfotosql; }
            set {
                _saveinfotosql = value;
                OnPropertyChanged(nameof(SaveInfoToSql));
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
        string _switchfirmwaremode;
        public string SwitchFirmwareMode {
            get { return _switchfirmwaremode; }
            set {
                _switchfirmwaremode = value;
                OnPropertyChanged(nameof(SwitchFirmwareMode));
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
        bool _ischecktransmission;
        public bool IsCheckTransmission {
            get { return _ischecktransmission; }
            set {
                _ischecktransmission = value;
                OnPropertyChanged(nameof(IsCheckTransmission));
            }
        }
        bool _ischecktemperature;
        public bool IsCheckTemperature {
            get { return _ischecktemperature; }
            set {
                _ischecktemperature = value;
                OnPropertyChanged(nameof(IsCheckTemperature));
            }
        }
        bool _ischeckhumidity;
        public bool IsCheckHumidity {
            get { return _ischeckhumidity; }
            set {
                _ischeckhumidity = value;
                OnPropertyChanged(nameof(IsCheckHumidity));
            }
        }
        bool _ischecksmokesensor;
        public bool IsCheckSmokeSensor {
            get { return _ischecksmokesensor; }
            set {
                _ischecksmokesensor = value;
                OnPropertyChanged(nameof(IsCheckSmokeSensor));
            }
        }
        bool _ischeckled;
        public bool IsCheckLED {
            get { return _ischeckled; }
            set {
                _ischeckled = value;
                OnPropertyChanged(nameof(IsCheckLED));
            }
        }
        bool _ischeckspeaker;
        public bool IsCheckSpeaker {
            get { return _ischeckspeaker; }
            set {
                _ischeckspeaker = value;
                OnPropertyChanged(nameof(IsCheckSpeaker));
            }
        }
        bool _isstorageinfotosql;
        public bool IsStorageInfoToSql {
            get { return _isstorageinfotosql; }
            set {
                _isstorageinfotosql = value;
                OnPropertyChanged(nameof(IsStorageInfoToSql));
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
        bool _isswitchfirmwaremode;
        public bool IsSwitchFirmwareMode {
            get { return _isswitchfirmwaremode; }
            set {
                _isswitchfirmwaremode = value;
                OnPropertyChanged(nameof(IsSwitchFirmwareMode));
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

    public class TableProductManagement {
        public string tb_ID { get; set; }
        public string DeviceID { get; set; }
        public string DeviceType { get; set; }
    }
}
