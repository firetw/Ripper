using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeiBoGrab.Core
{
    public interface ITask<T> where T:Context
    {
        void Process(T context);
    }
}
