using System.Runtime.Serialization;

// projname: SettingsManagerV72
// itemname: UserSettingInfoInfo
// username: jeffs
// created:  12/23/2018 1:14:35 PM

namespace SettingsManager
{

#region info class

	[DataContract(Name = "UserSettingInfoInfo")]
	public class UserSettingInfo<T> : UserSettingInfoBase<T>
		where T : new ()
	{
		[DataMember]
		public override string DataClassVersion => "7.2u";
		public override string Description => "user setting file for SettingsManager v7.2";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "UserSettingData")]
	public class UserSettingData
	{
		[DataMember(Order = 1)]
		public int UserSettingsValue { get; set; } = 7;
	}

#endregion


}