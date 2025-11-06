// SWS.ApiCore/Converters/JsonDateOnlyConverter.cs
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SWS.ApiCore.Converters
{
    /// <summary>Converter cho DateOnly -> "yyyy-MM-dd"</summary>
    public sealed class JsonDateOnlyConverter : JsonConverter<DateOnly>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateOnly.Parse(reader.GetString()!);

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(Format));
    }

    /// <summary>Converter cho DateOnly? -> "yyyy-MM-dd" hoặc null</summary>
    public sealed class JsonNullableDateOnlyConverter : JsonConverter<DateOnly?>
    {
        private const string Format = "yyyy-MM-dd";

        public override DateOnly? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;
            var s = reader.GetString();
            return string.IsNullOrWhiteSpace(s) ? null : DateOnly.Parse(s);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly? value, JsonSerializerOptions options)
        {
            if (value is null) { writer.WriteNullValue(); return; }
            writer.WriteStringValue(value.Value.ToString(Format));
        }
    }
}
