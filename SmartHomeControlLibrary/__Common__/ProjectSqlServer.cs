using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Reflection;

namespace SmartHomeControlLibrary.__Common__ {

    public class ProjectSqlServer {
        SqlConnection cn;
        SqlCommand cmd;

        bool IsConnected { get; set; }
        string server, database, userid, password;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_server"></param>
        /// <param name="_db"></param>
        /// <param name="_user"></param>
        /// <param name="_pass"></param>
        public ProjectSqlServer(string _server, string _db, string _user, string _pass) {
            this.server = _server;
            this.database = _db;
            this.userid = _user;
            this.password = _pass;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool Open() {
            try {
                cn = new SqlConnection(string.Format("server={0};database={1};user id={2};password={3};", this.server, this.database, this.userid, this.password));
                cn.Open();
                IsConnected = cn.State == System.Data.ConnectionState.Open;
                return IsConnected;
            } catch {
                return false;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool Close() {
            try {
                if (cn != null && IsConnected == true) {
                    cn.Close();
                }
                return true;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table_name"></param>
        /// <param name="t"></param>
        /// <param name="igr_column"></param>
        /// <returns></returns>
        public bool Insert_NewRow_To_SqlTable<T>(string table_name, T t, string igr_column) where T : class, new() {
            try {
                if (!IsConnected) { bool r = Open(); }
                if (!IsConnected) return false;

                Type itemType = typeof(T);
                var properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                string s1 = "", s2 = "";
                foreach (var p in properties) {
                    if (!p.Name.ToLower().Equals(igr_column.ToLower())) {
                        s1 += string.Format("[{0}],", p.Name);
                        s2 += string.Format("'{0}',", p.GetValue(t, null));
                    }
                }
                s1 = s1.Substring(0, s1.Length - 1);
                s2 = s2.Substring(0, s2.Length - 1);

                cmd = new SqlCommand();
                cmd.Connection = cn;
                cmd.CommandType = System.Data.CommandType.Text;
                cmd.CommandText = string.Format("INSERT INTO {0}({1}) VALUES({2})", table_name, s1, s2);
                cmd.ExecuteNonQuery();

                Close();
                return true;
            } catch {
                return false;
            }
        }




    }
}
