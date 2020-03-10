using System.Linq;
using System.Threading.Tasks;
using Crida.States;
using Crida.WorkerThreads;
using Ninject;
using NLog;
using static SDL2.SDL;

namespace Crida
{
    public sealed class MainThread
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public async Task Run(IKernel ninject)
        {
            var window = ninject.Get<WindowState>();
            var graphics = ninject.Get<GraphicsState>();
            if (!graphics.Device.Features.ShaderFloat64)
            {
                Log.Fatal("gpu doesn't support shaderFloat64");
                return;
            }

            var workers = new IWorkerThread[]
            {
                ninject.Get<LogicThread>(),
                ninject.Get<ViewThread>()
            };
            var workerTasks = workers.Select(x => Task.Run(x.Run)).ToArray();

            var (lastW, lastH) = (1280, 720);
            while (!window.ShouldQuit)
            {
                if (SDL_PollEvent(out var e) == 1)
                    switch (e.type)
                    {
                        case SDL_EventType.SDL_QUIT:
                            window.ShouldQuit = true;
                            continue;
                    }

                if (graphics.FrameReady)
                {
                    SDL_GetWindowSize(window.Handle, out var w, out var h);
                    if (w != lastW || h != lastH)
                    {
                        graphics.Device.ResizeMainWindow((uint) w, (uint) h);
                        lastW = w;
                        lastH = h;
                        Log.Debug("window resized");
                    }

                    graphics.Device.SwapBuffers();
                    graphics.FrameReady = false;
                }
            }

            await Task.WhenAll(workerTasks);

            // todo dispose ninject

            Log.Info("Window quit");
        }
    }
}
