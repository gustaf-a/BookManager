namespace Contracts;

public interface IIdGenerator
{
    public string GenerateId(string currentMaxId);
}
