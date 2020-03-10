using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using MessagePack;

namespace Crida.Shared.RawAsset
{
    [MessagePackObject]
    public class RawTextureArray : IRawAsset
    {
        public RawTextureArray(int width, int height, int mipLevels, PixelFormat pixelFormat,
            ImmutableArray<byte[]> data)
        {
            Width = width;
            Height = height;
            MipLevels = mipLevels;
            PixelFormat = pixelFormat;
            Data = data;
        }

        [Key(0)] public int Width { get; }
        [Key(1)] public int Height { get; }
        [Key(2)] public int MipLevels { get; }
        [Key(3)] public PixelFormat PixelFormat { get; }
        [Key(4)] public ImmutableArray<byte[]> Data { get; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum PixelFormat : uint
    {
        RGBA8888,
        A8
    }
}
