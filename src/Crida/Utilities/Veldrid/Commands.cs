using System;
using Crida.States;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public class Commands : IDisposable
    {
        public Commands(GraphicsState graphics)
        {
            Graphics = graphics;
            var rf = graphics.Device.ResourceFactory;
            CommandList = rf.CreateCommandList();
            Fence = rf.CreateFence(false);
        }

        private GraphicsState Graphics { get; }
        private CommandList CommandList { get; }
        private Fence Fence { get; }

        public void Dispose()
        {
            Fence.Dispose();
            CommandList.Dispose();
        }

        public void Submit(Action<CommandList> action)
        {
            CommandList.Begin();
            action(CommandList);
            CommandList.End();
            Graphics.Device.SubmitCommands(CommandList, Fence);
            Graphics.Device.WaitForFence(Fence);
            Fence.Reset();
        }
    }
}
