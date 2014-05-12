using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Logger
{

    public interface ILogger : IDisposable
    {
        void Log(string content);
        void Log(string content, LogLevel level);
        void Log(string className, string function, string content);
        void Log(string className, string function, string content, LogLevel level);
    }

    public enum LogLevel
    {
        DEBUG = 0,
        INFO = 1,
        WARN = 2,
        ERROR = 3
    }

}
