using System;
using System.Collections.Immutable;
using System.Linq;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;

namespace Crida.Shared.Serialization
{
    public class ReallyBadMathResolver : IFormatterResolver
    {
        public ReallyBadMathResolver()
        {
            SafeTypes = typeof(System.Numerics.Complex).Assembly.ExportedTypes
                .Concat(typeof(System.Numerics.Vector2).Assembly.ExportedTypes)
                .ToImmutableHashSet();
        }

        private ImmutableHashSet<Type> SafeTypes { get; }

        public IMessagePackFormatter<T>? GetFormatter<T>()
        {
            return SafeTypes.Contains(typeof(T)) ? DynamicContractlessObjectResolverAllowPrivate.Instance.GetFormatter<T>() : null;
        }

        public static ReallyBadMathResolver Instance { get; } = new ReallyBadMathResolver();
    }
}
