using System;
using System.Collections.Immutable;
using System.Linq;
using Crida.Asset;
using Crida.States;
using Veldrid;

namespace Crida.Utilities.Veldrid
{
    public class ComputePipeline : IDisposable
    {
        public ComputePipeline(GraphicsState graphics, ShaderSet shader, params IGraphicsResource[] resources)
        {
            Resources = resources.ToImmutableArray();
            var rf = graphics.Device.ResourceFactory;
            Pipeline = rf.CreateComputePipeline(new ComputePipelineDescription(shader.Description.Shaders[0],
                resources.Select(x => x.ResourceLayout).ToArray(), 1, 1, 1));
        }

        internal ImmutableArray<IGraphicsResource> Resources { get; }
        internal Pipeline Pipeline { get; }

        public void Dispose()
        {
            Pipeline.Dispose();
        }
    }

    public static class ComputePipelineExtensions
    {
        public static void SetPipeline(this CommandList commandList, ComputePipeline pipeline)
        {
            commandList.SetPipeline(pipeline.Pipeline);
            for (var i = 0; i < pipeline.Resources.Length; i++)
            {
                var resource = pipeline.Resources[i];
                commandList.SetComputeResourceSet((uint) i, resource.ResourceSet);
            }
        }
    }
}
