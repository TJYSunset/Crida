using System;
using System.Collections.Immutable;
using System.Linq;
using Crida.Asset;
using Crida.States;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public class GraphicsPipeline : IDisposable
    {
        public GraphicsPipeline(GraphicsState graphics, Blend blend, DepthTest depthTest, bool culling,
            ShaderSet shaders, OutputDescription output, params IGraphicsResource[] resources)
        {
            Resources = resources.ToImmutableArray();
            var rf = graphics.Device.ResourceFactory;
            Pipeline = rf.CreateGraphicsPipeline(new GraphicsPipelineDescription(
                blend switch
                {
                    Blend.Alpha => BlendStateDescription.SingleAlphaBlend,
                    Blend.Override => BlendStateDescription.SingleOverrideBlend,
                    _ => throw new ArgumentException()
                },
                depthTest switch
                {
                    DepthTest.Off => DepthStencilStateDescription.Disabled,
                    DepthTest.On => DepthStencilStateDescription.DepthOnlyLessEqual,
                    DepthTest.Readonly => DepthStencilStateDescription.DepthOnlyLessEqualRead,
                    _ => throw new ArgumentException()
                },
                culling ? RasterizerStateDescription.Default : RasterizerStateDescription.CullNone,
                PrimitiveTopology.TriangleList, shaders.Description, resources.Select(x => x.ResourceLayout).ToArray(),
                output
            ));
        }

        internal ImmutableArray<IGraphicsResource> Resources { get; }
        internal Pipeline Pipeline { get; }

        public void Dispose()
        {
            Pipeline.Dispose();
        }
    }

    public static class GraphicsPipelineExtensions
    {
        public static void SetPipeline(this CommandList commandList, GraphicsPipeline pipeline)
        {
            commandList.SetPipeline(pipeline.Pipeline);
            for (var i = 0; i < pipeline.Resources.Length; i++)
            {
                var resource = pipeline.Resources[i];
                commandList.SetGraphicsResourceSet((uint) i, resource.ResourceSet);
            }
        }
    }

    public enum Blend
    {
        Alpha,
        Override
    }

    public enum DepthTest
    {
        Off,
        On,
        Readonly
    }
}
