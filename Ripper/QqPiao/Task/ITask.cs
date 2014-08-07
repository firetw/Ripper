using QqPiao.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace QqPiao.Task
{

    public interface ITask<T> where T : Context
    {

        void Process(T context);
    }
    public delegate void WaitForHandler(string prompt, int times, bool expliciteQuitNeEvent);
    public delegate void ProcessHandler<T>(T context);
    public delegate void TimeoutHandler<T>(T context);
    public class BaseTask<T> : ITask<T> where T : Context
    {
        TimeoutHandlerPara _para = new TimeoutHandlerPara();
        private bool _isCompleted;
        public event Action<bool> OnTaskCompleted;
        protected static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public T Context
        {
            get;
            set;
        }

        /// <summary>
        /// 是否完成，
        /// 有可能为正常完成，也可能为异常完成
        /// </summary>
        public bool IsCompleted
        {
            get { return _isCompleted; }
            set
            {
                _isCompleted = value;
                if (OnTaskCompleted != null)
                    OnTaskCompleted(_isCompleted);
            }
        }

        public int Timeout { get; set; }

        public event TimeoutHandler<T> OnTaskTimeout;

        public BaseTask()
        {
            Timeout = 5 * 60 * 1000;
        }
        public virtual void Do(T context)
        {
        }

        public void Process(T context)
        {
            Context = context;
            WaitForHandler();
        }
        private void WrapProcess(T context)
        {
            try
            {
                Context.Status = TaskStatus.Execing;
                _para.ThreadToKill = Thread.CurrentThread;
                Do(context);
                Context.Status = TaskStatus.Success;
            }
            catch (Exception ex)
            {
                _para.Content += ex.ToString();
                Context.ExecInfo += "\r\n" + "异常信息" + _para.Content;
                Context.Status = TaskStatus.Failed;
                _log.Error(ex);
            }
            finally
            {
                IsCompleted = true;
            }
        }
        private void WaitForHandler()
        {
            ProcessHandler<T> wfh = new ProcessHandler<T>(WrapProcess);
            IAsyncResult iar = wfh.BeginInvoke(Context, null, null);
            if (iar.AsyncWaitHandle.WaitOne(Timeout))
            {
                wfh.EndInvoke(iar);
            }
            else
            {
                try
                {
                    _para.IsCanContinue = false;
                    Context.Status = TaskStatus.Timeout;
                    wfh = null;
                    if (_para.ThreadToKill != null)
                        _para.ThreadToKill.Abort();
                }
                catch (ThreadAbortException tae)
                {

                }
                TimeoutHandler();
            }
        }
        private void TimeoutHandler()
        {
            if (_para == null) return;
            if (_para.ExpliciteQuitEvent)
            {
                IsCompleted = true;
                HandlerTimeout();
                OnTaskTimeout(Context);
            }
        }

        protected virtual void HandlerTimeout()
        {

        }




        private class TimeoutHandlerPara
        {
            public string Content;
            public bool IsCanContinue;
            public bool ExpliciteQuitEvent;
            public Thread ThreadToKill;

            public override string ToString()
            {
                return Content;
            }

            internal void Reset()
            {
                Content = string.Empty;
                ExpliciteQuitEvent = false;
                IsCanContinue = true;
            }
        }

    }

    public class Context : INotifyPropertyChanged
    {
        public Thread CurrentTheread
        {
            get
            {
                return Thread.CurrentThread;
            }
        }
        public TaskStatus Status { get; set; }

        public String ExecInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;


        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                System.Threading.Interlocked.Exchange<string>(ref mChangedPropertyName, propertyName);
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private string mChangedPropertyName = string.Empty;
        /// <summary>
        /// 当前是那个属性值改变
        /// </summary>
        public string ChangedPropertyName
        {

            set { mChangedPropertyName = value; }
            get { return mChangedPropertyName; }
        }
    }
    public enum TaskStatus
    {
        /// <summary>
        /// 未知异常
        /// </summary>
        WaitForBegin = 0,
        Execing = 1,
        Success = 2,
        Timeout = 3,
        Failed = 4

    }
}
