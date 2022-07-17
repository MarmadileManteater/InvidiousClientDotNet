using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarmadileManteater.InvidiousClient.Enums;
using MarmadileManteater.InvidiousClient.Interfaces;

namespace MarmadileManteater.InvidiousClient.Objects
{
    public class ConsoleLogger : ILogger
    {
        public Task Log(string message, LogLevel level, Exception? exception)
        {
            ConsoleColor previousBackground = Console.BackgroundColor;
            ConsoleColor previousForeground = Console.ForegroundColor;
            switch (level)
            {
                case LogLevel.Error:
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Information:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Trace:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
            Console.WriteLine(message);
            if (exception != null)
            {
                Console.WriteLine(exception);
            }
            Console.BackgroundColor = previousBackground;
            Console.ForegroundColor = previousForeground;
            return Task.CompletedTask;
        }

        public async Task LogError(string message, Exception exception)
        {
            await Log(message, LogLevel.Error, exception);
        }

        public void LogErrorSync(string message, Exception exception)
        {
            LogError(message, exception).Wait();
        }

        public async Task LogInfo(string message)
        {
            await Log(message, LogLevel.Information, null);
        }

        public void LogInfoSync(string message)
        {
            LogInfo(message).Wait();
        }

        public void LogSync(string message, LogLevel level, Exception? exception)
        {
            Log(message, level, exception).Wait();
        }

        public async Task LogWarn(string message, Exception? exception)
        {
            await Log(message, LogLevel.Warning, exception);
        }

        public void LogWarnSync(string message, Exception? exception)
        {
            LogWarn(message, exception).Wait();
        }

        public async Task Trace(string message)
        {
            await Log(message, LogLevel.Trace, null);
        }

        public void TraceSync(string message)
        {
            Trace(message).Wait();
        }
        public IProgress<double> GetProgressInterface()
        {
            return new ConsoleProgressBar();
        }
    }
}
