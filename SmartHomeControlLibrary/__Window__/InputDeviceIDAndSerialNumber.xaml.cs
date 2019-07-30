using SmartHomeControlLibrary.__Common__;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace SmartHomeControlLibrary.__Window__ {
    /// <summary>
    /// Interaction logic for InputDeviceIDAndSerialNumber.xaml
    /// </summary>
    public partial class InputDeviceIDAndSerialNumber : Window {

        public string DeviceID = "";
        public string DeviceSN = "";

        public InputDeviceIDAndSerialNumber() {
            InitializeComponent();
            this.txtID.Focus();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            TextBox tb = sender as TextBox;
            if (e.Key == Key.Enter) {
                switch (tb.Tag.ToString()) {
                    case "id": {
                            if (ProjectUtility.IsSmartHomeDeviceID(tb.Text)) {
                                DeviceID = tb.Text;
                                this.txtSN.Focus();
                            }
                            else {
                                tb.Text = "";
                                tb.Focus();
                            }
                            break;
                        }
                    case "sn": {
                            if (UtilityPack.Validation.Parse.IsVnptProductSerialNumber(tb.Text, DeviceID, DeviceID.Length)) {
                                DeviceSN = tb.Text;
                                this.Close();
                            }
                            else {
                                tb.Text = "";
                                tb.Focus();
                            }
                            break;
                        }
                }
            }
        }
    }
}
