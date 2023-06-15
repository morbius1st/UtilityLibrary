using System.Runtime.Serialization;

// User settings (per user) 
//  - user's settings for a specific app
//	- located in the user's app data folder / app name

// ReSharper disable once CheckNamespace

namespace SettingsManager
{
#region info class

	[DataContract(Name = "UserSettings", Namespace = "")]
	public class UserSettingInfo<T> : UserSettingInfoBase<T>
		where T : new ()
	{
		public UserSettingInfo()
		{
			// these are specific to this data file
			DataClassVersion = "user 7.2u";
			Description = "user setting file for SettingsManager v7.2";
			Notes = "any notes go here";
		}

		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Namespace = "")]
	public class UserSettingData
	{
		[DataMember(Order = 1)]
		public int UserSettingsValue { get; set; } = 7;
	}

#endregion
}