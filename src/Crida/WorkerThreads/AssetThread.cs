using System.Threading;
using System.Threading.Tasks;
using Crida.States;
using NLog;

namespace Crida.WorkerThreads
{
    public class AssetThread : IWorkerThread
    {
        public AssetThread(WindowState window)
        {
            Window = window;
        }

        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();
        private WindowState Window { get; }

        public async Task Run()
        {
            while (!Window.ShouldQuit)
                // todo preload things, purge unused things, etc etc
                Thread.Sleep(1);
        }
    }
}
