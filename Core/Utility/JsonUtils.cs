using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using System;
using Newtonsoft.Json;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;

namespace FunnyExperience.Core.Utility {
	public static class JsonUtils {
		public static JsonSerializerSettings GetSerializationSettings() {
			return new JsonSerializerSettings() {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				DefaultValueHandling = DefaultValueHandling.Populate
			};
		}

		public static JsonSerializerSettings GetDeserializationSettings() {
			return new JsonSerializerSettings() {
				MissingMemberHandling = MissingMemberHandling.Ignore,
				ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
				FloatFormatHandling = FloatFormatHandling.DefaultValue,
				PreserveReferencesHandling = PreserveReferencesHandling.All
			};
		}
	}
}