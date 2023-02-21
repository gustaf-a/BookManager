namespace RepositorySql;

internal static class Extensions
{
    public static void AddPlaceholderParameters(this SqlQuery sqlQuery, Dictionary<string, object> properties)
    {
        foreach (var property in properties)
            sqlQuery.Parameters.Add(property.Key.GetPlaceHolder(), property.Value);
    }

    public static string GetPlaceHolderList(this IEnumerable<string> variableNames)
        => string.Join(',', variableNames.Select(vn => vn.GetPlaceHolder()));

    public static string GetPlaceHolder(this string variableName)
        => $"@{variableName}";
}
