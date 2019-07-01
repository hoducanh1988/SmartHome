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

namespace SmartHomeControlLibrary.ALLDETECTOR.USERFUNCTION {

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

            this.TimeOut = "300";
        }

        //CÀI ĐẶT USB DONGLE TRẠM TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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

        //CÀI ĐẶT TRẠM TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _timeout;
        public string TimeOut {
            get { return _timeout; }
            set {
                _timeout = value;
                OnPropertyChanged(nameof(TimeOut));
            }
        }

    }

}
