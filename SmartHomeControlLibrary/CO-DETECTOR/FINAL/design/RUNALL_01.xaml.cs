using SmartHomeControlLibrary.__Common__;
using SmartHomeControlLibrary.__Window__;
using SmartHomeControlLibrary.__Userctrl__;
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

namespace SmartHomeControlLibrary.CarbonMonoxideDetector.FINALFUNCTION {

    /// <summary>
    /// Interaction logic for RUNALL_01.xaml
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

            InputDeviceIDAndSerialNumber window = new InputDeviceIDAndSerialNumber();
            window.ShowDialog();
            string _id = window.DeviceID;
            string _sn = window.DeviceSN;

            if (string.IsNullOrEmpty(_id) || string.IsNullOrEmpty(_sn)) return;

            switch (tag) {
                case "runall": {
                        Thread t = new Thread(new ThreadStart(() => {
                            _RunAll(_id, _sn);
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }
                default: break;
            }
        }

        //
        private void _RunAll(string id, string sn) {
            myGlobal.myTesting.StartButtonContent = "STOP";
            timer_scrollsystemlog.Start();

            Stopwatch st = new Stopwatch();
            st.Start();

            bool r = false;
            int comretry = myGlobal.mySetting.CommonRetry;

            //release device under test
            if (ProjectTestItem.DUT != null) ProjectTestItem.DUT = null;

            //init control 
            myGlobal.myTesting.initValidating();
            myGlobal.myTesting.ID = id;
            myGlobal.myTesting.SerialNumber = sn;

            //1 - validate product id
            if (myGlobal.myTesting.IsVerifyID) {
                myGlobal.myTesting.ValidateID = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateID = "Failed";
                        return;
                    }
                }

                //validate ID
                r = ProjectTestItem.Validate_ID_Value_D<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_CO, comretry, myGlobal.mySetting.DelayRetry);
                myGlobal.myTesting.ValidateID = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //2 - validate firmware version
            if (myGlobal.myTesting.IsVerifyFW) {
                myGlobal.myTesting.ValidateFirmware = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateFirmware = "Failed";
                        return;
                    }
                }

                //validate firmware version
                r = ProjectTestItem.Validate_Firmware_Version_Value_D<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_CO, myGlobal.mySetting.FirmwareVersion, comretry, myGlobal.mySetting.DelayRetry);
                myGlobal.myTesting.ValidateFirmware = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //3 - validate model
            if (myGlobal.myTesting.IsVerifyModel) {
                myGlobal.myTesting.ValidateModel = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateModel = "Failed";
                        return;
                    }
                }

                //validate model
                r = ProjectTestItem.Validate_Model_Value_D<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_CO, myGlobal.mySetting.Model, comretry, myGlobal.mySetting.DelayRetry);
                myGlobal.myTesting.ValidateModel = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //4 - write serial number
            if (myGlobal.myTesting.IsWriteSN) {
                myGlobal.myTesting.WriteSerialNumber = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.WriteSerialNumber = "Failed";
                        return;
                    }
                }

                //write serial number
                r = ProjectTestItem.Write_SerialNumber_D<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_CO, myGlobal.myTesting.SerialNumber, comretry, myGlobal.mySetting.DelayRetry);
                if (!r) {
                    myGlobal.myTesting.WriteSerialNumber = "Failed";
                    goto END;
                }

                //validate serial number
                r = ProjectTestItem.Validate_SerialNumber_Value_D<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_CO, myGlobal.myTesting.SerialNumber, comretry, myGlobal.mySetting.DelayRetry);
                myGlobal.myTesting.WriteSerialNumber = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //5 - Validate temperature sensor
            if (myGlobal.myTesting.IsCheckTemperature) {
                myGlobal.myTesting.ValidateTemperature = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateTemperature = "Failed";
                        return;
                    }
                }
                //check temperature sensor
                r = ProjectTestItem.Is_Sensor_Valid_D<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_CO, SensorType.Temperature, myGlobal.mySetting.TemperatureValue, myGlobal.mySetting.TemperatureAccuracy, comretry, myGlobal.mySetting.DelayRetry);
                myGlobal.myTesting.ValidateTemperature = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //6 - Validate humidity sensor
            if (myGlobal.myTesting.IsCheckHumidity) {
                myGlobal.myTesting.ValidateHumidity = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateTemperature = "Failed";
                        return;
                    }
                }
                //check humidity sensor
                r = ProjectTestItem.Is_Sensor_Valid_D<TestingInformation>(myGlobal.myTesting, myGlobal.myTesting.ID, DeviceType.SMH_CO, SensorType.Humidity, myGlobal.mySetting.HumidityValue, myGlobal.mySetting.HumidityAccuracy, comretry, myGlobal.mySetting.DelayRetry);
                myGlobal.myTesting.ValidateHumidity = r == true ? "Passed" : "Failed";
                if (!r) goto END;
            }

            //7 - Validate LED
            if (myGlobal.myTesting.IsCheckLED) {
                myGlobal.myTesting.ValidateLED = "Waiting...";

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateTemperature = "Failed";
                        return;
                    }
                }
                //check led
                Dispatcher.Invoke(new Action(() => {
                    this.Opacity = 0.5;
                    myGlobal.myTesting.LogSystem += string.Format("\r\n+++ KIỂM TRA LED +++\r\n");
                    SingleLED window = new SingleLED(myGlobal.myTesting.ID, DeviceType.SMH_CO);
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

            //8 - Validate speaker
            if (myGlobal.myTesting.IsCheckSpeaker) {
                myGlobal.myTesting.ValidateSpeaker = "Waiting...";

                Thread.Sleep(3000); //

                //check connection between DUT (usb dongle) vs PC
                if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                    r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, comretry);
                    if (!r) {
                        myGlobal.myTesting.ValidateTemperature = "Failed";
                        return;
                    }
                }
                //check speaker
                Dispatcher.Invoke(new Action(() => {
                    this.Opacity = 0.5;
                    myGlobal.myTesting.LogSystem += string.Format("\r\n+++ KIỂM TRA CÒI BÁO ĐỘNG +++\r\n");
                    SingleSpeaker window = new SingleSpeaker(myGlobal.myTesting.ID, DeviceType.SMH_CO);
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
            string pddir = string.Format("{0}\\codetector", logdir);
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
            string _title = "DateTimeCreate,ID,SN,ConfID,ConfFW,ConfModel,WriteSN,Temperature,Humidity,LED,Speaker,Total";
            string _content = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                            myGlobal.myTesting.ID,
                                            myGlobal.myTesting.SerialNumber,
                                            myGlobal.myTesting.ValidateID,
                                            myGlobal.myTesting.ValidateFirmware,
                                            myGlobal.myTesting.ValidateModel,
                                            myGlobal.myTesting.WriteSerialNumber,
                                            myGlobal.myTesting.ValidateTemperature,
                                            myGlobal.myTesting.ValidateHumidity,
                                            myGlobal.myTesting.ValidateLED,
                                            myGlobal.myTesting.ValidateSpeaker,
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
            SerialNumber = "";
            TestMessage = "";

            ValidateID = "-";
            ValidateFirmware = "-";
            ValidateModel = "-";
            WriteSerialNumber = "-";
            ValidateHumidity = "-";
            ValidateLED = "-";
            ValidateSpeaker = "-";
            ValidateTemperature = "-";

            StartButtonContent = "START";
            StartButtonEnable = true;
        }

        public void initValidating() {
            LogSystem = "";
            TotalResult = "Waiting...";
            ID = "";
            SerialNumber = "";
            TestMessage = "";

            ValidateID = "-";
            ValidateFirmware = "-";
            ValidateModel = "-";
            WriteSerialNumber = "-";
            ValidateHumidity = "-";
            ValidateLED = "-";
            ValidateSpeaker = "-";
            ValidateTemperature = "-";
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
        string _sn;
        public string SerialNumber {
            get { return _sn; }
            set {
                _sn = value;
                OnPropertyChanged(nameof(SerialNumber));
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


        string _validateid;
        public string ValidateID {
            get { return _validateid; }
            set {
                _validateid = value;
                OnPropertyChanged(nameof(ValidateID));
            }
        }
        string _validatefirmware;
        public string ValidateFirmware {
            get { return _validatefirmware; }
            set {
                _validatefirmware = value;
                OnPropertyChanged(nameof(ValidateFirmware));
            }
        }
        string _validatemodel;
        public string ValidateModel {
            get { return _validatemodel; }
            set {
                _validatemodel = value;
                OnPropertyChanged(nameof(ValidateModel));
            }
        }
        string _writeserialnumber;
        public string WriteSerialNumber {
            get { return _writeserialnumber; }
            set {
                _writeserialnumber = value;
                OnPropertyChanged(nameof(WriteSerialNumber));
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
        bool _isverifyid;
        public bool IsVerifyID {
            get { return _isverifyid; }
            set {
                _isverifyid = value;
                OnPropertyChanged(nameof(IsVerifyID));
            }
        }
        bool _isverifyfw;
        public bool IsVerifyFW {
            get { return _isverifyfw; }
            set {
                _isverifyfw = value;
                OnPropertyChanged(nameof(IsVerifyFW));
            }
        }
        bool _isverifymodel;
        public bool IsVerifyModel {
            get { return _isverifymodel; }
            set {
                _isverifymodel = value;
                OnPropertyChanged(nameof(IsVerifyModel));
            }
        }
        bool _iswritesn;
        public bool IsWriteSN {
            get { return _iswritesn; }
            set {
                _iswritesn = value;
                OnPropertyChanged(nameof(IsWriteSN));
            }
        }
    }

}
