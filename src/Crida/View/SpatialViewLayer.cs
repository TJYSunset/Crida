using System;
using System.Threading.Tasks;
using Crida.Asset;
using Crida.States;
using Crida.View.Primitives.Spatial;
using Veldrid;

namespace Crida.View
{
    public sealed class SpatialViewLayer : IViewLayer
    {
        public SpatialViewLayer(GraphicsState graphics, SpatialState spatial, AssetFactoryState assets)
        {
            Graphics = graphics;
            Spatial = spatial;
            Assets = assets;

            spatial.Objects.Add(new MeshSpatialObject(graphics, spatial, assets,
                assets.Get<SpatialModel>("Characters.Raccoon").Meshes["Body"], true));
        }

        private GraphicsState Graphics { get; }
        private SpatialState Spatial { get; }
        private AssetFactoryState Assets { get; }

        public async Task Draw(Framebuffer target)
        {
            Spatial.UpdateViewTransform();
            foreach (var spatialObject in Spatial.Objects)
            {
                await spatialObject.Draw(target);
            }
        }

        public void Dispose()
        {
        }
    }
}
