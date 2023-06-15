using System.Runtime.Serialization;

// Solution:     SettingsManager
// Project:       SettingsManagerV60
// File:             SiteSettings.cs
// Created:      -- ()

namespace SettingsManager
{

#region info class

	[DataContract(Name = "SiteSettingInfoInfo")]
	public class SiteSettingInfo60<T> : SiteSettingInfoBase<T>
		where T : new ()
	{
		[DataMember]
		public override string DataClassVersion => "6.0as";
		public override string Description => "site setting file for SettingsManagerV60";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "SiteSettingData")]
	public class SiteSettingData60
	{
		[DataMember(Order = 1)]
		public int SiteSettingsValue { get; set; } = 6;
	}

#endregion

}