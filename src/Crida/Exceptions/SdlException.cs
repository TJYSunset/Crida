using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace Crida.Exceptions
{
    public class SdlException : Exception
    {
        public SdlException()
        {
        }

        protected SdlException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public SdlException([CanBeNull] string? message) : base(message)
        {
        }

        public SdlException([CanBeNull] string? message, [CanBeNull] Exception? innerException) : base(message,
            innerException)
        {
        }
    }
}
