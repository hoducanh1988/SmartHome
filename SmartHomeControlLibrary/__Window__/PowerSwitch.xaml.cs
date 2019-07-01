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
    /// Interaction logic for PowerSwitch.xaml
    /// </summary>
    public partial class PowerSwitch : Window {

        public bool Lamp1Result = false;
        public bool Lamp2Result = false;
        public bool Lamp3Result = false;


        public PowerSwitch() {
            InitializeComponent();
        }

        private void Path_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Path p = sender as Path;
                switch (p.Tag) {
                    case "lamp1": {
                            if (lbllamptext1.Content.ToString().Equals("???") || lbllamptext1.Content.ToString().Equals("Failed")) {
                                lbllamptext1.Content = "Passed";
                                p.Fill = Brushes.Lime;
                                Lamp1Result = true;
                            }
                            else {
                                lbllamptext1.Content = "Failed";
                                p.Fill = Brushes.Red;
                                Lamp1Result = false;
                            }
                            break;
                        }
                    case "lamp2": {
                            if (lbllamptext2.Content.ToString().Equals("???") || lbllamptext2.Content.ToString().Equals("Failed")) {
                                lbllamptext2.Content = "Passed";
                                p.Fill = Brushes.Lime;
                                Lamp2Result = true;
                            }
                            else {
                                lbllamptext2.Content = "Failed";
                                p.Fill = Brushes.Red;
                                Lamp2Result = false;
                            }
                            break;
                        }
                    case "lamp3": {
                            if (lbllamptext3.Content.ToString().Equals("???") || lbllamptext3.Content.ToString().Equals("Failed")) {
                                lbllamptext3.Content = "Passed";
                                p.Fill = Brushes.Lime;
                                Lamp3Result = true;
                            }
                            else {
                                lbllamptext3.Content = "Failed";
                                p.Fill = Brushes.Red;
                                Lamp3Result = false;
                            }
                            break;
                        }
                }
            }

        }

        private void Label_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                Label l = sender as Label;
                switch (l.Tag) {
                    case "lamp1": {
                            if (lbllamptext1.Content.ToString().Equals("???") || lbllamptext1.Content.ToString().Equals("Failed")) {
                                lbllamptext1.Content = "Passed";
                                path1.Fill = Brushes.Lime;
                                Lamp1Result = true;
                            }
                            else {
                                lbllamptext1.Content = "Failed";
                                path1.Fill = Brushes.Red;
                                Lamp1Result = false;
                            }
                            break;
                        }
                    case "lamp2": {
                            if (lbllamptext2.Content.ToString().Equals("???") || lbllamptext2.Content.ToString().Equals("Failed")) {
                                lbllamptext2.Content = "Passed";
                                path2.Fill = Brushes.Lime;
                                Lamp2Result = true;
                            }
                            else {
                                lbllamptext2.Content = "Failed";
                                path2.Fill = Brushes.Red;
                                Lamp2Result = false;
                            }
                            break;
                        }
                    case "lamp3": {
                            if (lbllamptext3.Content.ToString().Equals("???") || lbllamptext3.Content.ToString().Equals("Failed")) {
                                lbllamptext3.Content = "Passed";
                                path3.Fill = Brushes.Lime;
                                Lamp3Result = true;
                            }
                            else {
                                lbllamptext3.Content = "Failed";
                                path3.Fill = Brushes.Red;
                                Lamp3Result = false;
                            }
                            break;
                        }
                }
            }
        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Button b = sender as Button;
            this.Close();
        }
    }
}
