﻿using System;
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

namespace SmartHomeControlLibrary.SMOKEDETECTOR.FINALFUNCTION {

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
            this.FirmwareVersion = "";
            this.Model = "";

            this.IsVerifyID = true;
            this.IsVerifyModel = true;
            this.IsVerifyFW = true;
            this.IsWriteSN = true;
            this.IsCheckHumidity = true;
            this.IsCheckLED = true;
            this.IsCheckSpeaker = true;
            this.IsCheckTemperature = true;

            this.StopTest = "Yes";
            this.CommonRetry = 3;
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
        string _firmwareversion;
        public string FirmwareVersion {
            get { return _firmwareversion; }
            set {
                _firmwareversion = value;
                OnPropertyChanged(nameof(FirmwareVersion));
            }
        }
        string _model;
        public string Model {
            get { return _model; }
            set {
                _model = value;
                OnPropertyChanged(nameof(Model));
            }
        }

        //CÀI ĐẶT BÀI TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        bool _ischecktemperature;
        public bool IsCheckTemperature {
            get { return _ischecktemperature; }
            set {
                _ischecktemperature = value;
                myGlobal.myTesting.IsCheckTemperature = value;
                OnPropertyChanged(nameof(IsCheckTemperature));
            }
        }
        bool _ischeckhumidity;
        public bool IsCheckHumidity {
            get { return _ischeckhumidity; }
            set {
                _ischeckhumidity = value;
                myGlobal.myTesting.IsCheckHumidity = value;
                OnPropertyChanged(nameof(IsCheckHumidity));
            }
        }
        bool _ischeckled;
        public bool IsCheckLED {
            get { return _ischeckled; }
            set {
                _ischeckled = value;
                myGlobal.myTesting.IsCheckLED = value;
                OnPropertyChanged(nameof(IsCheckLED));
            }
        }
        bool _ischeckspeaker;
        public bool IsCheckSpeaker {
            get { return _ischeckspeaker; }
            set {
                _ischeckspeaker = value;
                myGlobal.myTesting.IsCheckSpeaker = value;
                OnPropertyChanged(nameof(IsCheckSpeaker));
            }
        }
        bool _isverifyid;
        public bool IsVerifyID {
            get { return _isverifyid; }
            set {
                _isverifyid = value;
                myGlobal.myTesting.IsVerifyID = value;
                OnPropertyChanged(nameof(IsVerifyID));
            }
        }
        bool _isverifyfw;
        public bool IsVerifyFW {
            get { return _isverifyfw; }
            set {
                _isverifyfw = value;
                myGlobal.myTesting.IsVerifyFW = value;
                OnPropertyChanged(nameof(IsVerifyFW));
            }
        }
        bool _isverifymodel;
        public bool IsVerifyModel {
            get { return _isverifymodel; }
            set {
                _isverifymodel = value;
                myGlobal.myTesting.IsVerifyModel = value;
                OnPropertyChanged(nameof(IsVerifyModel));
            }
        }
        bool _iswritesn;
        public bool IsWriteSN {
            get { return _iswritesn; }
            set {
                _iswritesn = value;
                myGlobal.myTesting.IsWriteSN = value;
                OnPropertyChanged(nameof(IsWriteSN));
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
