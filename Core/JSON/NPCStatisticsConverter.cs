using FunnyExperience.Core.Mechanics;
using FunnyExperience.Core.Utility;
using FunnyExperience.Core.Utility.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace FunnyExperience.Core.JSON{
	public class NpcStatisticsConverter : JsonConverter{
		public override bool CanConvert(Type objectType)
			=> objectType == typeof(NpcStatistics);

		public override bool CanRead => true;

		public override bool CanWrite => true;

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer){
			if(value is not NpcStatistics stat)
				return;

			writer.WriteStartObject();
			writer.WritePropertyName("mod");
			writer.WriteStartObject();
			writer.WritePropertyName("hp");
			serializer.Serialize(writer, stat.HealthModifier);
			writer.WritePropertyName("scale");
			serializer.Serialize(writer, stat.ScaleModifier);
			writer.WriteEndObject();

			writer.WriteEndObject();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer){
			if(reader.TokenType == JsonToken.Null)
				return null;

			NpcStatistics obj = new();
			var o = JToken.ReadFrom(reader) as JObject;
			obj.HealthModifier = o.GetObject<Modifier>("mod.hp");
			obj.ScaleModifier = o.GetObject<Modifier>("mod.scale");

			return obj;
		}
	}
}
