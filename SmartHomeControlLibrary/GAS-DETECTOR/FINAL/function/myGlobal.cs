using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeControlLibrary.GASDETECTOR.FINALFUNCTION {
    public class myGlobal {
        public static string SettingFileFullName = string.Format("{0}setting\\gasdetector_final.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string HelpFileFullName = string.Format("{0}guide\\gasdetector_final.xps", System.AppDomain.CurrentDomain.BaseDirectory);

        public static TestingInformation myTesting = new TestingInformation();
        public static SettingInformation mySetting = new SettingInformation();
    }
}
