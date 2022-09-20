using System.Net;

namespace JwtSample.Exceptions;

public class MaxPasswordFailureException : JwtSampleException
{
    public MaxPasswordFailureException() : base("The referenced account is currently locked out.", HttpStatusCode.Forbidden)
    {
    }
}
