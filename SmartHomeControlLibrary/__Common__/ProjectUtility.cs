using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace SmartHomeControlLibrary.__Common__ {

    public class ProjectUtility {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static List<string> GetSerialPortNames() {
            List<string> portnames = new List<string>();
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0) {
                foreach (var port in ports) {
                    portnames.Add(port);
                }
            }
            return portnames;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="referencedata"></param>
        /// <param name="portname"></param>
        /// <returns></returns>
        public static bool IsSmartHomeDeviceConnected(string referencedata, out string portname) {
            portname = "";
            var ports = ProjectUtility.GetSerialPortNames();
            if (ports.Count == 0) return false;

            foreach (var port in ports) {
                if (referencedata.ToLower().Contains(port.ToLower()) == false) {
                    portname = port;
                    break;
                }
            }
            return portname == "" ? false : true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        public static bool IsSmartHomeDeviceID(string deviceid) {
            //check null or empty
            if (string.IsNullOrEmpty(deviceid)) return false;

            //check length
            if (deviceid.Length != 16) return false;

            //check format
            string pattern = "^[0-9,A-F]{16}$";
            return Regex.IsMatch(deviceid.ToUpper(), pattern);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Cd"></param>
        /// <param name="Cch"></param>
        /// <returns></returns>
        public static double TinhSaiSoTuongDoi(double Cd, double Cch) {
            double sigma = ((Cd - Cch) / (Cch * 1.0)) * 100;
            return Math.Abs(sigma);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static double TinhDoLechChuan(params double[] values) {
            try {
                int n = values.Length;
                double avr = values.Average();

                double[] bs = new double[n];
                for (int i = 0; i < n; i++) {
                    bs[i] = Math.Pow(values[i] - avr, 2);
                }

                double sum = bs.Sum();
                return Math.Sqrt(sum / (n - 1));
            }
            catch {
                return double.MaxValue;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="item"></param>
        /// <param name="avr"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool WriteLogExcel_KiemTraDoLap<T, S>(DeviceType device, string path, string filename, T item, S setting, double avr, double s) where T : class, new() where S : class, new() {
            try {
                Excel.Application oXL;
                Excel.Workbook oWB;
                Excel.Worksheet oSheet;

                object misvalue = System.Reflection.Missing.Value;
                string f = System.IO.Path.Combine(path, string.Format("{0}.xls", filename));
                string sensor = "";
                switch (device) {
                    case DeviceType.SMH_CO: { sensor = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor = "GAS"; break; }
                    default: break;
                }

                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = false;

                //Get a new workbook.
                oWB = (Excel.Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Excel.Worksheet)oWB.ActiveSheet;

                //
                PropertyInfo product_id = item.GetType().GetProperty("ProductID");
                PropertyInfo lan_110 = item.GetType().GetProperty("Lan110");
                PropertyInfo ket_qua = item.GetType().GetProperty("KetQua");

                //
                PropertyInfo op = setting.GetType().GetProperty("Operator");
                PropertyInfo dltc = setting.GetType().GetProperty("DoLapTieuChuan");
                PropertyInfo dllc = setting.GetType().GetProperty("DoLapLechChuan");


                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "Tên thiết bị:";
                oSheet.Cells[1, 2] = string.Format("Thiết bị báo khí {0} thông minh", sensor);
                oSheet.Cells[2, 1] = "ID:";
                oSheet.Cells[2, 2].NumberFormat = "@";
                oSheet.Cells[2, 2] = product_id.GetValue(item, null).ToString();
                oSheet.Cells[3, 1] = "Người thực hiện:";
                oSheet.Cells[3, 2] = op.GetValue(setting, null).ToString();
                oSheet.Cells[4, 1] = "Ngày thực hiện:";
                oSheet.Cells[4, 2] = DateTime.Now.ToString("dd/MM/yyyy");

                oSheet.Cells[6, 1] = "Khí chuẩn";
                oSheet.Cells[6, 2] = sensor;

                oSheet.Cells[7, 1] = "Nồng độ";
                oSheet.Cells[7, 2] = dltc.GetValue(setting, null).ToString();

                oSheet.Cells[8, 1] = "Lần đo 1";
                oSheet.Cells[9, 1] = "Lần đo 2";
                oSheet.Cells[10, 1] = "Lần đo 3";
                oSheet.Cells[11, 1] = "Lần đo 4";
                oSheet.Cells[12, 1] = "Lần đo 5";
                oSheet.Cells[13, 1] = "Lần đo 6";
                oSheet.Cells[14, 1] = "Lần đo 7";
                oSheet.Cells[15, 1] = "Lần đo 8";
                oSheet.Cells[16, 1] = "Lần đo 9";
                oSheet.Cells[17, 1] = "Lần đo 10";

                string[] buffer = lan_110.GetValue(item, null).ToString().Split(',');
                for (int i = 0; i < 10; i++) {
                    oSheet.Cells[8 + i, 2] = buffer[i];
                }


                oSheet.Cells[18, 1] = "Giá trị trung bình";
                oSheet.Cells[18, 2] = avr.ToString();

                oSheet.Cells[19, 1] = "Độ lệch chuẩn";
                oSheet.Cells[19, 2] = s.ToString();

                oSheet.Cells[20, 1] = "Sai số cho phép";
                oSheet.Cells[20, 2] = (double.Parse(dllc.GetValue(setting, null).ToString()) / 3).ToString();

                oSheet.Cells[21, 1] = "Kết luận";
                oSheet.Cells[21, 2] = ket_qua.GetValue(item, null).ToString();

                oSheet.Cells.EntireColumn.AutoFit();
                //oXL.Visible = false;
                oXL.UserControl = false;

                oWB.SaveAs(f,
                           Excel.XlFileFormat.xlWorkbookDefault,
                           Type.Missing,
                           Type.Missing,
                           false,
                           false,
                           Excel.XlSaveAsAccessMode.xlNoChange,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing);

                oWB.Close();

                return true;
            }
            catch {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="item"></param>
        /// <param name="avr"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool WriteLogExcel_KiemTraDiemKhong<T, S>(DeviceType device, string path, string filename, T item, S setting, string saiso) where T : class, new() where S : class, new() {
            try {
                Excel.Application oXL;
                Excel.Workbook oWB;
                Excel.Worksheet oSheet;

                object misvalue = System.Reflection.Missing.Value;
                string f = System.IO.Path.Combine(path, string.Format("{0}.xls", filename));
                string sensor = "";
                switch (device) {
                    case DeviceType.SMH_CO: { sensor = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor = "GAS"; break; }
                    default: break;
                }

                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = false;

                //Get a new workbook.
                oWB = (Excel.Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Excel.Worksheet)oWB.ActiveSheet;

                //
                PropertyInfo product_id = item.GetType().GetProperty("ProductID");
                PropertyInfo lan_1 = item.GetType().GetProperty("Lan1");
                PropertyInfo lan_2 = item.GetType().GetProperty("Lan2");
                PropertyInfo lan_3 = item.GetType().GetProperty("Lan3");
                PropertyInfo ket_qua = item.GetType().GetProperty("KetQua");

                //
                PropertyInfo op = setting.GetType().GetProperty("Operator");
                PropertyInfo dkss = setting.GetType().GetProperty("DiemKhongSaiSo");


                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "Tên thiết bị:";
                oSheet.Cells[1, 2] = string.Format("Thiết bị báo khí {0} thông minh", sensor);
                oSheet.Cells[2, 1] = "ID:";
                oSheet.Cells[2, 2].NumberFormat = "@";
                oSheet.Cells[2, 2] = product_id.GetValue(item, null).ToString();
                oSheet.Cells[3, 1] = "Người thực hiện:";
                oSheet.Cells[3, 2] = op.GetValue(setting, null).ToString();
                oSheet.Cells[4, 1] = "Ngày thực hiện:";
                oSheet.Cells[4, 2] = DateTime.Now.ToString("dd/MM/yyyy");

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                oSheet.Cells[6, 1] = "Khí \"không\"";

                oSheet.Cells[7, 1] = "TT";
                oSheet.Cells[9, 1] = "1";

                oSheet.Cells[7, 2] = "Thông số";
                oSheet.Cells[9, 2] = sensor;

                oSheet.Cells[7, 3] = "Lần đo";
                oSheet.Cells[8, 3] = "1";
                oSheet.Cells[9, 3] = lan_1.GetValue(item, null).ToString();

                oSheet.Cells[8, 4] = "2";
                oSheet.Cells[9, 4] = lan_2.GetValue(item, null).ToString();

                oSheet.Cells[8, 5] = "3";
                oSheet.Cells[9, 5] = lan_3.GetValue(item, null).ToString();

                oSheet.Cells[7, 6] = "Sai số";
                oSheet.Cells[9, 6] = saiso;

                oSheet.Cells[7, 7] = "Sai số cho phép";
                oSheet.Cells[9, 7] = (double.Parse(dkss.GetValue(setting, null).ToString()) / 2).ToString();

                oSheet.Cells[7, 8] = "Kết luận";
                oSheet.Cells[9, 8] = ket_qua.GetValue(item, null).ToString();


                oSheet.Range["A6:H6"].Merge();
                oSheet.Range["A7:A8"].Merge();
                oSheet.Range["B7:B8"].Merge();
                oSheet.Range["C7:E7"].Merge();
                oSheet.Range["F7:F8"].Merge();
                oSheet.Range["G7:G8"].Merge();
                oSheet.Range["H7:H8"].Merge();
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

                oSheet.Cells.EntireColumn.AutoFit();
                //oXL.Visible = false;
                oXL.UserControl = false;

                oWB.SaveAs(f,
                           Excel.XlFileFormat.xlWorkbookDefault,
                           Type.Missing,
                           Type.Missing,
                           false,
                           false,
                           Excel.XlSaveAsAccessMode.xlNoChange,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing);

                oWB.Close();

                return true;
            }
            catch {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="item"></param>
        /// <param name="ss1"></param>
        /// <param name="ss2"></param>
        /// <param name="ss3"></param>
        /// <param name="kq1"></param>
        /// <param name="kq2"></param>
        /// <param name="kq3"></param>
        /// <returns></returns>
        public static bool WriteLogExcel_KiemTraSaiSoTuongDoi<T, S>(DeviceType device, string path, string filename, T item, S setting, string ss1, string ss2, string ss3, string kq1, string kq2, string kq3) where T : class, new() where S : class, new() {
            try {
                Excel.Application oXL;
                Excel.Workbook oWB;
                Excel.Worksheet oSheet;

                object misvalue = System.Reflection.Missing.Value;
                string f = System.IO.Path.Combine(path, string.Format("{0}.xls", filename));
                string sensor = "";
                switch (device) {
                    case DeviceType.SMH_CO: { sensor = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor = "GAS"; break; }
                    default: break;
                }

                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = false;

                //Get a new workbook.
                oWB = (Excel.Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Excel.Worksheet)oWB.ActiveSheet;

                //
                PropertyInfo product_id = item.GetType().GetProperty("ProductID");
                PropertyInfo lan_1 = item.GetType().GetProperty("Lan1");
                PropertyInfo lan_2 = item.GetType().GetProperty("Lan2");
                PropertyInfo lan_3 = item.GetType().GetProperty("Lan3");
                PropertyInfo ket_qua = item.GetType().GetProperty("KetQua");

                //
                PropertyInfo op = setting.GetType().GetProperty("Operator");
                PropertyInfo tdtc = setting.GetType().GetProperty("TuongDoiTieuChuan");
                PropertyInfo tdss = setting.GetType().GetProperty("TuongDoiSaiSo");

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "Tên thiết bị:";
                oSheet.Cells[1, 2] = string.Format("Thiết bị báo khí {0} thông minh", sensor);
                oSheet.Cells[2, 1] = "ID:";
                oSheet.Cells[2, 2].NumberFormat = "@";
                oSheet.Cells[2, 2] = product_id.GetValue(item, null).ToString();
                oSheet.Cells[3, 1] = "Người thực hiện:";
                oSheet.Cells[3, 2] = op.GetValue(setting, null).ToString();
                oSheet.Cells[4, 1] = "Ngày thực hiện:";
                oSheet.Cells[4, 2] = DateTime.Now.ToString("dd/MM/yyyy");

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                oSheet.Cells[6, 1] = "TT";
                oSheet.Cells[7, 1] = "1";

                oSheet.Cells[6, 2] = "Khí chuẩn";
                oSheet.Cells[7, 2] = sensor;

                oSheet.Cells[6, 3] = "Nồng độ (ppm)";
                oSheet.Cells[7, 3] = tdtc.GetValue(setting, null).ToString();

                oSheet.Cells[6, 4] = "Giá trị đọc của PTĐ (ppm)";
                oSheet.Cells[7, 4] = "Lần 1";
                oSheet.Cells[7, 5] = lan_1.GetValue(item, null).ToString();

                oSheet.Cells[8, 4] = "Lần 2";
                oSheet.Cells[8, 5] = lan_2.GetValue(item, null).ToString();

                oSheet.Cells[9, 4] = "Lần 3";
                oSheet.Cells[9, 5] = lan_3.GetValue(item, null).ToString();

                oSheet.Cells[6, 6] = "Sai số (%)";
                oSheet.Cells[7, 6] = ss1;
                oSheet.Cells[8, 6] = ss2;
                oSheet.Cells[9, 6] = ss3;

                oSheet.Cells[6, 7] = "Sai số cho phép (%)";
                oSheet.Cells[7, 7] = tdss.GetValue(setting, null).ToString();
                oSheet.Cells[8, 7] = tdss.GetValue(setting, null).ToString();
                oSheet.Cells[9, 7] = tdss.GetValue(setting, null).ToString();

                oSheet.Cells[6, 8] = "Kết luận";
                oSheet.Cells[7, 8] = kq1;
                oSheet.Cells[8, 8] = kq2;
                oSheet.Cells[9, 8] = kq3;


                oSheet.Range["D6:E6"].Merge();
                oSheet.Range["A7:A9"].Merge();
                oSheet.Range["B7:B9"].Merge();
                oSheet.Range["C7:C9"].Merge();
                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

                oSheet.Cells.EntireColumn.AutoFit();
                //oXL.Visible = false;
                oXL.UserControl = false;

                oWB.SaveAs(f,
                           Excel.XlFileFormat.xlWorkbookDefault,
                           Type.Missing,
                           Type.Missing,
                           false,
                           false,
                           Excel.XlSaveAsAccessMode.xlNoChange,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing);

                oWB.Close();

                return true;
            }
            catch {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <param name="item"></param>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <param name="t3"></param>
        /// <param name="ss1"></param>
        /// <param name="ss2"></param>
        /// <returns></returns>
        public static bool WriteLogExcel_KiemTraDoTroi<T, S>(DeviceType device, string path, string filename, T item, S setting, string t1, string t2, string t3, string ss1, string ss2) where T : class, new() where S : class, new() {
            try {
                Excel.Application oXL;
                Excel.Workbook oWB;
                Excel.Worksheet oSheet;

                object misvalue = System.Reflection.Missing.Value;
                string f = System.IO.Path.Combine(path, string.Format("{0}.xls", filename));
                string sensor = "";
                switch (device) {
                    case DeviceType.SMH_CO: { sensor = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor = "GAS"; break; }
                    default: break;
                }

                //Start Excel and get Application object.
                oXL = new Excel.Application();
                oXL.Visible = false;

                //Get a new workbook.
                oWB = (Excel.Workbook)(oXL.Workbooks.Add(""));
                oSheet = (Excel.Worksheet)oWB.ActiveSheet;

                //
                PropertyInfo product_id = item.GetType().GetProperty("ProductID");
                PropertyInfo lan_1 = item.GetType().GetProperty("Lan1");
                PropertyInfo lan_2 = item.GetType().GetProperty("Lan2");
                PropertyInfo lan_3 = item.GetType().GetProperty("Lan3");
                PropertyInfo ket_qua = item.GetType().GetProperty("KetQua");

                //
                PropertyInfo op = setting.GetType().GetProperty("Operator");
                PropertyInfo dttc = setting.GetType().GetProperty("DoTroiTieuChuan");
                PropertyInfo dtss = setting.GetType().GetProperty("DoTroiSaiSo");

                //Add table headers going cell by cell.
                oSheet.Cells[1, 1] = "Tên thiết bị:";
                oSheet.Cells[1, 2] = string.Format("Thiết bị báo khí {0} thông minh", sensor);
                oSheet.Cells[2, 1] = "ID:";
                oSheet.Cells[2, 2].NumberFormat = "@";
                oSheet.Cells[2, 2] = product_id.GetValue(item, null).ToString();
                oSheet.Cells[3, 1] = "Người thực hiện:";
                oSheet.Cells[3, 2] = op.GetValue(setting, null).ToString();
                oSheet.Cells[4, 1] = "Ngày thực hiện:";
                oSheet.Cells[4, 2] = DateTime.Now.ToString("dd/MM/yyyy");

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
                oSheet.Cells[6, 1] = "Khí chuẩn";
                oSheet.Cells[8, 1] = sensor;

                oSheet.Cells[6, 2] = "Nồng độ (ppm)";
                oSheet.Cells[8, 2] = dttc.GetValue(setting, null).ToString();

                oSheet.Cells[6, 3] = "Lần đo và thời gian đo";
                oSheet.Cells[7, 3] = string.Format("1 \r\n({0})", t1);
                oSheet.Cells[8, 3] = lan_1.GetValue(item, null).ToString();

                oSheet.Cells[7, 4] = string.Format("2 \r\n({0})", t2);
                oSheet.Cells[8, 4] = lan_2.GetValue(item, null).ToString();

                oSheet.Cells[7, 5] = string.Format("3 \r\n({0})", t3);
                oSheet.Cells[8, 5] = lan_3.GetValue(item, null).ToString();

                oSheet.Cells[6, 6] = "Sai số với phép đo đầu tiên";
                oSheet.Cells[7, 6] = "(2)-(1)";
                oSheet.Cells[8, 6] = ss1;

                oSheet.Cells[7, 7] = "(3)-(1)";
                oSheet.Cells[8, 7] = ss2;

                oSheet.Cells[6, 8] = "Sai số cho phép";
                oSheet.Cells[8, 8] = dtss.GetValue(setting, null).ToString();

                oSheet.Cells[6, 9] = "Kết luận";
                oSheet.Cells[8, 9] = ket_qua.GetValue(item, null).ToString();


                oSheet.Range["A6:A7"].Merge();
                oSheet.Range["B6:B7"].Merge();
                oSheet.Range["C6:E6"].Merge();
                oSheet.Range["F6:G6"].Merge();
                oSheet.Range["H6:H7"].Merge();
                oSheet.Range["I6:I7"].Merge();

                //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//

                oSheet.Cells.EntireColumn.AutoFit();
                //oXL.Visible = false;
                oXL.UserControl = false;

                oWB.SaveAs(f,
                           Excel.XlFileFormat.xlWorkbookDefault,
                           Type.Missing,
                           Type.Missing,
                           false,
                           false,
                           Excel.XlSaveAsAccessMode.xlNoChange,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing,
                           Type.Missing);

                oWB.Close();

                return true;
            }
            catch {
                return false;
            }
        }

    }
}
