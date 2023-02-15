using BookApi.Configuration;
using BookApi.Data;

namespace BookApi.Database.SQLite;

public class SqliteDatabaseIdGenerator : IDatabaseIdGenerator
{
    private readonly string _idCharacterPrefix;
    private readonly int _idSequenceStartNumber;
    private readonly int _prefixLength;

    private readonly IDatabaseAccess _databaseAccess;

    public SqliteDatabaseIdGenerator(IDatabaseAccess databaseAccess, IConfiguration configuration)
    {
        _databaseAccess = databaseAccess ?? throw new ArgumentNullException(nameof(IDatabaseAccess)); ;

        var databaseOptions = configuration.GetSection(DatabaseOptions.Database).Get<DatabaseOptions>()
                                ?? throw new ArgumentNullException(nameof(DatabaseOptions));

        _idCharacterPrefix = databaseOptions.IdCharacterPrefix;
        _idSequenceStartNumber = databaseOptions.IdSequenceStartNumber;
        _prefixLength = databaseOptions.IdCharacterPrefixLength;
    }

    public async Task<string> GenerateId()
    {
        var getValuesRequest = new GetValueRequest
        {
            ColumnName = nameof(BookSqlite.Id),
            IgnoreFirstCharacters = _prefixLength + 1,
            GetMaxValue = true
        };

        var currentMaxId = await _databaseAccess.GetValue(getValuesRequest);

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
