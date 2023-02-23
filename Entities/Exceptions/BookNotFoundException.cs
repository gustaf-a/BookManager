namespace Entities.Exceptions;

public sealed class BookNotFoundException :NotFoundException
{
	public BookNotFoundException(string bookId)
		: base($"Book with ID: {bookId} couldn't be found.")
	{

	}
}
