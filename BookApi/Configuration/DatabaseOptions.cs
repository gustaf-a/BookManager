﻿namespace BookApi.Configuration;

public class DatabaseOptions
{
    public const string Database = "Database";

    public string SqliteConnectionStringName { get; set; } = string.Empty;
    public string BooksTableName { get; set; } = string.Empty;
    public int IdNumberMaxLength { get; set; } = 10;
    public int IdCharacterPrefixLength { get; set; } = 1;
}