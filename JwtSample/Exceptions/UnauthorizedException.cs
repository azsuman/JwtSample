using System.Net;

namespace JwtSample.Exceptions;

public class UnauthorizedException : JwtSampleException
{
    public UnauthorizedException() : base("Authentication failed.", HttpStatusCode.Unauthorized)
    {
    }
}
