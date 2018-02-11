using System;

namespace WebBackProcess
{
    public delegate void AfterExecCallback(ExecDTO data);

    public interface IExecutor : IDisposable
    {
        void ExecToQueue(string data);

        AfterExecCallback AfterExec { get; set; }

    }
}