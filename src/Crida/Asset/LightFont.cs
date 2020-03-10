using Crida.Shared.RawAsset;
using Crida.States;
using Crida.Utilities.Veldrid;
using Veldrid;

namespace Crida.Asset
{
    public class LightFont : IManagedAsset
    {
        public LightFont(AssetFactoryState assets, RawLightFont raw)
        {
            RawFont = raw.FontData;
            PrerenderSize = raw.PrerenderSize;
            GlyphInfoBuffer = new ArrayBuffer<PrerenderGlyphInfo>(assets.Graphics, BufferUsage.StructuredBufferReadOnly,
                raw.PrerenderGlyphInfos, ShaderStages.Vertex);
            TextureArray = new TextureArray(assets, raw.TextureArray);
        }

        public byte[] RawFont { get; }
        public int PrerenderSize { get; }
        public ArrayBuffer<PrerenderGlyphInfo> GlyphInfoBuffer { get; }
        public TextureArray TextureArray { get; }

        public void Dispose()
        {
            TextureArray.Dispose();
            GlyphInfoBuffer.Dispose();
        }
    }
}
