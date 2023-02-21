namespace Contracts;

public interface IDatabaseIdGenerator
{
    public Task<string> GenerateId();
}
