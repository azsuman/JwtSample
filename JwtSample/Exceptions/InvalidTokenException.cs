using System.Net;

namespace JwtSample.Exceptions;

public class InvalidTokenException : JwtSampleException
{
    public InvalidTokenException() : base("Unauthorized token.", HttpStatusCode.Unauthorized)
    {
    }
}
