using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Crida.Shared.Utilities;
using JetBrains.Annotations;
using MessagePack;
using static System.Math;

namespace Crida.Shared.Math
{
    /// <summary>
    ///     Represents a normalized homogeneous 3D vector.
    /// </summary>
    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Vector3D : IEquatable<Vector3D>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3D(double x, double y, double z, double w)
        {
            if (w != 0)
            {
                X = x / w;
                Y = y / w;
                Z = z / w;
                W = 1;
            }
            else
            {
                X = x;
                Y = y;
                Z = z;
                W = 0;
            }
        }

        [Key(0)] public readonly double X;
        [Key(1)] public readonly double Y;
        [Key(2)] public readonly double Z;
        [Key(3)] public readonly double W;

        public override string ToString()
        {
            return $"({X:F1}, {Y:F1}, {Z:F1}, {W:F0})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Length()
        {
            return Sqrt(X.Squared() + Y.Squared() + Z.Squared());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator +(Vector3D left, Vector3D right)
        {
            return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z, Max(left.W, right.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator -(Vector3D left, Vector3D right)
        {
            return new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z, Max(left.W, right.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator -(Vector3D right)
        {
            return new Vector3D(-right.X, -right.Y, -right.Z, right.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator *(Vector3D left, double right)
        {
            return new Vector3D(left.X * right, left.Y * right, left.Z * right, left.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator /(Vector3D left, double right)
        {
            return new Vector3D(left.X / right, left.Y / right, left.Z / right, left.W);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator *(double left, Vector3D right)
        {
            return right * left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3D operator /(double left, Vector3D right)
        {
            return right / left;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector3D Cross(Vector3D right)
        {
            return new Vector3D(Y * right.Z - Z * right.Y, Z * right.X - X * right.Z, X * right.Y - Y * right.X,
                Max(W, right.W));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Dot(Vector3D right)
        {
            return X * right.X + Y * right.Y + Z * right.Z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform3D AsTranslation()
        {
            return new Transform3D(
                1, 0, 0, X,
                0, 1, 0, Y,
                0, 0, 1, Z,
                0, 0, 0, 1
            );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Transform3D ToScaling()
        {
            return new Transform3D(
                X, 0, 0,
                0, Y, 0,
                0, 0, Z
            );
        }

        #region IEquatable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Vector3D other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj)
        {
            return obj is Vector3D other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Vector3D left, Vector3D right)
        {
            return left.Equals(right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Vector3D left, Vector3D right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}
