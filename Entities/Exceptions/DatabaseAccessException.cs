namespace Entities.Exceptions;

public abstract class DatabaseAccessException : Exception
{
	protected DatabaseAccessException(string msg)
        : base(msg) { }
}
