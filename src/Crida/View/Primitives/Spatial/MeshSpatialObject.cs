using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Crida.Asset;
using Crida.Shared.Math;
using Crida.Shared.RawAsset;
using Crida.States;
using Crida.Utilities.Veldrid;
using Veldrid;

namespace Crida.View.Primitives.Spatial
{
    public sealed class MeshSpatialObject : ISpatialObject
    {
        public MeshSpatialObject(GraphicsState graphics, SpatialState spatial, AssetFactoryState assets, Mesh mesh,
            bool culling)
        {
            Graphics = graphics;
            Spatial = spatial;
            Mesh = mesh;
            // TextureLayer = textureLayer;

            VertexBuffer = new ArrayBuffer<Vertex>(graphics, BufferUsage.StructuredBufferReadOnly,
                mesh.Vertices, ShaderStages.Compute);
            WorldVertexBuffer = new ArrayBuffer<Vertex>(graphics, BufferUsage.StructuredBufferReadWrite,
                mesh.Vertices.Length, ShaderStages.Compute | ShaderStages.Vertex);
            ScreenCoordsBuffer = new ArrayBuffer<Vector3D>(graphics, BufferUsage.StructuredBufferReadWrite,
                mesh.Vertices.Length, ShaderStages.Compute | ShaderStages.Vertex);
            ParameterBuffer = new StructBuffer<Parameters>(graphics, BufferUsage.UniformBuffer,
                ShaderStages.Compute);
            // Texture = texture;

            FaceBuffer = new ResourcelessArrayBuffer<Face>(graphics, BufferUsage.VertexBuffer, mesh.Faces);

            TransformPipeline = new ComputePipeline(graphics,
                assets.Get<ShaderSet>("Shaders.MeshSpatialObject.Transform"),
                VertexBuffer, WorldVertexBuffer, ScreenCoordsBuffer, ParameterBuffer);
            ColorPipeline = new GraphicsPipeline(graphics, Blend.Override, DepthTest.On, culling,
                assets.Get<ShaderSet>("Shaders.MeshSpatialObject.Color"), graphics.IntermediateOutputDescription,
                WorldVertexBuffer, ScreenCoordsBuffer);
            Commands = new Commands(graphics);
        }

        private GraphicsState Graphics { get; }
        private SpatialState Spatial { get; }
        private Mesh Mesh { get; }

        /// <summary>
        ///     "Vertex" as in blender. NOT the vertex buffer in graphics pipeline.
        /// </summary>
        private ArrayBuffer<Vertex> VertexBuffer { get; }

        private ArrayBuffer<Vertex> WorldVertexBuffer { get; }
        private ArrayBuffer<Vector3D> ScreenCoordsBuffer { get; }
        private StructBuffer<Parameters> ParameterBuffer { get; }

        private ResourcelessArrayBuffer<Face> FaceBuffer { get; }
        // private TextureArray Texture { get; }
        // private int TextureLayer { get; }

        private ComputePipeline TransformPipeline { get; }
        private GraphicsPipeline ColorPipeline { get; }
        private Commands Commands { get; }

        public Vector3D Position { get; set; }
        public Angle RotationZ { get; set; }

        public async Task Draw(Framebuffer target)
        {
            ParameterBuffer.Update(new Parameters(Transform3D.Identity, Spatial.CameraTransform));
            Commands.Submit(x =>
            {
                x.SetPipeline(TransformPipeline);
                x.Dispatch((uint) VertexBuffer.Length, 1, 1);
            });
            Commands.Submit(x =>
            {
                x.SetFramebuffer(target);
                var sfWidth = Graphics.Device.SwapchainFramebuffer.Width;
                var sfHeight = Graphics.Device.SwapchainFramebuffer.Height;
                x.SetViewport(0, sfWidth > sfHeight
                    ? new Viewport(0, -(sfWidth - sfHeight) / 2f, sfWidth, sfWidth, 0, 1)
                    : new Viewport(-(sfHeight - sfWidth) / 2f, 0, sfHeight, sfHeight, 0, 1));
                x.SetPipeline(ColorPipeline);
                x.SetVertexBuffer(FaceBuffer);
                x.Draw((uint) FaceBuffer.Length * 3);
            });
        }

        public void Dispose()
        {
            Commands.Dispose();
            ColorPipeline.Dispose();
            TransformPipeline.Dispose();
            ParameterBuffer.Dispose();
            ScreenCoordsBuffer.Dispose();
            WorldVertexBuffer.Dispose();
            VertexBuffer.Dispose();
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private readonly struct Parameters
        {
            public Parameters(Transform3D worldTransform, Transform3D screenTransform)
            {
                WorldTransform = worldTransform;
                ScreenTransform = screenTransform;
            }

            public readonly Transform3D WorldTransform;
            public readonly Transform3D ScreenTransform;
        }
    }
}
