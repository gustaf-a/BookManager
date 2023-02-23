namespace Entities.Exceptions;

public sealed class TooManyBooksFoundException : DatabaseAccessException
{
	public TooManyBooksFoundException(string bookId, int booksFound)
		: base($"When looking for book with ID: {bookId} found too many books. Found: {booksFound} books")
	{
	}
}
