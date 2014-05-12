using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.SimpleCollector
{
    public class WebCollector : ICollector
    {
        public event EventHandler<CollectorArgs> CollectorStatusChanged;

        public void Collector()
        {
           
        }

        public string ID
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
