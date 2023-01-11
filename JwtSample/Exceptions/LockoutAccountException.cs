using System.Net;

namespace JwtSample.Exceptions;

public class LockoutAccountException : JwtSampleException
{
    public LockoutAccountException() : base("The referenced account is currently locked out.", HttpStatusCode.Forbidden)
    {
    }
}
