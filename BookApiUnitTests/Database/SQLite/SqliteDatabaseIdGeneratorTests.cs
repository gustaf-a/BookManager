using Microsoft.Extensions.Options;
using RepositorySql;
using RepositorySql.Configuration;

namespace BookApiUnitTests.Database.SQLite;

public class SqliteDatabaseIdGeneratorTests
{
    private readonly IOptions<DatabaseOptions> _databaseOptions;

    public SqliteDatabaseIdGeneratorTests()
    {
        var databaseOptions = new DatabaseOptions
        {
            IdCharacterPrefix= "B",
            IdNumberMaxLength= 9
        };

        _databaseOptions = Options.Create(databaseOptions);
    }

    [Fact]
    public void GenerateId_StartsNewSequence_When_Database_Returns_Null()
    {
        // Arrange
        var databaseIdGenerator = new SqliteDatabaseIdGenerator(_databaseOptions);

        // Act
        var newId = databaseIdGenerator.GenerateId("");

        // Assert
        newId.Should().Be("B1");
    }

    [Theory]
    [InlineData("B1", "B2")]
    [InlineData("B100", "B101")]
    public void GenerateId_Increments_IdReturned_By1(string currentMaxId, string expectedNewId)
    {
        // Arrange
        var databaseIdGenerator = new SqliteDatabaseIdGenerator(_databaseOptions);

        // Act
        var newId = databaseIdGenerator.GenerateId(currentMaxId);

        // Assert
        newId.Should().Be(expectedNewId);
    }

    [Theory]
    [InlineData("A5", "B1")]
    [InlineData("A1", "B1")]
    public void GenerateId_StartsOnNewIdSequence_WhenCurrentIdPrefix_DifferentFromConfigured(string currentMaxId, string expectedNewId)
    {
        // Arrange
        var databaseIdGenerator = new SqliteDatabaseIdGenerator(_databaseOptions);

        // Act
        var newId = databaseIdGenerator.GenerateId(currentMaxId);

        // Assert
        newId.Should().Be(expectedNewId);
    }
}
