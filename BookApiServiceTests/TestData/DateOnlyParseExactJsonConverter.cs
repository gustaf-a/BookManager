using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace BookApiServiceTests.TestData;

/// <summary>
/// Helps to convert yyyy-MM-dd strings to DateOnly
/// It should also be possible to use the new NewtonSoft v 13.0.2, but I include this way of doing it 
/// as it's important for me to show that I can implement the a custom JsonConverter attribute.
/// </summary>
public class DateOnlyParseExactJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString() ?? string.Empty;

        return DateOnly.ParseExact(value, DateFormat, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
    }
}
