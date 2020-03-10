using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MessagePack;
using static Crida.Shared.Math.Angle;

namespace Crida.Shared.Math
{
    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct EulerXyz
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public EulerXyz(Angle x, Angle y, Angle z)
        {
            X = x;
            Y = y;
            Z = z;
            Pad = 0;
        }

        public Angle X;
        public Angle Y;
        public Angle Z;
        private double Pad;

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform3D ToTransform()
        {
            var x = new Transform3D(
                1, 0, 0,
                0, Cosine(X), -Sine(X),
                0, Sine(X), Cosine(X));
            var y = new Transform3D(
                Cosine(Y), 0, Sine(Y),
                0, 1, 0,
                -Sine(Y), 0, Cosine(Y));
            var z = new Transform3D(
                Cosine(Z), -Sine(Z), 0,
                Sine(Z), Cosine(Z), 0,
                0, 0, 1);
            return z * y * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform3D ToReverseTransform()
        {
            var x = new Transform3D(
                1, 0, 0,
                0, Cosine(-X), -Sine(-X),
                0, Sine(-X), Cosine(-X));
            var y = new Transform3D(
                Cosine(-Y), 0, Sine(-Y),
                0, 1, 0,
                -Sine(-Y), 0, Cosine(-Y));
            var z = new Transform3D(
                Cosine(-Z), -Sine(-Z), 0,
                Sine(-Z), Cosine(-Z), 0,
                0, 0, 1);
            return x * y * z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static EulerXyz operator +(EulerXyz left, EulerXyz right)
        {
            return new EulerXyz(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }
    }
}
