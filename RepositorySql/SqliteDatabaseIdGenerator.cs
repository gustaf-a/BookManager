using Contracts;
using Microsoft.Extensions.Options;
using RepositorySql.Configuration;

namespace RepositorySql;

public class SqliteDatabaseIdGenerator : IDatabaseIdGenerator
{
    private readonly string _idCharacterPrefix;
    private readonly int _idSequenceStartNumber;
    private readonly int _prefixLength;

    public SqliteDatabaseIdGenerator(IOptions<DatabaseOptions> options)
    {
        var databaseOptions = options.Value;

        _idCharacterPrefix = databaseOptions.IdCharacterPrefix;
        _idSequenceStartNumber = databaseOptions.IdSequenceStartNumber;
        _prefixLength = databaseOptions.IdCharacterPrefixLength;
    }

    public string GenerateId(string currentMaxId)
    {
        if (string.IsNullOrWhiteSpace(currentMaxId))
            return GetStartOfNewSequence();

        if (!ConfigurationPrefixMatches(currentMaxId))
            return GetStartOfNewSequence();

        var newId = GetNextId(currentMaxId);

        return newId;
    }

    private bool ConfigurationPrefixMatches(ReadOnlySpan<char> currentMaxId)
        => _idCharacterPrefix.Equals(currentMaxId[.._prefixLength].ToString());

    private string GetStartOfNewSequence()
        => _idCharacterPrefix + _idSequenceStartNumber;

    private string GetNextId(ReadOnlySpan<char> currentMaxId)
        => _idCharacterPrefix + GetIncrementedNumber(GetNumberFromId(currentMaxId));

    private static string GetIncrementedNumber(int number)
    {
        number++;
        return number.ToString();
    }

    private int GetNumberFromId(ReadOnlySpan<char> currentMaxId)
    {
        if (int.TryParse(currentMaxId[_prefixLength..], out int result))
            return result;

        throw new Exception($"Failed to get number from ID: {currentMaxId}");
    }
}
