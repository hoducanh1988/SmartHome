using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeControlLibrary.ALLDETECTOR.USERFUNCTION {

    public class myGlobal {
        public static string SettingFileFullName = string.Format("{0}setting\\alldetector_final.xml", AppDomain.CurrentDomain.BaseDirectory);
        public static string HelpFileFullName = string.Format("{0}guide\\alldetector_final.xps", System.AppDomain.CurrentDomain.BaseDirectory);

        public static SettingInformation mySetting = new SettingInformation();
    }

}
