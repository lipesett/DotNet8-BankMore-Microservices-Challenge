namespace ContaCorrente.Api.Application.Exceptions
{
    public class BusinessRuleViolationException : Exception
    {
        public string FailureType { get; }

        public BusinessRuleViolationException(string message, string failureType) : base(message)
        {
            FailureType = failureType;
        }
    }
}