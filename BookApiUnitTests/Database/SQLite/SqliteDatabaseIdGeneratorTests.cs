using BookApi.Data;
using BookApi.Database;
using BookApi.Database.SQLite;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Text;

namespace BookApiUnitTests.Database.SQLite;

public class SqliteDatabaseIdGeneratorTests
{

    private const string AppSettingsJson =
@"{
    ""Database"": {
        ""IdNumberMaxLength"": 9,
        ""IdCharacterPrefix"": ""B""
    }
}";

    private readonly ConfigurationBuilder _configurationBuilder;

    public SqliteDatabaseIdGeneratorTests()
    {
        //It's not possible to mock the IConfiguration as Get<>() is an extension method, so this is a working method
        _configurationBuilder = new ConfigurationBuilder();
        _configurationBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(AppSettingsJson)));
    }

    [Fact]
    public void GenerateId_StartsNewSequence_When_Database_Returns_Null()
    {
        // Arrange
        var databaseAccessMock = new Mock<IDatabaseAccess>();
        databaseAccessMock.Setup(d => d.GetValue(It.IsAny<GetValueRequest>())).Returns("");

        var databaseIdGenerator = new SqliteDatabaseIdGenerator(databaseAccessMock.Object, _configurationBuilder.Build());

        // Act
        var newId = databaseIdGenerator.GenerateId();

        // Assert
        newId.Should().Be("B1");
    }

    [Theory]
    [InlineData("B1", "B2")]
    [InlineData("B100", "B101")]
    public void GenerateId_Increments_IdReturned_By1(string currentMaxId, string expectedNewId)
    {
        // Arrange
        var databaseAccessMock = new Mock<IDatabaseAccess>();
        databaseAccessMock.Setup(d => d.GetValue(It.IsAny<GetValueRequest>())).Returns(currentMaxId);

        var databaseIdGenerator = new SqliteDatabaseIdGenerator(databaseAccessMock.Object, _configurationBuilder.Build());

        // Act
        var newId = databaseIdGenerator.GenerateId();

        // Assert
        newId.Should().Be(expectedNewId);
    }

    [Theory]
    [InlineData("A5", "B1")]
    [InlineData("A1", "B1")]
    public void GenerateId_StartsOnNewIdSequence_WhenCurrentIdPrefix_DifferentFromConfigured(string currentMaxId, string expectedNewId)
    {
        // Arrange
        var databaseAccessMock = new Mock<IDatabaseAccess>();
        databaseAccessMock.Setup(d => d.GetValue(It.IsAny<GetValueRequest>())).Returns(currentMaxId);

        var databaseIdGenerator = new SqliteDatabaseIdGenerator(databaseAccessMock.Object, _configurationBuilder.Build());

        // Act
        var newId = databaseIdGenerator.GenerateId();

        // Assert
        newId.Should().Be(expectedNewId);
    }
}
