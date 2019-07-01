using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace SmartHomeControlLibrary.CarbonMonoxideDetector.FINALFUNCTION {
    /// <summary>
    /// Interaction logic for LOG.xaml
    /// </summary>
    public partial class LOG : UserControl
    {
        SettingInformation mySetting = null;

        public LOG()
        {
            InitializeComponent();

            if (File.Exists(myGlobal.SettingFileFullName)) mySetting = XmlHelper<SettingInformation>.FromXmlFile(myGlobal.SettingFileFullName);
            this.cbb_logtype.ItemsSource = new List<string>() { "LogTotal", "LogSingle", "LogDetail" };
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            switch (b.Content) {
                case "Go": {
                        if (cbb_logtype.SelectedValue != null && mySetting != null) {
                            string text = cbb_logtype.SelectedValue.ToString();
                            string logpath = "";
                            string dir = string.Format(@"{0}\{1}\{2}",
                                string.Format("{0}log", AppDomain.CurrentDomain.BaseDirectory),
                                "codetector",
                                "asm");

                            switch (text) {
                                case "LogTotal": {
                                        logpath = System.IO.Path.Combine(dir, "logtotal");
                                        break;
                                    }
                                case "LogSingle": {
                                        logpath = System.IO.Path.Combine(dir, "logsingle", DateTime.Now.ToString("yyyyMMdd"));
                                        break;
                                    }
                                case "LogDetail": {
                                        logpath = System.IO.Path.Combine(dir, "logdetail");
                                        break;
                                    }
                                case "Default": {
                                        logpath = AppDomain.CurrentDomain.BaseDirectory;
                                        break;
                                    }
                                default: {
                                        logpath = AppDomain.CurrentDomain.BaseDirectory;
                                        break;
                                    }
                            }

                            if (System.IO.Directory.Exists(logpath)) {
                                Process.Start(logpath);
                            }
                            else {
                                MessageBox.Show(string.Format("path {0} is not exist.", logpath), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else {
                            MessageBox.Show("Please select a log type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                        break;
                    }
                default: break;
            }
        }
    }
}
