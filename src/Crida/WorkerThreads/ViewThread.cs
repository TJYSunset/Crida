using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Crida.States;
using Crida.Utilities.Veldrid;
using Crida.View;
using Crida.View.Primitives;
using Ninject;
using NLog;
using Veldrid;

namespace Crida.WorkerThreads
{
    public sealed class ViewThread : IWorkerThread
    {
        public ViewThread(WindowState window, GraphicsState graphics, SpatialState spatial, AssetFactoryState assets)
        {
            Window = window;
            Graphics = graphics;
            Spatial = spatial;
            Assets = assets;
        }

        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();
        private WindowState Window { get; }
        private GraphicsState Graphics { get; }
        private SpatialState Spatial { get; }
        private AssetFactoryState Assets { get; }

        public async Task Run()
        {
            var rf = Graphics.Device.ResourceFactory;
            var ninject = new StandardKernel(new FluentNinjectModule(_ => _
                .BindExisting(Graphics)
                .BindExisting(Assets)
                .BindExisting(Spatial)
            ));
            var layers = new IViewLayer[]
            {
                // order is bottom to top!
                ninject.Get<SpatialViewLayer>(),
                ninject.Get<DebugViewLayer>()
            }.ToImmutableArray();
            using var commands = new Commands(Graphics);
            using var intermediate = new IntermediateFramebuffer(Graphics, Assets, Blend.Override, DepthTest.Off,
                Graphics.Device.SwapchainFramebuffer.OutputDescription);
            while (!Window.ShouldQuit)
            {
                if (Graphics.FrameReady) continue; // main thread hadn't sent last frame to screen yet
                try
                {
                    intermediate.Validate();

                    // ReSharper disable AccessToDisposedClosure
                    commands.Submit(x =>
                    {
                        x.SetFramebuffer(intermediate);
                        x.ClearColorTarget(0, RgbaFloat.White);
                        x.ClearDepthStencil(1);
                    });

                    foreach (var layer in layers) await layer.Draw(intermediate);
                    await intermediate.Draw(Graphics.Device.SwapchainFramebuffer);

                    Graphics.FrameReady = true;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error rendering frame.");
                }
            }

            foreach (var viewLayer in layers) viewLayer.Dispose();
        }
    }
}
