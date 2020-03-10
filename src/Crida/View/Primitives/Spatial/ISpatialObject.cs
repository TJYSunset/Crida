using Crida.Shared.Math;

namespace Crida.View.Primitives.Spatial
{
    public interface ISpatialObject : IViewLayer
    {
        public Vector3D Position { get; set; }

        /// <summary>
        ///     Objects should be facing +Y when this is 0.
        /// </summary>
        public Angle RotationZ { get; set; }

        // todo drawtransparent()
    }
}
