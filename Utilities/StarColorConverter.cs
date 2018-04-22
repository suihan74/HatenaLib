using HatenaLib.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HatenaLib.Utilities
{
    public class StarColorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(StarColor);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new JsonReaderException($"Unexpected token parsing date. Expected String, got {reader.TokenType}");
            }

            var str = reader.Value as string;
            switch (str)
            {
                case "red": return StarColor.Red;
                case "green": return StarColor.Green;
                case "blue": return StarColor.Blue;
                default: return StarColor.Yellow;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is StarColor color)
            {
                string str = string.Empty;
                switch (color)
                {
                    case StarColor.Red: str = "red"; break;
                    case StarColor.Green: str = "green"; break;
                    case StarColor.Blue: str = "blue"; break;
                    default: str = "yellow"; break;
                }
                writer.WriteValue(str);
            }
            else
            {
                throw new JsonWriterException("Expected DateTime value.");
            }
        }
    }
}
