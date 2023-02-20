namespace Entities.ModelsEf;

public static class BooksDbEfNames
{
    public const string SqlConnectionString = "sqlConnection";

    //Has to be named as main project where ContextFolder and RepositoryContextFactor is (for inital migration and seeding)
    public const string Database = "BookApi";
}
