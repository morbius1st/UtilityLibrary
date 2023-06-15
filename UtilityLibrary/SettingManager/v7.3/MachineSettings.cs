using System.Runtime.Serialization;

// Mach settings (per suite)
//	- applies to a specific machine / all users on that machine
//	- holds information needed by or shared between all users on the machine
//	- located in the common app data folder (currently c:\program data)		


// ReSharper disable once CheckNamespace

namespace SettingsManager
{
#region info class

	[DataContract(Name = "MachSettings", Namespace = "")]
	public class MachSettingInfo<T> : MachSettingInfoBase<T>
		where T : new ()
	{
		public MachSettingInfo()
		{
			// these are specific to this data file
			DataClassVersion =  "mach 7.2m";
			Description  =  "machine setting file for SettingsManager v7.2";
			Notes = "any notes go here";
		}

		public override void UpgradeFromPrior(SettingInfoBase<T> prior) { }
	}

#endregion

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Namespace = "")]
	public class MachSettingData
	{
		[DataMember(Order = 1)]
		public int MachSettingsValue { get; set; } = 7;
	}

#endregion
}