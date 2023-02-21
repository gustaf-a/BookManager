namespace Contracts;

public interface IDatabaseIdGenerator
{
    public string GenerateId(string currentMaxId);
}
