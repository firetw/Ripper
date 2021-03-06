﻿using SamSung;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Tools;
using Uuwise;

namespace Ripper.View.Model
{
    public class WebTask : ITask<RegisterContext>
    {
        RegisterContext _context = null;
        TimeoutHandlerPara _para = new TimeoutHandlerPara();
        private bool _isCompleted;
        public event Action<bool> OnTaskCompleted;

        public RegisterContext Context
        {
            get { return _context; }
            set { _context = value; }
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

        public event TimeoutHandler<RegisterContext> OnTaskTimeout;

        public WebTask()
        {
            Timeout = 200 * 60 * 1000;
        }
        public virtual void Do(RegisterContext context)
        {
        }

        public void Process(RegisterContext context)
        {
            _context = context;
            WaitForHandler();
        }
        private void WrapProcess(RegisterContext context)
        {
            try
            {
                _context.Status = TaskStatus.Execing;
                _para.ThreadToKill = Thread.CurrentThread;
                Do(context);
                //_context.Status = TaskStatus.Success;
            }
            catch (Exception ex)
            {
                _para.Content += ex.ToString();
                _context.ExecInfo += "\r\n" + "异常信息" + _para.Content;
                _context.Status = TaskStatus.Failed;

            }
        }
        private void WaitForHandler()
        {
            ProcessHandler<RegisterContext> wfh = new ProcessHandler<RegisterContext>(WrapProcess);
            IAsyncResult iar = wfh.BeginInvoke(_context, null, null);
            if (iar.AsyncWaitHandle.WaitOne(Timeout))
            {
                wfh.EndInvoke(iar);
            }
            else
            {
                try
                {
                    _para.IsCanContinue = false;
                    _context.Status = TaskStatus.Timeout;
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
                OnTaskTimeout(_context);
            }
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


    public class XjWTTask : ITask<Entity>
    {
        Entity _context = null;
        TimeoutHandlerPara _para = new TimeoutHandlerPara();
        private bool _isCompleted;
        public event Action<bool> OnTaskCompleted;
        static readonly log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public Entity Context
        {
            get { return _context; }
            set { _context = value; }
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

        public event TimeoutHandler<Entity> OnTaskTimeout;

        public XjWTTask()
        {
            Timeout = 5 * 60 * 1000;
        }
        public virtual void Do(Entity context)
        {
        }

        public void Process(Entity context)
        {
            _context = context;
            WaitForHandler();
        }
        private void WrapProcess(Entity context)
        {
            try
            {
                _context.Status = TaskStatus.Execing;
                _para.ThreadToKill = Thread.CurrentThread;
                Do(context);
                //_context.Status = TaskStatus.Success;
            }
            catch (Exception ex)
            {
                _para.Content += ex.ToString();
                _context.ExecInfo += "\r\n" + "异常信息" + _para.Content;
                _context.Status = TaskStatus.Failed;
                _log.Error(ex);
            }
            finally
            {
                IsCompleted = true;
            }
        }
        private void WaitForHandler()
        {
            ProcessHandler<Entity> wfh = new ProcessHandler<Entity>(WrapProcess);
            IAsyncResult iar = wfh.BeginInvoke(_context, null, null);
            if (iar.AsyncWaitHandle.WaitOne(Timeout))
            {
                wfh.EndInvoke(iar);
            }
            else
            {
                try
                {
                    _para.IsCanContinue = false;
                    _context.Status = TaskStatus.Timeout;
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
                OnTaskTimeout(_context);
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

    
    public class RegisterContext : Context
    {

        private string _id;
        private string _execInfo;

        public RegisterContext(string line)
        {
            string[] array = Utils.Split(line);

            if (array == null) return;

            if (array.Length == 3)
            {
                string name = array[0];
                if (name.Length >= 3)
                {
                    LastName = name.Substring(0, 2);
                    if (FuXingRep.Map.Contains(LastName))
                    {
                        FirstName = name.Substring(2, name.Length - 2);
                    }
                    else
                    {
                        LastName = name.Substring(0, 1);
                        FirstName = name.Substring(1, name.Length - 1);
                    }
                }
                else
                {
                    LastName = name.Substring(0, 1);
                    FirstName = name.Substring(1, name.Length - 1);
                }
                Tel = array[1];
                Id = array[2];
            }
            else if (array.Length == 4)
            {
                LastName = array[0];
                FirstName = array[1];
                Tel = array[2];
                Id = array[3];
            }
        }

        public bool IsValidate()
        {
            return !string.IsNullOrEmpty(LastName) && !string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(Tel) && !string.IsNullOrEmpty(Id);
        }

        public event Action<string> OnExecInfoChanged;

        /// <summary>
        /// 名
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// 姓
        /// </summary>
        public string LastName { get; set; }

        public int Seq { get; set; }

        public string FullName
        {
            get { return LastName + FirstName; }
        }

        public string Tel { get; set; }
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;

                Match match = Regex.Match(_id, @"\d{6}(\d{4})(\d{2})(\d{2})");
                if (match.Success)
                {
                    Year = match.Groups[1].Value;
                    Month = match.Groups[2].Value;
                    Day = match.Groups[3].Value;

                    Year = Year.Length < 2 ? "0" + Year : Year;
                    Month = Month.Length < 2 ? "0" + Month : Month;
                    Day = Day.Length < 2 ? "0" + Day : Day;

                    Birthday = Year + Month + Day;
                }
            }
        }

        public string GenderTypeCode
        {
            get
            {
                int num = Convert.ToInt32(Id.Substring(Id.Length - 2, 1));

                if (num % 2 == 0)
                    return "F";
                else
                    return "M";
            }
        }

        public string Year { get; private set; }
        public string Month { get; private set; }
        public string Day { get; private set; }
        public string Birthday { get; private set; }


        public string Pwd { get; set; }
        private string _registerInfo;
        public string RegisterInfo
        {
            get { return _registerInfo; }
            set
            {
                _registerInfo = value;
                OnPropertyChanged("RegisterInfo");
            }
        }


        public bool IsNewBroser { get; set; }

        public string ExecInfo
        {
            get { return _execInfo; }
            set
            {
                _execInfo = value;
                if (OnExecInfoChanged != null)
                    OnExecInfoChanged(_execInfo);
            }
        }

        public UUVerifyImp Imp { get; set; }


        public override string ToString()
        {
            return string.Format("姓名:{0},手机号:{1},身份证号:{2}", LastName + FirstName, Tel, Id);
        }
        public string Format()
        {
            return string.Format("{0},{1},{2}", LastName + FirstName, Tel, Id);
        }
    }
}
