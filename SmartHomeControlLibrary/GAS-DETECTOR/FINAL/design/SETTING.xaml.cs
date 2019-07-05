using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
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

namespace SmartHomeControlLibrary.GASDETECTOR.FINALFUNCTION {
    /// <summary>
    /// Interaction logic for SETTING.xaml
    /// </summary>
    public partial class SETTING : UserControl {

        public SETTING() {
            //init control
            InitializeComponent();

            //load setting from file
            if (File.Exists(myGlobal.SettingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.SettingFileFullName);

            //binding data
            this.DataContext = myGlobal.mySetting;

            //
            this.cbbstoptest.ItemsSource = new List<string>() { "Yes", "No" };

        }

        private void FrameWorkElement_Focus(object sender, RoutedEventArgs e) {
            //string data;

            //FrameworkElement element = sender as FrameworkElement;
            //MyFunction.Global.MyParameter.dictGuide.TryGetValue(element.Tag.ToString(), out data);
            //tbGuide.Text = data;
        }

        private void ComboBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {
            e.Handled = !((ComboBox)sender).IsDropDownOpen;
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            switch (tag) {
                case "save_setting": {
                        XmlHelper<SettingInformation>.ToXmlFile(myGlobal.mySetting, myGlobal.SettingFileFullName); //save setting to xml file
                        MessageBox.Show("Success.", "Save Setting", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
                default: break;
            }

        }
    }


    public class SettingInformation : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public SettingInformation() {
            this.SerialPortName = "";
            this.SerialBaudRate = "115200";
            this.SerialDataBits = "8";
            this.SerialParity = "None";
            this.SerialStopBits = "1";

            this.TemperatureValue = "0";
            this.TemperatureAccuracy = "0";
            this.HumidityValue = "0";
            this.HumidityAccuracy = "0";
            this.PPMValue = "0";
            this.PPMAccuracy = "0";

            this.IsCheckPPM = false;
            this.IsCheckHumidity = true;
            this.IsCheckLED = true;
            this.IsCheckSpeaker = true;
            this.IsCheckTemperature = true;
            this.IsSwitchFirmwareMode = false;

            this.StopTest = "Yes";
            this.CommonRetry = 5;
            this.DelayRetry = 3000;
        }

        //CÀI ĐẶT MODULE ZIGBEE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _portname;
        public string SerialPortName {
            get { return _portname; }
            set {
                _portname = value;
                OnPropertyChanged(nameof(SerialPortName));
            }
        }
        string _baudrate;
        public string SerialBaudRate {
            get { return _baudrate; }
            set {
                _baudrate = value;
                OnPropertyChanged(nameof(SerialBaudRate));
            }
        }
        string _databit;
        public string SerialDataBits {
            get { return _databit; }
            set {
                _databit = value;
                OnPropertyChanged(nameof(SerialDataBits));
            }
        }
        string _parity;
        public string SerialParity {
            get { return _parity; }
            set {
                _parity = value;
                OnPropertyChanged(nameof(SerialParity));
            }
        }
        string _stopbit;
        public string SerialStopBits {
            get { return _stopbit; }
            set {
                _stopbit = value;
                OnPropertyChanged(nameof(SerialStopBits));
            }
        }


        //CÀI ĐẶT GIÁ TRỊ TIÊU CHUẨN ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _temperaturevalue;
        public string TemperatureValue {
            get { return _temperaturevalue; }
            set {
                _temperaturevalue = value;
                OnPropertyChanged(nameof(TemperatureValue));
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

        //CÀI ĐẶT BÀI TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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
        bool _ischeckppm;
        public bool IsCheckPPM {
            get { return _ischeckppm; }
            set {
                _ischeckppm = value;
                OnPropertyChanged(nameof(IsCheckPPM));
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
        bool _isswitchfirmwaremode;
        public bool IsSwitchFirmwareMode {
            get { return _isswitchfirmwaremode; }
            set {
                _isswitchfirmwaremode = value;
                OnPropertyChanged(nameof(IsSwitchFirmwareMode));
            }
        }

        //CÀI ĐẶT CHẾ ĐỘ TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _stoptest;
        public string StopTest {
            get { return _stoptest; }
            set {
                _stoptest = value;
                OnPropertyChanged(nameof(StopTest));
            }
        }
        int _commomretry;
        public int CommonRetry {
            get { return _commomretry; }
            set {
                _commomretry = value <= 0 ? 1 : value;
                OnPropertyChanged(nameof(CommonRetry));
            }
        }
        int _delayretry;
        public int DelayRetry {
            get { return _delayretry; }
            set {
                _delayretry = value < 0 ? 0 : value;
                OnPropertyChanged(nameof(DelayRetry));
            }
        }
    }
}
