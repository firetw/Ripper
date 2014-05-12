using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.SimpleCollector
{
    public interface ICollector
    {
        //http://weibo.com/p/1005052093778914/weibo?is_search=0&visible=0&is_tag=0&profile_ftype=1&page=2#feedtop
        //Starting
        //Processing
        //Completed

        string ID { get; set; }

        void Collector();

        event EventHandler<CollectorArgs> CollectorStatusChanged;

    }
    public class CollectorArgs : EventArgs
    {


    }

    public class CollectorContext
    {
        public string Url { get; set; }
        public Status CollectorStatus { get; set; }
    }

    public enum Status
    {
        UnStart = 0,
        Start = 1,
        Processing = 2,
        Completed = 3,
        Failed = 4
    }
}

