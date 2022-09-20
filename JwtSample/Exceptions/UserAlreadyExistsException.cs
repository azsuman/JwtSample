using System.Net;

namespace JwtSample.Exceptions;

public class UserAlreadyExistsException : JwtSampleException
{
    public UserAlreadyExistsException() : base("Username already exists", HttpStatusCode.Conflict)
    {
    }
}
