using System.Runtime.Serialization;

namespace CrossCuttingConcerns.Exceptions.Types;

public class AuthorizationException : Exception
{
    public AuthorizationException() { }

    public AuthorizationException(string? message) : base(message) { }

    public AuthorizationException(string? message, Exception? innerException) : base(message, innerException) { }

#pragma warning disable SYSLIB0051
    protected AuthorizationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#pragma warning restore SYSLIB0051
}