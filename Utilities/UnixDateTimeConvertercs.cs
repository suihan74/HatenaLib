using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Utilities
{
    public class UnixDateTimeConverter : DateTimeConverterBase
    {
        private static readonly DateTime EpocDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.Integer)
            {
                throw new JsonReaderException($"Unexpected token parsing date. Expected Integer, got {reader.TokenType}");
            }
            var ticks = (long)reader.Value;
            return EpocDateTime.AddSeconds(ticks).ToLocalTime();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime dt)
            {
                var delta = dt.ToUniversalTime() - EpocDateTime;
                var unixtime = (long)delta.TotalSeconds;
                writer.WriteValue(unixtime);
            }
            else
            {
                throw new JsonWriterException("Expected DateTime value.");
            }
        }
    }
}
