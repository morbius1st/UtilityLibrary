using System.Runtime.Serialization;

// projname: SettingsManagerV40
// itemname: AppSettingInfo70
// username: jeffs
// Created:      -- ()

namespace SettingsManager
{

#region info class

	[DataContract(Name = "SuiteSettingInfoInfo")]
	public class SuiteSettingInfo70<T> : SuiteSettingInfoBase<T>
		where T : new ()
	{

		[DataMember]
		public override string DataClassVersion => "7.su";
		public override string Description => "suite setting file for SettingsManagerV70";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "SuiteSettingData")]
	public class SuiteSettingData70
	{
		[DataMember(Order = 1)]
		public int SuiteSettingsValue { get; set; } = 7;

		[DataMember(Order = 2)]
		public string SiteRootPath { get; set; }
			= @"B:\Programming\VisualStudioProjects\UtilityLibrary\UtilityLibrary\SettingManager\v7.0" ;

	}

#endregion

}