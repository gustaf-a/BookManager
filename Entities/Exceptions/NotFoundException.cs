namespace Entities.Exceptions;

public abstract class NotFoundException : Exception
{
    protected NotFoundException(string msg)
        : base(msg) { }
}
