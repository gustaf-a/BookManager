namespace BookApi.Database;

public interface IDatabaseIdGenerator
{
    public Task<string> GenerateId();
}
