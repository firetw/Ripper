using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Logger
{
    public class LoggerManager
    {
        public static Dictionary<LogType, ILogger> _logDicts = new Dictionary<LogType, ILogger>();
        static Dictionary<string, ILogger> _neDicts = new Dictionary<string, ILogger>();
        static readonly object lockObject = new object();


        static LoggerManager()
        {
            LogUtils.OnLoggerConfigChanged += new Action(LogUtils_OnLoggerConfigChanged);
        }
        static void LogUtils_OnLoggerConfigChanged()
        {
            lock (lockObject)
            {
                GetLog().Log("Logger.LoggerManager", "LogUtils_OnLoggerConfigChanged", "日志配置发生变化[" + Thread.CurrentThread.Name + "] [" + Thread.CurrentThread.ManagedThreadId + "]", LogLevel.INFO);
                for (int i = 0; i < _logDicts.Count(); i++)
                {
                    KeyValuePair<LogType, ILogger> item = _logDicts.ElementAt(i);
                    FileLogger logger = item.Value as FileLogger;
                    if (logger == null) continue;
                    LoggerConfig config = LogUtils.Find(item.Key.ToString());
                    if (config == null) continue;
                    logger.ChanagerConfig(config.Clone());
                }
                for (int i = 0; i < _neDicts.Count(); i++)
                {
                    KeyValuePair<string, ILogger> item = _neDicts.ElementAt(i);
                    FileLogger logger = item.Value as FileLogger;
                    if (logger == null) continue;
                    LoggerConfig config = LogUtils.Find(LogType.SubTask.ToString());
                    if (config == null) continue;
                    config.FileDicts.Clear();
                    string logFile = item.Key + ".log";
                    string errorFile = item.Key + "_error.log";
                    config.FileDicts.Add(LogLevel.DEBUG, logFile);
                    config.FileDicts.Add(LogLevel.INFO, logFile);
                    config.FileDicts.Add(LogLevel.WARN, errorFile);
                    config.FileDicts.Add(LogLevel.ERROR, errorFile);
                    logger.ChanagerConfig(config.Clone());
                }
            }
        }

        public static ILogger GetLog(string logName, LogType type = LogType.Common)
        {
            lock (lockObject)
            {
                ILogger logger;
                LoggerConfig config = LogUtils.Find(type.ToString());
                if (config == null) throw new Exception(string.Format("请检查配置,Type:[{0}]是否存在", type));
                switch (type)
                {
                    case LogType.SubTask:
                        if (_neDicts.ContainsKey(logName))
                        {
                            logger = _neDicts[logName];
                        }
                        else
                        {
                            logger = new FileLogger(config);
                            config.FileDicts.Clear();
                            string logFile = logName + ".log";
                            string errorFile = logName + "_error.log";
                            config.FileDicts.Add(LogLevel.DEBUG, logFile);
                            config.FileDicts.Add(LogLevel.INFO, logFile);
                            config.FileDicts.Add(LogLevel.WARN, errorFile);
                            config.FileDicts.Add(LogLevel.ERROR, errorFile);
                            _neDicts.Add(logName, logger);
                        }
                        break;
                    default:
                        if (_logDicts.ContainsKey(type))
                        {
                            logger = _logDicts[type];
                        }
                        else
                        {
                            logger = new FileLogger(config);
                            _logDicts.Add(type, logger);
                        }
                        break;
                }
                return logger;
            }
        }

        public static ILogger GetLog(LogType type = LogType.Common)
        {
            if (type == LogType.SubTask) throw new Exception("SubTask 必须指定文件名");
            return GetLog(string.Empty, type);
        }

        public static void RemoveLogger(string logName)
        {
            if (_neDicts.ContainsKey(logName))
            {
                _neDicts.Remove(logName);
            }
        }
    }
    public enum LogType
    {
        Common = 0,
        Task = 1,
        SubTask = 2,
        ScanTask = 3,
        DbWriter = 4,
        TaskControl = 5
    }
}
