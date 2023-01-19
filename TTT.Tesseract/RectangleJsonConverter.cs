using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TTT.Tesseract;

public class RectangleJsonConverter : JsonConverter<Rectangle>
{
    public static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true, Converters = { new RectangleJsonConverter() }
    };

    public override Rectangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var split = reader.GetString()?.Split(",");
        if (split is null) throw new NullReferenceException(nameof(split));
        return new Rectangle(
            int.Parse(split[0]), 
            int.Parse(split[1]), 
            int.Parse(split[2]), 
            int.Parse(split[3]));
    }

    public override void Write(Utf8JsonWriter writer, Rectangle rect, JsonSerializerOptions options)
    {
        writer.WriteStringValue($"{rect.X},{rect.Y},{rect.Width},{rect.Height}");
    }
}