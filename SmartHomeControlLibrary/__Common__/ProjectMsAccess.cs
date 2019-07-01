using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Access = Microsoft.Office.Interop.Access;
using UtilityPack.Protocol;

namespace SmartHomeControlLibrary.__Common__ {

    public class ProjectMsAccess {

        public MSAccessDB accessDB = null;
        public string Access_FileFullName = "";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="access_file_name"></param>
        public ProjectMsAccess(string access_file_fullname) {
            Access_FileFullName = access_file_fullname;
            accessDB = new MSAccessDB(Access_FileFullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool OpenConnection() {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                return accessDB.IsConnected;
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CloseConnection() {
            try {
                //close access database
                if (accessDB.IsConnected) accessDB.Close();
                Thread.Sleep(100);
                return true;
            } catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public bool Input_New_DataRow_To_Access_DB_Table<T>(string table_name, T t) {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return false;

                return accessDB.InsertDataToTable<T>(t, table_name);
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public bool Input_New_DataRow_To_Access_DB_Table<T>(string table_name, T t, string ignore_column_name) {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return false;

                return accessDB.InsertDataToTable<T>(t, table_name, ignore_column_name);
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table_name"></param>
        /// <returns></returns>
        public bool Delete_All_DataRow_From_Access_DB_Table(string table_name) {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return false;

                return accessDB.QueryDeleteOrUpdate(string.Format("DELETE FROM {0}", table_name));
            }
            catch {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table_name"></param>
        /// <param name="row_quantity"></param>
        /// <returns></returns>
        public List<T> Get_Newest_DataRow_From_Access_DB_Table<T>(string table_name, int row_quantity, string ref_Field) where T : class, new() {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return null;

                return accessDB.QueryDataReturnListObject<T>(string.Format("SELECT TOP {0} * FROM {1} ORDER BY {2} DESC", row_quantity, table_name, ref_Field));
            }
            catch {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table_name"></param>
        /// <param name="row_quantity"></param>
        /// <returns></returns>
        public List<T> Get_Distinct_Newest_DataRow_From_Access_DB_Table<T>(string table_name, int row_quantity, string selected_Field) where T : class, new() {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return null;

                return accessDB.QueryDataReturnListObject<T>(string.Format("SELECT DISTINCT TOP {0} {1} FROM {2}", row_quantity, selected_Field, table_name));
            }
            catch {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table_name"></param>
        /// <param name="row_quantity"></param>
        /// <returns></returns>
        public List<T> Get_Distinct_Newest_DataRow_From_Access_DB_Table<T>(string table_name, int row_quantity, string selected_Field, string field_value) where T : class, new() {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return null;

                return accessDB.QueryDataReturnListObject<T>(string.Format("SELECT DISTINCT TOP {0} {1} FROM {2} WHERE {1} LIKE '%{3}%'", row_quantity, selected_Field, table_name, field_value));
            }
            catch {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table_name"></param>
        /// <param name="row_quantity"></param>
        /// <returns></returns>
        public List<T> Get_Specified_DataRow_From_Access_DB_Table<T>(string table_name, int row_quantity, string Field_Order, string ref_Field1, string field_Value1, string ref_Field2, string field_Value2, string ref_Field3, string field_Value3) where T : class, new() {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return null;

                string cmdStr = string.Format("SELECT TOP {0} * FROM {1}", row_quantity, table_name);
                if (field_Value1.Trim() != "") {
                    cmdStr += string.Format(" WHERE {0} LIKE '%{1}%'", ref_Field1, field_Value1);
                }
                if (field_Value2.Trim() != "") {
                    bool r = cmdStr.Contains("WHERE");
                    cmdStr += string.Format(" {2} {0} LIKE '%{1}%'", ref_Field2, field_Value2, r == true ? "AND" : "WHERE");
                }
                if (field_Value3.Trim() != "") {
                    bool r = cmdStr.Contains("WHERE");
                    cmdStr += string.Format(" {2} {0} LIKE '%{1}%'", ref_Field3, field_Value3, r == true ? "AND" : "WHERE");
                }
                cmdStr += string.Format(" ORDER BY {0} DESC", Field_Order);

                return accessDB.QueryDataReturnListObject<T>(cmdStr);
            }
            catch {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table_name"></param>
        /// <param name="ref_Field"></param>
        /// <param name="field_Value"></param>
        /// <returns></returns>
        public T Get_Specified_DataRow_From_Access_DB_Table<T>(string table_name, string ref_Field, string field_Value) where T : class, new() {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return null;

                return accessDB.QueryDataReturnObject<T>(string.Format("SELECT TOP 1 * FROM {0} WHERE {1}='{2}' AND Rework='-' ORDER BY DateTimeCreated DESC", table_name, ref_Field, field_Value));
            }
            catch {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="query_string"></param>
        /// <returns></returns>
        public bool QueryData(string query_string) {
            try {
                if (!accessDB.IsConnected) accessDB.OpenConnection();
                Thread.Sleep(100);
                if (!accessDB.IsConnected) return false;

                return accessDB.QueryDeleteOrUpdate(query_string);
            }
            catch {
                return false;
            }
        }


        public bool Print_Access_Report(params string[] reports) {
            try {
                //close access database
                if (accessDB.IsConnected) accessDB.Close();
                Thread.Sleep(100);

                //init access file
                Access.Application oAccess = null;

                // Start a new instance of Access for Automation:
                oAccess = new Access.Application();
                oAccess.Visible = false;

                // Open a database in exclusive mode:
                oAccess.OpenCurrentDatabase(Access_FileFullName);

                //print out report
                foreach (var report in reports) {

                    // Select the Employees report in the database window: //3
                    oAccess.DoCmd.SelectObject(
                       Access.AcObjectType.acReport, //ObjectType
                       report, //ObjectName
                       true //InDatabaseWindow
                       );

                    // Print 1 copies of the selected object:
                    oAccess.DoCmd.PrintOut(
                       Access.AcPrintRange.acPrintAll, //PrintRange
                       System.Reflection.Missing.Value, //PageFrom
                       System.Reflection.Missing.Value, //PageTo
                       Access.AcPrintQuality.acHigh, //PrintQuality
                       1, //Copies
                       false //CollateCopies
                       );
                }

                //quit and release resource
                oAccess.CloseCurrentDatabase();
                oAccess.Quit();

                Marshal.ReleaseComObject(oAccess);

                return true;
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="databaseLocation"></param>
        /// <param name="queryNameToExport"></param>
        /// <param name="locationToExportTo"></param>
        /// <returns></returns>
        public bool ExportQuery(string tableName, string locationToExportTo) {
            try {
                //init access file
                Access.Application oAccess = null;

                // Start a new instance of Access for Automation:
                oAccess = new Access.Application();
                oAccess.Visible = false;

                // Open a database in exclusive mode:
                oAccess.OpenCurrentDatabase(Access_FileFullName);

                //transfer access data to excel file
                oAccess.DoCmd.TransferSpreadsheet(Access.AcDataTransferType.acExport, Access.AcSpreadSheetType.acSpreadsheetTypeExcel12, tableName, locationToExportTo, true);

                //close database
                oAccess.CloseCurrentDatabase();
                oAccess.Quit();

                Marshal.ReleaseComObject(oAccess);

                return true;
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show(ex.ToString());
                return false;
            }
        }



    }
}
