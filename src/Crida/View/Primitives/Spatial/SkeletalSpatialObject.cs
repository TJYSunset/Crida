// using System.Runtime.InteropServices;
// using System.Threading.Tasks;
// using Crida.Asset;
// using Crida.Shared.Math;
// using Crida.States;
// using Veldrid;
//
// namespace Crida.View.Primitives.Spatial
// {
//     public sealed class SkeletalSpatialObject : ISpatialObject
//     {
//         public Vector3D Position { get; set; }
//         public Angle RotationZ { get; set; }
//
//         private GraphicsState Graphics { get; }
//         private SpatialModel Model { get; }
//         private DeviceBuffer VertexBuffer { get; }
//         private DeviceBuffer FaceBuffer { get; }
//
//         public SkeletalSpatialObject(GraphicsState graphics, SpatialModel model)
//         {
//             Graphics = graphics;
//             Model = model;
//             var rf = graphics.Device.ResourceFactory;
//         }
//
//         public Task Draw(Framebuffer target, Transform3D transform, ResourceSet strokeTarget, int strokeTargetOffset)
//         {
//             throw new System.NotImplementedException();
//         }
//     }
// }


