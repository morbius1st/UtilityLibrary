using System.Runtime.Serialization;

// projname: SettingsManagerV40
// itemname: AppSettingInfo70
// username: jeffs
// Created:      -- ()

namespace SettingsManager
{

#region info class

	[DataContract(Name = "AppSettingInfoInfo", Namespace = "")]
	public class AppSettingInfo70<T> : AppSettingInfoBase<T>
		where T : new ()
	{
		public override string DataClassVersion => "7.0a";
		public override string Description => "app setting file for SettingsManagerV70";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "AppSettingData")]
	public class AppSettingData70
	{
		[DataMember(Order = 1)]
		public int AppSettingsValue { get; set; } = 7;


	}

#endregion

}