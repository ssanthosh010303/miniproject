/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.Data.Common;

namespace WebApi.Exceptions;

public class RepositoryException : DbException
{
    public RepositoryException()
    {
    }

    public RepositoryException(string message) : base(message)
    {
    }

    public RepositoryException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

public class EntityNotFoundException : RepositoryException
{
    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(string message) : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
