using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using Crida.Shared.Math;
using Crida.View.Primitives.Spatial;
using Veldrid;

namespace Crida.States
{
    /// <remarks>
    ///     Coordinate system is right-handed z-up (like Blender).
    /// </remarks>
    public class SpatialState : IState
    {
        public SpatialState(GraphicsState graphics)
        {
            Graphics = graphics;
            var rf = graphics.Device.ResourceFactory;
        }

        private GraphicsState Graphics { get; }

        public Vector3D CameraFocus { get; set; } = new Vector3D(0, 0, 0.8, 1);

        /// <summary>
        ///     Rotation relative to facing down (-Z) with no roll, like in blender.
        /// </summary>
        public EulerXyz CameraEulerXyz { get; set; } = new EulerXyz(Angle.FromDegrees(45), Angle.Zero, Angle.Zero);

        /// <summary>
        ///     Unsigned distance of camera origin to focus.
        /// </summary>
        public double CameraFocusDistance { get; set; } = 10;

        public Angle CameraMinimumFov { get; set; } = Angle.FromDegrees(45);
        public Transform3D CameraTransform { get; private set; }

        public ConcurrentBag<ISpatialObject> Objects { get; } = new ConcurrentBag<ISpatialObject>();

        public void UpdateViewTransform()
        {
            var cameraOrigin =
                CameraFocus + CameraEulerXyz.ToTransform() * new Vector3D(0, 0, 1, 0) * CameraFocusDistance;
            var translation = (-cameraOrigin).AsTranslation();
            var rotation = CameraEulerXyz.ToReverseTransform();
            var screen = new Transform3D(
                1, 0, 0, 0,
                0, -1, 0, 0,
                0, 0, -1, 0,
                0, 0, 0, 1);
            var perspective = new Transform3D(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                0, 0, Angle.Tangent(CameraMinimumFov * 0.5), 0);
            CameraTransform = perspective * screen * rotation * translation;
        }

        public void Dispose()
        {
            // todo
        }
    }
}
