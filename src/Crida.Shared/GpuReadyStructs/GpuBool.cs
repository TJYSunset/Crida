using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using MessagePack;

namespace Crida.Shared.GpuReadyStructs
{
    [MessagePackObject]
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public readonly struct GpuBool
    {
        [UsedImplicitly]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public GpuBool(uint value)
        {
            Value = value;
        }

        [Key(0)] public readonly uint Value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool(GpuBool source)
        {
            return source.Value == 1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator GpuBool(bool source)
        {
            return new GpuBool(source ? 1u : 0u);
        }
    }
}
