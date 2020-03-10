using System;
using System.Collections.Immutable;
using Crida.States;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public class ArrayBuffer<T> : ResourcelessArrayBuffer<T>, IGraphicsResource where T : struct
    {
        public ArrayBuffer(GraphicsState graphics, BufferUsage usage, int length, ShaderStages stages)
            : base(graphics, usage, length)
        {
            var rf = graphics.Device.ResourceFactory;
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

        public ArrayBuffer(GraphicsState graphics, BufferUsage usage, ImmutableArray<T> data, ShaderStages stages) :
            this(graphics, usage, data.Length, stages)
        {
            Update(data);
        }

        public ResourceLayout ResourceLayout { get; }
        public ResourceSet ResourceSet { get; }

        public override void Dispose()
        {
            ResourceSet.Dispose();
            ResourceLayout.Dispose();
            base.Dispose();
        }
    }
}
