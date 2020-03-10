using System.Threading.Tasks;
using Crida.Asset;
using Crida.States;
using Crida.Utilities.Veldrid;
using Veldrid;

namespace Crida.View.Primitives
{
    public class IntermediateFramebuffer : IViewLayer
    {
        private OutputDescription OutputDescription { get; }

        public IntermediateFramebuffer(GraphicsState graphics, AssetFactoryState assets, Blend blend,
            DepthTest depthTest, OutputDescription outputDescription)
        {
            OutputDescription = outputDescription;
            Graphics = graphics;
            Assets = assets;
            Blend = blend;
            DepthTest = depthTest;
            Validate();
        }

        private GraphicsState Graphics { get; }
        private AssetFactoryState Assets { get; }
        private Blend Blend { get; }
        private DepthTest DepthTest { get; }
        private (uint width, uint height) LastUpdatedFor { get; set; }
        private Instance? Data { get; set; }

        public void Dispose()
        {
            Data?.Dispose();
        }

        public async Task Draw(Framebuffer target)
        {
            await Data!.Draw(target);
        }

        public void Validate()
        {
            var main = Graphics.Device.SwapchainFramebuffer;
            var size = (main.Width, main.Height);
            if (size == LastUpdatedFor) return;
            Data?.Dispose();
            LastUpdatedFor = size;
            Data = new Instance(Graphics, Assets, Blend, DepthTest, OutputDescription, size.Width, size.Height);
        }

        public static implicit operator Framebuffer(IntermediateFramebuffer x)
        {
            return x.Data!.Framebuffer;
        }

        private class Instance : IViewLayer
        {
            private GraphicsState Graphics { get; }

            public Instance(GraphicsState graphics, AssetFactoryState assets, Blend blend, DepthTest depthTest,
                OutputDescription outputDescription, uint width, uint height)
            {
                Graphics = graphics;
                var rf = graphics.Device.ResourceFactory;
                Color = rf.CreateTexture(new TextureDescription(
                    width, height, 1, 1, 1, PixelFormat.R32_G32_B32_A32_Float,
                    TextureUsage.Sampled | TextureUsage.RenderTarget, TextureType.Texture2D));
                Depth = rf.CreateTexture(new TextureDescription(
                    width, height, 1, 1, 1, PixelFormat.D32_Float_S8_UInt,
                    TextureUsage.Sampled | TextureUsage.DepthStencil, TextureType.Texture2D));
                Framebuffer = rf.CreateFramebuffer(new FramebufferDescription(Depth, Color));
                ResourceLayout = rf.CreateResourceLayout(new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription(nameof(Color), ResourceKind.TextureReadOnly,
                        ShaderStages.Fragment | ShaderStages.Compute),
                    new ResourceLayoutElementDescription(nameof(Depth), ResourceKind.TextureReadOnly,
                        ShaderStages.Fragment | ShaderStages.Compute),
                    new ResourceLayoutElementDescription(nameof(Sampler), ResourceKind.Sampler,
                        ShaderStages.Fragment | ShaderStages.Compute)
                ));
                ResourceSet = rf.CreateResourceSet(new ResourceSetDescription(
                    ResourceLayout, Color, Depth, graphics.Device.PointSampler));
                Pipeline = new GraphicsPipeline(graphics, blend, depthTest, false,
                    assets.Get<ShaderSet>("Shaders.IntermediateFramebuffer"), outputDescription,
                    new ManualGraphicsResource(ResourceLayout, ResourceSet));
                Commands = new Commands(graphics);
            }

            public Texture Color { get; }
            public Texture Depth { get; }
            public Framebuffer Framebuffer { get; }
            private ResourceLayout ResourceLayout { get; }
            private ResourceSet ResourceSet { get; }
            private GraphicsPipeline Pipeline { get; }
            private Commands Commands { get; }

            public void Dispose()
            {
                Commands.Dispose();
                Pipeline.Dispose();
                ResourceSet.Dispose();
                ResourceLayout.Dispose();
                Framebuffer.Dispose();
                Depth.Dispose();
                Color.Dispose();
            }

            public async Task Draw(Framebuffer target)
            {
                Commands.Submit(x =>
                {
                    x.SetFramebuffer(target);
                    x.SetPipeline(Pipeline);
                    x.SetIndexBuffer(Graphics.SingleRectIndexBuffer);
                    x.DrawIndexed(6);
                });
            }

            private class ManualGraphicsResource : IGraphicsResource
            {
                public ManualGraphicsResource(ResourceLayout resourceLayout, ResourceSet resourceSet)
                {
                    ResourceLayout = resourceLayout;
                    ResourceSet = resourceSet;
                }

                public void Dispose()
                {
                }

                public ResourceLayout ResourceLayout { get; }
                public ResourceSet ResourceSet { get; }
            }
        }
    }
}
