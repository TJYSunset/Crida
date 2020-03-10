using System.Collections.Immutable;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using MessagePack;

namespace Crida.Shared.RawAsset
{
    [MessagePackObject]
    public class RawLightFont : IRawAsset
    {
        public RawLightFont(byte[] fontData, int prerenderSize, ImmutableArray<PrerenderGlyphInfo> prerenderGlyphInfos,
            RawTextureArray textureArray)
        {
            FontData = fontData;
            PrerenderSize = prerenderSize;
            PrerenderGlyphInfos = prerenderGlyphInfos;
            TextureArray = textureArray;
        }

        [Key(0)] public byte[] FontData { get; }
        [Key(1)] public int PrerenderSize { get; }
        [Key(2)] public ImmutableArray<PrerenderGlyphInfo> PrerenderGlyphInfos { get; }
        [Key(3)] public RawTextureArray TextureArray { get; }
    }

    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct PrerenderGlyphInfo
    {
        public PrerenderGlyphInfo(int layer, int x, int y, int width, int height, int offsetX, int offsetY)
        {
            Layer = layer;
            Pad = 0;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }

        [Key(0)] public readonly int Layer;
        [IgnoreMember, UsedImplicitly] private readonly int Pad;
        [Key(1)] public readonly int X;
        [Key(2)] public readonly int Y;
        [Key(3)] public readonly int Width;
        [Key(4)] public readonly int Height;
        [Key(5)] public readonly int OffsetX;

        /// <summary>
        ///     In vulkan coordinate system (top left origin), cursor position (bottom left to the glyph) + YOffset
        ///     = Actual top left coordinates of the glyph rectangle in world space
        /// </summary>
        [Key(6)] public readonly int OffsetY;
    }
}
