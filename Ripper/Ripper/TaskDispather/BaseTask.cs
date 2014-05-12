using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.TaskDispather
{
    public abstract class BaseTask : ITask
    {
        #region Fields
        private bool _disposed;
        #endregion


        #region Property
        public string ID
        {
            get;
            set;
        }

        public string JobID
        {
            get;
            set;
        }
        public DateTime StartTime
        {
            get;
            set;
        }

        public Status Status
        {
            get;
            set;
        }
        #endregion


        public TaskContext Context { get; set; }

        #region ITask
        public virtual void DoTask()
        {

        }

        public virtual void Suspend()
        {

        }

        public virtual void Stop()
        {

        }

        public virtual void Restart()
        {

        }

        public virtual bool CanRestart()
        {
            return false;
        }
        #endregion

        public event EventHandler OnStatusCompleted;

        public void Dispose()
        {
            if (_disposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
        }

        public virtual Encoding GetEncoding()
        {
            return Encoding.UTF8;
        }

    }
}
