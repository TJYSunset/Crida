using System;
using Crida.Shared.RawAsset;
using Crida.States;
using Crida.Utilities.Veldrid;
using Veldrid;
using PixelFormat = Crida.Shared.RawAsset.PixelFormat;

namespace Crida.Asset
{
    public class TextureArray : IManagedAsset, IGraphicsResource
    {
        public TextureArray(AssetFactoryState assets, RawTextureArray raw)
        {
            var graphics = assets.Graphics;
            var rf = graphics.Device.ResourceFactory;

            // hack: limit arrayLayers to a minimum of 2 to prevent veldrid creating a non-array texture2D
            Texture = rf.CreateTexture(new TextureDescription(
                (uint) raw.Width, (uint) raw.Height, 1, (uint) raw.MipLevels, Math.Max((uint) raw.Data.Length, 2),
                raw.PixelFormat switch
                {
                    PixelFormat.A8 => Veldrid.PixelFormat.R8_UNorm,
                    PixelFormat.RGBA8888 => Veldrid.PixelFormat.R8_G8_B8_A8_UNorm_SRgb,
                    _ => throw new FormatException()
                }, TextureUsage.Sampled | TextureUsage.GenerateMipmaps, TextureType.Texture2D));
            for (var i = 0; i < raw.Data.Length; i++)
                graphics.Device.UpdateTexture(Texture, raw.Data[i], 0, 0, 0, (uint) raw.Width, (uint) raw.Height,
                    1, 0, (uint) i);

            using var commands = new Commands(assets.Graphics);
            commands.Submit(x => x.GenerateMipmaps(Texture));

            ResourceLayout = rf.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription(nameof(Texture), ResourceKind.TextureReadOnly,
                    ShaderStages.Fragment | ShaderStages.Compute),
                new ResourceLayoutElementDescription(nameof(Sampler), ResourceKind.Sampler,
                    ShaderStages.Fragment | ShaderStages.Compute)
            ));
            ResourceSet = rf.CreateResourceSet(new ResourceSetDescription(
                ResourceLayout, Texture, graphics.Device.Aniso4xSampler));
        }

        private Texture Texture { get; }
        public ResourceLayout ResourceLayout { get; }
        public ResourceSet ResourceSet { get; }

        public void Dispose()
        {
            ResourceSet.Dispose();
            ResourceLayout.Dispose();
            Texture.Dispose();
        }
    }
}
