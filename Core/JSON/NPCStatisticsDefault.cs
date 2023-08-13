using FunnyExperience.Core.Mechanics;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FunnyExperience.Core.JSON{
	[DataContract]
	public class NpcStatisticsDefault{
		[DataMember(Name = "name")]
		[DefaultValue(null)]
		public string Name{ get; set; }
	}
}
