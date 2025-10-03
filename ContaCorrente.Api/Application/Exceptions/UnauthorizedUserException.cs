namespace ContaCorrente.Api.Application.Exceptions
{
    public class UnauthorizedUserException : Exception
    {
        public UnauthorizedUserException(string message) : base(message)
        {
        }
    }
}