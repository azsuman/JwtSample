using System.Net;

namespace JwtSample.Exceptions;

public class UserNotFoundException : JwtSampleException
{
    public UserNotFoundException() : base("User not found.", HttpStatusCode.NotFound)
    {
    }
}
