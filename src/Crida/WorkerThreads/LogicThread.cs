using System.Threading;
using System.Threading.Tasks;
using Crida.Shared.Math;
using Crida.States;

namespace Crida.WorkerThreads
{
    public class LogicThread : IWorkerThread
    {
        public LogicThread(WindowState window, SpatialState spatial)
        {
            Window = window;
            Spatial = spatial;
        }

        private WindowState Window { get; }
        private SpatialState Spatial { get; }

        public async Task Run()
        {
            var lastTime = Window.Stopwatch.Elapsed;
            while (!Window.ShouldQuit)
            {
                var now = Window.Stopwatch.Elapsed;
                var elapsed = now - lastTime;
                Spatial.CameraEulerXyz +=
                    new EulerXyz(Angle.Zero, Angle.Zero, Angle.FromDegrees(90 * elapsed.TotalSeconds));
                lastTime = now;
            }
        }
    }
}
