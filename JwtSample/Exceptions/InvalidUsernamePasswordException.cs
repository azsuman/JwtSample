using System.Net;

namespace JwtSample.Exceptions;

public class InvalidUsernamePasswordException : JwtSampleException
{
    public InvalidUsernamePasswordException() : base("Invalid Username or Password.", HttpStatusCode.Unauthorized)
    {
    }
}
