using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Logger;
using Ripper.Config;

namespace Ripper.TaskDispather
{
    /// <summary>
    /// 提供一个执行任务的管理器
    /// Fifo
    /// 
    /// </summary>
    public class TaskManager : IDisposable
    {
        #region Fields
        Timer tickTimer;
        Timer maintainTimer;
        private object lockObject = new object();
        private ILogger logger;
        private string ClassName;

        private Queue<ITask> TaskCache = new Queue<ITask>();
        /// <summary>
        /// 正在执行的Task
        /// </summary>
        private List<ITask> _taskDoing = new List<ITask>();
        private List<ITask> _SuspendTasks = new List<ITask>();
        private static readonly TaskManager instance = new TaskManager();
        public static TaskManager Instance
        {
            get { return instance; }
        }

        #endregion

        #region Property
        public int MaxTaskCount { get; set; }
        public int Period
        {
            get;
            private set;
        }
        public int CurrentTaskCount { get; private set; }
        public ManagerState ManagerState { get; set; }
        public Dictionary<String, IJob> JobsMap = new Dictionary<string, IJob>();
        internal event Action<IList<string>> OnSuspendTask;
        #endregion

        #region Constructor
        private TaskManager()
        {
            ClassName = typeof(TaskManager).FullName;
            Period = 500;
            MaxTaskCount = ConfigUtils.MaxTaskCount;
            ManagerState = ManagerState.Waiting;
            tickTimer = new Timer(new TimerCallback(DispatherTask), null, -1, Period);
            int Span = 10 * 3600 * 1000;
            maintainTimer = new Timer(new TimerCallback(Maintain), null, Span, Span);
            logger = LoggerManager.GetLog();
        }
        #endregion

        #region Public Methods

        public void DispatherTask(object state)
        {
            string function = "DispatherTask";
            lock (lockObject)
            {
                while (TaskCache.Count > 0 && CurrentTaskCount < MaxTaskCount)
                {
                    ITask task = TaskCache.Dequeue();
                    CurrentTaskCount++;
                    Thread thread = new Thread(new ParameterizedThreadStart(StartTask));
                    thread.Priority = ThreadPriority.Normal;
                    thread.Start(task);

                    ManagerState = ManagerState.Running;
                }
                if (CurrentTaskCount == MaxTaskCount)
                {
                    ManagerState = ManagerState.Full;
                    tickTimer.Change(-1, Period);
                    logger.Log(ClassName, function, ManagerState.ToString(), LogLevel.INFO);
                }
                else if (TaskCache.Count == 0)
                {
                    ManagerState = ManagerState.Waiting;
                    tickTimer.Change(-1, Period);
                    logger.Log(ClassName, function, ManagerState.ToString(), LogLevel.INFO);
                }
            }
        }
        /// <summary>
        /// 设置周期,单位毫秒
        /// </summary>
        /// <param name="period"></param>
        public void SetPeriod(int period)
        {
            Period = period;
            tickTimer.Change(100, Period);
        }

        public string Report()
        {
            return string.Format(@"当前执行任务数:{0}
        任务池总数:{1}
        最大任务数:{2}", CurrentTaskCount, TaskCache.Count, MaxTaskCount);
        }

        public void AddTask(IList<ITask> tasks)
        {
            string function = "AddTask";
            lock (lockObject)
            {
                logger.Log(ClassName, function, Report(), LogLevel.INFO);
                foreach (var item in tasks)
                {
                    TaskCache.Enqueue(item);
                }
                if (ManagerState == ManagerState.Waiting)//说明任务执行完毕
                {
                    tickTimer.Change(100, Period);
                }
            }
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="tasksIDs"></param>
        internal void SuspendTask(IList<string> tasksIDs)
        {
            lock (TaskCache)
            {
                var query = from task in TaskCache
                            join id in tasksIDs on task.ID equals id
                            select task;
                List<ITask> deleteList = query.ToList();
                if (deleteList.Count > 0)
                {
                    Queue<ITask> tmpQueue = new Queue<ITask>();
                    foreach (var task in TaskCache)
                    {
                        if (!deleteList.Contains(task))
                        {
                            tmpQueue.Enqueue(task);
                        }
                    }
                    TaskCache.Clear();
                    foreach (var item in tmpQueue)
                    {
                        TaskCache.Enqueue(item);
                    }
                }
                foreach (var item in _taskDoing)
                {
                    item.Suspend();
                }
                //通知正在执行的任务停止执行
                if (OnSuspendTask != null)
                {
                    OnSuspendTask(tasksIDs);
                }
            }
        }
        #endregion

        #region Ctl Methods
        /// <summary>
        /// 停止任务
        /// </summary>
        /// <param name="JobID"></param>
        public void StopTask(string jobID)
        {
            string function = "StopTask";
            lock (TaskCache)
            {
                int taskCount = TaskCache.Count(item => item.JobID == jobID);
                if (taskCount > 0)
                {
                    Queue<ITask> tmpQueue = new Queue<ITask>();
                    while (TaskCache.Count > 0)
                    {
                        ITask task = TaskCache.Dequeue();
                        task.Suspend();
                        if (task.JobID != jobID)
                        {
                            tmpQueue.Enqueue(task);
                            logger.Log(ClassName, function, string.Format("Job:[{0}] Task:[{1}]从任务队列中移除", task.JobID, task.ID), LogLevel.INFO);
                        }
                    }
                    TaskCache.Clear();
                    foreach (var item in tmpQueue)
                    {
                        TaskCache.Enqueue(item);
                    }
                }
                for (int i = 0; i < _taskDoing.Count; i++)
                {
                    ITask te = _taskDoing[i];
                    te.Stop();
                    logger.Log(ClassName, function, string.Format("Job:[{0}] Task:[{1}]停止正在执行中的任务", te.JobID, te.ID), LogLevel.INFO);
                }
                if (JobsMap.ContainsKey(jobID))
                {
                    JobsMap[jobID].Stop();
                    JobsMap.Remove(jobID);
                }
            }
        }

        public void Suspend(string jobID)
        {
            string function = "Suspend";
            lock (TaskCache)
            {
                foreach (var task in TaskCache)
                {
                    if (task.JobID == jobID)
                    {
                        task.Suspend();
                        _SuspendTasks.Add(task);
                        logger.Log(ClassName, function, string.Format("Job:[{0}] Task:[{1}]任务暂停排队", task.JobID, task.ID), LogLevel.INFO);
                    }
                }
                foreach (var task in _taskDoing)
                {
                    task.Suspend();
                    logger.Log(ClassName, function, string.Format("Job:[{0}] Task:[{1}]暂停正在执行中的任务", task.JobID, task.ID), LogLevel.INFO);
                }
                if (JobsMap.ContainsKey(jobID))
                {
                    JobsMap[jobID].Suspend();
                }
            }
        }
        public void ReStart(string JobID)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(ReStartTask));
            thread.Start(JobID);

        }
        private void ReStartTask(object state)
        {
            string function = "ReStart";
            string JobID = state.ToString();
            if (JobsMap.ContainsKey(JobID))
            {
                IJob job = JobsMap[JobID];
                if (job.CanRestart())
                    job.Suspend();
            }
            else
            {
                return;
            }
            lock (TaskCache)
            {
                List<ITask> tmpList = new List<ITask>();
                foreach (var task in _SuspendTasks)
                {
                    if (task.JobID == JobID)
                    {
                        tmpList.Add(task);
                    }
                }
                foreach (var task in tmpList)
                {
                    _SuspendTasks.Remove(task);
                }
                var addTasks = TaskCache.Where(item => tmpList.Count(t => t.JobID == item.JobID) < 1);
                foreach (var task in addTasks)
                {
                    TaskCache.Enqueue(task);
                    logger.Log(ClassName, function, string.Format("Job:[{0}] Task:[{1}]任务重新进入队列", task.JobID, task.ID), LogLevel.INFO);
                }
                for (int i = 0; i < _taskDoing.Count; i++)
                {
                    ITask task = _taskDoing[i];
                    if (task.CanRestart())
                    {
                        task.Restart();
                        logger.Log(ClassName, function, string.Format("Job:[{0}] Task:[{1}]重新启动暂停中的任务", task.JobID, task.ID), LogLevel.INFO);
                    }
                    else
                    {
                        logger.Log(ClassName, function, string.Format("Job:[{0}] Task:[{1}]", task.JobID, task.ID), LogLevel.WARN);
                    }

                }
            }
        }

        #endregion

        #region Private Methods
        private void Maintain(object state)
        {
            lock (_taskDoing)
            {
                if (_taskDoing.Count > 10000)
                {
                    _taskDoing.RemoveRange(10000, _SuspendTasks.Count - 10000);
                }
            }
            lock (_SuspendTasks)
            {
                if (_SuspendTasks.Count > 10000)
                {
                    _SuspendTasks.RemoveRange(10000, _SuspendTasks.Count - 10000);
                }
            }
        }
        private void StartTask(object state)
        {
            string function = "StartTask";
            DateTime beginTime = DateTime.Now;
            ITask task = state as ITask;
            if (task == null) return;
            try
            {
                task.OnStatusCompleted += new EventHandler(OnStatusCompletedHandler);
                task.DoTask();
            }
            catch (Exception ex)
            {
                logger.Log(ClassName, function, ex.ToString(), LogLevel.ERROR);
            }
            lock (lockObject)
            {
                if (ManagerState == ManagerState.Full || ManagerState == ManagerState.Waiting)
                {
                    tickTimer.Change(100, Period);
                }
            }
        }

        void OnStatusCompletedHandler(object sender, EventArgs args)
        {
            string function = "OnStatusCompletedHandler";
            IHandler handler = sender as IHandler;
            if (handler.Status != Status.Executed && handler.Status != Status.Fail)
            {
                return;
            }
            lock (sender)
            {
                ITask task = sender as ITask;
                if (task == null) return;
                CurrentTaskCount--;//释放了一个锁
                ITask te = null;
                try
                {
                    foreach (var item in _taskDoing)
                    {
                        if (item == task)
                        {
                            te = item;
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Log(ClassName, function, ex.ToString(), LogLevel.INFO);
                }

                if (te != null)
                {
                    te.Dispose();
                    _taskDoing.Remove(te);
                }
                logger.Log(ClassName, function, string.Format("{0}\r\n\t执行耗时:[{1}]秒", task, (DateTime.Now - task.StartTime).TotalSeconds), LogLevel.INFO);
            }
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            try
            {
                tickTimer.Dispose();
            }
            catch
            {
            }
        }
        #endregion
    }
    public enum ManagerState
    {
        Waiting = 0,//任务池为空,等待执行任务
        Running = 1,//正在分配任务,任务没有达到最大上限
        Full = 2,//正在执行,任务已经达到上限
    }
}
