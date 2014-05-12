using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using System.Reflection;

namespace Logger
{
    public class FileLogger : ILogger
    {
        #region 基础配置
        /// <summary>
        /// 单个文件大小,默认为5M
        /// </summary>
        public int MaxSize { get; set; }
        /// <summary>
        /// 文件个数
        /// </summary>
        public int MaxCount { get; set; }
        /// <summary>
        /// 文件级别和文件
        /// </summary>
        public Dictionary<LogLevel, string> FileMap { get; private set; }
        string format = "[{0}] CLASS:{1},FUNCTION:{2},LEVEL:{3}\r\n\t{4}";
        LoggerConfig config;
        Object lockObject = new object();
        string ParentPath = String.Empty;
        Encoding _encode = null;
        #endregion


        #region Logger
        internal FileLogger(LoggerConfig config)
        {
            ChanagerConfig(config);
            ParentPath = System.AppDomain.CurrentDomain.BaseDirectory;
        }
        #endregion

        #region CheckFileSize
        private void CheckFileSize(FileInfo info)
        {
            string fileName = info.FullName;
            if (info.Length > MaxSize * 1024 * 1024)
            {
                int tmpFileCount = MaxCount;
                if (File.Exists(fileName + "." + tmpFileCount))
                {
                    File.Delete(fileName + "." + tmpFileCount);
                }
                if (MaxCount == 0)
                {
                    using (File.Create(fileName))
                    {
                    }
                    return;
                }
                while (tmpFileCount >= 0)
                {
                    if (tmpFileCount > 0)
                    {
                        if (File.Exists(fileName + "." + (tmpFileCount - 1)))
                        {
                            File.Move(fileName + "." + (tmpFileCount - 1), fileName + "." + tmpFileCount);
                        }
                    }
                    else
                    {
                        File.Move(fileName, fileName + "." + tmpFileCount);
                        using (File.Create(fileName))
                        {
                        }
                    }
                    tmpFileCount--;
                }
            }
        }
        #endregion

        #region WriteLog
        public void Log(string content)
        {
            Log(string.Empty, string.Empty, content, LogLevel.INFO);
        }

        public void Log(string content, LogLevel level)
        {
            Log(string.Empty, string.Empty, content, level);
        }
        public void Log(string className, string function, string content)
        {
            Log(className, function, content, LogLevel.INFO);
        }
        public void Log(string className, string function, string content, LogLevel level)
        {
            try
            {
                lock (lockObject)
                {
                    if (level < config.Level) return;
                    string debugFile = string.Empty;
                    string infoFile = string.Empty;
                    if (level == LogLevel.DEBUG)
                    {
                        debugFile = CheckFile(LogLevel.DEBUG);
                        WriteFile(className, function, content, level, debugFile);
                    }
                    else if (level == LogLevel.INFO)
                    {
                        infoFile = CheckFile(LogLevel.INFO);
                        WriteFile(className, function, content, level, infoFile);
                    }
                    else if (level == LogLevel.ERROR || level == LogLevel.WARN)
                    {
                        string infoName = CheckFile(level);
                        WriteFile(className, function, content, level, infoName);

                        if (config.FileDicts[level] != config.FileDicts[LogLevel.INFO])
                        {
                            infoFile = CheckFile(LogLevel.INFO);
                            WriteFile(className, function, content, level, infoFile);
                        }
                        if (level > LogLevel.INFO && config.Level == LogLevel.DEBUG && config.FileDicts[LogLevel.DEBUG] != config.FileDicts[LogLevel.INFO])
                        {
                            debugFile = CheckFile(LogLevel.DEBUG);
                            WriteFile(className, function, content, level, debugFile);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void WriteFile(string className, string function, string content, LogLevel level, string fileName)
        {
            try
            {
                FileStream fs;
                if (!File.Exists(fileName))
                {
                    fs = File.Create(fileName);
                }
                else
                {
                    fs = File.OpenWrite(fileName);
                }
                fs.Seek(0, SeekOrigin.End);
                using (StreamWriter writer = new StreamWriter(fs, _encode))
                {
                    string result = string.Format(format, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), string.IsNullOrEmpty(className) ? string.Empty : className, string.IsNullOrEmpty(function) ? string.Empty : function, level.ToString(), string.IsNullOrEmpty(content) ? string.Empty : content);
                    writer.WriteLine(result);
                    Console.WriteLine(result);
                }
                fs.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private string CheckFile(LogLevel level)
        {
            string fileName = string.Empty;
            if (config.Dir.Trim().StartsWith("."))
            {
                fileName = Path.Combine(ParentPath, config.Dir, FileMap[level]);
            }
            else
            {
                fileName = Path.Combine(config.Dir, FileMap[level]);
            }
            FileInfo info = new FileInfo(fileName);
            if (!Directory.Exists(info.Directory.FullName))
            {
                Directory.CreateDirectory(info.Directory.FullName);
            }
            if (File.Exists(info.FullName))
            {
                CheckFileSize(info);
            }
            return fileName;
        }
        #endregion


        internal void ChanagerConfig(LoggerConfig config)
        {
            this.MaxCount = config.FileCount;
            this.FileMap = config.FileDicts;
            this.MaxSize = config.MaxSize;
            this.config = config;
            _encode = Encoding.GetEncoding(config.Encode);
        }

        public void Dispose()
        {
        }



    }
}
