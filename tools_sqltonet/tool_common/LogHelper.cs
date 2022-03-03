using System;
using System.IO;
using System.Reflection;

namespace tool_common
{
    public  class LogHelper
    {
        #region Instance
        private static object logLock;

        private static LogHelper _instance;

        private static string logFileName;
        private LogHelper() { }

        /// <summary>
        /// Logger instance
        /// </summary>
        public static LogHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LogHelper();
                    logLock = new object();
                    //logFileName = Guid.NewGuid() + ".log";
                    logFileName = DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                }
                return _instance;
            }
        }
        #endregion

        /// <summary>
        /// Write log to log file
        /// </summary>
        /// <param name="logContent">Log content</param>
        /// <param name="logType">Log type</param>
        public void WriteLog(string logContent, LogType logType = LogType.Information, string fileName = null)
        {
            try
            {
                string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                //basePath = @"C:\APILogs";
                if (!Directory.Exists(basePath + "\\Log"))
                {
                    Directory.CreateDirectory(basePath + "\\Log");
                }

                string dataString = DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(basePath + "\\Log\\" + dataString))
                {
                    Directory.CreateDirectory(basePath + "\\Log\\" + dataString);
                }

                string[] logText = new string[] { DateTime.Now.ToString("HH:mm:ss") + ": " + logType.ToString() + ": " + logContent };
                if (!string.IsNullOrEmpty(fileName))
                {
                    fileName = fileName + "_" + logFileName;
                }
                else
                {
                    fileName = logFileName;
                }

                lock (logLock)
                {
                    File.AppendAllLines(basePath + "\\Log\\" + dataString + "\\" + fileName, logText);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Write exception to log file
        /// </summary>
        /// <param name="exception">Exception</param>
        public void WriteException(Exception exception, string specialText = null)
        {
            if (exception != null)
            {
                Type exceptionType = exception.GetType();
                string text = string.Empty;
                if (!string.IsNullOrEmpty(specialText))
                {
                    text = text + specialText + Environment.NewLine;
                }
                text += "Exception: " + exceptionType.Name + Environment.NewLine;
                text += "               " + "Message: " + exception.Message + Environment.NewLine;
                text += "               " + "Source: " + exception.Source + Environment.NewLine;
                text += "               " + "StackTrace: " + exception.StackTrace + Environment.NewLine;
                WriteLog(text, LogType.Error);
            }
        }
    }
}