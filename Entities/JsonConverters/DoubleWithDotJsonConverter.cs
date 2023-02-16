using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Entities.JsonConverters;

/// <summary>
/// System.Text.Json JsonSerializer has a problem converting the X.XX double values and they're working on being able to set culture,
/// but right now it doesn't seem possible. (2023-02-12, see RefToMadeUpJiraTicket00001)
/// </summary>
public class DoubleWithDotJsonConverter : JsonConverter<double>
{
    public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return reader.GetDouble();

        var stringValue = reader.GetString() ?? string.Empty;

        return Convert.ToDouble(stringValue, CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(CultureInfo.InvariantCulture));
    }
}