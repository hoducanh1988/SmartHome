using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeControlLibrary.GASDETECTOR.RD {

    public class myGlobal {

        public static string SettingFileFullName = string.Format("{0}setting\\gasdetector_rd.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string HelpFileFullName = string.Format("{0}guide\\gasdetector_rd.xps", System.AppDomain.CurrentDomain.BaseDirectory);

        public static TestingInformation myTesting = new TestingInformation();
        public static SettingInformation mySetting = new SettingInformation();

        public static ObservableCollection<ItemProductID> ObservableCollectionProductID = new ObservableCollection<ItemProductID>();
        public static ObservableCollection<ItemKiemTraDiemKhong> ObservableCollectionDiemKhong = new ObservableCollection<ItemKiemTraDiemKhong>();
        public static ObservableCollection<ItemKiemTraTuongDoi> ObservableCollectionTuongDoi = new ObservableCollection<ItemKiemTraTuongDoi>();
        public static ObservableCollection<ItemKiemTraDoLap> ObservableCollectionDoLap = new ObservableCollection<ItemKiemTraDoLap>();
        public static ObservableCollection<ItemKiemTraDoTroi> ObservableCollectionDoTroi = new ObservableCollection<ItemKiemTraDoTroi>();

    }
}
