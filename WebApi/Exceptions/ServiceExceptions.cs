namespace WebApi.Exceptions;

public class ServiceException : Exception
{
    public ServiceException()
    {
    }

    public ServiceException(string message)
        : base(message)
    {
    }

    public ServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

public class ServiceValidationException : ServiceException
{
    public ServiceValidationException()
    {
    }

    public ServiceValidationException(string message)
        : base(message)
    {
    }

    public ServiceValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
