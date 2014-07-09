using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.View.Model
{
    public class Options
    {
        [Option('p', "port", HelpText = "端口号。", Required = false)]
        public int Port { get; set; }

        [Option('f', "file", HelpText = "号码文件。", Required = false)]
        public string InputFile { get; set; }

        [Option('c', "command", HelpText = "命令。", Required = false)]
        public string Command { get; set; }
        [Option('m', "master", HelpText = "是否为主程序。", DefaultValue = 1, Required = false)]
        public int Master { get; set; }

        [HelpOption(HelpText = "示例")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }
}
