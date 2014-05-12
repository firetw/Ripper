using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Ripper.TaskDispather
{
    public class WebContext : TaskContext
    {
        public string Url { get; set; }

        public CookieContainer CookieContainer { get; set; }

        public string Dir { get; set; }

        public int Span { get; set; }

        public int Start { get; set; }

        public int End { get; set; }

        public string Flag { get; set; }
         
    }
}
