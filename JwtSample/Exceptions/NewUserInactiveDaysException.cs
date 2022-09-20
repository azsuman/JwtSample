using System.Net;

namespace JwtSample.Exceptions
{
    public class NewUserInactiveDaysException : JwtSampleException
    {
        public NewUserInactiveDaysException(int inactiveDays) : base($"This account has been locked out due to inactivity for more than {inactiveDays} days after being created.", HttpStatusCode.Forbidden)
        {
        }
    }
}
