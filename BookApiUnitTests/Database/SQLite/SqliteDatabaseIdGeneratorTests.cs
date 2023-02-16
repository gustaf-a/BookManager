using Entities.Data;
using Microsoft.Extensions.Options;
using Moq;
using RepositorySql.Configuration;
using RepositorySql.Database;
using RepositorySql.Database.SQLite;

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
    public async Task GenerateId_StartsNewSequence_When_Database_Returns_Null()
    {
        // Arrange
        var databaseAccessMock = new Mock<IDatabaseAccess>();
        databaseAccessMock.Setup(d => d.GetValue(It.IsAny<GetValueRequest>())).ReturnsAsync("");

        var databaseIdGenerator = new SqliteDatabaseIdGenerator(databaseAccessMock.Object, _databaseOptions);

        // Act
        var newId = await databaseIdGenerator.GenerateId();

        // Assert
        newId.Should().Be("B1");
    }

    [Theory]
    [InlineData("B1", "B2")]
    [InlineData("B100", "B101")]
    public async Task GenerateId_Increments_IdReturned_By1(string currentMaxId, string expectedNewId)
    {
        // Arrange
        var databaseAccessMock = new Mock<IDatabaseAccess>();
        databaseAccessMock.Setup(d => d.GetValue(It.IsAny<GetValueRequest>())).ReturnsAsync(currentMaxId);

        var databaseIdGenerator = new SqliteDatabaseIdGenerator(databaseAccessMock.Object, _databaseOptions);

        // Act
        var newId = await databaseIdGenerator.GenerateId();

        // Assert
        newId.Should().Be(expectedNewId);
    }

    [Theory]
    [InlineData("A5", "B1")]
    [InlineData("A1", "B1")]
    public async Task GenerateId_StartsOnNewIdSequence_WhenCurrentIdPrefix_DifferentFromConfigured(string currentMaxId, string expectedNewId)
    {
        // Arrange
        var databaseAccessMock = new Mock<IDatabaseAccess>();
        databaseAccessMock.Setup(d => d.GetValue(It.IsAny<GetValueRequest>())).ReturnsAsync(currentMaxId);

        var databaseIdGenerator = new SqliteDatabaseIdGenerator(databaseAccessMock.Object, _databaseOptions);

        // Act
        var newId = await databaseIdGenerator.GenerateId();

        // Assert
        newId.Should().Be(expectedNewId);
    }
}
