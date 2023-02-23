using Entities.ModelsEf;
using FluentAssertions;
using RepositoryEFCore.QueryHelper;
using Shared;
using System.Linq.Expressions;
using Neleus.LambdaCompare;
using Microsoft.EntityFrameworkCore;

namespace RepositoryEFCoreTests;

public class QueryHelperTests
{
    private List<BookEf> _books;

    public QueryHelperTests()
    {
        _books = new List<BookEf>
        {
            new BookEf
            {
                Id = "B1",
                Author = "Kutner, Joe",
                Title = "Deploying with JRuby",
                Genre = "Computer",
                Price = 33.00,
                PublishDate = "2012-08-15",
                Description = "Deploying with JRuby is the missing link between enjoying JRuby and using it in the real world to build high-performance, scalable applications."
            },
            new BookEf
            {
                Id = "B10",
                Author = "O'Brien, Tim",
                Title = "Microsoft .NET: The Programming Bible",
                Genre = "Computer",
                Price = 36.95,
                PublishDate = "2000-12-09",
                Description = "Microsoft's .NET initiative is explored in detail in this deep programmer's reference."
            },
            new BookEf
            {
                Id = "B11",
                Author = "Sydik, Jeremy J",
                Title = "Design Accessible Web Sites",
                Genre = "Computer",
                Price = 34.95,
                PublishDate = "2007-12-01",
                Description = "Accessibility has a reputation of being dull, dry, and unfriendly toward graphic design. But there is a better way: well-styled semantic markup that lets you provide the best possible results for all of your users. This book will help you provide images, video, Flash and PDF in an accessible way that looks great to your sighted users, but is still accessible to all users."
            },
            new BookEf
            {
                Id = "B2",
                Author = "Ralls, Kim",
                Title = "Midnight Rain",
                Genre = "Fantasy",
                Price = 5.95,
                PublishDate = "2000-12-16",
                Description = "A former architect battles corporate zombies, an evil sorceress, and her own childhood to become queen of the world."
            }
        };
    }

    [Fact]
    public void CreateFindExpression_ThrowsException_When_Book_Null()
    {
        Action callCreateFindExpression =
            () => QueryHelperBookEf.CreateFindExpression(null);

        callCreateFindExpression.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'readBooksRequest')");
    }

    [Fact]
    public void CreateFindExpression_ThrowsNotSupported_When_NoFilters()
    {
        var readBooksRequest = new ReadBooksRequest();

        Action callCreateFindExpression =
            () => QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        callCreateFindExpression.Should().Throw<NotImplementedException>()
            .WithMessage($"{nameof(ReadBooksRequest)} configuration not supported.");
    }

    [Fact]
    public void CreateFindExpression_SortById_Returns_FilterByContains_Id()
    {
        // Arrange
        var textFilter = "B";

        Expression<Func<BookEf, bool>> expectedExpression = b => EF.Functions.Like(b.Id, $"%{textFilter}%");

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Id),
            FilterByTextValue = textFilter
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_SortByAuthor_Returns_FilterByContains_Author()
    {
        // Arrange
        var textFilter = "Joe";

        Expression<Func<BookEf, bool>> expectedExpression = b => EF.Functions.Like(b.Author, $"%{textFilter}%");

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Author),
            FilterByTextValue = textFilter
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_SortByDescription_Returns_FilterByContains_Description()
    {
        // Arrange
        var textFilter = "best-seller";

        Expression<Func<BookEf, bool>> expectedExpression = b => EF.Functions.Like(b.Description, $"%{textFilter}%");

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Description),
            FilterByTextValue = textFilter
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_SortByGenre_Returns_FilterByContains_Genre()
    {
        // Arrange
        var textFilter = "Fantasy";

        Expression<Func<BookEf, bool>> expectedExpression = b => EF.Functions.Like(b.Genre, $"%{textFilter}%");

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Genre),
            FilterByTextValue = textFilter
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_SortByTitle_Returns_FilterByContains_Title()
    {
        // Arrange
        var textFilter = ".NET";

        Expression<Func<BookEf, bool>> expectedExpression = b => EF.Functions.Like(b.Title, $"%{textFilter}%");

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Title),
            FilterByTextValue = textFilter
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_SortByPublishDate_PrecisionDay_Returns_FilterBy_StartsWith_CompleteDate()
    {
        // Arrange
        var filterByDateSubstring = "2023-02-03";

        Expression<Func<BookEf, bool>> expectedExpression = b => b.PublishDate.StartsWith(filterByDateSubstring);

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.PublishDate),
            FilterByDateValue = new DateOnly(2023, 2, 3),
            FilterByDatePrecision = ReadBooksRequest.DatePrecision.Day
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_SortByPublishDate_PrecisionMonth_Returns_FilterBy_StartsWith_Month()
    {
        // Arrange
        var filterByDateSubstring = "2023-02";

        Expression<Func<BookEf, bool>> expectedExpression = b => b.PublishDate.StartsWith(filterByDateSubstring);

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Title),
            FilterByDateValue = new DateOnly(2023, 2, 3),
            FilterByDatePrecision = ReadBooksRequest.DatePrecision.Month
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_SortByPublishDate_PrecisionYear_Returns_FilterBy_StartsWith_Year()
    {
        // Arrange
        var filterByDateSubstring = "2023";

        Expression<Func<BookEf, bool>> expectedExpression = b => b.PublishDate.StartsWith(filterByDateSubstring);

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Title),
            FilterByDateValue = new DateOnly(2023, 2, 3),
            FilterByDatePrecision = ReadBooksRequest.DatePrecision.Year
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_ByPrice()
    {
        // Arrange
        var filterByPrice = 30.0;

        Expression<Func<BookEf, bool>> expectedExpression = b => b.Price == filterByPrice;

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Price),
            FilterByDoubleValue = filterByPrice,
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void CreateFindExpression_ByPrice_Ranged()
    {
        // Arrange
        var filterByPrice = 30.0;
        var filterByPrice2 = 35.0;

        Expression<Func<BookEf, bool>> expectedExpression = b => filterByPrice <= b.Price && b.Price <= filterByPrice2;

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = nameof(Book.Price),
            FilterByDoubleValue = filterByPrice,
            FilterByDoubleValue2= filterByPrice2,
        };

        // Act
        var result = QueryHelperBookEf.CreateFindExpression(readBooksRequest);

        // Assert
        Assert.True(Lambda.Eq(expectedExpression, result));
    }

    [Fact]
    public void OrderBooksBy_Throws_If_Request_Null()
    {
        Action callOrderBooksBy =
                    () => QueryHelperBookEf.OrderBooksBy(null, null);

        callOrderBooksBy.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'readBooksRequest')");
    }

    [Fact]
    public void OrderBooksBy_Throws_If_Books_Null()
    {
        var readBooksRequest = new ReadBooksRequest();

        Action callOrderBooksBy =
                    () => QueryHelperBookEf.OrderBooksBy(null, readBooksRequest);

        callOrderBooksBy.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'books')");
    }

    [Fact]
    public void OrderBooksBy_Returns_IfEmpty()
    {
        // Arrange
        IQueryable<BookEf> books = new List<BookEf>().AsQueryable();

        var readBooksRequest = new ReadBooksRequest
        {
        };

        // Act
        QueryHelperBookEf.OrderBooksBy(books, readBooksRequest);

        // Assert
        Assert.Empty(books);
    }

    [Theory]
    [InlineData(nameof(Book.Id), "B1 B2 B10 B11")]
    [InlineData(nameof(Book.Author), "B1 B10 B2 B11")]
    [InlineData(nameof(Book.Description), "B2 B11 B1 B10")]
    [InlineData(nameof(Book.Genre), "B1 B10 B11 B2")]
    [InlineData(nameof(Book.Price), "B2 B1 B11 B10")]
    [InlineData(nameof(Book.PublishDate), "B10 B2 B11 B1")]
    [InlineData(nameof(Book.Title), "B1 B11 B10 B2")]
    public void OrderBooksBy_SortByField(string sortByField, string expectedIdOrder)
    {
        // Arrange
        IQueryable<BookEf> books = _books.AsQueryable();

        var readBooksRequest = new ReadBooksRequest
        {
            SortResultByField = sortByField
        };

        // Act
        var sortedBooks = QueryHelperBookEf.OrderBooksBy(books, readBooksRequest);

        // Assert
        var sortedBooksIdOrder = string.Join(" ", sortedBooks.Select(b => b.Id));

        sortedBooksIdOrder.Should().Be(expectedIdOrder);
    }

    [Fact]
    public void FindMaxCurrentId_Returns_Empty_WhenNoBooks()
    {
        // Arrange
        IQueryable<BookEf> books = new List<BookEf>().AsQueryable();

        // Act
        var foundMaxId = QueryHelperBookEf.FindMaxCurrentId(books, "B");

        // Assert
        foundMaxId.Should().BeEmpty();
    }

    [Fact]
    public void FindMaxCurrentId_Returns_MaxId_AmongBooks()
    {
        // Arrange
        var expectedMaxId = "B11";

        IQueryable<BookEf> books = _books.AsQueryable();

        // Act
        var foundMaxId = QueryHelperBookEf.FindMaxCurrentId(books, "B");

        // Assert
        foundMaxId.Should().Be(expectedMaxId);
    }

    [Theory]
    [InlineData(1, 10, 0)]
    [InlineData(2, 10, 10)]
    [InlineData(5, 10, 40)]
    [InlineData(2, 5, 5)]
    [InlineData(3, 5, 10)]
    [InlineData(2, 50, 50)]
    public void GetItemsToSkip_Returns_Correct(int pageNumber, int pageSize, int expectedToSkip)
    {
        // Act
        var itemsToSkip = QueryHelperBookEf.GetItemsToSkip(pageNumber, pageSize);

        // Assert
        itemsToSkip.Should().Be(expectedToSkip);
    }
}