using System.Net;

namespace JwtSample.Exceptions;

public class UserAlreadyExistsException : JwtSampleException
{
    public UserAlreadyExistsException(string username) : base($"Username '{username}' is not available.", HttpStatusCode.Conflict)
    {
    }
}
