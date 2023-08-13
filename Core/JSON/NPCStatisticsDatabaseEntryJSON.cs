using FunnyExperience.Core.Mechanics;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FunnyExperience.Core.JSON{
	[DataContract]
	public class NpcStatisticsDatabaseEntryJson{
		[DataMember(Name = "prefix")]
		[DefaultValue(null)]
		public string NamePrefix{ get; set; }

		[DataMember(Name = "weight")]
		[DefaultValue(0f)]
		public float Weight{ get; set; }

		[DataMember(Name = "stats")]
		public NpcStatistics Stats{ get; set; }

		[DataMember(Name = "requirements")]
		[DefaultValue(null)]
		public string RequirementKeys{ get; set; }
	}
}
