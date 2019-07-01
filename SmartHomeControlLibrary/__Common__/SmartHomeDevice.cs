using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using UtilityPack.Protocol;
using UtilityPack.Converter;

namespace SmartHomeControlLibrary.__Common__ {

    public class SmartHomeDevice {

        UART device = null;
        string serialportname = "";
        string baudrate = "";
        string databits = "";
        System.IO.Ports.Parity parity;
        System.IO.Ports.StopBits stopbits;

        public bool IsConnected { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_serialportname"></param>
        /// <param name="_baudrate"></param>
        /// <param name="_databits"></param>
        /// <param name="_parity"></param>
        /// <param name="_stopbits"></param>
        public SmartHomeDevice(string _serialportname, string _baudrate, string _databits, string _parity, string _stopbits) {
            this.serialportname = _serialportname;
            this.baudrate = _baudrate;
            this.databits = _databits;
            this.parity = myConverter.FromStringToSerialParity(_parity);
            this.stopbits = myConverter.FromStringToSerialStopBits(_stopbits);

            IsConnected = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Open() {
            try {
                if (device != null) device = null;
                device = new UART();
                IsConnected = device.Open(this.serialportname, this.baudrate, this.databits, this.parity, this.stopbits);
                return IsConnected;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Close() {
            try {
                if (device != null && IsConnected == true) {
                    return device.Close();
                }
                return true;
            }
            catch {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetIDDevice() {
            if (!IsConnected) return "";
            return "";
           
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool Write(string cmd) {
            if (!IsConnected) return false;
            device.Write(cmd);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool WriteLine(string cmd) {
            if (!IsConnected) return false;
            device.WriteLine(cmd);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Read() {
            if (!IsConnected) return "";
            return device.Read();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string QueryData(string cmd, int wait_time) {
            if (!IsConnected) return "";
            //write command
            device.Read();
            device.WriteLine(cmd);

            //get data
            Thread.Sleep(wait_time);
            return device.Read();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string QueryData_S(string id, string cmd, int wait_time) {
            if (!IsConnected) return "";
            //write command
            device.Read();
            device.WriteLine(cmd);

            //get data
            string s = "";
            int c = wait_time / 100;
            for (int i = 0; i < c; i++) {
                s += device.Read();
                if (string.IsNullOrEmpty(s) == false) {
                    if (s.Contains("!") && s.ToLower().Contains(id.ToLower())) break;
                }
                Thread.Sleep(100);
            }
            return s;
        }

    }
}
