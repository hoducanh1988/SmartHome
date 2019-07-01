using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeControlLibrary.ZIGBEE.PCBAFUNCTION {
    public class myGlobal {

        public static string SettingFileFullName = string.Format("{0}setting\\zigbee_pcba.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string HelpFileFullName = string.Format("{0}guide\\zigbee_pcba.xps", System.AppDomain.CurrentDomain.BaseDirectory);

        public static TestingInformation myTesting = new TestingInformation();
        public static SettingInformation mySetting = new SettingInformation();

    }
}
