using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.TaskDispather
{
   public  class BaseJob:IJob
    {
        public Queue<ITask> Tasks
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

        public string JobID
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

        public DateTime StartTime
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

        public Status Status
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

        public void DoTask()
        {
            throw new NotImplementedException();
        }

        public void Suspend()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Restart()
        {
            throw new NotImplementedException();
        }

        public bool CanRestart()
        {
            throw new NotImplementedException();
        }

        public event EventHandler OnStatusCompleted;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
