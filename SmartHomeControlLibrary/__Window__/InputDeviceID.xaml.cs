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
using SmartHomeControlLibrary.__Common__;

namespace SmartHomeControlLibrary.__Window__ {
    /// <summary>
    /// Interaction logic for InputDeviceID.xaml
    /// </summary>
    public partial class InputDeviceID : Window {

        public string DeviceID = "";

        public InputDeviceID() {
            InitializeComponent();

            this.textbox.Focus();
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) {
            TextBox tb = sender as TextBox;
            if (e.Key == Key.Enter) {
                if (ProjectUtility.IsSmartHomeDeviceID(tb.Text)) {
                    DeviceID = tb.Text;
                    this.Close();
                }
                else {
                    tb.Text = "";
                    tb.Focus();
                }
                
            }
        }
    }
}
