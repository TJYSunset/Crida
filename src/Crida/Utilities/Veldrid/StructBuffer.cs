using System;
using System.Runtime.InteropServices;
using Crida.Shared.Utilities;
using Crida.States;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public class StructBuffer<T> : IGraphicsResource where T : struct
    {
        public StructBuffer(GraphicsState graphics, BufferUsage usage, ShaderStages stages)
        {
            Graphics = graphics;
            var rf = graphics.Device.ResourceFactory;
            DeviceBuffer = rf.CreateBuffer(new BufferDescription(
                (uint) Marshal.SizeOf<T>().CeilToMultiplesOf(16), usage));
            ResourceLayout = rf.CreateResourceLayout(new ResourceLayoutDescription(new ResourceLayoutElementDescription(
                typeof(T).Name,
                usage switch
                {
                    BufferUsage.UniformBuffer => ResourceKind.UniformBuffer,
                    BufferUsage.StructuredBufferReadOnly => ResourceKind.StructuredBufferReadOnly,
                    BufferUsage.StructuredBufferReadWrite => ResourceKind.StructuredBufferReadWrite,
                    _ => throw new ArgumentException($"Invalid buffer usage {usage}", nameof(usage))
                }, stages
            )));
            ResourceSet = rf.CreateResourceSet(new ResourceSetDescription(ResourceLayout, DeviceBuffer));
        }

        private GraphicsState Graphics { get; }
        private DeviceBuffer DeviceBuffer { get; }

        public ResourceLayout ResourceLayout { get; }
        public ResourceSet ResourceSet { get; }

        public virtual void Dispose()
        {
            ResourceSet.Dispose();
            ResourceLayout.Dispose();
            DeviceBuffer.Dispose();
        }

        public void Update(T value)
        {
            Graphics.Device.UpdateBuffer(DeviceBuffer, 0, value);
        }
    }
}
