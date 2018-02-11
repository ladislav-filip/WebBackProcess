using System;

namespace WebBackProcess
{
    public interface IProtocol : IDisposable
    {
        void AddToQueue(string data);
    }
}