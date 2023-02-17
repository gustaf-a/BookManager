using Shared;

namespace RepositorySql.Database.SQLite;

internal static class Extensions
{
    public static int ToSubstringLength(this ReadBooksRequest.DatePrecision datePrecision)
        => datePrecision switch
        {
            ReadBooksRequest.DatePrecision.None => 0,
            ReadBooksRequest.DatePrecision.Year => 4,
            ReadBooksRequest.DatePrecision.Month => 7,
            ReadBooksRequest.DatePrecision.Day => 10,
            _ => throw new NotImplementedException($"Substring length not found for DatePrecision {datePrecision}.")
        };

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
