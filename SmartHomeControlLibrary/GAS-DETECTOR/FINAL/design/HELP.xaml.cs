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
using System.Windows.Xps.Packaging;

namespace SmartHomeControlLibrary.GASDETECTOR.FINALFUNCTION {
    /// <summary>
    /// Interaction logic for HELP.xaml
    /// </summary>
    public partial class HELP : UserControl
    {
        public HELP()
        {
            InitializeComponent();

            if (System.IO.File.Exists(myGlobal.HelpFileFullName)) {
                XpsDocument xpsDocument = new XpsDocument(myGlobal.HelpFileFullName, System.IO.FileAccess.Read);
                FixedDocumentSequence fds = xpsDocument.GetFixedDocumentSequence();
                _docViewer.Document = fds;
            }
        }
    }
}
