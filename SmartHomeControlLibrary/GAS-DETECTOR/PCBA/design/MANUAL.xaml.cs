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

namespace SmartHomeControlLibrary.GASDETECTOR.PCBAFUNCTION {

    /// <summary>
    /// Interaction logic for MANUAL.xaml
    /// </summary>
    public partial class MANUAL : UserControl {

        Dictionary<string, string> dictCommands = new Dictionary<string, string>() {
            { "Ping!", "Lệnh đọc ID của sản phẩm smarthome"},
            { "SMH_ZIGBEE,RF!", "Lệnh kiểm tra kết nối đến node RF" },
            { "Readsensor!", "Lệnh đọc giá trị cảm biến nhiệt độ, độ ẩm, ppm" },
            { "Led_Green_On!", "Lệnh điều khiển led xanh sáng"},
            { "Led_Red_On!", "Lệnh điều khiển led đỏ sáng" },
            { "Led_Off_All!", "Lệnh điều khiển tắt tất cả các led" },
            { "Test_Buzzer_Off!", "Lệnh điều khiển tắt còi" },
            { "Test_Buzzer_On!", "Lệnh điều khiển bật còi" }
        };


        public MANUAL() {
            InitializeComponent();

            foreach (var item in dictCommands) this.cbbCommands.Items.Add(item);

        }









    }
}
