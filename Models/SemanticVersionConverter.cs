using Newtonsoft.Json;
using NuGet.Versioning;
using System;
using System.Diagnostics.CodeAnalysis;

namespace FoundryGet.Models
{
    public class SemanticVersionConverter : JsonConverter<SemanticVersion>
    {
        public override SemanticVersion ReadJson(JsonReader reader, Type objectType, [AllowNull] SemanticVersion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var jsonValue = (string)reader.Value;
            return SemanticVersion.Parse(jsonValue);
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] SemanticVersion value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToNormalizedString());
        }
    }
}
