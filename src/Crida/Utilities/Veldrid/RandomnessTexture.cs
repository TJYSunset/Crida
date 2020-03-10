using System.Numerics;
using Crida.Shared.Utilities;
using Crida.States;
using Troschuetz.Random;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public class RandomnessTexture : IGraphicsResource
    {
        public RandomnessTexture(GraphicsState graphics)
        {
            const int size = 2048;

            var rf = graphics.Device.ResourceFactory;
            Texture = rf.CreateTexture(new TextureDescription(size, size, 1, 1, 1, PixelFormat.R32_G32_B32_A32_Float,
                TextureUsage.Sampled, TextureType.Texture2D));
            Sampler = rf.CreateSampler(new SamplerDescription(SamplerAddressMode.Wrap, SamplerAddressMode.Wrap,
                SamplerAddressMode.Wrap, SamplerFilter.MinPoint_MagPoint_MipPoint, null, 0, 0, 0, 0,
                SamplerBorderColor.TransparentBlack));
            ResourceLayout = rf.CreateResourceLayout(new ResourceLayoutDescription(
                new ResourceLayoutElementDescription(nameof(Texture), ResourceKind.TextureReadOnly,
                    ShaderStages.Compute | ShaderStages.Fragment | ShaderStages.Vertex),
                new ResourceLayoutElementDescription(nameof(Sampler), ResourceKind.Sampler,
                    ShaderStages.Compute | ShaderStages.Fragment | ShaderStages.Vertex)
            ));
            ResourceSet = rf.CreateResourceSet(new ResourceSetDescription(ResourceLayout, Texture, Sampler));

            var random = new TRandom(42);
            var texture = new RgbaFloat[size * size];
            for (var i = 0; i < size * size; i++)
            {
                var vec = Vector2.Normalize(new Vector2((float) random.Normal(0, 1), (float) random.Normal(0, 1)))
                          * (float) random.NextDouble().Squared().Squared().Squared() * 2;
                texture[i] = new RgbaFloat(
                    vec.X, vec.Y, (float) random.NextDouble(), (float) random.NextDouble()
                );
            }

            graphics.Device.UpdateTexture(Texture, texture, 0, 0, 0, size, size, 1, 0, 0);
        }

        private Texture Texture { get; }
        private Sampler Sampler { get; }
        public ResourceLayout ResourceLayout { get; }
        public ResourceSet ResourceSet { get; }


        public void Dispose()
        {
            ResourceSet.Dispose();
            ResourceLayout.Dispose();
            Sampler.Dispose();
            Texture.Dispose();
        }
    }
}
