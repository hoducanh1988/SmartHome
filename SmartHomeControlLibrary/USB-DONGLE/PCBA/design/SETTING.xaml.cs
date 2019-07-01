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

namespace SmartHomeControlLibrary.USBDongle.PCBAFUNCTION {
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

            //load itemsource cho combobox
            this.cbb_node_rf_type.ItemsSource = new List<string>() { "CO", "GAS", "SMOKE" };
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
                case "select_msdb": {
                        System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                        fileDialog.InitialDirectory = string.Format("{0}access_db", AppDomain.CurrentDomain.BaseDirectory);
                        fileDialog.Filter = "*.accdb|*.accdb";
                        if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                            //get csv file
                            myGlobal.mySetting.MSAccessFile = fileDialog.SafeFileName;
                        }

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

            this.NodeRFType = "GAS";
            this.NodeRFID = "";

            this.MSAccessFile = "ID_LABELPRINT.accdb";

            this.IsCheckComunication = true;
            this.IsCheckFunction = true;
            this.IsPrintLabel = true;
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
        string _portname;
        public string DUTPortName {
            get { return _portname; }
            set {
                _portname = value;
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


        //CÀI ĐẶT NODE RF ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _noderftype;
        public string NodeRFType {
            get { return _noderftype; }
            set {
                _noderftype = value;
                OnPropertyChanged(nameof(NodeRFType));
            }
        }
        string _noderfid;
        public string NodeRFID {
            get { return _noderfid; }
            set {
                _noderfid = value;
                OnPropertyChanged(nameof(NodeRFID));
            }
        }


        //CÀI ĐẶT IN TEM ID ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _msaccessfile;
        public string MSAccessFile {
            get { return _msaccessfile; }
            set {
                _msaccessfile = value;
                OnPropertyChanged(nameof(MSAccessFile));
            }
        }


        //CÀI ĐẶT BÀI TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        bool _ischeckcomunication;
        public bool IsCheckComunication {
            get { return _ischeckcomunication; }
            set {
                _ischeckcomunication = value;
                myGlobal.myTesting.IsCheckComunication = value;
                OnPropertyChanged(nameof(IsCheckComunication));
            }
        }
        bool _ischeckfunction;
        public bool IsCheckFunction {
            get { return _ischeckfunction; }
            set {
                _ischeckfunction = value;
                myGlobal.myTesting.IsCheckFunction = value;
                OnPropertyChanged(nameof(IsCheckFunction));
            }
        }
        bool _isprintlabel;
        public bool IsPrintLabel {
            get { return _isprintlabel; }
            set {
                _isprintlabel = value;
                myGlobal.myTesting.IsPrintLabel = value;
                OnPropertyChanged(nameof(IsPrintLabel));
            }
        }
    }
}
