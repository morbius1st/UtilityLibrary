using System.Runtime.Serialization;

// App settings (per user)
//	- applies to a specific app in the suite
//	- holds information specific to the app
//	- located in the user's app data folder / app name / AppSettings

// ReSharper disable once CheckNamespace

namespace SettingsManager
{
#region info class

	[DataContract(Name = "AppSettings", Namespace = "")]
	public class AppSettingInfo<T> : AppSettingInfoBase<T>
		where T : new ()
	{
		public AppSettingInfo()
		{
			// these are specific to this data file
			DataClassVersion =  "app 7.2a";
			Description = "app setting file for SettingsManager v7.2";
			Notes = "any notes go here";
		}

		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Namespace = "")]
	public class AppSettingData
	{
		[DataMember(Order = 1)]
		public int AppSettingsValue { get; set; } = 7;
	}

#endregion
}