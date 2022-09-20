using System.Net;

namespace JwtSample.Exceptions;

public class RegularUserInactiveDaysException : JwtSampleException
{
    public RegularUserInactiveDaysException(int inactiveDays) : base($"This account has been locked out due to inactivity for more than {inactiveDays} days.", HttpStatusCode.Forbidden)
    {
    }
}
