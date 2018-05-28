﻿using MongoDB.Bson;
using Newtonsoft.Json;
using System;

namespace Tools.Response.Json
{
    public class ObjectIdConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ObjectId);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType!=JsonToken.String)
            {
                throw new Exception(String.Format("Unexpected token parsing ObjectId. Expected String, got {0}.",reader.TokenType));
            }
            var value = (string)reader.Value;
            return String.IsNullOrEmpty(value) ? ObjectId.Empty : new ObjectId(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is ObjectId)
            {
                var objId = (ObjectId)value;

                writer.WriteValue(objId !=ObjectId.Empty ? objId.ToString():String.Empty);
            }
            else
            {
                throw new Exception("Expected ObjectId value.");
            }
        }
    }
}
