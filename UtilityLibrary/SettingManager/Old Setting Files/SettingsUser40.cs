using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using SettingManager;

using static UtilityLibrary.MessageUtilities2;

// projname: SettingsManagerV40
// itemname: UserSettingInfo40
// username: jeffs
// created:  12/23/2018 1:14:35 PM


namespace SettingsManagerV40
{

#region user data class

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "UserSettingData40")]
	public class UserSettingData40
	{
		[DataMember(Order = 1)]
		public int UserSettingsValue { get; set; } = 4;

	}

#endregion

#region management info class

	[DataContract(Name = "UserSettingInfo40")]
	public class UserSettingInfo40 : UserSettingBase
	{
		[DataMember]
		public UserSettingData40 Data = new UserSettingData40();

		public override string ClassVersion => "4.0u";
		public override string Description => "user setting file for SettingsManagerV40";
		public override void UpgradeFromPrior(SettingBase prior) { }
	}

#endregion

/*
#region management root class

	public static class UserSettings
	{
		// this is the primary data structure - it holds the settings
		// configuration information as well as the setting data
		public static SettingsMgr<UserSettingInfo40> Admin { get; private set; }

		// this is just the setting data - this is a shortcut to
		// the setting data
		public static UserSettingInfo40 Info { get; private set; }
		public static UserSettingData40 Data { get; private set; }

		// initialize and create the setting objects
		static UserSettings()
		{
			Admin = new SettingsMgr<UserSettingInfo40>(ResetData);
			Info = Admin.Info;
			Data = Info.Data;

			MainWindow.Instance.MsgLeftLine("");
			MainWindow.Instance.MsgLeftLine("at ctor UserSettings|   status", Admin.Status.ToString());
			MainWindow.Instance.MsgLeftLine("at ctor UserSettings|     path", Admin.Info.SettingPath);
			MainWindow.Instance.MsgLeftLine("at ctor UserSettings| filename", Admin.Info.FileName);

		}

		public static void ResetData()
		{
			// this makes sure the above static class points
			// to the current data structure
			Info  = Admin.Info;
			Data  = Info.Data;
		}
	}

#endregion
*/
}
