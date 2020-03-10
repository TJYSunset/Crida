using System;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public interface IGraphicsResource : IDisposable
    {
        public ResourceLayout ResourceLayout { get; }
        public ResourceSet ResourceSet { get; }
    }
}
