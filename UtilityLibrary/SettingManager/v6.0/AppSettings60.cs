using System.Runtime.Serialization;

// projname: SettingsManagerV40
// itemname: AppSettingInfo60
// username: jeffs
// Created:      -- ()

namespace SettingsManager
{

#region info class

	[DataContract(Name = "AppSettingInfoInfo")]
	public class AppSettingInfo60<T> : AppSettingInfoBase<T>
		where T : new ()
	{
		[DataMember]
		public override string DataClassVersion => "6.0a";
		public override string Description => "app setting file for SettingsManagerV60";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "AppSettingData")]
	public class AppSettingData60
	{
		[DataMember(Order = 1)]
		public int AppSettingsValue { get; set; } = 6;
		
		[DataMember(Order = 2)]
		public string SiteRootPath { get; set; } =
			@"D:\Users\Jeff\OneDrive\Prior Folders\Office Stuff\CAD\Copy Y Drive & Office Standards\AppData";

	}

#endregion

}