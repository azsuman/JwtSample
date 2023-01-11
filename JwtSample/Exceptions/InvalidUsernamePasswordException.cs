using System.Net;

namespace JwtSample.Exceptions;

public class InvalidUsernamePasswordException : JwtSampleException
{
    public InvalidUsernamePasswordException() : base("Invalid username or password.", HttpStatusCode.Unauthorized)
    {
    }
}
