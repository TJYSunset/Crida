using System.Runtime.CompilerServices;

namespace Crida.Shared.Utilities
{
    public static class MathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint CeilToMultiplesOf(this uint source, uint x)
        {
            return source % x == 0 ? source : (source / x + 1) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToMultiplesOf(this int source, int x)
        {
            return source % x == 0 ? source : (source / x + 1) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Squared(this float source)
        {
            return source * source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Squared(this double source)
        {
            return source * source;
        }
    }
}
