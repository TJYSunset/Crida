using System;
using System.Threading.Tasks;
using Veldrid;

namespace Crida.View
{
    public interface IViewLayer : IDisposable
    {
        public Task Draw(Framebuffer target);
    }
}
