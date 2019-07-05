using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO.Ports;
using System.Diagnostics;

namespace SmartHomeControlLibrary.__Common__ {

    public enum SensorType { Temperature = 0, Humidity = 1, CO = 2, GAS = 2, Smoke = 2 };
    public enum DeviceType { SMH_GAS = 0, SMH_CO = 1, SMH_SMOKE = 2, SMH_SW3 = 3, SMH_ZIGBEE = 4 };

    public class ProjectTestItem {

        public static SmartHomeDevice DUT = null;


        /// <summary>
        /// FUNCTION: KIEM TRA SAN PHAM DA KET NOI COM TOI MAY TINH TRAM TEST HAY CHUA ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = Connect,  FALSE = Disconnect
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="settinginfo"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Is_DUT_Connected_To_Client_PC<T, S>(T testinfo, S settinginfo, int retrytime) where T : class, new() where S : class, new() {

            //Get ErrorMessage from Testing Object
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");

            //Get LogSystem from Testing Object
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            //Get FixComport from Setting Object
            PropertyInfo fx = settinginfo.GetType().GetProperty("FixComport");
            string FixComport = fx.GetValue(settinginfo, null).ToString();

            //Check New Comport connected to client PC or not
            LogSystem += string.Format("\r\n+++ KIỂM TRA KẾT NỐI COM GIỮA SẢN PHẨM VỚI MÁY TÍNH  +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //retry = retrytime
            int count = 0;
        REP:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            string tmp = "";
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length > 0) {
                string data = "";
                foreach (string port in ports) {
                    data += port + ",";
                }
                tmp = data.Substring(0, data.Length - 1);
            }

            string portname = "";
            LogSystem += string.Format(".........Tiêu chuẩn: \"{0}\"\r\n", FixComport);
            LogSystem += string.Format(".........Thực tế: \"{0}\"\r\n", tmp);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            bool r = ProjectUtility.IsSmartHomeDeviceConnected(FixComport, out portname);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Sản phẩm chưa kết nối tới máy tính trạm test.\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    Thread.Sleep(1000);
                    LogSystem += string.Format(".........Chờ 1 giây\r\n");
                    lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                    goto REP;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("Sản phẩm chưa kết nối tới máy tính trạm test.", em.PropertyType), null);
                    goto END;
                }
            }
            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            LogSystem += string.Format(".........Thông tin: Sản phẩm đã kết nối tại cổng {0}\r\n", portname);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);


            //Open comport
            LogSystem += string.Format("\r\n+++ MỞ CỔNG KẾT NỐI COM TỚI SẢN PHẨM +++\r\n");

            string br = settinginfo.GetType().GetProperty("DUTBaudRate").GetValue(settinginfo, null).ToString();
            string db = settinginfo.GetType().GetProperty("DUTDataBits").GetValue(settinginfo, null).ToString();
            string pr = settinginfo.GetType().GetProperty("DUTParity").GetValue(settinginfo, null).ToString();
            string sb = settinginfo.GetType().GetProperty("DUTStopBits").GetValue(settinginfo, null).ToString();

            LogSystem += string.Format(".........Thông tin COM sản phẩm: {0}, {1} , {2}, {3}, {4}\r\n", portname, br, db, pr, sb);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            DUT = new SmartHomeDevice(portname, br, db, pr, sb);

            r = DUT.Open();
            LogSystem += string.Format(".........Kết quả: {0}\r\n", r == true ? "Passed : Kết nối thành công" : "Failed : Không kết nối");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
            if (!r) {
                em.SetValue(testinfo, Convert.ChangeType("Không kết nối COM được tới sản phẩm cần test.", em.PropertyType), null);
            }

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: PHAN MEM KET NOI COM TOI THIET BI USB DONGLE ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = Openned,  FALSE = Closed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="settinginfo"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Open_Device_USB_Dongle<T, S>(T testinfo, S settinginfo, int retrytime) {
            //Get LogSystem from Testing Object
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();
            LogSystem += string.Format("+++ MỞ CỔNG GIAO TIẾP COM GIỮA PHẦN MỀM VỚI USB DONGLE TRẠM TEST +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //Open comport
            string portname = settinginfo.GetType().GetProperty("SerialPortName").GetValue(settinginfo, null).ToString();
            string br = settinginfo.GetType().GetProperty("SerialBaudRate").GetValue(settinginfo, null).ToString();
            string db = settinginfo.GetType().GetProperty("SerialDataBits").GetValue(settinginfo, null).ToString();
            string pr = settinginfo.GetType().GetProperty("SerialParity").GetValue(settinginfo, null).ToString();
            string sb = settinginfo.GetType().GetProperty("SerialStopBits").GetValue(settinginfo, null).ToString();

            int count = 0;
            bool r = false;
        REP:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Thông tin COM USB DONGLE: {0}, {1} , {2}, {3}, {4}\r\n", portname, br, db, pr, sb);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            DUT = new SmartHomeDevice(portname, br, db, pr, sb);
            r = DUT.Open();
            LogSystem += string.Format(".........Kết quả: {0}\r\n", r == true ? "Passed" : "Failed");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
            if (!r) {
                if (count < retrytime) {
                    goto REP;
                }
                else goto END;
            }

        END:
            //return value
            return r;
        }



        /// <summary>
        /// FUNCTION: PHAN MEM KET NOI COM TOI THIET BI USB DONGLE KHONG RETURN SYSTEMLOG ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="settinginfo"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Open_Device_USB_Dongle<S>(S settinginfo, int retrytime) {
            //Open comport
            string portname = settinginfo.GetType().GetProperty("SerialPortName").GetValue(settinginfo, null).ToString();
            string br = settinginfo.GetType().GetProperty("SerialBaudRate").GetValue(settinginfo, null).ToString();
            string db = settinginfo.GetType().GetProperty("SerialDataBits").GetValue(settinginfo, null).ToString();
            string pr = settinginfo.GetType().GetProperty("SerialParity").GetValue(settinginfo, null).ToString();
            string sb = settinginfo.GetType().GetProperty("SerialStopBits").GetValue(settinginfo, null).ToString();

            int count = 0;
            bool r = false;
        REP:
            count++;

            DUT = new SmartHomeDevice(portname, br, db, pr, sb);
            r = DUT.Open();
            if (!r) {
                if (count < retrytime) {
                    goto REP;
                }
                else goto END;
            }

        END:
            //return value
            return r;
        }



        /// <summary>
        /// FUNCTION: GUI VA NHAN BAN TIN GIUA MAY TINH VOI SAN PHAM SMARTHOME => DOC VE ID CUA SAN PHAM ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = send and receive ok, FAIL = send or receive not ok
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="dut_ID"></param>
        /// <returns></returns>
        public static bool Is_DUT_Transmitted_To_Client_PC<T>(T testinfo, int retrytime, string dut_command, out string dut_ID) where T : class, new() {
            dut_ID = "";
            bool r = false;
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ KIỂM TRA TRUYỀN NHẬN BẢN TIN GIỮA SẢN PHẨM VỚI PHẦN MỀM TEST +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Sản phẩm chưa kết nối với máy tính\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("Sản phẩm chưa kết nối với máy tính trạm test.", em.PropertyType), null);
                goto END;
            }

            //send and get data from DUT
            int count = 0;
            string feedback_data = "";
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", dut_command);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(dut_command, 1000);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);

            r = !(string.IsNullOrEmpty(feedback_data) || (!feedback_data.ToLower().Contains("ok!")));
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Dữ liệu phản hồi từ sản phầm là sai định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType(string.Format("Dữ liệu phản hồi từ sản phầm là sai định dạng \"{0}\"", feedback_data), em.PropertyType), null);
                    goto END;
                }
            }

            //get DUT ID
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            string id = "";
            try {
                id = buffer[1];
            }
            catch { id = ""; }
            LogSystem += string.Format(".........Thông tin: ID sản phẩm là {0}\r\n", id);
            r = ProjectUtility.IsSmartHomeDeviceID(id);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: ID sản phẩm không đúng định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType(string.Format("ID sản phẩm không đúng định dạng \"{0}\".", id), em.PropertyType), null);
                    goto END;
                }
            }
            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
            dut_ID = id;

        END:
            //return value
            return r;

        }


        /// <summary>
        /// FUNCTION: GUI VA NHAN BAN TIN GIUA USB DONGLE (SAN PHAM TEST) VOI NODE RF (ZIG TEST) ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = send and receive ok, FAIL = send or receive not ok
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool Is_DUT_Transmitted_To_Node_RF<T>(T testinfo, int retrytime, string command) where T : class, new() {
            bool r = false;
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ KIỂM TRA TRUYỀN NHẬN BẢN TIN GIỮA SẢN PHẨM VỚI THIẾT BỊ NODE RF +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test", em.PropertyType), null);
                goto END;
            }

            //send and get data with node RF
            int count = 0;
            string feedback_data = "";
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", command);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(command, 1000);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);

            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("ok!");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới Node RF\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới Node RF.", em.PropertyType), null);
                    goto END;
                }
            }
            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: GUI VA NHAN BAN TIN GIUA USB DONGLE (SAN PHAM TEST) VOI NODE RF (ZIG TEST) ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = send and receive ok, FAIL = send or receive not ok
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool Is_DUT_Transmitted_To_Node_RF_D<T>(T testinfo, int retrytime, int delay, string command) where T : class, new() {
            bool r = false;
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ KIỂM TRA TRUYỀN NHẬN BẢN TIN GIỮA SẢN PHẨM VỚI THIẾT BỊ NODE RF +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test", em.PropertyType), null);
                goto END;
            }

            //send and get data with node RF
            int count = 0;
            string feedback_data = "";
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", command);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(command, delay);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);

            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("ok!");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới Node RF\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới Node RF.", em.PropertyType), null);
                    goto END;
                }
            }
            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }



        /// <summary>
        /// FUNCTION: GUI VA NHAN BAN TIN GIUA USB DONGLE (SAN PHAM TEST) VOI NODE RF (ZIG TEST) ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = send and receive ok, FAIL = send or receive not ok
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public static bool Is_DUT_Transmitted_To_Node_RF<T>(T testinfo, int retrytime, string command, string nodeID, string nodeType) where T : class, new() {
            bool r = false;
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ KIỂM TRA TRUYỀN NHẬN BẢN TIN GIỮA SẢN PHẨM VỚI THIẾT BỊ NODE RF +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test", em.PropertyType), null);
                goto END;
            }

            //send and get data with node RF
            int count = 0;
            string feedback_data = "";
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", command);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(command, 1000);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);
            string refData = string.Format("RESP,{0},{1},RF_OK!", nodeID.ToUpper(), nodeType.ToUpper());
            LogSystem += string.Format(".........Tiêu chuẩn: {0}\r\n", refData);

            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToUpper().Contains(refData);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới Node RF\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới Node RF.", em.PropertyType), null);
                    goto END;
                }
            }
            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: KIEM TRA MODULE ZIGBEE DA JOIN VAO MANG CUA USB DONGLE, NEU CHUA THI REBOOT DUT ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = joined, FAIL = not joined
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="reboot_time"></param>
        /// <param name="dut_id"></param>
        /// <returns></returns>
        public static bool Is_Module_ZigBee_Join_To_Network<T>(T testinfo, int retrytime, int reboot_time, out string dut_id) where T : class, new() {
            bool r = false;
            dut_id = "";
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            int count = 0;
            string reboot_command = "CHECK,RESETNOW!";
        REP:
            count++;
            r = Is_Module_ZigBee_Join_To_Network<T>(testinfo, retrytime, out dut_id);
            if (!r) {
                if (count < reboot_time) {
                    DUT.WriteLine(reboot_command);
                    Thread.Sleep(5000);
                    goto REP;
                }
                else goto END;
            }
            goto END;

        END:
            //return value
            return r;
        }



        /// <summary>
        /// FUNCTION: KIEM TRA MODULE ZIGBEE DA JOIN VAO MANG CUA USB DONGLE, NEU CHUA THI REBOOT DUT ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = joined, FAIL = not joined
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="reboot_time"></param>
        /// <param name="dut_id"></param>
        /// <returns></returns>
        public static bool Is_Module_ZigBee_Join_To_Network_D<T>(T testinfo, int retrytime, int reboot_time, int delay, out string dut_id) where T : class, new() {
            bool r = false;
            dut_id = "";
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            int count = 0;
            string reboot_command = "CHECK,RESETNOW!";
        REP:
            count++;
            r = Is_Module_ZigBee_Join_To_Network_D<T>(testinfo, retrytime, delay, out dut_id);
            if (!r) {
                if (count < reboot_time) {
                    DUT.WriteLine(reboot_command);
                    Thread.Sleep(5000);
                    goto REP;
                }
                else goto END;
            }
            goto END;

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: KIEM TRA MODULE ZIGBEE DA JOIN VAO MANG CUA USB DONGLE ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = joined, FAIL = not joined
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="dut_id"></param>
        /// <returns></returns>
        public static bool Is_Module_ZigBee_Join_To_Network<T>(T testinfo, int retrytime, out string dut_id) where T : class, new() {
            bool r = false;
            dut_id = "";
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ KIỂM TRA KẾT NỐI MẠNG RF CỦA SẢN PHẨM +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test.", em.PropertyType), null);
                goto END;
            }
            //ping network
            int count = 0;
            string feedback_data = "";
            string ping_command = "CHECK,Ping!";
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", ping_command);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(ping_command, 1000);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);

            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,connected,");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối RF vào mạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    //em.SetValue(testinfo, Convert.ChangeType("Message: DUT is not joined to network.", em.PropertyType), null);
                    goto END;
                }
            }

            //get node id
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            try {
                dut_id = buffer[2].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim();
            }
            catch { dut_id = ""; }
            LogSystem += string.Format(".........Thông tin: ID sản phẩm là {0}\r\n", dut_id);

            r = ProjectUtility.IsSmartHomeDeviceID(dut_id);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: ID sản phẩm không đúng định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    goto END;
                }
            }
            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: KIEM TRA MODULE ZIGBEE DA JOIN VAO MANG CUA USB DONGLE ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = joined, FAIL = not joined
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="retrytime"></param>
        /// <param name="dut_id"></param>
        /// <returns></returns>
        public static bool Is_Module_ZigBee_Join_To_Network_D<T>(T testinfo, int retrytime, int delay, out string dut_id) where T : class, new() {
            bool r = false;
            dut_id = "";
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ KIỂM TRA KẾT NỐI MẠNG RF CỦA SẢN PHẨM +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test.", em.PropertyType), null);
                goto END;
            }
            //ping network
            int count = 0;
            string feedback_data = "";
            string ping_command = "CHECK,Ping!";
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", ping_command);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(ping_command, delay);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);

            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,connected,");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối RF vào mạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    //em.SetValue(testinfo, Convert.ChangeType("Message: DUT is not joined to network.", em.PropertyType), null);
                    goto END;
                }
            }

            //get node id
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            try {
                dut_id = buffer[2].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim();
            }
            catch { dut_id = ""; }
            LogSystem += string.Format(".........Thông tin: ID sản phẩm là {0}\r\n", dut_id);

            r = ProjectUtility.IsSmartHomeDeviceID(dut_id);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: ID sản phẩm không đúng định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    goto END;
                }
            }
            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: IN MA ID SAN PHAM RA TEM 40x10 ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = no error, FAIL = error
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <returns></returns>
        public static bool Print_DUT_ID_Labels<T, U, V>(T testinfo, string id, string ms_file_full_name, string id_table_name, U idinfo, string log_table_name, V loginfo, string report_id_name)
            where T : class, new()
            where U : class, new()
            where V : class, new() {

            bool r = false;
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ IN NHÃN ID CỦA THIẾT BỊ RA TEM 40x10 +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //connect ms access file
            LogSystem += string.Format(".........phần mềm kết nối tới file ms access {0}, ", ms_file_full_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
            ProjectMsAccess access = new ProjectMsAccess(ms_file_full_name);
            r = access.OpenConnection();
            if (!r) {
                LogSystem += string.Format("Failed\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType(string.Format("Phần mềm không kết nối được file ms access {0}.", ms_file_full_name), em.PropertyType), null);
                goto END;
            }
            LogSystem += string.Format("Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //delete id table
            LogSystem += string.Format(".........xóa tất cả dữ liệu trong bảng ID {0}, ", id_table_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
            r = access.Delete_All_DataRow_From_Access_DB_Table(id_table_name);
            if (!r) {
                LogSystem += string.Format("Failed\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType(string.Format("Phần mềm không thể xóa dữ liệu trong bảng ID {0}.", id_table_name), em.PropertyType), null);
                goto END;
            }
            LogSystem += string.Format("Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //write device id to id table
            LogSystem += string.Format(".........ghi ID thiết bị {0} vào bảng ID {1}, ", id, id_table_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
            r = access.Input_New_DataRow_To_Access_DB_Table<U>(id_table_name, idinfo);
            if (!r) {
                LogSystem += string.Format("Failed\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType(string.Format("Phần mềm không thể ghi ID {0} thiết bị vào bảng ID {1}.", id, id_table_name), em.PropertyType), null);
                goto END;
            }
            LogSystem += string.Format("Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //write log to log table
            LogSystem += string.Format(".........ghi dữ liệu log vào bảng log {0}, ", log_table_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
            r = access.Input_New_DataRow_To_Access_DB_Table<V>(log_table_name, loginfo, "tb_ID");
            if (!r) {
                LogSystem += string.Format("Failed\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType(string.Format("Phần mềm không thể ghi dữ liệu log vào bảng {0}.", log_table_name), em.PropertyType), null);
                goto END;
            }
            LogSystem += string.Format("Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //send report data to label printer
            LogSystem += string.Format(".........gửi lệnh in report {0} tới máy in nhãn, ", report_id_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //07.06.2019 ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~`//
            access.accessDB.Close();
            Thread.Sleep(100);
            
            string pf = string.Format("{0}PrintReportAccess.exe", AppDomain.CurrentDomain.BaseDirectory);
            string tf = string.Format("{0}T.tmp", AppDomain.CurrentDomain.BaseDirectory);
            string ff = string.Format("{0}F.tmp", AppDomain.CurrentDomain.BaseDirectory);

            //kill process

            //delete tmp file
            if (System.IO.File.Exists(tf)) System.IO.File.Delete(tf);
            if (System.IO.File.Exists(ff)) System.IO.File.Delete(ff);
            Thread.Sleep(100);

            Process.Start(pf);
            Thread.Sleep(1000);

            for (; ; ) {
                if (System.IO.File.Exists(tf) || System.IO.File.Exists(ff)) {
                    break;
                }
                Thread.Sleep(500);
            }

            r = System.IO.File.Exists(tf);
            //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~//
            
            //r = access.Print_Access_Report(report_id_name);
            if (!r) {
                LogSystem += string.Format("Failed\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType(string.Format("Không thể in report {0}.", report_id_name), em.PropertyType), null);
                goto END;
            }
            LogSystem += string.Format("Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: KIEM TRA TINH CHINH XAC CUA CAM BIEN ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = passed, FAIL = failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="dut_id"></param>
        /// <param name="dut_type"></param>
        /// <param name="sensor"></param>
        /// <param name="limit_value"></param>
        /// <param name="limit_accuracy"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Is_Sensor_Valid<T>(T testinfo, string dut_id, DeviceType dut_type, SensorType sensor, string limit_value, string limit_accuracy, int retrytime) where T : class, new() {
            bool r = false;
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            string sensor_name = "";
            int sensor_value = (int)sensor;
            if (sensor_value == 2) {
                switch (dut_type) {
                    case DeviceType.SMH_CO: { sensor_name = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor_name = "GAS"; break; }
                    case DeviceType.SMH_SMOKE: { sensor_name = "SMOKE"; break; }
                }
            }
            else sensor_name = sensor.ToString().ToUpper();

            LogSystem += string.Format("\r\n+++ KIỂM TRA ĐỘ CHÍNH XÁC CỦA CẢM BIẾN {0} +++\r\n", sensor_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test.", em.PropertyType), null);
                goto END;
            }
            //get sensor value
            string cmd = string.Format("CHECK,{0},{1},Readsensor!", dut_id, dut_type.ToString().ToUpper());
            string feedback_data = "";
            int count = 0;
            int idx = (int)sensor + 3;
            double up_value = double.Parse(limit_value) + double.Parse(limit_accuracy);
            double lw_value = double.Parse(limit_value) - double.Parse(limit_accuracy);
            double se_value = 0.0;
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", cmd);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(cmd, 1000);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);
            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Dữ liệu phản hồi từ sản phầm là sai định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType(string.Format("Dữ liệu phản hồi từ sản phầm \"{0}\" là sai định dạng.", feedback_data), em.PropertyType), null);
                    goto END;
                }
            }
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            try {
                se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
            }
            catch { se_value = double.MaxValue; }
            LogSystem += string.Format(".........Giá trị: {0}\r\n", se_value);
            LogSystem += string.Format(".........Tiêu chuẩn: {0} ~ {1}\r\n", lw_value, up_value);
            //validate sensor value
            r = (se_value >= lw_value) && (se_value <= up_value);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: giá trị cảm biến nằm ngoài dải tiêu chuẩn\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("giá trị cảm biến nằm ngoài dải tiêu chuẩn.", em.PropertyType), null);
                    goto END;
                }
            }

            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: KIEM TRA TINH CHINH XAC CUA CAM BIEN ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = passed, FAIL = failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="dut_id"></param>
        /// <param name="dut_type"></param>
        /// <param name="sensor"></param>
        /// <param name="limit_value"></param>
        /// <param name="limit_accuracy"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Is_Sensor_Valid_D<T>(T testinfo, string dut_id, DeviceType dut_type, SensorType sensor, string limit_value, string limit_accuracy, int retrytime, int delay) where T : class, new() {
            bool r = false;
            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            string sensor_name = "";
            int sensor_value = (int)sensor;
            if (sensor_value == 2) {
                switch (dut_type) {
                    case DeviceType.SMH_CO: { sensor_name = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor_name = "GAS"; break; }
                    case DeviceType.SMH_SMOKE: { sensor_name = "SMOKE"; break; }
                }
            }
            else sensor_name = sensor.ToString().ToUpper();

            LogSystem += string.Format("\r\n+++ KIỂM TRA ĐỘ CHÍNH XÁC CỦA CẢM BIẾN {0} +++\r\n", sensor_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test.", em.PropertyType), null);
                goto END;
            }
            //get sensor value
            string cmd = string.Format("CHECK,{0},{1},Readsensor!", dut_id, dut_type.ToString().ToUpper());
            string feedback_data = "";
            int count = 0;
            int idx = (int)sensor + 3;
            double up_value = double.Parse(limit_value) + double.Parse(limit_accuracy);
            double lw_value = double.Parse(limit_value) - double.Parse(limit_accuracy);
            double se_value = 0.0;
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", cmd);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(cmd, delay);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);
            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Dữ liệu phản hồi từ sản phầm là sai định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType(string.Format("Dữ liệu phản hồi từ sản phầm \"{0}\" là sai định dạng.", feedback_data), em.PropertyType), null);
                    goto END;
                }
            }
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            try {
                se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
            }
            catch { se_value = double.MaxValue; }
            LogSystem += string.Format(".........Giá trị: {0}\r\n", se_value);
            LogSystem += string.Format(".........Tiêu chuẩn: {0} ~ {1}\r\n", lw_value, up_value);
            //validate sensor value
            r = (se_value >= lw_value) && (se_value <= up_value);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: giá trị cảm biến nằm ngoài dải tiêu chuẩn\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("giá trị cảm biến nằm ngoài dải tiêu chuẩn.", em.PropertyType), null);
                    goto END;
                }
            }

            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: KIEM TRA TINH CHINH XAC CUA CAM BIEN ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = passed, FAIL = failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="dut_id"></param>
        /// <param name="dut_type"></param>
        /// <param name="sensor"></param>
        /// <param name="limit_value"></param>
        /// <param name="limit_accuracy"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Is_Sensor_Valid<T>(T testinfo, string dut_id, DeviceType dut_type, SensorType sensor, string limit_value, string limit_accuracy, int retrytime, out double ss_value) where T : class, new() {
            bool r = false;
            ss_value = double.MaxValue;

            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            string sensor_name = "";
            int sensor_value = (int)sensor;
            if (sensor_value == 2) {
                switch (dut_type) {
                    case DeviceType.SMH_CO: { sensor_name = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor_name = "GAS"; break; }
                    case DeviceType.SMH_SMOKE: { sensor_name = "SMOKE"; break; }
                }
            }
            else sensor_name = sensor.ToString().ToUpper();

            LogSystem += string.Format("\r\n+++ KIỂM TRA ĐỘ CHÍNH XÁC CỦA CẢM BIẾN {0} +++\r\n", sensor_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test.", em.PropertyType), null);
                goto END;
            }
            //get sensor value
            string cmd = string.Format("CHECK,{0},{1},Readsensor!", dut_id, dut_type.ToString().ToUpper());
            string feedback_data = "";
            int count = 0;
            int idx = (int)sensor + 3;
            double up_value = double.Parse(limit_value) + double.Parse(limit_accuracy);
            double lw_value = double.Parse(limit_value) - double.Parse(limit_accuracy);
            //double se_value = 0.0;
            double se_value = -999;
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", cmd);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(cmd, 1000);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);
            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Dữ liệu phản hồi từ sản phầm là sai định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType(string.Format("Dữ liệu phản hồi từ sản phầm \"{0}\" là sai định dạng.", feedback_data), em.PropertyType), null);
                    goto END;
                }
            }
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            try {
                if (feedback_data.ToLower().Contains(dut_id.ToLower())) {
                    se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
                }
                else se_value = -999;
                    //se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
            }
            catch { se_value = -999; }
            //catch { se_value = double.MaxValue; }
            ss_value = se_value;

            LogSystem += string.Format(".........Giá trị: {0}\r\n", se_value);
            LogSystem += string.Format(".........Tiêu chuẩn: {0} ~ {1}\r\n", lw_value, up_value);
            //validate sensor value
            r = (se_value >= lw_value) && (se_value <= up_value);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: giá trị cảm biến nằm ngoài dải tiêu chuẩn\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("giá trị cảm biến nằm ngoài dải tiêu chuẩn.", em.PropertyType), null);
                    goto END;
                }
            }

            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: KIEM TRA TINH CHINH XAC CUA CAM BIEN ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = passed, FAIL = failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="dut_id"></param>
        /// <param name="dut_type"></param>
        /// <param name="sensor"></param>
        /// <param name="limit_value"></param>
        /// <param name="limit_accuracy"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Is_Sensor_Valid_D<T>(T testinfo, string dut_id, DeviceType dut_type, SensorType sensor, string limit_value, string limit_accuracy, int retrytime, int delay, out double ss_value) where T : class, new() {
            bool r = false;
            ss_value = double.MaxValue;

            PropertyInfo em = testinfo.GetType().GetProperty("TestMessage");
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            string sensor_name = "";
            int sensor_value = (int)sensor;
            if (sensor_value == 2) {
                switch (dut_type) {
                    case DeviceType.SMH_CO: { sensor_name = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor_name = "GAS"; break; }
                    case DeviceType.SMH_SMOKE: { sensor_name = "SMOKE"; break; }
                }
            }
            else sensor_name = sensor.ToString().ToUpper();

            LogSystem += string.Format("\r\n+++ KIỂM TRA ĐỘ CHÍNH XÁC CỦA CẢM BIẾN {0} +++\r\n", sensor_name);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: sản phẩm chưa kết nối tới máy tính trạm test\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);
                em.SetValue(testinfo, Convert.ChangeType("sản phẩm chưa kết nối tới máy tính trạm test.", em.PropertyType), null);
                goto END;
            }
            //get sensor value
            string cmd = string.Format("CHECK,{0},{1},Readsensor!", dut_id, dut_type.ToString().ToUpper());
            string feedback_data = "";
            int count = 0;
            int idx = (int)sensor + 3;
            double up_value = double.Parse(limit_value) + double.Parse(limit_accuracy);
            double lw_value = double.Parse(limit_value) - double.Parse(limit_accuracy);
            //double se_value = 0.0;
            double se_value = -999;
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", cmd);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(cmd, delay);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);
            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Dữ liệu phản hồi từ sản phầm là sai định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType(string.Format("Dữ liệu phản hồi từ sản phầm \"{0}\" là sai định dạng.", feedback_data), em.PropertyType), null);
                    goto END;
                }
            }
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            try {
                if (feedback_data.ToLower().Contains(dut_id.ToLower())) {
                    se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
                }
                else se_value = -999;
                //se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
            }
            catch { se_value = -999; }
            //catch { se_value = double.MaxValue; }
            ss_value = se_value;

            LogSystem += string.Format(".........Giá trị: {0}\r\n", se_value);
            LogSystem += string.Format(".........Tiêu chuẩn: {0} ~ {1}\r\n", lw_value, up_value);
            //validate sensor value
            r = (se_value >= lw_value) && (se_value <= up_value);
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: giá trị cảm biến nằm ngoài dải tiêu chuẩn\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else {
                    em.SetValue(testinfo, Convert.ChangeType("giá trị cảm biến nằm ngoài dải tiêu chuẩn.", em.PropertyType), null);
                    goto END;
                }
            }

            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }

        /// <summary>
        /// FUNCTION: CHUYEN MODE FIRMWARE SANG THUONG MAI ///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: TRUE = passed, FAIL = failed
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="testinfo"></param>
        /// <param name="dut_id"></param>
        /// <param name="dut_type"></param>
        /// <param name="limit_value"></param>
        /// <param name="retrytime"></param>
        /// <returns></returns>
        public static bool Switch_Firmware_To_User_Mode<T>(T testinfo, string dut_id, DeviceType dut_type, string limit_value, int retrytime) where T : class, new() {
            bool r = false;
            PropertyInfo lg = testinfo.GetType().GetProperty("LogSystem");
            string LogSystem = lg.GetValue(testinfo, null).ToString();

            LogSystem += string.Format("\r\n+++ CHUYỂN FIRMWARE SANG MODE THƯƠNG MẠI +++\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            limit_value = "Mode_Trade";

            //string cmd = string.Format("MODE,{0},{1},{2}!", dut_id, dut_type.ToString().ToUpper(), limit_value); //fix trùng mode update từ xa
            string cmd = string.Format("CHECK,{0},{1},{2}!", dut_id, dut_type.ToString().ToUpper(), limit_value);
            string feedback_data = "";
            string fwmode = "";
            int count = 0;
        REP_SEND:
            count++;
            LogSystem += string.Format(">>> Kiểm tra lần thứ {0}\r\n", count);
            LogSystem += string.Format(".........Gửi lệnh tới sản phẩm: {0}\r\n", cmd);
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

            feedback_data = DUT.QueryData(cmd, 1000);
            LogSystem += string.Format(".........Dữ liệu phản hồi từ sản phẩm: {0}\r\n", feedback_data);
            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: Dữ liệu phản hồi từ sản phầm là sai định dạng\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else { goto END; }
            }
            feedback_data = feedback_data.Split('!')[0];
            string[] buffer = feedback_data.Split(',');
            try {
                fwmode = buffer[3].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim();
            }
            catch { fwmode = ""; }
            //validate firmware mode
            r = fwmode.ToLower().Equals("34");
            if (!r) {
                LogSystem += string.Format(".........Kết quả: Failed\r\n");
                LogSystem += string.Format(".........Thông tin lỗi: firmware chưa chuyển sang mode thương mại\r\n");
                lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

                if (count < retrytime) {
                    goto REP_SEND;
                }
                else { goto END; }
            }

            LogSystem += string.Format(".........Kết quả: Passed\r\n");
            lg.SetValue(testinfo, Convert.ChangeType(LogSystem, lg.PropertyType), null);

        END:
            //return value
            return r;
        }


        /// <summary>
        /// FUNCTION: LAY GIA TRI CAM BIEN CO, GAS, SMOKE///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: value is double (value = double.MaxValue error)
        /// </summary>
        /// <param name="dut_id"></param>
        /// <param name="dut_type"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static double Get_Sensor_Value(string dut_id, DeviceType dut_type, SensorType sensor, int retrytime, out string msg) {
            double se_value = -999;
            string sensor_name = "";
            bool r = false;
            msg = "";


            int sensor_value = (int)sensor;
            if (sensor_value == 2) {
                switch (dut_type) {
                    case DeviceType.SMH_CO: { sensor_name = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor_name = "GAS"; break; }
                    case DeviceType.SMH_SMOKE: { sensor_name = "SMOKE"; break; }
                }
            }
            else sensor_name = sensor.ToString().ToUpper();

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                goto END;
            }

            //get sensor value
            string cmd = string.Format("CHECK,{0},{1},Readsensor!", dut_id, dut_type.ToString().ToUpper());
            string feedback_data = "";
            int count = 0;
            int idx = (int)sensor + 3;

        REP_SEND:
            count++;
            msg += string.Format("Gửi lệnh đọc giá trị cảm biến ppm [{0}]: {1}\r\n", count, cmd);
            feedback_data = DUT.QueryData_S(dut_id, cmd, 1000);
            msg += string.Format("Dữ liệu phản hồi [{0}]: {1}\r\n", count, feedback_data);
            //check data format
            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,");
            if (!r) {
                if (count < retrytime) goto REP_SEND;
                else goto END;
            }
            //check sensor value
            string[] s = feedback_data.Split('!');
            feedback_data = feedback_data.Split('!')[s.Length - 2];
            string[] buffer = feedback_data.Split(',');
            try {
                if (feedback_data.ToLower().Contains(dut_id.ToLower())) {
                    se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
                }
                else se_value = -999;
            }
            catch { se_value = -999; }
            msg += string.Format("Giá trị cảm biến ppm [{0}]: {1}\r\n", count, se_value);

            if (se_value == -999) {
                if (count < retrytime) goto REP_SEND;
                else goto END;
            }

        END:
            //return value
            return se_value;
        }


        /// <summary>
        /// FUNCTION: LAY GIA TRI CAM BIEN CO, GAS, SMOKE///
        /// !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        /// Result: value is double (value = double.MaxValue error)
        /// </summary>
        /// <param name="dut_id"></param>
        /// <param name="dut_type"></param>
        /// <param name="sensor"></param>
        /// <returns></returns>
        public static double Get_Sensor_Value_D(string dut_id, DeviceType dut_type, SensorType sensor, int retrytime, int delay, out string msg) {
            double se_value = -999;
            string sensor_name = "";
            bool r = false;
            msg = "";


            int sensor_value = (int)sensor;
            if (sensor_value == 2) {
                switch (dut_type) {
                    case DeviceType.SMH_CO: { sensor_name = "CO"; break; }
                    case DeviceType.SMH_GAS: { sensor_name = "GAS"; break; }
                    case DeviceType.SMH_SMOKE: { sensor_name = "SMOKE"; break; }
                }
            }
            else sensor_name = sensor.ToString().ToUpper();

            //check DUT connected to PC or not
            if (DUT == null || DUT.IsConnected == false) {
                goto END;
            }

            //get sensor value
            string cmd = string.Format("CHECK,{0},{1},Readsensor!", dut_id, dut_type.ToString().ToUpper());
            string feedback_data = "";
            int count = 0;
            int idx = (int)sensor + 3;

        REP_SEND:
            count++;
            msg += string.Format("Gửi lệnh đọc giá trị cảm biến ppm [{0}]: {1}\r\n", count, cmd);
            feedback_data = DUT.QueryData_S(dut_id, cmd, delay);
            msg += string.Format("Dữ liệu phản hồi [{0}]: {1}\r\n", count, feedback_data);
            //check data format
            r = (string.IsNullOrEmpty(feedback_data) == false) && feedback_data.ToLower().Contains("resp,");
            if (!r) {
                if (count < retrytime) goto REP_SEND;
                else goto END;
            }
            //check sensor value
            string[] s = feedback_data.Split('!');
            feedback_data = feedback_data.Split('!')[s.Length - 2];
            string[] buffer = feedback_data.Split(',');
            try {
                if (feedback_data.ToLower().Contains(dut_id.ToLower())) {
                    se_value = double.Parse(buffer[idx].Replace("!", "").Replace("\r", "").Replace("\n", "").Trim());
                }
                else se_value = -999;
            }
            catch { se_value = -999; }
            msg += string.Format("Giá trị cảm biến ppm [{0}]: {1}\r\n", count, se_value);

            if (se_value == -999) {
                if (count < retrytime) goto REP_SEND;
                else goto END;
            }

        END:
            //return value
            return se_value;
        }



    }
}
