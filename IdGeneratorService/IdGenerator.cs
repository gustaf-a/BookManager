using Contracts;
using Microsoft.Extensions.Options;
using Shared.Configuration;

namespace IdGeneratorService;

public class IdGenerator : IIdGenerator
{
    private readonly string _idCharacterPrefix;
    private readonly int _idSequenceStartNumber;
    private readonly int _prefixLength;

    public IdGenerator(IOptions<DatabaseOptions> options)
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
            throw new Exception($"Found max ID '{currentMaxId}' doesn't match expected prefix {_idCharacterPrefix}");

        var newId = GetNextId(currentMaxId);

        return newId;
    }

    private bool ConfigurationPrefixMatches(ReadOnlySpan<char> currentMaxId)
        => currentMaxId.StartsWith(_idCharacterPrefix);

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
