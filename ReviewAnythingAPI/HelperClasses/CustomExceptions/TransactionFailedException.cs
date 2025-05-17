namespace ReviewAnythingAPI.HelperClasses.CustomExceptions;

public class TransactionFailedException : Exception
{
    public TransactionFailedException(string message, Exception? innerException = null) : base(message, innerException) {}
}