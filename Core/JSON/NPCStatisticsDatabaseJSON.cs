using FunnyExperience.Core.Mechanics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FunnyExperience.Core.JSON{
	[DataContract]
	public class NpcStatisticsDatabaseJson{
		[DataMember(Name = "entries")]
		[DefaultValue(null)]
		public IList<NpcStatisticsDatabaseEntryJson> Database{ get; set; }
		[DataMember(Name = "defaults")]
		[DefaultValue(null)]
		public IList<NpcStatisticsDefault> Defaults { get; set; }
	}
}
