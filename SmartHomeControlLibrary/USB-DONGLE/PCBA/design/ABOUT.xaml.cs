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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartHomeControlLibrary.USBDongle.PCBAFUNCTION {
    /// <summary>
    /// Interaction logic for ABOUT.xaml
    /// </summary>
    public partial class ABOUT : UserControl {

        private class history {
            public string ID { get; set; }
            public string VERSION { get; set; }
            public string CONTENT { get; set; }
            public string DATE { get; set; }
            public string CHANGETYPE { get; set; }
            public string PERSON { get; set; }
        }

        List<history> listHist = new List<history>();

        public ABOUT() {
            InitializeComponent();

            listHist.Add(new history() {
                ID = "1",
                VERSION = "Prototype",
                CONTENT = "- Phát hành lần đầu",
                DATE = "2019/05/16",
                CHANGETYPE = "Tạo mới",
                PERSON = "Hồ Đức Anh"
            });

            this.GridAbout.ItemsSource = listHist;
        }
    }
}
