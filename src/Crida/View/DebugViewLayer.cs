using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Crida.Asset;
using Crida.Shared.Math;
using Crida.States;
using Crida.View.Primitives;
using HardFuzz.HarfBuzz;
using Veldrid;

namespace Crida.View
{
    public sealed class DebugViewLayer : IViewLayer
    {
        public DebugViewLayer(GraphicsState graphics, SpatialState spatial, AssetFactoryState assets)
        {
            Graphics = graphics;
            Spatial = spatial;
            Assets = assets;
            TextViewLayer = new LightTextViewLayer(graphics, assets, 8, 0, 1024, 1024,
                assets.Get<LightFont>("Fonts.Light.FiraSansItalic"),
                new[]
                {
                    new Feature("kern"), new Feature("liga"), new Feature("clig"), new Feature("onum"),
                    new Feature("tnum")
                },
                24, 24, new RgbaFloat(0, 0, 0, 0.6f), "");
        }

        private GraphicsState Graphics { get; }
        private SpatialState Spatial { get; }
        private AssetFactoryState Assets { get; }

        private LightTextViewLayer TextViewLayer { get; }
        private TimeSpan LastDraw { get; set; } = TimeSpan.Zero;
        private ConcurrentBag<TimeSpan> RecentFrameTimes { get; } = new ConcurrentBag<TimeSpan>();
        private double FrameTime { get; set; } = double.NaN;

        public async Task Draw(Framebuffer target)
        {
            var now = Graphics.Stopwatch.Elapsed;
            RecentFrameTimes.Add(now - LastDraw);
            if (RecentFrameTimes.Sum(x => x.TotalSeconds) > 0.25)
            {
                FrameTime = RecentFrameTimes.Average(x => x.TotalMilliseconds);
                RecentFrameTimes.Clear();
            }

            LastDraw = now;
            var fps = 1000 / FrameTime;
            TextViewLayer.Content =
                $"{now:hh\\:mm\\:ss}\n" +
                $"frame time {FrameTime:F1} ms ({(fps > 240 ? ">240" : $"{fps:F1}")} fps)\n" +
                $"camera focus {Spatial.CameraFocus}\n" +
                $"camera origin {Spatial.CameraFocus + Spatial.CameraEulerXyz.ToTransform() * new Vector3D(0, 0, 1, 0) * Spatial.CameraFocusDistance}\n" +
                $"camera rotation {Spatial.CameraEulerXyz}";
            await TextViewLayer.Draw(target);
        }

        public void Dispose()
        {
            TextViewLayer.Dispose();
        }
    }
}
