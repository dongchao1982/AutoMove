using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    public class INI
    {
        public static string IniFilePath;

        private static byte[] getBytes(string s, string encodingName)
        {
            return null == s ? null : Encoding.GetEncoding(encodingName).GetBytes(s);
        }

        /// <summary>  
        /// 写操作  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);


        /// <summary>  
        /// 读操作  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="def">未读取到的默认值</param>  
        /// <param name="retVal">读取到的值</param>  
        /// <param name="size">大小</param>  
        /// <param name="filePath">路径</param>  
        /// <returns></returns>  
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);

        /// <summary>  
        /// 读ini文件  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="defValue">未读取到值时的默认值</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        public static string ReadIni(string section, string key,string encodingName = "utf-8")
        {
            int size = 256;
            byte[] buffer = new byte[size];
            int count = GetPrivateProfileString(getBytes(section, encodingName), getBytes(key, encodingName), getBytes("", encodingName), buffer, size, IniFilePath);
            return Encoding.GetEncoding(encodingName).GetString(buffer, 0, count).Trim();
        }

        /// <summary>  
        /// 写入ini文件  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="value">值</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        public static void WriteIni(string section, string key, string value, string encodingName = "utf-8")
        {
            WritePrivateProfileString(getBytes(section, encodingName), getBytes(key, encodingName), getBytes(value, encodingName), IniFilePath);
        }
        /// <summary>  
        /// 删除节  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="filePath"></param>  
        /// <returns></returns>  
        public static long DeleteSection(string section, string encodingName = "utf-8")
        {
            return WritePrivateProfileString(getBytes(section, encodingName), null, null, IniFilePath);
        }

        /// <summary>  
        /// 删除键  
        /// </summary>  
        /// <param name="section">节</param>  
        /// <param name="key">键</param>  
        /// <param name="filePath">文件路径</param>  
        /// <returns></returns>  
        public static long DeleteKey(string section, string key, string encodingName = "utf-8")
        {
            return WritePrivateProfileString(getBytes(section, encodingName), getBytes(key, encodingName), null, IniFilePath);
        }

    }
}
