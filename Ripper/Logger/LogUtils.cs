using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using System.Threading;


namespace Logger
{
    class LogUtils
    {
        static Dictionary<string, LoggerConfig> Dicts = new Dictionary<string, LoggerConfig>();
        static readonly object lockObject = new object();
        static internal event Action OnLoggerConfigChanged;
        private const int TimeoutMillis = 500;
        private static Timer m_timer;

        static LogUtils()
        {
            Load();
            MonitorConfig();
            m_timer = new Timer(new TimerCallback(OnWatchedFileChange), null, Timeout.Infinite, Timeout.Infinite);
        }
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        static void MonitorConfig()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config");
            watcher.Filter = "Logger.xml";

            // Set the notification filters
            watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;
            // Add event handlers. OnChanged will do for all event handlers that fire a FileSystemEventArgs
            watcher.Changed += new FileSystemEventHandler(OnConfigChanged);
            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        static void OnConfigChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType == WatcherChangeTypes.Changed)
                {
                    m_timer.Change(TimeoutMillis, Timeout.Infinite);
                }
            }
            catch (Exception ex)
            {
                LoggerManager.GetLog().Log("Logger.LogUtils", "OnConfigChanged", "日志变更发生异常[" + Thread.CurrentThread.ManagedThreadId + "]\r\n" + ex.ToString(), LogLevel.ERROR);
            }
        }

        private static void OnWatchedFileChange(object state)
        {
            try
            {
                try
                {
                    Load();
                    if (OnLoggerConfigChanged != null)
                    {
                        OnLoggerConfigChanged();
                    }
                }
                catch (Exception ex)
                {
                    LoggerManager.GetLog().Log("Logger.LogUtils", "OnConfigChanged", "日志变更发生异常[" + Thread.CurrentThread.ManagedThreadId + "]\r\n" + ex.ToString(), LogLevel.ERROR);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        static void Load()
        {
            using (System.IO.FileStream fs = System.IO.File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Logger.xml")))
            {
                XElement element = XElement.Load(fs);
                Dicts.Clear();
                foreach (XElement item in element.Elements("Logger"))
                {
                    string type = item.Attribute("type").Value;
                    XAttribute maxSizeAt = item.Attribute("maxSize");
                    int maxSize = maxSizeAt == null ? 10 : Convert.ToInt32(maxSizeAt.Value);

                    XAttribute fileCountAt = item.Attribute("fileCount");
                    int fileCount = fileCountAt == null ? 5 : Convert.ToInt32(fileCountAt.Value);

                    XAttribute dirAt = item.Attribute("dir");
                    string dir = dirAt == null ? "log" : dirAt.Value;

                    LogLevel level = LogLevel.DEBUG;
                    XAttribute minLevelAt = item.Attribute("minLevel");

                    XAttribute encodeAt = item.Attribute("encode");
                    string encode = encodeAt == null ? "utf-8" : encodeAt.Value;
                    string minLevel = minLevelAt == null ? "debug" : minLevelAt.Value.Trim().ToLower();
                    switch (minLevel)
                    {
                        case "debug":
                            level = LogLevel.DEBUG;
                            break;
                        case "info":
                            level = LogLevel.INFO;
                            break;
                        case "warn":
                            level = LogLevel.WARN;
                            break;
                        case "error":
                            level = LogLevel.ERROR;
                            break;
                        default:
                            level = LogLevel.DEBUG;
                            break;
                    }
                    LoggerConfig config = new LoggerConfig()
                    {
                        Type = type.ToLower(),
                        MaxSize = maxSize,
                        FileCount = fileCount,
                        Level = level,
                        Dir = dir,
                        Encode = encode
                    };
                    if (item.Element("DEBUG") != null)
                    {
                        var xElement = item.Element("DEBUG");
                        if (xElement != null)
                            config.FileDicts.Add(Logger.LogLevel.DEBUG, xElement.Value);
                    }
                    if (item.Element("INFO") != null)
                    {
                        var xElement = item.Element("INFO");
                        if (xElement != null)
                            config.FileDicts.Add(Logger.LogLevel.INFO, xElement.Value);
                    }
                    if (item.Element("WARN") != null)
                    {
                        var xElement = item.Element("WARN");
                        if (xElement != null)
                            config.FileDicts.Add(Logger.LogLevel.WARN, xElement.Value);
                    }
                    if (item.Element("ERROR") != null)
                    {
                        var xElement = item.Element("ERROR");
                        if (xElement != null)
                            config.FileDicts.Add(Logger.LogLevel.ERROR, xElement.Value);
                    }
                    Dicts.Add(config.Type, config);
                }
            }
        }

        public static LoggerConfig Find(string type)
        {
            if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("type");
            string key = type.ToLower();

            if (Dicts.ContainsKey(key)) return Dicts[key].Clone();
            return null;
        }
    }
    internal class LoggerConfig
    {

        public string Type { get; set; }
        public int MaxSize { get; set; }
        public int FileCount { get; set; }
        public string Dir { get; set; }
        public LogLevel Level { get; set; }
        public string Encode { get; set; }

        public Dictionary<LogLevel, string> FileDicts { get; set; }

        public LoggerConfig()
        {
            FileDicts = new Dictionary<LogLevel, string>();
        }
        public LoggerConfig Clone()
        {
            LoggerConfig config = new LoggerConfig();
            config.Type = this.Type;
            config.MaxSize = this.MaxSize;
            config.FileCount = this.FileCount;
            config.Dir = this.Dir;
            config.Level = this.Level;
            config.Encode = this.Encode;
            foreach (var item in this.FileDicts)
            {
                config.FileDicts.Add(item.Key, item.Value);
            }
            return config;

        }
    }
}
