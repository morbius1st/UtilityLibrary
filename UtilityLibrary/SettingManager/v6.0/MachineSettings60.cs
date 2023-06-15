using System.Runtime.Serialization;

// Solution:     SettingsManager
// Project:       SettingsManagerV60
// File:             MachineSettings.cs
// Created:      -- ()

namespace SettingsManager
{

#region info class

	[DataContract(Name = "MachSettingInfoInfo")]
	public class MachSettingInfo60<T> : MachSettingInfoBase<T>
		where T : new ()
	{
		[DataMember]
		public override string DataClassVersion => "6.0m";
		public override string Description => "machine setting file for SettingsManagerV60";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "MachSettingData")]
	public class MachSettingData60
	{
		[DataMember(Order = 1)]
		public int MachSettingsValue { get; set; } = 6;
	}

#endregion

}