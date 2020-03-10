using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Crida.Exceptions;
using static SDL2.SDL;

namespace Crida.States
{
    public sealed class WindowState : IState
    {
        public volatile bool ShouldQuit = false;

        public WindowState()
        {
            if (SDL_Init(SDL_INIT_EVERYTHING) != 0)
                throw new SdlException($"Could not initialize SDL: {SDL_GetError()}");
            Handle = SDL_CreateWindow("Crida", SDL_WINDOWPOS_CENTERED, SDL_WINDOWPOS_CENTERED, 1280, 720,
                SDL_WindowFlags.SDL_WINDOW_RESIZABLE | SDL_WindowFlags.SDL_WINDOW_VULKAN);
            if (Handle == IntPtr.Zero) throw new SdlException($"Could not create window: {SDL_GetError()}");
            Stopwatch = Stopwatch.StartNew();
        }

        public IntPtr Handle { get; }
        public ConcurrentQueue<SDL_Event> PendingEvents { get; } = new ConcurrentQueue<SDL_Event>();
        public Stopwatch Stopwatch { get; }

        public void Dispose()
        {
            SDL_DestroyWindow(Handle);
            SDL_Quit();
        }
    }
}
