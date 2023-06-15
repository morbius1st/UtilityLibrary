using System.Runtime.Serialization;

// Solution:     SettingsManager
// Project:       SettingsManagerV70
// File:             SiteSettings.cs
// Created:      -- ()

namespace SettingsManager
{

#region info class

	[DataContract(Name = "SiteSettingInfoInfo")]
	public class SiteSettingInfo70<T> : SiteSettingInfoBase<T>
		where T : new ()
	{
		[DataMember]
		public override string DataClassVersion => "7.0as";
		public override string Description => "site setting file for SettingsManagerV70";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "SiteSettingData")]
	public class SiteSettingData70
	{
		[DataMember(Order = 1)]
		public int SiteSettingsValue { get; set; } = 7;
	}

#endregion

}