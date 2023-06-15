using System.Runtime.Serialization;

// Solution:     SettingsManager
// Project:       SettingsManagerV70
// File:             MachineSettings.cs
// Created:      -- ()

namespace SettingsManager
{

#region info class

	[DataContract(Name = "MachSettingInfoInfo")]
	public class MachSettingInfo70<T> : MachSettingInfoBase<T>
		where T : new ()
	{
		[DataMember]
		public override string DataClassVersion => "7.0m";
		public override string Description => "machine setting file for SettingsManagerV70";
		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "MachSettingData")]
	public class MachSettingData70
	{
		[DataMember(Order = 1)]
		public int MachSettingsValue { get; set; } = 7;
	}

#endregion

}