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
using System.IO;
using UtilityPack.IO;

using SmartHomeControlLibrary.__Common__;
using SmartHomeControlLibrary.__Window__;

namespace SmartHomeControlLibrary.CarbonMonoxideDetector.RD {

    /// <summary>
    /// Interaction logic for RUNALL.xaml
    /// </summary>
    public partial class RUNALL : UserControl {
        public RUNALL() {
            InitializeComponent();

            //load setting from file
            if (File.Exists(myGlobal.SettingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.SettingFileFullName);

            //binding data
            this.DataContext = myGlobal.myTesting;

            this.dg_productid.DataContext = myGlobal.ObservableCollectionProductID;
            this._load_product_id();

            //
            this.dg_diemkhong.DataContext = myGlobal.ObservableCollectionDiemKhong;
            this.dg_tuongdoi.DataContext = myGlobal.ObservableCollectionTuongDoi;
            this.dg_dolap.DataContext = myGlobal.ObservableCollectionDoLap;
            this.dg_dotroi.DataContext = myGlobal.ObservableCollectionDoTroi;
        }


        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter) {
                //check ID
                string id = (sender as TextBox).Text.Trim();
                if (ProjectUtility.IsSmartHomeDeviceID(id)) {
                    //check trùng lặp
                    bool r = false;
                    if (myGlobal.ObservableCollectionProductID.Count == 0) r = true;
                    else {
                        foreach (var item in myGlobal.ObservableCollectionProductID) {
                            if (item.ProductID.ToLower().Contains(id.ToLower())) {
                                r = true;
                                break;
                            }
                        }
                    }
                    if (!r) {
                        myGlobal.ObservableCollectionProductID.Add(new ItemProductID() { ProductID = (sender as TextBox).Text.Trim() });
                        _save_product_id();
                    }
                    else {
                        MessageBox.Show(string.Format("ID {0} đã tồn tại trong danh sách.\r\nVui lòng check lại.", id), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }



                }
                else {
                    MessageBox.Show(string.Format("ID {0} không đúng định dạng.\r\nVui lòng check lại.", id), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                (sender as TextBox).Clear();
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e) {
            CheckBox box = sender as CheckBox;
            string cktag = box.Tag.ToString();

            switch (cktag) {
                case "check_dk": {
                        if (myGlobal.ObservableCollectionProductID.Count > 0) {
                            foreach (var item in myGlobal.ObservableCollectionProductID) {
                                item.IsTest0 = (bool)box.IsChecked;
                            }
                        }
                        break;
                    }
                case "check_td": {
                        if (myGlobal.ObservableCollectionProductID.Count > 0) {
                            foreach (var item in myGlobal.ObservableCollectionProductID) {
                                item.IsTest1 = (bool)box.IsChecked;
                            }
                        }
                        break;
                    }
                case "check_dl": {
                        if (myGlobal.ObservableCollectionProductID.Count > 0) {
                            foreach (var item in myGlobal.ObservableCollectionProductID) {
                                item.IsTest2 = (bool)box.IsChecked;
                            }
                        }
                        break;
                    }
                case "check_dt": {
                        if (myGlobal.ObservableCollectionProductID.Count > 0) {
                            foreach (var item in myGlobal.ObservableCollectionProductID) {
                                item.IsTest3 = (bool)box.IsChecked;
                            }
                        }
                        break;
                    }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) {
            MenuItem menu = sender as MenuItem;
            string mtag = menu.Tag.ToString();

            switch (mtag) {
                case "delete": {
                        var item = this.dg_productid.SelectedItem as ItemProductID;
                        string ID = item.ProductID;
                        string msg = string.Format("Bạn muốn xóa ID {0} khỏi danh sách phải không?\r\nChọn 'Yes' để xóa, 'No' để thoát.", ID);
                        bool r = MessageBox.Show(msg, "Xóa ID", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                        if (r == true) {
                            myGlobal.ObservableCollectionProductID.Remove(item);
                            _save_product_id();
                            MessageBox.Show("Success.", "Xóa ID", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    }
                case "deleteall": {
                        string msg = string.Format("Bạn muốn xóa tất cả ID khỏi danh sách phải không?\r\nChọn 'Yes' để xóa, 'No' để thoát.");
                        bool r = MessageBox.Show(msg, "Xóa Tất Cả ID", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
                        if (r == true) {
                            myGlobal.ObservableCollectionProductID.Clear();
                            _save_product_id();
                            MessageBox.Show("Success.", "Xóa Tất Cả ID", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    }
                case "refresh": {
                        this.dg_productid.UnselectAllCells();
                        break;
                    }
                case "savetofile": {
                        _save_product_id();
                        MessageBox.Show("Success.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        break;
                    }
            }

        }

        void _save_product_id() {
            string f = string.Format("{0}co_rdid.dll", AppDomain.CurrentDomain.BaseDirectory);
            if (myGlobal.ObservableCollectionProductID.Count == 0) {
                if (File.Exists(f)) File.Delete(f);
                else return;
            }

            using (var sw = new StreamWriter(f, false, Encoding.Unicode)) {
                foreach (var item in myGlobal.ObservableCollectionProductID) {
                    string line = string.Format("{0},{1},{2},{3},{4}", item.ProductID, item.IsTest0, item.IsTest1, item.IsTest2, item.IsTest3);
                    sw.WriteLine(line);
                }
            }
        }

        void _load_product_id() {
            string f = string.Format("{0}co_rdid.dll", AppDomain.CurrentDomain.BaseDirectory);
            if (myGlobal.ObservableCollectionProductID.Count != 0) return;
            if (!File.Exists(f)) return;

            string[] lines = File.ReadAllLines(f);
            foreach (var item in lines) {
                if (string.IsNullOrEmpty(item) == false) {
                    string[] buffer = item.Split(',');

                    ItemProductID itemProduct = new ItemProductID();
                    itemProduct.ProductID = buffer[0];
                    itemProduct.IsTest0 = StringToBoolean(buffer[1]);
                    itemProduct.IsTest1 = StringToBoolean(buffer[2]);
                    itemProduct.IsTest2 = StringToBoolean(buffer[3]);
                    itemProduct.IsTest3 = StringToBoolean(buffer[4]);

                    myGlobal.ObservableCollectionProductID.Add(itemProduct);
                }
            }
        }

        bool StringToBoolean(string x) {
            return x.ToLower().Contains("true");
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string btag = b.Tag.ToString();

            switch (btag) {

                case "diemkhong_start": {
                        Thread t = new Thread(new ThreadStart(() => {
                            myGlobal.myTesting.DiemKhongButtonContent = "STOP";
                            myGlobal.myTesting.DiemKhongButtonEnable = false;
                            myGlobal.myTesting.LogSystem = myGlobal.myTesting.DiemKhongSystemLog = "";

                            bool r = _diemkhong_runall_();

                            myGlobal.myTesting.DiemKhongButtonContent = "START";
                            myGlobal.myTesting.DiemKhongButtonEnable = true;
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "tuongdoi_start": {
                        Thread t = new Thread(new ThreadStart(() => {
                            myGlobal.myTesting.TuongDoiButtonContent = "STOP";
                            myGlobal.myTesting.TuongDoiButtonEnable = false;
                            myGlobal.myTesting.LogSystem = myGlobal.myTesting.TuongDoiSystemLog = "";

                            bool r = _tuongdoi_runall_();

                            myGlobal.myTesting.TuongDoiButtonContent = "START";
                            myGlobal.myTesting.TuongDoiButtonEnable = true;
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "dolap_start": {
                        Thread t = new Thread(new ThreadStart(() => {
                            myGlobal.myTesting.DoLapButtonContent = "STOP";
                            myGlobal.myTesting.DoLapButtonEnable = false;
                            myGlobal.myTesting.LogSystem = myGlobal.myTesting.DoLapSystemLog = "";

                            bool r = _dolap_runall_();

                            myGlobal.myTesting.DoLapButtonContent = "START";
                            myGlobal.myTesting.DoLapButtonEnable = true;
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "dotroi_start": {
                        Thread t = new Thread(new ThreadStart(() => {
                            myGlobal.myTesting.DoTroiButtonContent = "STOP";
                            myGlobal.myTesting.DoTroiButtonEnable = false;
                            myGlobal.myTesting.LogSystem = myGlobal.myTesting.DoTroiSystemLog = "";

                            bool r = _dotroi_runall_();

                            myGlobal.myTesting.DoTroiButtonContent = "START";
                            myGlobal.myTesting.DoTroiButtonEnable = true;
                        }));
                        t.IsBackground = true;
                        t.Start();
                        break;
                    }

                case "diemkhong_showlog": {
                        LogWindow window = new LogWindow("", "", myGlobal.myTesting.DiemKhongSystemLog);
                        window.Show();
                        break;
                    }

                case "tuongdoi_showlog": {
                        LogWindow window = new LogWindow("", "", myGlobal.myTesting.TuongDoiSystemLog);
                        window.Show();
                        break;
                    }

                case "dolap_showlog": {
                        LogWindow window = new LogWindow("", "", myGlobal.myTesting.DoLapSystemLog);
                        window.Show();
                        break;
                    }

                case "dotroi_showlog": {
                        LogWindow window = new LogWindow("", "", myGlobal.myTesting.DoTroiSystemLog);
                        window.Show();
                        break;
                    }

                default: break;
            }
        }

        #region Test_Fcuntion

        //KIEM TRA DIEM KHONG - LUU LOG OK
        bool _diemkhong_runall_() {
            bool r = false;
            int commonretry = myGlobal.mySetting.CommonRetry;
            int delayretry = myGlobal.mySetting.DelayRetry;


            if (myGlobal.ObservableCollectionProductID.Count == 0) return r;
            Dispatcher.Invoke(new Action(() => { myGlobal.ObservableCollectionDiemKhong.Clear(); }));

            //check connection between DUT (usb dongle) vs PC
            myGlobal.myTesting.LogSystem = "";
            if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, commonretry);
                myGlobal.myTesting.DiemKhongSystemLog += myGlobal.myTesting.LogSystem;
                if (!r) {
                    return r;
                }
            }

            //get sensor value
            foreach (var product in myGlobal.ObservableCollectionProductID) {
                if (product.IsTest0 == true) {
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Product ID: {0}\r\n", product.ProductID);
                    //add id
                    int c = 0;
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDiemKhong.Add(new ItemKiemTraDiemKhong() { ProductID = product.ProductID, KetQua = "Waiting..." });
                        c = myGlobal.ObservableCollectionDiemKhong.Count - 1;
                    }));

                    double v, ab;
                    string logstr, saiso = "";
                    bool r1, r2, r3;

                    //doc ket qua lan1
                    int d = 0;
                    try {
                        d = (int)(double.Parse(myGlobal.mySetting.DiemKhongThoiGian));
                    }
                    catch { d = 0; }
                    d = d < 0 ? 0 : d;

                    Thread.Sleep(d);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Đo lần 1\r\n");
                    v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DiemKhongSystemLog += logstr;

                    ab = Math.Abs(v - double.Parse(myGlobal.myTesting.DiemKhongTieuChuan));
                    saiso += ab + ", ";
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Sai số tuyệt đối lần 1: {0} ppm\r\n", ab);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Sai số cho phép / 2: {0} / 2 = {1} ppm\r\n", myGlobal.myTesting.DiemKhongSaiSo, double.Parse(myGlobal.myTesting.DiemKhongSaiSo) / 2);
                    r1 = ab <= (double.Parse(myGlobal.myTesting.DiemKhongSaiSo) / 2);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Kết quả lần 1 : {0}\r\n", r1 == true ? "Passed" : "Failed");
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDiemKhong[c].Lan1 = v.ToString();
                    }));

                    //doc ket qua lan2
                    Thread.Sleep(d);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Đo lần 2\r\n");
                    v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DiemKhongSystemLog += logstr;

                    ab = Math.Abs(v - double.Parse(myGlobal.myTesting.DiemKhongTieuChuan));
                    saiso += ab + ", ";
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Sai số tuyệt đối lần 2: {0} ppm\r\n", ab);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Sai số cho phép / 2: {0} / 2 = {1} ppm\r\n", myGlobal.myTesting.DiemKhongSaiSo, double.Parse(myGlobal.myTesting.DiemKhongSaiSo) / 2);
                    r2 = ab <= (double.Parse(myGlobal.myTesting.DiemKhongSaiSo) / 2);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Kết quả lần 2: {0}\r\n", r2 == true ? "Passed" : "Failed");
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDiemKhong[c].Lan2 = v.ToString();
                    }));


                    //doc ket qua lan3
                    Thread.Sleep(d);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Đo lần 3\r\n");
                    v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DiemKhongSystemLog += logstr;

                    ab = Math.Abs(v - double.Parse(myGlobal.myTesting.DiemKhongTieuChuan));
                    saiso += ab;
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Sai số tuyệt đối lần 3: {0} ppm\r\n", ab);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Sai số cho phép / 2: {0} / 2 = {1} ppm\r\n", myGlobal.myTesting.DiemKhongSaiSo, double.Parse(myGlobal.myTesting.DiemKhongSaiSo) / 2);
                    r3 = ab <= (double.Parse(myGlobal.myTesting.DiemKhongSaiSo) / 2);
                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("Kết quả lần 3: {0}\r\n", r3 == true ? "Passed" : "Failed");
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDiemKhong[c].Lan3 = v.ToString();
                    }));

                    //phan dinh pass/fail
                    r = r1 && r2 && r3;
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDiemKhong[c].KetQua = r == true ? "Passed" : "Failed";
                    }));

                    myGlobal.myTesting.DiemKhongSystemLog += string.Format("\r\nKết quả sản phẩm : {0}\r\n", r == true ? "Passed" : "Failed");

                    //save excel log
                    Dispatcher.Invoke(new Action(() => {
                        string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                        dir = string.Format("{0}\\codetector", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\rd", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\{1}", dir, myGlobal.ObservableCollectionDiemKhong[c].ProductID);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }

                        string f = string.Format("DK_{0}_{1}_{2}",
                                                 myGlobal.ObservableCollectionDiemKhong[c].ProductID,
                                                 DateTime.Now.ToString("yyyyMMddHHmmss"),
                                                 myGlobal.ObservableCollectionDiemKhong[c].KetQua
                                                 );

                        ProjectUtility.WriteLogExcel_KiemTraDiemKhong(DeviceType.SMH_CO, dir, f, myGlobal.ObservableCollectionDiemKhong[c], myGlobal.mySetting, saiso);
                    }));


                    //save log detail
                    Dispatcher.Invoke(new Action(() => {
                        string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                        dir = string.Format("{0}\\codetector", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\rd", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\logdetail", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        string f = string.Format("{0}\\{1}_DK.txt",
                                                 dir,
                                                 DateTime.Now.ToString("yyyyMMdd"));
                        using (var sw = new StreamWriter(f, true, Encoding.Unicode)) { sw.WriteLine(myGlobal.myTesting.DiemKhongSystemLog); }
                    }));

                }
            }

            return r;
        }

        //KIEM TRA SAI SO TUONG DOI - LUU LOG OK
        bool _tuongdoi_runall_() {
            bool r = false;
            int commonretry = myGlobal.mySetting.CommonRetry;
            int delayretry = myGlobal.mySetting.DelayRetry;

            if (myGlobal.ObservableCollectionProductID.Count == 0) return r;
            Dispatcher.Invoke(new Action(() => { myGlobal.ObservableCollectionTuongDoi.Clear(); }));

            //check connection between DUT (usb dongle) vs PC
            myGlobal.myTesting.LogSystem = "";
            if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, commonretry);
                myGlobal.myTesting.TuongDoiSystemLog += myGlobal.myTesting.LogSystem;
                if (!r) {
                    return r;
                }
            }

            //get sensor value
            foreach (var product in myGlobal.ObservableCollectionProductID) {
                if (product.IsTest1 == true) {
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Product ID: {0}\r\n", product.ProductID);
                    //add id
                    int c = 0;
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionTuongDoi.Add(new ItemKiemTraTuongDoi() { ProductID = product.ProductID, KetQua = "Waiting..." });
                        c = myGlobal.ObservableCollectionTuongDoi.Count - 1;
                    }));

                    double v, ab, a1, a2, a3;
                    string logstr;
                    bool r1, r2, r3;

                    //doc ket qua lan1
                    int d = 0;
                    try {
                        d = (int)(double.Parse(myGlobal.mySetting.TuongDoiThoiGian));
                    }
                    catch { d = 0; }
                    d = d < 0 ? 0 : d;

                    Thread.Sleep(d);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Đo lần 1\r\n");
                    v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.TuongDoiSystemLog += logstr;

                    ab = ProjectUtility.TinhSaiSoTuongDoi(v, double.Parse(myGlobal.mySetting.TuongDoiTieuChuan));
                    a1 = ab;
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Sai số tương đối lần 1: {0} %\r\n", ab);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Sai số tương đối cho phép: {0} %\r\n", myGlobal.myTesting.TuongDoiSaiSo);
                    r1 = ab <= double.Parse(myGlobal.myTesting.TuongDoiSaiSo);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Kết quả lần 1 : {0}\r\n", r1 == true ? "Passed" : "Failed");

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionTuongDoi[c].Lan1 = v.ToString();
                    }));

                    //doc ket qua lan2
                    Thread.Sleep(d);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Đo lần 2\r\n");
                    v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.TuongDoiSystemLog += logstr;

                    ab = ProjectUtility.TinhSaiSoTuongDoi(v, double.Parse(myGlobal.mySetting.TuongDoiTieuChuan));
                    a2 = ab;
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Sai số tương đối lần 2: {0} %\r\n", ab);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Sai số tương đối cho phép: {0} %\r\n", myGlobal.myTesting.TuongDoiSaiSo);
                    r2 = ab <= double.Parse(myGlobal.myTesting.TuongDoiSaiSo);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Kết quả lần 2 : {0}\r\n", r2 == true ? "Passed" : "Failed");

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionTuongDoi[c].Lan2 = v.ToString();
                    }));


                    //doc ket qua lan3
                    Thread.Sleep(d);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Đo lần 3\r\n");
                    v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.TuongDoiSystemLog += logstr;

                    ab = ProjectUtility.TinhSaiSoTuongDoi(v, double.Parse(myGlobal.mySetting.TuongDoiTieuChuan));
                    a3 = ab;
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Sai số tương đối lần 3: {0} %\r\n", ab);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Sai số tương đối cho phép: {0} %\r\n", myGlobal.myTesting.TuongDoiSaiSo);
                    r3 = ab <= double.Parse(myGlobal.myTesting.TuongDoiSaiSo);
                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("Kết quả lần 3 : {0}\r\n", r3 == true ? "Passed" : "Failed");

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionTuongDoi[c].Lan3 = v.ToString();
                    }));

                    //phan dinh pass/fail
                    r = r1 && r2 && r3;
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionTuongDoi[c].KetQua = r == true ? "Passed" : "Failed";
                    }));

                    myGlobal.myTesting.TuongDoiSystemLog += string.Format("\r\nKết quả sản phẩm : {0}\r\n", r == true ? "Passed" : "Failed");


                    //save excel log
                    Dispatcher.Invoke(new Action(() => {

                        string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                        dir = string.Format("{0}\\codetector", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\rd", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\{1}", dir, myGlobal.ObservableCollectionTuongDoi[c].ProductID);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }

                        string f = string.Format("TD_{0}_{1}_{2}",
                                                 myGlobal.ObservableCollectionTuongDoi[c].ProductID,
                                                 DateTime.Now.ToString("yyyyMMddHHmmss"),
                                                 myGlobal.ObservableCollectionTuongDoi[c].KetQua
                                                 );

                        ProjectUtility.WriteLogExcel_KiemTraSaiSoTuongDoi(DeviceType.SMH_CO, dir, f,
                                                                          myGlobal.ObservableCollectionTuongDoi[c],
                                                                          myGlobal.mySetting,
                                                                          a1.ToString(),
                                                                          a2.ToString(),
                                                                          a3.ToString(),
                                                                          r1 == true ? "Passed" : "Failed",
                                                                          r2 == true ? "Passed" : "Failed",
                                                                          r3 == true ? "Passed" : "Failed");
                    }));

                    //save log detail
                    Dispatcher.Invoke(new Action(() => {
                        string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                        dir = string.Format("{0}\\codetector", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\rd", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\logdetail", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        string f = string.Format("{0}\\{1}_TD.txt",
                                                 dir,
                                                 DateTime.Now.ToString("yyyyMMdd"));
                        using (var sw = new StreamWriter(f, true, Encoding.Unicode)) { sw.WriteLine(myGlobal.myTesting.TuongDoiSystemLog); }
                    }));
                }
            }

            return r;
        }

        //KIEM TRA DO LAP - LUU LOG OK
        bool _dolap_runall_() {
            bool r = false;
            int commonretry = myGlobal.mySetting.CommonRetry;
            int delayretry = myGlobal.mySetting.DelayRetry;


            if (myGlobal.ObservableCollectionProductID.Count == 0) return r;
            Dispatcher.Invoke(new Action(() => { myGlobal.ObservableCollectionDoLap.Clear(); }));

            //check connection between DUT (usb dongle) vs PC
            myGlobal.myTesting.LogSystem = "";
            if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, commonretry);
                myGlobal.myTesting.DoLapSystemLog += myGlobal.myTesting.LogSystem;
                if (!r) {
                    return r;
                }
            }

            //get sensor value
            foreach (var product in myGlobal.ObservableCollectionProductID) {
                if (product.IsTest2 == true) {
                    myGlobal.myTesting.DoLapSystemLog += string.Format("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Product ID: {0}\r\n", product.ProductID);
                    //add id
                    int c = 0;
                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap.Add(new ItemKiemTraDoLap() { ProductID = product.ProductID, KetQua = "Waiting..." });
                        c = myGlobal.ObservableCollectionDoLap.Count - 1;
                    }));

                    double[] vs = new double[10];
                    double s = 0;
                    string logstr = "";

                    int d = 0;
                    try {
                        d = (int)(double.Parse(myGlobal.mySetting.DoLapThoiGian));
                    }
                    catch { d = 0; }
                    d = d < 0 ? 0 : d;

                    //doc ket qua lan1
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 1\r\n");
                    vs[0] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[0].ToString() + ", ";
                    }));

                    //doc ket qua lan2
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 2\r\n");
                    vs[1] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[1].ToString() + ", ";
                    }));

                    //doc ket qua lan3
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 3\r\n");
                    vs[2] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[2].ToString() + ", ";
                    }));

                    //doc ket qua lan4
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 4\r\n");
                    vs[3] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[3].ToString() + ", ";
                    }));

                    //doc ket qua lan5
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 5\r\n");
                    vs[4] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[4].ToString() + ", ";
                    }));

                    //doc ket qua lan6
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 6\r\n");
                    vs[5] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[5].ToString() + ", ";
                    }));


                    //doc ket qua lan7
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 7\r\n");
                    vs[6] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[6].ToString() + ", ";
                    }));

                    //doc ket qua lan8
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 8\r\n");
                    vs[7] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[7].ToString() + ", ";
                    }));

                    //doc ket qua lan9
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 9\r\n");
                    vs[8] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[8].ToString() + ", ";
                    }));

                    //doc ket qua lan10
                    Thread.Sleep(d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nChờ {0} ms\r\n", d);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Đo lần 10\r\n");
                    vs[9] = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                    myGlobal.myTesting.DoLapSystemLog += logstr;

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].Lan110 += vs[9].ToString() + ", ";
                    }));

                    //phan dinh pass/fail
                    if (vs.Average() == -999) s = 999;
                    else s = ProjectUtility.TinhDoLechChuan(vs);

                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nĐộ lệch chuẩn sản phẩm: {0} ppm\r\n", s);
                    myGlobal.myTesting.DoLapSystemLog += string.Format("Độ lệch chuẩn cho phép / 3: {0} / 3 = {1} ppm\r\n", myGlobal.mySetting.DoLapLechChuan, double.Parse(myGlobal.mySetting.DoLapLechChuan) / 3);
                    r = s <= (double.Parse(myGlobal.mySetting.DoLapLechChuan) / 3);

                    Dispatcher.Invoke(new Action(() => {
                        myGlobal.ObservableCollectionDoLap[c].KetQua = r == true ? "Passed" : "Failed";
                    }));

                    myGlobal.myTesting.DoLapSystemLog += string.Format("\r\nKết quả sản phẩm : {0}\r\n", r == true ? "Passed" : "Failed");

                    //save excel log
                    Dispatcher.Invoke(new Action(() => {

                        string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                        dir = string.Format("{0}\\codetector", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\rd", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\{1}", dir, myGlobal.ObservableCollectionDoLap[c].ProductID);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }

                        string f = string.Format("DL_{0}_{1}_{2}",
                                                 myGlobal.ObservableCollectionDoLap[c].ProductID,
                                                 DateTime.Now.ToString("yyyyMMddHHmmss"),
                                                 myGlobal.ObservableCollectionDoLap[c].KetQua
                                                 );

                        ProjectUtility.WriteLogExcel_KiemTraDoLap(DeviceType.SMH_CO, dir, f, myGlobal.ObservableCollectionDoLap[c], myGlobal.mySetting, vs.Average(), s);
                    }));


                    //save log detail
                    Dispatcher.Invoke(new Action(() => {
                        string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                        dir = string.Format("{0}\\codetector", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\rd", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        dir = string.Format("{0}\\logdetail", dir);
                        if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                        string f = string.Format("{0}\\{1}_DL.txt",
                                                 dir,
                                                 DateTime.Now.ToString("yyyyMMdd"));
                        using (var sw = new StreamWriter(f, true, Encoding.Unicode)) { sw.WriteLine(myGlobal.myTesting.DoLapSystemLog); }
                    }));

                }
            }

            return r;
        }

        //KIEM TRA DO TROI
        int[] step_time = null;
        bool _dotroi_runall_() {
            bool r = false;
            int commonretry = myGlobal.mySetting.CommonRetry;
            int delayretry = myGlobal.mySetting.DelayRetry;


            if (myGlobal.ObservableCollectionProductID.Count == 0) return r;
            if (step_time == null || step_time[0] > 3) {
                step_time = new int[myGlobal.ObservableCollectionProductID.Count];
                for (int i = 0; i < step_time.Length; i++) step_time[i] = 0;
            }

            Dispatcher.Invoke(new Action(() => {
                if (step_time[0] == 0 || step_time[0] > 3) {
                    myGlobal.ObservableCollectionDoTroi.Clear();
                }
            }));

            //check connection between DUT (usb dongle) vs PC
            myGlobal.myTesting.LogSystem = "";
            if (ProjectTestItem.DUT == null || ProjectTestItem.DUT.IsConnected == false) {
                r = ProjectTestItem.Open_Device_USB_Dongle(myGlobal.myTesting, myGlobal.mySetting, commonretry);
                myGlobal.myTesting.DoTroiSystemLog += myGlobal.myTesting.LogSystem;
                if (!r) {
                    return r;
                }
            }

            //get sensor value
            for (int i = 0; i < myGlobal.ObservableCollectionProductID.Count; i++) {
                var product = myGlobal.ObservableCollectionProductID[i];

                if (product.IsTest3 == true) {

                    double v;
                    string logstr;
                    bool r21, r31;
                    //int c = 0;

                    //step 0
                    if (step_time[i] == 0) {
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("Product ID: {0}\r\n", product.ProductID);
                        //add id
                        Dispatcher.Invoke(new Action(() => {
                            myGlobal.ObservableCollectionDoTroi.Add(new ItemKiemTraDoTroi() { ProductID = product.ProductID, KetQua = "Waiting..." });
                            //c = myGlobal.ObservableCollectionDoTroi.Count - 1;
                        }));
                        step_time[i] = 1;
                    }

                    //step 1 - kiem tra lan 1
                    if (step_time[i] == 1) {
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("Đo lần 1\r\n");
                        v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                        myGlobal.myTesting.DoTroiSystemLog += logstr;

                        Dispatcher.Invoke(new Action(() => {
                            myGlobal.ObservableCollectionDoTroi[i].Lan1 = v.ToString();
                            myGlobal.ObservableCollectionDoTroi[i].t1 = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        }));
                        step_time[i] = 2;
                        goto CNN;
                    }

                    //step 2 - kiem tra lan 2
                    if (step_time[i] == 2) {
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("Đo lần 2\r\n");
                        v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                        myGlobal.myTesting.DoTroiSystemLog += logstr;

                        Dispatcher.Invoke(new Action(() => {
                            myGlobal.ObservableCollectionDoTroi[i].Lan2 = v.ToString();
                            myGlobal.ObservableCollectionDoTroi[i].t2 = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        }));
                        step_time[i] = 3;
                        goto CNN;
                    }

                    //step 3 - kiem tra lan 3
                    if (step_time[i] == 3) {
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("Đo lần 3\r\n");
                        v = ProjectTestItem.Get_Sensor_Value_D(product.ProductID, DeviceType.SMH_CO, SensorType.CO, commonretry, delayretry, out logstr);
                        myGlobal.myTesting.DoTroiSystemLog += logstr;

                        Dispatcher.Invoke(new Action(() => {
                            myGlobal.ObservableCollectionDoTroi[i].Lan3 = v.ToString();
                            myGlobal.ObservableCollectionDoTroi[i].t3 = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                        }));
                        step_time[i] = 4;
                    }

                    //step 4 - phan dinh passed/failed
                    if (step_time[i] == 4) {
                        double v1 = double.Parse(myGlobal.ObservableCollectionDoTroi[i].Lan1);
                        double v2 = double.Parse(myGlobal.ObservableCollectionDoTroi[i].Lan2);
                        double v3 = double.Parse(myGlobal.ObservableCollectionDoTroi[i].Lan3);
                        double ss = double.Parse(myGlobal.mySetting.DoTroiSaiSo);

                        r21 = (v1 == -999 && v2 == -999 && v3 == -999) ? false : Math.Abs(v2 - v1) <= ss;
                        r31 = (v1 == -999 && v2 == -999 && v3 == -999) ? false : Math.Abs(v3 - v1) <= ss;

                        myGlobal.myTesting.DoTroiSystemLog += string.Format("Sai số cho phép: {0} ppm\r\n", ss);
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("Sai lệch giữa lần 2 và lần 1: {0} ppm => phán định: {1}\r\n", Math.Abs(v2 - v1), r21);
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("Sai lệch giữa lần 3 và lần 1: {0} ppm => phán định: {1}\r\n", Math.Abs(v3 - v1), r31);

                        r = r21 && r31;

                        Dispatcher.Invoke(new Action(() => {
                            myGlobal.ObservableCollectionDoTroi[i].KetQua = r == true ? "Passed" : "Failed";
                        }));
                        myGlobal.myTesting.DoTroiSystemLog += string.Format("\r\nKết quả sản phẩm : {0}\r\n", r == true ? "Passed" : "Failed");

                        //save excel log
                        Dispatcher.Invoke(new Action(() => {

                            string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                            dir = string.Format("{0}\\codetector", dir);
                            if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                            dir = string.Format("{0}\\rd", dir);
                            if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                            dir = string.Format("{0}\\{1}", dir, myGlobal.ObservableCollectionDoTroi[i].ProductID);
                            if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }

                            string f = string.Format("DT_{0}_{1}_{2}",
                                                     myGlobal.ObservableCollectionDoTroi[i].ProductID,
                                                     DateTime.Now.ToString("yyyyMMddHHmmss"),
                                                     myGlobal.ObservableCollectionDoTroi[i].KetQua
                                                     );

                            ProjectUtility.WriteLogExcel_KiemTraDoTroi(DeviceType.SMH_CO, dir, f,
                                                                       myGlobal.ObservableCollectionDoTroi[i],
                                                                       myGlobal.mySetting,
                                                                       myGlobal.ObservableCollectionDoTroi[i].t1,
                                                                       myGlobal.ObservableCollectionDoTroi[i].t2,
                                                                       myGlobal.ObservableCollectionDoTroi[i].t3,
                                                                       Math.Abs(v2 - v1).ToString(),
                                                                       Math.Abs(v3 - v1).ToString());

                        }));

                        //save log detail
                        Dispatcher.Invoke(new Action(() => {
                            string dir = string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory);
                            dir = string.Format("{0}\\codetector", dir);
                            if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                            dir = string.Format("{0}\\rd", dir);
                            if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                            dir = string.Format("{0}\\logdetail", dir);
                            if (Directory.Exists(dir) == false) { Directory.CreateDirectory(dir); Thread.Sleep(100); }
                            string f = string.Format("{0}\\{1}_DT.txt",
                                                     dir,
                                                     DateTime.Now.ToString("yyyyMMdd"));
                            using (var sw = new StreamWriter(f, true, Encoding.Unicode)) { sw.WriteLine(myGlobal.myTesting.DoTroiSystemLog); }
                        }));

                        step_time[i] = 5;
                    }
                }

            CNN:
                Thread.Sleep(10);
            }

            return r;
        }

        #endregion
    }

    public class TestingInformation : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public TestingInformation() {

            //Diem khong
            DiemKhongButtonContent = "START";
            DiemKhongButtonEnable = true;
            DiemKhongSystemLog = "";

            //Tuong doi
            TuongDoiButtonContent = "START";
            TuongDoiButtonEnable = true;
            TuongDoiSystemLog = "";

            //Do lap
            DoLapButtonContent = "START";
            DoLapButtonEnable = true;
            DoLapSystemLog = "";

            //Do troi
            DoTroiButtonContent = "START";
            DoTroiButtonEnable = true;
            DoTroiSystemLog = "";

            //
            LogSystem = "";
        }

        //KIEM TRA DIEM KHONG ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        string _dk_tieuchuan;
        public string DiemKhongTieuChuan {
            get { return _dk_tieuchuan; }
            set {
                _dk_tieuchuan = value;
                OnPropertyChanged(nameof(DiemKhongTieuChuan));
            }
        }
        string _dk_saiso;
        public string DiemKhongSaiSo {
            get { return _dk_saiso; }
            set {
                _dk_saiso = value;
                OnPropertyChanged(nameof(DiemKhongSaiSo));
            }
        }
        string _dk_thoigian;
        public string DiemKhongThoiGian {
            get { return _dk_thoigian; }
            set {
                _dk_thoigian = value;
                OnPropertyChanged(nameof(DiemKhongThoiGian));
            }
        }
        string _dk_systemlog;
        public string DiemKhongSystemLog {
            get { return _dk_systemlog; }
            set {
                _dk_systemlog = value;
                OnPropertyChanged(nameof(DiemKhongSystemLog));
            }
        }
        string _dk_buttoncontent;
        public string DiemKhongButtonContent {
            get { return _dk_buttoncontent; }
            set {
                _dk_buttoncontent = value;
                OnPropertyChanged(nameof(DiemKhongButtonContent));
            }
        }
        bool _dk_buttonenable;
        public bool DiemKhongButtonEnable {
            get { return _dk_buttonenable; }
            set {
                _dk_buttonenable = value;
                OnPropertyChanged(nameof(DiemKhongButtonEnable));
            }
        }

        //KIEM TRA SAI SO TUONG DOI ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        string _td_tieuchuan;
        public string TuongDoiTieuChuan {
            get { return _td_tieuchuan; }
            set {
                _td_tieuchuan = value;
                OnPropertyChanged(nameof(TuongDoiTieuChuan));
            }
        }
        string _td_saiso;
        public string TuongDoiSaiSo {
            get { return _td_saiso; }
            set {
                _td_saiso = value;
                OnPropertyChanged(nameof(TuongDoiSaiSo));
            }
        }
        string _td_thoigian;
        public string TuongDoiThoiGian {
            get { return _td_thoigian; }
            set {
                _td_thoigian = value;
                OnPropertyChanged(nameof(TuongDoiThoiGian));
            }
        }
        string _td_systemlog;
        public string TuongDoiSystemLog {
            get { return _td_systemlog; }
            set {
                _td_systemlog = value;
                OnPropertyChanged(nameof(TuongDoiSystemLog));
            }
        }
        string _td_buttoncontent;
        public string TuongDoiButtonContent {
            get { return _td_buttoncontent; }
            set {
                _td_buttoncontent = value;
                OnPropertyChanged(nameof(TuongDoiButtonContent));
            }
        }
        bool _td_buttonenable;
        public bool TuongDoiButtonEnable {
            get { return _td_buttonenable; }
            set {
                _td_buttonenable = value;
                OnPropertyChanged(nameof(TuongDoiButtonEnable));
            }
        }


        //KIEM TRA DO LAP ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        string _dl_lechchuan;
        public string DoLapLechChuan {
            get { return _dl_lechchuan; }
            set {
                _dl_lechchuan = value;
                OnPropertyChanged(nameof(DoLapLechChuan));
            }
        }
        string _dl_thoigian;
        public string DoLapThoiGian {
            get { return _dl_thoigian; }
            set {
                _dl_thoigian = value;
                OnPropertyChanged(nameof(DoLapThoiGian));
            }
        }
        string _dl_systemlog;
        public string DoLapSystemLog {
            get { return _dl_systemlog; }
            set {
                _dl_systemlog = value;
                OnPropertyChanged(nameof(DoLapSystemLog));
            }
        }
        string _dl_buttoncontent;
        public string DoLapButtonContent {
            get { return _dl_buttoncontent; }
            set {
                _dl_buttoncontent = value;
                OnPropertyChanged(nameof(DoLapButtonContent));
            }
        }
        bool _dl_buttonenable;
        public bool DoLapButtonEnable {
            get { return _dl_buttonenable; }
            set {
                _dl_buttonenable = value;
                OnPropertyChanged(nameof(DoLapButtonEnable));
            }
        }


        //KIEM TRA DO TROI ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        string _dt_saiso;
        public string DoTroiSaiSo {
            get { return _dt_saiso; }
            set {
                _dt_saiso = value;
                OnPropertyChanged(nameof(DoTroiSaiSo));
            }
        }
        string _dt_systemlog;
        public string DoTroiSystemLog {
            get { return _dt_systemlog; }
            set {
                _dt_systemlog = value;
                OnPropertyChanged(nameof(DoTroiSystemLog));
            }
        }
        string _dt_buttoncontent;
        public string DoTroiButtonContent {
            get { return _dt_buttoncontent; }
            set {
                _dt_buttoncontent = value;
                OnPropertyChanged(nameof(DoTroiButtonContent));
            }
        }
        bool _dt_buttonenable;
        public bool DoTroiButtonEnable {
            get { return _dt_buttonenable; }
            set {
                _dt_buttonenable = value;
                OnPropertyChanged(nameof(DoTroiButtonEnable));
            }
        }


        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
        string _logsystem;
        public string LogSystem {
            get { return _logsystem; }
            set {
                _logsystem = value;
                OnPropertyChanged(nameof(LogSystem));
            }
        }
    }

    public class ItemProductID : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public ItemProductID() {
            ProductID = "";
            IsTest0 = true;
            IsTest1 = true;
            IsTest2 = true;
            IsTest3 = true;
        }


        string _id;
        public string ProductID {
            get { return _id; }
            set {
                _id = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }
        bool _istest_0;
        public bool IsTest0 {
            get { return _istest_0; }
            set {
                _istest_0 = value;
                OnPropertyChanged(nameof(IsTest0));
            }
        }
        bool _istest_1;
        public bool IsTest1 {
            get { return _istest_1; }
            set {
                _istest_1 = value;
                OnPropertyChanged(nameof(IsTest1));
            }
        }
        bool _istest_2;
        public bool IsTest2 {
            get { return _istest_2; }
            set {
                _istest_2 = value;
                OnPropertyChanged(nameof(IsTest2));
            }
        }
        bool _istest_3;
        public bool IsTest3 {
            get { return _istest_3; }
            set {
                _istest_3 = value;
                OnPropertyChanged(nameof(IsTest3));
            }
        }
    }

    public class ItemKiemTraDiemKhong : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        string _id;
        public string ProductID {
            get { return _id; }
            set {
                _id = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }
        string _lan_1;
        public string Lan1 {
            get { return _lan_1; }
            set {
                _lan_1 = value;
                OnPropertyChanged(nameof(Lan1));
            }
        }
        string _lan_2;
        public string Lan2 {
            get { return _lan_2; }
            set {
                _lan_2 = value;
                OnPropertyChanged(nameof(Lan2));
            }
        }
        string _lan_3;
        public string Lan3 {
            get { return _lan_3; }
            set {
                _lan_3 = value;
                OnPropertyChanged(nameof(Lan3));
            }
        }
        string _ketqua;
        public string KetQua {
            get { return _ketqua; }
            set {
                _ketqua = value;
                OnPropertyChanged(nameof(KetQua));
            }
        }
    }

    public class ItemKiemTraTuongDoi : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        string _id;
        public string ProductID {
            get { return _id; }
            set {
                _id = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }
        string _lan_1;
        public string Lan1 {
            get { return _lan_1; }
            set {
                _lan_1 = value;
                OnPropertyChanged(nameof(Lan1));
            }
        }
        string _lan_2;
        public string Lan2 {
            get { return _lan_2; }
            set {
                _lan_2 = value;
                OnPropertyChanged(nameof(Lan2));
            }
        }
        string _lan_3;
        public string Lan3 {
            get { return _lan_3; }
            set {
                _lan_3 = value;
                OnPropertyChanged(nameof(Lan3));
            }
        }
        string _ketqua;
        public string KetQua {
            get { return _ketqua; }
            set {
                _ketqua = value;
                OnPropertyChanged(nameof(KetQua));
            }
        }
    }

    public class ItemKiemTraDoLap : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        string _id;
        public string ProductID {
            get { return _id; }
            set {
                _id = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }
        string _lan_1_10;
        public string Lan110 {
            get { return _lan_1_10; }
            set {
                _lan_1_10 = value;
                OnPropertyChanged(nameof(Lan110));
            }
        }
        string _ketqua;
        public string KetQua {
            get { return _ketqua; }
            set {
                _ketqua = value;
                OnPropertyChanged(nameof(KetQua));
            }
        }
    }

    public class ItemKiemTraDoTroi : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        string _id;
        public string ProductID {
            get { return _id; }
            set {
                _id = value;
                OnPropertyChanged(nameof(ProductID));
            }
        }
        string _lan_1;
        public string Lan1 {
            get { return _lan_1; }
            set {
                _lan_1 = value;
                OnPropertyChanged(nameof(Lan1));
            }
        }
        string _lan_2;
        public string Lan2 {
            get { return _lan_2; }
            set {
                _lan_2 = value;
                OnPropertyChanged(nameof(Lan2));
            }
        }
        string _lan_3;
        public string Lan3 {
            get { return _lan_3; }
            set {
                _lan_3 = value;
                OnPropertyChanged(nameof(Lan3));
            }
        }
        string _ketqua;
        public string KetQua {
            get { return _ketqua; }
            set {
                _ketqua = value;
                OnPropertyChanged(nameof(KetQua));
            }
        }

        public string t1 { get; set; }
        public string t2 { get; set; }
        public string t3 { get; set; }
    }

}
