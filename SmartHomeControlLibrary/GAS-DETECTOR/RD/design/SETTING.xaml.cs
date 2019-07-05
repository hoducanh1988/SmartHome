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

namespace SmartHomeControlLibrary.GASDETECTOR.RD {

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

            Operator = "";

            this.SerialPortName = "";
            this.SerialBaudRate = "115200";
            this.SerialDataBits = "8";
            this.SerialParity = "None";
            this.SerialStopBits = "1";

            this.CommonRetry = 3;
            this.DelayRetry = 3000;

            this.DiemKhongTieuChuan = "0";
            this.DiemKhongSaiSo = "0";
            this.DiemKhongThoiGian = "1000";

            this.TuongDoiTieuChuan = "0";
            this.TuongDoiSaiSo = "0";
            this.TuongDoiThoiGian = "1000";

            this.DoLapTieuChuan = "0";
            this.DoLapLechChuan = "0";
            this.DoLapThoiGian = "1000";

            this.DoTroiSaiSo = "0";
            this.DoTroiTieuChuan = "0";

        }

        //CÀI ĐẶT TRẠM TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _operator;
        public string Operator {
            get { return _operator; }
            set {
                _operator = value;
                OnPropertyChanged(nameof(Operator));
            }
        }


        //CÀI ĐẶT USB DONGLE ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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


        //CÀI ĐẶT CHẾ ĐỘ TEST ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
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


        //CÀI ĐẶT KIEM TRA DIEM KHONG ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _dk_tieuchuan;
        public string DiemKhongTieuChuan {
            get { return _dk_tieuchuan; }
            set {
                _dk_tieuchuan = value;
                myGlobal.myTesting.DiemKhongTieuChuan = value;
                OnPropertyChanged(nameof(DiemKhongTieuChuan));
            }
        }
        string _dk_saiso;
        public string DiemKhongSaiSo {
            get { return _dk_saiso; }
            set {
                _dk_saiso = value;
                myGlobal.myTesting.DiemKhongSaiSo = value;
                OnPropertyChanged(nameof(DiemKhongSaiSo));
            }
        }
        string _dk_thoigian;
        public string DiemKhongThoiGian {
            get { return _dk_thoigian; }
            set {
                _dk_thoigian = value;
                myGlobal.myTesting.DiemKhongThoiGian = value;
                OnPropertyChanged(nameof(DiemKhongThoiGian));
            }
        }

        //CÀI ĐẶT KIEM TRA SAI SO TUONG DOI ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _td_tieuchuan;
        public string TuongDoiTieuChuan {
            get { return _td_tieuchuan; }
            set {
                _td_tieuchuan = value;
                myGlobal.myTesting.TuongDoiTieuChuan = value;
                OnPropertyChanged(nameof(TuongDoiTieuChuan));
            }
        }
        string _td_saiso;
        public string TuongDoiSaiSo {
            get { return _td_saiso; }
            set {
                _td_saiso = value;
                myGlobal.myTesting.TuongDoiSaiSo = value;
                OnPropertyChanged(nameof(TuongDoiSaiSo));
            }
        }
        string _td_thoigian;
        public string TuongDoiThoiGian {
            get { return _td_thoigian; }
            set {
                _td_thoigian = value;
                myGlobal.myTesting.TuongDoiThoiGian = value;
                OnPropertyChanged(nameof(TuongDoiThoiGian));
            }
        }


        //CÀI ĐẶT KIEM TRA DO LAP ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _dl_tieuchuan;
        public string DoLapTieuChuan {
            get { return _dl_tieuchuan; }
            set {
                _dl_tieuchuan = value;
                OnPropertyChanged(nameof(DoLapTieuChuan));
            }
        }
        string _dl_lechchuan;
        public string DoLapLechChuan {
            get { return _dl_lechchuan; }
            set {
                _dl_lechchuan = value;
                myGlobal.myTesting.DoLapLechChuan = value;
                OnPropertyChanged(nameof(DoLapLechChuan));
            }
        }
        string _dl_thoigian;
        public string DoLapThoiGian {
            get { return _dl_thoigian; }
            set {
                _dl_thoigian = value;
                myGlobal.myTesting.DoLapThoiGian = value;
                OnPropertyChanged(nameof(DoLapThoiGian));
            }
        }


        //CÀI ĐẶT KIEM TRA DO TROI ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        string _dt_tieuchuan;
        public string DoTroiTieuChuan {
            get { return _dt_tieuchuan; }
            set {
                _dt_tieuchuan = value;
                OnPropertyChanged(nameof(DoTroiTieuChuan));
            }
        }
        string _dt_saiso;
        public string DoTroiSaiSo {
            get { return _dt_saiso; }
            set {
                _dt_saiso = value;
                myGlobal.myTesting.DoTroiSaiSo = value;
                OnPropertyChanged(nameof(DoTroiSaiSo));
            }
        }


    }


}
