﻿namespace RepositorySql.Database;

public interface IDatabaseIdGenerator
{
    public Task<string> GenerateId();
}