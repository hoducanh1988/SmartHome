using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
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

namespace SmartHomeControlLibrary.ZIGBEE.PCBAFUNCTION {
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
                case "serialize_comport": {
                        myGlobal.mySetting.FixComport = "";
                        string[] ports = SerialPort.GetPortNames();
                        if (ports.Length > 0) {
                            string data = "";
                            foreach (string port in ports) {
                                data += port + ",";
                            }
                            myGlobal.mySetting.FixComport = data.Substring(0, data.Length - 1);
                        }
                        MessageBox.Show("Success.", "Serialize Comport", MessageBoxButton.OK, MessageBoxImage.Information);
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
            this.FixComport = "";
            this.DUTPortName = "";
            this.DUTBaudRate = "115200";
            this.DUTDataBits = "8";
            this.DUTParity = "None";
            this.DUTStopBits = "1";


            this.IsCheckConnection = true;
            this.IsCheckTransmission = true;

            this.StopTest = "Yes";
            this.CommonRetry = 5;
            this.GetIDRetry = 1;
            this.DelayRetry = 3000;
        }

        //CÀI ĐẶT USB DONGLE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _fixcomport;
        public string FixComport {
            get { return _fixcomport; }
            set {
                _fixcomport = value;
                OnPropertyChanged(nameof(FixComport));
            }
        }
        string _dutportname;
        public string DUTPortName {
            get { return _dutportname; }
            set {
                _dutportname = value;
                OnPropertyChanged(nameof(DUTPortName));
            }
        }
        string _baudrate;
        public string DUTBaudRate {
            get { return _baudrate; }
            set {
                _baudrate = value;
                OnPropertyChanged(nameof(DUTBaudRate));
            }
        }
        string _databit;
        public string DUTDataBits {
            get { return _databit; }
            set {
                _databit = value;
                OnPropertyChanged(nameof(DUTDataBits));
            }
        }
        string _parity;
        public string DUTParity {
            get { return _parity; }
            set {
                _parity = value;
                OnPropertyChanged(nameof(DUTParity));
            }
        }
        string _stopbit;
        public string DUTStopBits {
            get { return _stopbit; }
            set {
                _stopbit = value;
                OnPropertyChanged(nameof(DUTStopBits));
            }
        }


        //CÀI ĐẶT BÀI TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        bool _ischeckconnection;
        public bool IsCheckConnection {
            get { return _ischeckconnection; }
            set {
                _ischeckconnection = value;
                myGlobal.myTesting.IsCheckConnection = value;
                OnPropertyChanged(nameof(IsCheckConnection));
            }
        }
        bool _ischecktransmision;
        public bool IsCheckTransmission {
            get { return _ischecktransmision; }
            set {
                _ischecktransmision = value;
                myGlobal.myTesting.IsCheckTransmission = value;
                OnPropertyChanged(nameof(IsCheckTransmission));
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
        int _getidretry;
        public int GetIDRetry {
            get { return _getidretry; }
            set {
                _getidretry = value <= 0 ? 1 : value;
                OnPropertyChanged(nameof(GetIDRetry));
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
