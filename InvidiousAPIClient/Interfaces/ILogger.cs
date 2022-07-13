using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Enums;

namespace MarmadileManteater.InvidiousClient.Interfaces
{
    public interface ILogger
    {
        Task Log(string message, LogLevel level, Exception? exception);
        void LogSync(string message, LogLevel level, Exception? exception);
        Task Trace(string message);
        void TraceSync(string message);
        Task LogInfo(string message);
        void LogInfoSync(string message);
        Task LogWarn(string message, Exception? exception);
        void LogWarnSync(string message, Exception? exception);
        Task LogError(string message, Exception exception);
        void LogErrorSync(string message, Exception exception);
        IProgress<double> GetProgressInterface();
    }
}
