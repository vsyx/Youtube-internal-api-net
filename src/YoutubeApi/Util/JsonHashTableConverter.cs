using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#nullable disable
namespace YoutubeApi.Util
{
    // source: https://github.com/dotnet/runtime/issues/29960
    public class JsonHashtableConverter : JsonConverterFactory
    {
        private static JsonConverter<Hashtable> _valueConverter = null;

        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(Hashtable);
        }

        public override JsonConverter CreateConverter(Type type, JsonSerializerOptions options)
        {
            return _valueConverter ?? (_valueConverter = new HashtableConverterInner(options));
        }

        private class HashtableConverterInner : JsonConverter<Hashtable>
        {
            private JsonSerializerOptions _options;
            private JsonConverter<Hashtable> _valueConverter = null;

            JsonConverter<Hashtable> converter
            {
                get
                {
                    return _valueConverter ?? (_valueConverter = (JsonConverter<Hashtable>)_options.GetConverter(typeof(Hashtable)));
                }
            }

            public HashtableConverterInner(JsonSerializerOptions options)
            {
                _options = options;
            }

            public override Hashtable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                Hashtable hashtable = new Hashtable();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return hashtable;
                    }

                    // Get the key.
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string propertyName = reader.GetString();
                    reader.Read();

                    hashtable[propertyName] = getValue(ref reader, options);
                }
                return hashtable;
            }

            private object getValue(ref Utf8JsonReader reader, JsonSerializerOptions options)
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.String:
                        return reader.GetString();
                    case JsonTokenType.False:
                        return false;
                    case JsonTokenType.True:
                        return true;
                    case JsonTokenType.Null:
                        return null;
                    case JsonTokenType.Number:
                        if (reader.TryGetInt64(out long _long))
                            return _long;
                        else if (reader.TryGetDecimal(out decimal _dec))
                            return _dec;
                        throw new JsonException($"Unhandled Number value");
                    case JsonTokenType.StartObject:
                        return JsonSerializer.Deserialize<Hashtable>(ref reader, options);
                    case JsonTokenType.StartArray:
                        List<object> array = new List<object>();
                        while (reader.Read() &&
                                reader.TokenType != JsonTokenType.EndArray)
                        {
                            array.Add(getValue(ref reader, options));
                        }
                        return array.ToArray();
                }
                throw new JsonException($"Unhandled TokenType {reader.TokenType}");
            }

            public override void Write(Utf8JsonWriter writer, Hashtable hashtable, JsonSerializerOptions options)
            {
                writer.WriteStartObject();

                foreach (KeyValuePair<string, object> kvp in hashtable)
                {
                    writer.WritePropertyName(kvp.Key);

                    if (converter != null &&
                            kvp.Value is Hashtable)
                    {
                        converter.Write(writer, (Hashtable)kvp.Value, options);
                    }
                    else
                    {
                        JsonSerializer.Serialize(writer, kvp.Value, options);
                    }
                }

                writer.WriteEndObject();
            }
        }
    }
}
