using System.Runtime.InteropServices;
using MessagePack;

namespace Crida.Shared.Math
{
    /// <summary>
    ///     Column major multiplication, row major storage
    /// </summary>
    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    // ReSharper disable once InconsistentNaming
    public readonly struct Transform3D
    {
        [Key(0)] public readonly double M00;
        [Key(1)] public readonly double M01;
        [Key(2)] public readonly double M02;
        [Key(3)] public readonly double M03;
        [Key(4)] public readonly double M10;
        [Key(5)] public readonly double M11;
        [Key(6)] public readonly double M12;
        [Key(7)] public readonly double M13;
        [Key(8)] public readonly double M20;
        [Key(9)] public readonly double M21;
        [Key(10)] public readonly double M22;
        [Key(11)] public readonly double M23;
        [Key(12)] public readonly double M30;
        [Key(13)] public readonly double M31;
        [Key(14)] public readonly double M32;
        [Key(15)] public readonly double M33;

        public Transform3D(double m00, double m01, double m02, double m03, double m10, double m11, double m12,
            double m13, double m20, double m21, double m22, double m23, double m30, double m31, double m32, double m33)
        {
            (M00, M01, M02, M03) = (m00, m01, m02, m03);
            (M10, M11, M12, M13) = (m10, m11, m12, m13);
            (M20, M21, M22, M23) = (m20, m21, m22, m23);
            (M30, M31, M32, M33) = (m30, m31, m32, m33);
        }

        public Transform3D(double m00, double m01, double m02, double m10, double m11, double m12, double m20,
            double m21, double m22)
        {
            (M00, M01, M02, M03) = (m00, m01, m02, 0);
            (M10, M11, M12, M13) = (m10, m11, m12, 0);
            (M20, M21, M22, M23) = (m20, m21, m22, 0);
            (M30, M31, M32, M33) = (0, 0, 0, 1);
        }

        public static Transform3D Identity { get; } = new Transform3D(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1);

        public static Transform3D operator *(Transform3D l, Transform3D r)
        {
            return new Transform3D(
                l.M00 * r.M00 + l.M01 * r.M10 + l.M02 * r.M20 + l.M03 * r.M30,
                l.M00 * r.M01 + l.M01 * r.M11 + l.M02 * r.M21 + l.M03 * r.M31,
                l.M00 * r.M02 + l.M01 * r.M12 + l.M02 * r.M22 + l.M03 * r.M32,
                l.M00 * r.M03 + l.M01 * r.M13 + l.M02 * r.M23 + l.M03 * r.M33,
                l.M10 * r.M00 + l.M11 * r.M10 + l.M12 * r.M20 + l.M13 * r.M30,
                l.M10 * r.M01 + l.M11 * r.M11 + l.M12 * r.M21 + l.M13 * r.M31,
                l.M10 * r.M02 + l.M11 * r.M12 + l.M12 * r.M22 + l.M13 * r.M32,
                l.M10 * r.M03 + l.M11 * r.M13 + l.M12 * r.M23 + l.M13 * r.M33,
                l.M20 * r.M00 + l.M21 * r.M10 + l.M22 * r.M20 + l.M23 * r.M30,
                l.M20 * r.M01 + l.M21 * r.M11 + l.M22 * r.M21 + l.M23 * r.M31,
                l.M20 * r.M02 + l.M21 * r.M12 + l.M22 * r.M22 + l.M23 * r.M32,
                l.M20 * r.M03 + l.M21 * r.M13 + l.M22 * r.M23 + l.M23 * r.M33,
                l.M30 * r.M00 + l.M31 * r.M10 + l.M32 * r.M20 + l.M33 * r.M30,
                l.M30 * r.M01 + l.M31 * r.M11 + l.M32 * r.M21 + l.M33 * r.M31,
                l.M30 * r.M02 + l.M31 * r.M12 + l.M32 * r.M22 + l.M33 * r.M32,
                l.M30 * r.M03 + l.M31 * r.M13 + l.M32 * r.M23 + l.M33 * r.M33);
        }

        public static Vector3D operator *(Transform3D l, Vector3D r)
        {
            return new Vector3D(
                l.M00 * r.X + l.M01 * r.Y + l.M02 * r.Z + l.M03 * r.W,
                l.M10 * r.X + l.M11 * r.Y + l.M12 * r.Z + l.M13 * r.W,
                l.M20 * r.X + l.M21 * r.Y + l.M22 * r.Z + l.M23 * r.W,
                l.M30 * r.X + l.M31 * r.Y + l.M32 * r.Z + l.M33 * r.W);
        }
    }
}
