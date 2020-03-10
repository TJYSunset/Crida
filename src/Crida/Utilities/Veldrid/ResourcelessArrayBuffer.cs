using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using Crida.Shared.Utilities;
using Crida.States;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public class ResourcelessArrayBuffer<T> where T : struct
    {
        public ResourcelessArrayBuffer(GraphicsState graphics, BufferUsage usage, int length)
        {
            Length = length;

            Graphics = graphics;
            var rf = graphics.Device.ResourceFactory;
            var stride = (uint) Marshal.SizeOf<T>();
            Size = (stride * (uint) length).CeilToMultiplesOf(16);
            StagingBuffer = rf.CreateBuffer(new BufferDescription(Size, BufferUsage.Staging));
            switch (usage)
            {
                case BufferUsage.IndexBuffer:
                case BufferUsage.UniformBuffer:
                case BufferUsage.VertexBuffer:
                    DeviceBuffer = rf.CreateBuffer(new BufferDescription(Size, usage));
                    break;
                case BufferUsage.StructuredBufferReadOnly:
                case BufferUsage.StructuredBufferReadWrite:
                    DeviceBuffer = rf.CreateBuffer(new BufferDescription(Size, usage, stride));
                    break;
                default:
                    throw new ArgumentException($"Invalid buffer usage {usage}", nameof(usage));
            }
        }

        public ResourcelessArrayBuffer(GraphicsState graphics, BufferUsage usage, ImmutableArray<T> data)
            : this(graphics, usage, data.Length)
        {
            Update(data);
        }

        public int Length { get; }
        private GraphicsState Graphics { get; }
        private uint Size { get; }
        private DeviceBuffer StagingBuffer { get; }
        internal DeviceBuffer DeviceBuffer { get; }

        public void Update(ImmutableArray<T> value)
        {
            Graphics.Device.UpdateBuffer(DeviceBuffer, 0, value.ToArray());
        }

        public void Update(Action<MappedResourceView<T>> updater)
        {
            var staging = Graphics.Device.Map<T>(StagingBuffer, MapMode.Write);
            updater(staging);
            var rf = Graphics.Device.ResourceFactory;
            using var commands = new Commands(Graphics);
            commands.Submit(x => x.CopyBuffer(StagingBuffer, 0, DeviceBuffer, 0, Size));
        }

        public virtual void Dispose()
        {
            StagingBuffer.Dispose();
            DeviceBuffer.Dispose();
        }
    }

    public static class ResourcelessArrayBufferExtensions
    {
        public static void SetVertexBuffer<T>(this CommandList commandList, ResourcelessArrayBuffer<T> buffer)
            where T : struct
        {
            commandList.SetVertexBuffer(0, buffer.DeviceBuffer);
        }

        public static void SetIndexBuffer(this CommandList commandList, ResourcelessArrayBuffer<uint> buffer)
        {
            commandList.SetIndexBuffer(buffer.DeviceBuffer, IndexFormat.UInt32);
        }
    }
}
