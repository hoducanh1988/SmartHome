using SmartHomeControlLibrary.__Common__;
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
using System.Windows.Threading;
using System.Diagnostics;

namespace SmartHomeControlLibrary.__Userctrl__ {
    /// <summary>
    /// Interaction logic for ucUserFinal.xaml
    /// </summary>
    public partial class ucUserFinal : UserControl {

        public UserTestingInformation myTesting = null;
        Thread t = null;
        int timeout = 300;

        public ucUserFinal() {
            InitializeComponent();
        }

        //get seting 
        public void UserSetBindingData(string _idx, string _id, int _timeout) {
            this.myTesting = new UserTestingInformation();
            this.timeout = _timeout;

            //set id, index, devicetype
            this.myTesting.DeviceIndex = _idx;
            this.myTesting.DeviceID = _id;
            this.myTesting.DeviceType = "Unknown";
            this.myTesting.ElapsedTime = "00:00:00";
            
            //binding data
            this.DataContext = this.myTesting;
        }

        public void StopThread() {
            if (t != null && t.IsAlive == true) {
                t.Abort();
            }
        }

        public void StartThread() {
            this.myTesting.TotalResult = "Waiting...";
            this.myTesting.DeviceType = "Unknown";
            this.myTesting.ElapsedTime = "00:00:00";

            t = new Thread(new ThreadStart(() => {
                try {
                    Stopwatch st = new Stopwatch();
                    st.Start();

                    while (true) {
                        TimeSpan result = TimeSpan.FromMilliseconds(st.ElapsedMilliseconds);
                        this.myTesting.ElapsedTime = result.ToString("hh':'mm':'ss");
                        Thread.Sleep(500);
                        if ((st.ElapsedMilliseconds / 1000) > timeout) {
                            this.myTesting.TotalResult = "Failed";
                            break;
                        }
                    }
                }
                catch { }
            }));
            t.IsBackground = true;
            t.Start();
        }
    }


    public class UserTestingInformation : INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        //constructor
        public UserTestingInformation() {
            initParameter();
        }

        //method
        public void initParameter() {
            TotalResult = "-";
            DeviceID = "";
            DeviceType = "Unknown";
            DeviceIndex = "";
            ElapsedTime = "00:00:00";
        }


        public bool initPassed() {
            TotalResult = "Passed";
            return true;
        }

        public bool initFailed() {
            TotalResult = "Failed";
            return true;
        }


        string _deviceindex;
        public string DeviceIndex {
            get { return _deviceindex; }
            set {
                _deviceindex = value;
                OnPropertyChanged(nameof(DeviceIndex));
            }
        }
        string _devicetype;
        public string DeviceType {
            get { return _devicetype; }
            set {
                _devicetype = value;
                OnPropertyChanged(nameof(DeviceType));
            }
        }
        string _deviceid;
        public string DeviceID {
            get { return _deviceid; }
            set {
                _deviceid = value;
                OnPropertyChanged(nameof(DeviceID));
            }
        }
        string _elapsedtime;
        public string ElapsedTime {
            get { return _elapsedtime; }
            set {
                _elapsedtime = value;
                OnPropertyChanged(nameof(ElapsedTime));
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

    }

}
