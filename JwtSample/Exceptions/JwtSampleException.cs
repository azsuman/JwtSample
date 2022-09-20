using System.Net;

namespace JwtSample.Exceptions;

public abstract class JwtSampleException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public JwtSampleException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}
