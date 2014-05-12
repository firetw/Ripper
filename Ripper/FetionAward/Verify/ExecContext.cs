using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiBoGrab.Verify
{
    public class ExecContext
    {
        public bool Status { get; set; }


        public string Msg { get; set; }

        public string ClassName { get; set; }
        public string FunctionName { get; set; }
    }
}
