using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MessagePack;
using static System.Math;

namespace Crida.Shared.Math
{
    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct Angle : IEquatable<Angle>
    {
        [Key(0)] public readonly double Radians;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double Degrees()
        {
            return Radians / PI * 180;
        }

        public override string ToString()
        {
            return $"{Degrees():F1}°";
        }

        public static Angle Zero { get; } = new Angle(0);

        #region Constructor

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Angle(double radians)
        {
            const double doublePi = PI * 2;
            Radians = radians >= 0 ? radians % doublePi : doublePi + (radians % doublePi);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle FromRadians(double radians)
        {
            return new Angle(radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle FromDegrees(double degrees)
        {
            return new Angle(degrees / 180 * PI);
        }

        #endregion

        #region Operators

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator +(Angle left, Angle right)
        {
            return new Angle(left.Radians + right.Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator -(Angle left, Angle right)
        {
            return new Angle(left.Radians - right.Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator -(Angle right)
        {
            return new Angle(-right.Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator *(Angle left, double right)
        {
            return new Angle(left.Radians * right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator *(double left, Angle right)
        {
            return new Angle(left * right.Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Angle operator /(Angle left, double right)
        {
            return new Angle(left.Radians / right);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Sine(Angle angle)
        {
            return Sin(angle.Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Cosine(Angle angle)
        {
            return Cos(angle.Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Tangent(Angle angle)
        {
            return Tan(angle.Radians);
        }

        #endregion

        #region Equality

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Angle other)
        {
            return Radians.Equals(other.Radians);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object? obj)
        {
            return obj is Angle other && Equals(other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return Radians.GetHashCode();
        }

        #endregion
    }
}
