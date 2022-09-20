using System.Net;

namespace JwtSample.Exceptions;

public class UserNotFoundException : JwtSampleException
{
    public UserNotFoundException() : base("User Not Found", HttpStatusCode.NotFound)
    {
    }
}
