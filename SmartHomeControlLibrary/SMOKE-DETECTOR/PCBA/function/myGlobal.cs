using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeControlLibrary.SMOKEDETECTOR.PCBAFUNCTION {
    public class myGlobal {
        public static string SettingFileFullName = string.Format("{0}setting\\smokedetector_pcba.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string HelpFileFullName = string.Format("{0}guide\\smokedetector_pcba.xps", System.AppDomain.CurrentDomain.BaseDirectory);

        public static TestingInformation myTesting = new TestingInformation();
        public static SettingInformation mySetting = new SettingInformation();
    }
}
