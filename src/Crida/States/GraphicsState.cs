using System;
using System.Collections.Immutable;
using System.Diagnostics;
using Crida.Exceptions;
using Crida.Utilities.Veldrid;
using NLog;
using Veldrid;
using Veldrid.Vk;
using static SDL2.SDL;

namespace Crida.States
{
    public class GraphicsState : IState
    {
        public volatile bool FrameReady = false;

        public GraphicsState(WindowState window)
        {
            Window = window;
            Stopwatch = window.Stopwatch;

            var syswm = new SDL_SysWMinfo();
            if (SDL_GetWindowWMInfo(window.Handle, ref syswm) == SDL_bool.SDL_FALSE)
                throw new SdlException($"Could not get window wminfo: {SDL_GetError()}");

            SDL_GetWindowSize(window.Handle, out var width, out var height);
            Device = GraphicsDevice.CreateVulkan(
                new GraphicsDeviceOptions(true, null, false, ResourceBindingModel.Default,
                    true, false, false),
                VkSurfaceSource.CreateWin32(syswm.info.win.hinstance, syswm.info.win.window),
                (uint) width, (uint) height);
            Log.Debug("main swapchain color format is {format}",
                Device.MainSwapchain.Framebuffer.ColorTargets[0].Target.Format);

            SingleRectIndexBuffer = new ResourcelessArrayBuffer<uint>(this, BufferUsage.IndexBuffer,
                new uint[] {0, 2, 3, 0, 3, 1}.ToImmutableArray());

            RandomnessTexture = new RandomnessTexture(this);
        }

        private static ILogger Log { get; } = LogManager.GetCurrentClassLogger();

        private WindowState Window { get; }
        public GraphicsDevice Device { get; }
        public Stopwatch Stopwatch { get; }

        /// <summary>
        ///     <code>0 --- 2</code>
        ///     <code>|  \  |</code>
        ///     <code>1 --- 3</code>
        /// </summary>
        public ResourcelessArrayBuffer<uint> SingleRectIndexBuffer { get; }

        public RandomnessTexture RandomnessTexture { get; }
        private TimeSpan RandomnessFrameRate { get; } = TimeSpan.FromSeconds(1d / 8);

        public float RandomnessSeed =>
            (Stopwatch.Elapsed.Ticks / RandomnessFrameRate.Ticks).GetHashCode() % 10000 / 10000f;

        public OutputDescription IntermediateOutputDescription { get; } = new OutputDescription(
            new OutputAttachmentDescription(PixelFormat.D32_Float_S8_UInt),
            new OutputAttachmentDescription(PixelFormat.R32_G32_B32_A32_Float));


        public void Dispose()
        {
            SingleRectIndexBuffer.Dispose();
            Device.Dispose();
        }
    }
}
