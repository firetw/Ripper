using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ripper.TaskDispather
{
    public interface IHandler : IDisposable
    {
        string ID { get; set; }

        string JobID { get; set; }

        DateTime StartTime { get; set; }

        Status Status { get; set; }

        void DoTask();

        void Suspend();

        void Stop();

        void Restart();

        bool CanRestart();

        event EventHandler OnStatusCompleted;
    }

    public interface ITask : IHandler
    { 

    }
    

    /// <summary>
    /// Task->Job->TaskManager
    /// </summary>
    public interface IJob : IHandler
    {
        Queue<ITask> Tasks { get; set; }
    }
    
    public abstract class TaskContext
    {
    }

    public enum Status
    {
        /// <summary>
        /// 未执行
        /// </summary>
        UnStart = 0,
        /// <summary>
        /// 正在执行
        /// </summary>
        Executing = 1,
        /// <summary>
        /// 已执行
        /// </summary>
        Executed = 2,
        /// <summary>
        /// 暂停
        /// </summary>
        Pause = 3,
        /// <summary>
        /// 已中断
        /// </summary>
        Suspend = 4,
        /// <summary>
        /// 已失败
        /// </summary>
        Fail = 5,
        /// <summary>
        ///暂停后启动 
        /// </summary>
        Restart = 6,
        /// <summary>
        /// 在执行过程中出现失败，等待用户手工参与重新执行
        /// </summary>
        SuspendWithFail = 7
    }
}
