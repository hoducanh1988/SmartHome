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
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : Window {


        public LogWindow(string _id, string _idx, string data) {
            InitializeComponent();

            this.Title = string.Format("Device Index: {0} - Device ID: {1}", _idx, _id);
            this.rtb_content.AppendText(data);
        }
    }
}
