using System.Net;

namespace JwtSample.Exceptions;

public class InactiveAccountException : JwtSampleException
{
    public InactiveAccountException() : base("This account is currently not active.", HttpStatusCode.Forbidden)
    {
    }
}
