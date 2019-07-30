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

namespace SmartHomeControlLibrary.CarbonMonoxideDetector.FINALFUNCTION1 {
    /// <summary>
    /// Interaction logic for RUNALL.xaml
    /// </summary>
    public partial class RUNALL : UserControl {

        //List<string> listDevice = new List<string>();
        //TestInfo testinfo = new TestInfo();

        //constructor
        public RUNALL() {
            //init control
            InitializeComponent();

            ////load setting
            //if (File.Exists(myGlobal.SettingFileFullName)) myGlobal.mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.SettingFileFullName);

            ////
            //this.DataContext = testinfo;

            ////get pass fail
            //Thread t = new Thread(new ThreadStart(() => {
            //    while (true) {
            //        try {
            //            if (listDevice.Count > 0) {
            //                Dispatcher.Invoke(new Action(() => {
            //                    int p = 0, f = 0;
            //                    foreach (var c in this.stackpanel_content.Children) {
            //                        string result = (c as ucFinal).myTesting.TotalResult;
            //                        if (result == "Passed") p++;
            //                        if (result == "Failed") f++;
            //                    }
            //                    testinfo.PassedDevice = p;
            //                    testinfo.FailedDevice = f;
            //                }));
            //            }
            //        } catch { }
            //        Thread.Sleep(250);
            //    }
            //}));
            //t.IsBackground = true;
            //t.Start();
        }

        //start button
        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            string tag = b.Tag.ToString();

            //switch (tag) {
            //    case "add": {
            //            InputDeviceID window = new InputDeviceID();
            //            window.ShowDialog();
            //            string id = window.DeviceID;
            //            if (id != "" && listDevice.Contains(id) == false) {
            //                var c = new ucFinal();
            //                c.SetBindingData<SettingInformation>((testinfo.TotalDevice + 1).ToString(),id, DeviceType.SMH_CO, myGlobal.mySetting);
            //                this.stackpanel_content.Children.Add(c);
            //                testinfo.TotalDevice++;
            //                listDevice.Add(id);
            //            }
            //            break;
            //        }
            //    case "clear": {
            //            this.stackpanel_content.Children.Clear();
            //            testinfo.TotalDevice = 0;
            //            testinfo.PassedDevice = 0;
            //            testinfo.FailedDevice = 0;
            //            listDevice.Clear();
            //            break;
            //        }
            //    default: break;
            //}
        }

    }


    //public class TestInfo : INotifyPropertyChanged {
    //    public event PropertyChangedEventHandler PropertyChanged;
    //    protected void OnPropertyChanged(string name) {
    //        PropertyChangedEventHandler handler = PropertyChanged;
    //        if (handler != null) {
    //            handler(this, new PropertyChangedEventArgs(name));
    //        }
    //    }

    //    public TestInfo() {
    //        TotalDevice = 0;
    //        PassedDevice = 0;
    //        FailedDevice = 0;
    //    }
        
    //    int _totaldevice;
    //    public int TotalDevice {
    //        get { return _totaldevice; }
    //        set {
    //            _totaldevice = value;
    //            TestProgress = string.Format("{0}/{1}", PassedDevice, TotalDevice);
    //            OnPropertyChanged(nameof(TotalDevice));
    //        }
    //    }
    //    int _passeddevice;
    //    public int PassedDevice {
    //        get { return _passeddevice; }
    //        set {
    //            _passeddevice = value;
    //            TestProgress = string.Format("{0}/{1}", PassedDevice, TotalDevice);
    //            OnPropertyChanged(nameof(PassedDevice));
    //        }
    //    }
    //    int _faileddevice;
    //    public int FailedDevice {
    //        get { return _faileddevice; }
    //        set {
    //            _faileddevice = value;
    //            OnPropertyChanged(nameof(FailedDevice));
    //        }
    //    }
    //    string _testprogress;
    //    public string TestProgress {
    //        get { return _testprogress; }
    //        set {
    //            _testprogress = value;
    //            OnPropertyChanged(nameof(TestProgress));
    //        }
    //    }
    //}

}
