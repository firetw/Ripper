using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Logger
{
    public class ConsoleLogger : ILogger
    {

        string format = "[{0}] CLASS:{1},FUNCTION:{2},LEVEL:{3}\r\n\t{4}";
        string fileName = "F:/PerlTest/bom.log";
        public void Log(string content)
        {
            Log(content, LogLevel.ERROR);
        }

        public void Log(string content, LogLevel level)
        {
            Log(null, null, content, level);
        }

        public void Log(string className, string function, string content)
        {
            Log(className, function, content, LogLevel.ERROR);
        }

        public void Log(string className, string function, string content, LogLevel level)
        {
            string result = string.Format(format, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), string.IsNullOrEmpty(className) ? string.Empty : className, string.IsNullOrEmpty(function) ? string.Empty : function, level.ToString(), string.IsNullOrEmpty(content) ? string.Empty : content);
            Console.WriteLine(result);
            using (StreamWriter writer = File.AppendText(fileName))
            {
                writer.WriteLine(result);
            }
        }

        public void Dispose()
        {

        }
    }

}
