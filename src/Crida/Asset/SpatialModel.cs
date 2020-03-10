using System.Collections.Immutable;
using Crida.Shared.RawAsset;
using Crida.States;

namespace Crida.Asset
{
    public class SpatialModel : IManagedAsset
    {
        public SpatialModel(AssetFactoryState assets, RawSpatialModel raw)
        {
            Bones = raw.Bones;
            Meshes = raw.Meshes;
            // TextureArray = new TextureArray(raw.TextureArray);
        }

        public ImmutableArray<Bone> Bones { get; }

        public ImmutableDictionary<string, Mesh> Meshes { get; }

        // public TextureArray TextureArray { get; }

        public void Dispose()
        {
            // TextureArray.Dispose();
        }
    }
}
