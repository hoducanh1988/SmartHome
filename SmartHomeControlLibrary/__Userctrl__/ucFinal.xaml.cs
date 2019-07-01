using SmartHomeControlLibrary.__Common__;
using SmartHomeControlLibrary.__Window__;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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

namespace SmartHomeControlLibrary.__Userctrl__ {
    /// <summary>
    /// Interaction logic for ucFinal.xaml
    /// </summary>
    public partial class ucFinal : UserControl {

        List<string> _listjud = new List<string>() { "-" , "Passed" , "Failed" };

        public TestingInformation myTesting = null;
        dynamic settinginfo = null;


        //constructor
        public ucFinal () {
            InitializeComponent();

            this.cbb_LED.ItemsSource = _listjud;
            this.cbb_Speaker.ItemsSource = _listjud;
        }

        //get seting 
        public void SetBindingData <S> (string _idx, string _id, DeviceType _type, S _settinginfo) {
            this.myTesting = new TestingInformation();
            this.settinginfo = _settinginfo;

            //set id, index, devicetype
            this.myTesting.FdeviceIndex = _idx;
            this.myTesting.FID = _id;
            this.myTesting.FdeviceType = _type;

            //get check item
            this.myTesting.IsCheckHumidity = (bool) settinginfo.GetType().GetProperty("IsCheckHumidity").GetValue(settinginfo, null);
            this.myTesting.IsCheckLED = (bool)settinginfo.GetType().GetProperty("IsCheckLED").GetValue(settinginfo, null);
            this.myTesting.IsCheckPPM = (bool)settinginfo.GetType().GetProperty("IsCheckPPM").GetValue(settinginfo, null);
            this.myTesting.IsCheckSpeaker = (bool)settinginfo.GetType().GetProperty("IsCheckSpeaker").GetValue(settinginfo, null);
            this.myTesting.IsCheckTemperature = (bool)settinginfo.GetType().GetProperty("IsCheckTemperature").GetValue(settinginfo, null);
            this.myTesting.IsSwitchFirmwareMode = (bool)settinginfo.GetType().GetProperty("IsSwitchFirmwareMode").GetValue(settinginfo, null);

            //get standard value
            this.myTesting.TemperatureValue = (string) settinginfo.GetType().GetProperty("TemperatureValue").GetValue(settinginfo, null);
            this.myTesting.TemperatureAccuracy = (string)settinginfo.GetType().GetProperty("TemperatureAccuracy").GetValue(settinginfo, null);
            this.myTesting.HumidityValue = (string)settinginfo.GetType().GetProperty("HumidityValue").GetValue(settinginfo, null);
            this.myTesting.HumidityAccuracy = (string)settinginfo.GetType().GetProperty("HumidityAccuracy").GetValue(settinginfo, null);
            this.myTesting.PPMValue = (string)settinginfo.GetType().GetProperty("PPMValue").GetValue(settinginfo, null);
            this.myTesting.PPMAccuracy = (string)settinginfo.GetType().GetProperty("PPMAccuracy").GetValue(settinginfo, null);
            this.myTesting.FirmwareModeValue = "111";

            this.DataContext = this.myTesting;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string _tag = b.Tag.ToString();
            bool r = false;

            switch (_tag) {
                case "test_temperature": {
                        myTesting.ValidateTemperature = "Waiting...";

                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.ValidateTemperature = "Failed";
                                return;
                            }
                        }
                        //check temperature sensor
                        double sensor_value = 0;
                        r = ProjectTestItem.Is_Sensor_Valid<TestingInformation>(myTesting, myTesting.FID, myTesting.FdeviceType, SensorType.Temperature, myTesting.TemperatureValue, myTesting.TemperatureAccuracy, 10, out sensor_value);
                        myTesting.TemperatureActual = sensor_value.ToString();
                        myTesting.ValidateTemperature = r == true ? "Passed" : "Failed";
                        break;
                    }
                case "test_humidity": {
                        myTesting.ValidateHumidity = "Waiting...";

                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.ValidateHumidity = "Failed";
                                return;
                            }
                        }
                        //check humidity sensor
                        double sensor_value = 0;
                        r = ProjectTestItem.Is_Sensor_Valid<TestingInformation>(myTesting, myTesting.FID, myTesting.FdeviceType, SensorType.Humidity, myTesting.HumidityValue, myTesting.HumidityAccuracy, 10, out sensor_value);
                        myTesting.HumidityActual = sensor_value.ToString();
                        myTesting.ValidateHumidity = r == true ? "Passed" : "Failed";
                        break;
                    }
                case "test_ppm": {
                        myTesting.ValidatePPM = "Waiting...";

                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.ValidatePPM = "Failed";
                                return;
                            }
                        }
                        //check ppm sensor
                        SensorType sensor = SensorType.CO;
                        switch (myTesting.FdeviceType) {
                            case DeviceType.SMH_CO: {
                                    sensor = SensorType.CO;
                                    break;
                                }
                            case DeviceType.SMH_GAS: {
                                    sensor = SensorType.GAS;
                                    break;
                                }
                            case DeviceType.SMH_SMOKE: {
                                    sensor = SensorType.Smoke;
                                    break;
                                }
                        }
                        
                        r = ProjectTestItem.Is_Sensor_Valid<TestingInformation>(myTesting, myTesting.FID, myTesting.FdeviceType, sensor, myTesting.PPMValue, myTesting.PPMAccuracy, 10);
                        myTesting.ValidatePPM = r == true ? "Passed" : "Failed";
                        break;
                    }
                case "test_usermode": {
                        myTesting.SwitchFirmwareMode = "Waiting...";

                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.SwitchFirmwareMode = "Failed";
                                return;
                            }
                        }
                        //switch firmware to user mode
                        r = ProjectTestItem.Switch_Firmware_To_User_Mode<TestingInformation>(myTesting, myTesting.FID, myTesting.FdeviceType, myTesting.FirmwareModeValue, 10);
                        myTesting.SwitchFirmwareMode = r == true ? "Passed" : "Failed";
                        break;
                    }
                case "led_on": {
                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.ValidateLED = "Failed";
                                return;
                            }
                        }
                        //set led on
                        try {
                            string cmd = string.Format("CHECK,{0},{1},Test_Led_On!", myTesting.FID, myTesting.FdeviceType.ToString().ToUpper());
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                            this.cbb_LED.IsEnabled = true;
                        } catch { }
                        break;
                    }
                case "led_off": {
                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.ValidateLED = "Failed";
                                return;
                            }
                        }
                        //set led off
                        try {
                            string cmd = string.Format("CHECK,{0},{1},Test_Led_Off!", myTesting.FID, myTesting.FdeviceType.ToString().ToUpper());
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }
                        catch { }
                        break;
                    }
                case "speaker_on": {
                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.ValidateSpeaker = "Failed";
                                return;
                            }
                        }
                        //set speaker on
                        try {
                            string cmd = string.Format("CHECK,{0},{1},Test_Buzzer_On!", myTesting.FID, myTesting.FdeviceType.ToString().ToUpper());
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                            this.cbb_Speaker.IsEnabled = true;
                        }
                        catch { }
                        break;
                    }
                case "speaker_off": {
                        //check connection between DUT (usb dongle) vs PC
                        if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                            r = ProjectTestItem.Open_Device_USB_Dongle(myTesting, settinginfo, 10);
                            if (!r) {
                                myTesting.ValidateSpeaker = "Failed";
                                return;
                            }
                        }
                        //set speaker off
                        try {
                            string cmd = string.Format("CHECK,{0},{1},Test_Buzzer_Off!", myTesting.FID, myTesting.FdeviceType.ToString().ToUpper());
                            ProjectTestItem.DUT.WriteLine(cmd);
                            Thread.Sleep(100);
                        }
                        catch { }
                        break;
                    }
                default: break;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            CheckBox box = sender as CheckBox;
            if (box.IsChecked == true) {
                LogWindow window = new LogWindow(myTesting.FID, myTesting.FdeviceIndex, myTesting.LogSystem);
                window.Show();
                box.IsChecked = false;
            }
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
            FID = "";
            FdeviceType = DeviceType.SMH_CO;
            FdeviceIndex = "";
            TestMessage = "";

            ValidatePPM = "-";
            ValidateHumidity = "-";
            ValidateLED = "-";
            ValidateSpeaker = "-";
            ValidateTemperature = "-";
            SwitchFirmwareMode = "-";

            TemperatureActual = "0";
            HumidityActual = "0";
            PPMActual = "0";
            FirmwareModeActual = "-";
        }

        public void initValidating() {
            LogSystem = "";
            TotalResult = "Waiting...";
            FID = "";
            FdeviceType = DeviceType.SMH_CO;
            FdeviceIndex = "";
            TestMessage = "";

            ValidatePPM = "-";
            ValidateHumidity = "-";
            ValidateLED = "-";
            ValidateSpeaker = "-";
            ValidateTemperature = "-";
            SwitchFirmwareMode = "-";

            TemperatureActual = "0";
            HumidityActual = "0";
            PPMActual = "0";
            FirmwareModeActual = "-";
        }

        public bool initPassed() {
            TotalResult = "Passed";
            return true;
        }

        public bool initFailed() {
            TotalResult = "Failed";
            return true;
        }

        void totalpassed() {
            if (ValidateTemperature == "Passed" && ValidateHumidity == "Passed" && ValidateLED == "Passed" &&
                    ValidatePPM == "Passed" && ValidateSpeaker == "Passed" && SwitchFirmwareMode == "Passed") {
                TotalResult = "Passed";
                savelog();
            }
        }
        void totalchecking() {
            if (ValidateTemperature != "-" || ValidateHumidity != "-" || ValidateLED != "-" ||
                    ValidatePPM != "-" || ValidateSpeaker != "-" || SwitchFirmwareMode != "-") {
                TotalResult = "Waiting...";
            }
        }
        void totalfailed() {
            if (ValidateTemperature == "Failed" || ValidateHumidity == "Failed" || ValidateLED == "Failed" ||
                    ValidatePPM == "Failed" || ValidateSpeaker == "Failed" || SwitchFirmwareMode == "Failed") {
                TotalResult = "Failed";
                savelog();
            }
        }
        void totalready() {
            if (ValidateTemperature == "-" && ValidateHumidity == "-" && ValidateLED == "-" &&
                    ValidatePPM == "-" && ValidateSpeaker == "-" && SwitchFirmwareMode == "-") {
                TotalResult = "-";
            }
        }
        void totalupdate() {
            totalchecking();
            totalfailed();
            totalpassed();
            totalready();
        }

        void savelog() {
            //save log
            string logdir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
            if (!Directory.Exists(logdir)) { Directory.CreateDirectory(logdir); Thread.Sleep(100); }
            string aaaa = "_";
            switch (FdeviceType) {
                case DeviceType.SMH_CO: { aaaa = "codetector"; break; }
                case DeviceType.SMH_GAS: { aaaa = "gasdetector"; break; }
                case DeviceType.SMH_SMOKE: { aaaa = "smokedetector"; break; }
            }
            string pddir = string.Format("{0}\\{1}", logdir, aaaa);
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
            string file = string.Format("{0}\\{1}_{2}_{3}_{4}.txt", dedir, DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss"), string.IsNullOrEmpty(FID) ? "NULL" : FID, TotalResult);
            
            StreamWriter sw = new StreamWriter(file, true, Encoding.Unicode);
            sw.WriteLine(LogSystem);
            sw.Dispose();

            //log total
            //string _title = "DateTimeCreate,ID,Temperature,Humidity,LED,Speaker,PPM,SwitchFW,Total";
            //string _content = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}",
            //                                DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
            //                                FID,
            //                                ValidateTemperature,
            //                                ValidateHumidity,
            //                                ValidateLED,
            //                                ValidateSpeaker,
            //                                ValidatePPM,
            //                                SwitchFirmwareMode,
            //                                TotalResult);

            string _title = "DateTimeCreate,ID,Temperature,Humidity,LED,Speaker,Total";
            string _content = string.Format("{0},{1},{2},{3},{4},{5},{6}",
                                            DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                                            FID,
                                            ValidateTemperature,
                                            ValidateHumidity,
                                            ValidateLED,
                                            ValidateSpeaker,
                                            TotalResult);


            string ft = string.Format("{0}\\{1}.csv", lgtotal, DateTime.Now.ToString("yyyyMMdd"));
            if (!File.Exists(ft)) {
                sw = new StreamWriter(ft, true, Encoding.Unicode);
                sw.WriteLine(_title);
            }
            else sw = new StreamWriter(ft, true, Encoding.Unicode);
            sw.WriteLine(_content);
            sw.Close();


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
        string _fid;
        public string FID {
            get { return _fid; }
            set {
                _fid = value;
                OnPropertyChanged(nameof(FID));
            }
        }
        DeviceType _fdevicetype;
        public DeviceType FdeviceType {
            get { return _fdevicetype; }
            set {
                _fdevicetype = value;
                OnPropertyChanged(nameof(FdeviceType));
            }
        }
        string _fdeviceindex;
        public string FdeviceIndex {
            get { return _fdeviceindex; }
            set {
                _fdeviceindex = value;
                OnPropertyChanged(nameof(FdeviceIndex));
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

        //validate
        string _validatetemperature;
        public string ValidateTemperature {
            get { return _validatetemperature; }
            set {
                _validatetemperature = value;
                OnPropertyChanged(nameof(ValidateTemperature));
                totalupdate();
            }
        }
        string _validatehumidity;
        public string ValidateHumidity {
            get { return _validatehumidity; }
            set {
                _validatehumidity = value;
                OnPropertyChanged(nameof(ValidateHumidity));
                totalupdate();
            }
        }
        string _validateppm;
        public string ValidatePPM {
            get { return _validateppm; }
            set {
                _validateppm = value;
                OnPropertyChanged(nameof(ValidatePPM));
                totalupdate();
            }
        }
        string _validateled;
        public string ValidateLED {
            get { return _validateled; }
            set {
                _validateled = value;
                OnPropertyChanged(nameof(ValidateLED));
                totalupdate();
            }
        }
        string _validatespeaker;
        public string ValidateSpeaker {
            get { return _validatespeaker; }
            set {
                _validatespeaker = value;
                OnPropertyChanged(nameof(ValidateSpeaker));
                totalupdate();
            }
        }
        string _switchfirmwaremode;
        public string SwitchFirmwareMode {
            get { return _switchfirmwaremode; }
            set {
                _switchfirmwaremode = value;
                OnPropertyChanged(nameof(SwitchFirmwareMode));
                totalupdate();
            }
        }

        //temperature
        string _tempvalue;
        public string TemperatureValue {
            get { return _tempvalue; }
            set {
                _tempvalue = value;
                OnPropertyChanged(nameof(TemperatureValue));
            }
        }
        string _tempact;
        public string TemperatureActual {
            get { return _tempact; }
            set {
                _tempact = value;
                OnPropertyChanged(nameof(TemperatureActual));
            }
        }
        string _temperatureaccuracy;
        public string TemperatureAccuracy {
            get { return _temperatureaccuracy; }
            set {
                _temperatureaccuracy = value;
                OnPropertyChanged(nameof(TemperatureAccuracy));
            }
        }

        //humidity
        string _humidityvalue;
        public string HumidityValue {
            get { return _humidityvalue; }
            set {
                _humidityvalue = value;
                OnPropertyChanged(nameof(HumidityValue));
            }
        }
        string _humidityaccuracy;
        public string HumidityAccuracy {
            get { return _humidityaccuracy; }
            set {
                _humidityaccuracy = value;
                OnPropertyChanged(nameof(HumidityAccuracy));
            }
        }
        string _humidityact;
        public string HumidityActual {
            get { return _humidityact; }
            set {
                _humidityact = value;
                OnPropertyChanged(nameof(HumidityActual));
            }
        }

        //ppm
        string _ppmvalue;
        public string PPMValue {
            get { return _ppmvalue; }
            set {
                _ppmvalue = value;
                OnPropertyChanged(nameof(PPMValue));
            }
        }
        string _ppmaccuracy;
        public string PPMAccuracy {
            get { return _ppmaccuracy; }
            set {
                _ppmaccuracy = value;
                OnPropertyChanged(nameof(PPMAccuracy));
            }
        }
        string _ppmact;
        public string PPMActual {
            get { return _ppmact; }
            set {
                _ppmact = value;
                OnPropertyChanged(nameof(PPMActual));
            }
        }

        //firmware
        string _firmwaremodevalue;
        public string FirmwareModeValue {
            get { return _firmwaremodevalue; }
            set {
                _firmwaremodevalue = value;
                OnPropertyChanged(nameof(FirmwareModeValue));
            }
        }
        string _firmwaremodeact;
        public string FirmwareModeActual {
            get { return _firmwaremodeact; }
            set {
                _firmwaremodeact = value;
                OnPropertyChanged(nameof(FirmwareModeActual));
            }
        }
        

        //is validate
        bool _ischecktemperature;
        public bool IsCheckTemperature {
            get { return _ischecktemperature; }
            set {
                _ischecktemperature = value;
                if (!value) ValidateTemperature = "Passed";
                OnPropertyChanged(nameof(IsCheckTemperature));
            }
        }
        bool _ischeckhumidity;
        public bool IsCheckHumidity {
            get { return _ischeckhumidity; }
            set {
                _ischeckhumidity = value;
                if (!value) ValidateHumidity = "Passed";
                OnPropertyChanged(nameof(IsCheckHumidity));
            }
        }
        bool _ischeckppm;
        public bool IsCheckPPM {
            get { return _ischeckppm; }
            set {
                _ischeckppm = value;
                if (!value) ValidatePPM = "Passed";
                OnPropertyChanged(nameof(IsCheckPPM));
            }
        }
        bool _ischeckled;
        public bool IsCheckLED {
            get { return _ischeckled; }
            set {
                _ischeckled = value;
                if (!value) ValidateLED = "Passed";
                OnPropertyChanged(nameof(IsCheckLED));
            }
        }
        bool _ischeckspeaker;
        public bool IsCheckSpeaker {
            get { return _ischeckspeaker; }
            set {
                _ischeckspeaker = value;
                if (!value) ValidateSpeaker = "Passed";
                OnPropertyChanged(nameof(IsCheckSpeaker));
            }
        }
        bool _isswitchfirmwaremode;
        public bool IsSwitchFirmwareMode {
            get { return _isswitchfirmwaremode; }
            set {
                _isswitchfirmwaremode = value;
                if (!value) SwitchFirmwareMode = "Passed";
                OnPropertyChanged(nameof(IsSwitchFirmwareMode));
            }
        }
    }
}
