using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace tool_common
{
    /// <summary>
    /// 写入cs文件
    /// </summary>
    public class WriteCSHelper
    {
        #region Instance
        private static object logLock;

        private static WriteCSHelper _instance;

        private WriteCSHelper() { }

        /// <summary>
        /// Logger instance
        /// </summary>
        public static WriteCSHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WriteCSHelper();
                    logLock = new object();
                }
                return _instance;
            }
        }
        #endregion
        /// <summary>
        /// 删除cs文件
        /// </summary>
        public void DeleteCS(string tbname)
        {
            if (string.IsNullOrEmpty(tbname)) tbname = DateTime.Now.ToString("yyyyMMdd");
            string fileName = tbname + ".cs";
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string path = basePath + "\\ac_database\\" + fileName;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 写入.cs文件
        /// </summary>
        /// <param name="tbname">表名 </param>
        /// <param name="logContent">内容</param>
        public void WriteCS(string tbname, string logContent)
        {
            if (string.IsNullOrEmpty(tbname)) tbname = DateTime.Now.ToString("yyyyMMdd");
            string fileName = tbname + ".cs";
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!Directory.Exists(basePath + "\\ac_database"))
                {
                    Directory.CreateDirectory(basePath + "\\ac_database");
                }

                string[] logText = new string[] { logContent };
                string path = basePath + "\\ac_database\\" + fileName;

                lock (logLock)
                {
                    File.AppendAllLines(path, logText);
                }
            }
            catch (Exception) { }
        }
    }
}