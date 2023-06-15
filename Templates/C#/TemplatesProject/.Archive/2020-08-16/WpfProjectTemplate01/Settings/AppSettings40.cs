using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using SettingManager;
using UtilityLibrary;

// projname: SettingsManagerV40
// itemname: AppSettingInfo40
// username: jeffs

namespace SettingsManagerV40
{
#region data classes

#region app settings

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "AppSettingData40")]
	public class AppSettingData40
	{
		[DataMember(Order = 1)]
		public int AppSettingsValue { get; set; } = 3;
	}

#endregion

#region site settings

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "SiteSettingData40")]
	public class SiteSettingData40
	{
		[DataMember(Order = 1)]
		public int SiteSettingsValue { get; set; } = 1;
	}

#endregion

#region machine settings

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "MachSettingData40")]
	public class MachSettingData40
	{
		[DataMember(Order = 1)]
		public int MachSettingsValue { get; set; } = 2;
	}

#endregion

#endregion

#region management info classes

#region app setting

	[DataContract(Name = "AppSettingInfo40")]
	public class AppSettingInfo40 : AppSettingBase
	{
		[DataMember]
		public AppSettingData40 Data = new AppSettingData40();

		public override string ClassVersion => "4.0a";
		public override string Description => "app setting file for SettingsManagerV40";

		public override void UpgradeFromPrior(SettingBase prior) { }
		public bool UseMachData { get; set; }
		public bool AutoReadMachData { get; set; }
		public override string SiteRootPath { get; set; }
		public bool UseSiteData => !SiteRootPath.IsVoid();
		public bool AutoReadSiteData { get; set; }

		public override void Configure()
		{
			UseMachData = true;
			AutoReadMachData = true;
			AutoReadSiteData = true;
			SiteRootPath =
				@"D:\Users\Jeff\OneDrive\Prior Folders\Office Stuff\CAD\Copy Y Drive & Office Standards\AppData";
		}
	}

#endregion

#region machine settings

	[DataContract(Name = "MachSettingInfo40")]
	public class MachSettingInfo40 : MachSettingBase
	{
		[DataMember]
		public MachSettingData40 Data = new MachSettingData40();

		public override string ClassVersion => "4.0c";
		public override string Description => "machine setting file for SettingsManagerV40";

		public override void UpgradeFromPrior(SettingBase prior) { }
	}

#endregion

#region site settings

	[DataContract(Name = "SiteSettingInfo40")]
	public class SiteSettingInfo40 : SiteSettingBase
	{
		[DataMember]
		public SiteSettingData40 Data = new SiteSettingData40();

		public override string ClassVersion => "4.0s";
		public override string Description => "site setting file for SettingsManagerV40";

		public override void UpgradeFromPrior(SettingBase prior) { }
	}

#endregion

#endregion

#region management root class

#region app settings

	public static class AppSettings
	{
		// this is the primary data structure - it holds the settings
		// configuration information as well as the setting data
		public static SettingsMgr<AppSettingInfo40> Admin { get; private set; }

		// this is just the setting data - this is a shortcut to
		// the setting data
		public static AppSettingInfo40 Info { get; private set; }
		public static AppSettingData40 Data { get; private set; }
		public static MachSettingData40 MachData { get; private set; } = null;
		public static MachSettingInfo40 MachInfo { get; private set; } = null;
		public static SiteSettingData40 SiteData { get; private set; } = null;
		public static SiteSettingInfo40 SiteInfo { get; private set; } = null;

		private static SettingsMgr<MachSettingInfo40> MachAdmin { get; set; } = null;
		private static SettingsMgr<SiteSettingInfo40> SiteAdmin { get; set; } = null;

		// initialize and create the setting objects
		static AppSettings()
		{
			Admin = new SettingsMgr<AppSettingInfo40>(ResetData);
			Info = Admin.Info;
			Data = Info.Data;

			Info.Configure();

			if (Info.AutoReadSiteData) ReadSiteSettings();

			if (Info.AutoReadMachData) ReadMachineSettings();

		}

		// if we need to reset to the "factory" default
		public static void ResetData()
		{
			Info = Admin.Info;
			Data = Info.Data;

			Info.Configure();
		}

		public static void ReadMachineSettings()
		{

			if (Info.UseMachData)
			{
				MachAdmin = MachSettings.Admin;

				MachAdmin.Read();

				MachInfo = MachSettings.Info;
				MachData = MachSettings.Data;
			}
		}

		public static void ReadSiteSettings()
		{
			Info.SetSiteRootPath(Info.SiteRootPath);

			if (Info.UseSiteData)
			{
				SiteAdmin = SiteSettings.Admin;

				SiteAdmin.Read();

				SiteInfo = SiteSettings.Info;
				SiteData = SiteSettings.Data;
			}
		}

	#endregion


	#region machine settings

/*
	#region machine settings data class

		// this is the actual data set saved to the user's configuration file
		// this is unique for each program
		[DataContract(Name = "MachSettingData40")]
		public class MachSettingData40
		{
			[DataMember(Order = 1)]
			public int MachSettingsValue { get; set; } = 2;
		}

	#endregion

	#region machine settings management info class

		[DataContract(Name = "MachSettingInfo40")]
		public class MachSettingInfo40 : MachSettingBase
		{
			[DataMember]
			public MachSettingData40 Data = new MachSettingData40();

			public override string ClassVersion => "4.0c";

			public override void UpgradeFromPrior(SettingBase prior) { }
		}

	#endregion
*/

	#region machine settings root class

		public static class MachSettings
		{
			// this is the primary data structure - it holds the settings
			// configuration information as well as the setting data
			public static SettingsMgr<MachSettingInfo40> Admin { get; private set; }

			// this is just the setting data - this is a shortcut to
			// the setting data
			public static MachSettingInfo40 Info { get; private set; }
			public static MachSettingData40 Data { get; private set; }

			// initialize and create the setting objects
			static MachSettings()
			{
				Admin = new SettingsMgr<MachSettingInfo40>(ResetData);
				Info = Admin.Info;
				Data = Info.Data;

				Info.Configure();

			}

			public static void ResetData()
			{
				if (Admin == null) return;
				// this makes sure the above static class points
				// to the current data structure
				Info  = Admin.Info;
				Data  = Info.Data;

				Info.Configure();
			}
		}

	#endregion

	#endregion

	#region site settings

/*
	#region site settings data class

		// this is the actual data set saved to the user's configuration file
		// this is unique for each program
		[DataContract(Name = "SiteSettingData40")]
		public class SiteSettingData40
		{
			[DataMember(Order = 1)]
			public int SiteSettingsValue { get; set; } = 1;
		}

	#endregion

	#region site settings management info class

		[DataContract(Name = "SiteSettingInfo40")]
		public class SiteSettingInfo40 : SiteSettingBase
		{
			[DataMember]
			public SiteSettingData40 Data = new SiteSettingData40();

			public override string ClassVersion => "4.0s";

			public override void UpgradeFromPrior(SettingBase prior) { }
		}

	#endregion
*/

	#region site settings root class

		public static class SiteSettings
		{
			// this is the primary data structure - it holds the settings
			// configuration information as well as the setting data
			public static SettingsMgr<SiteSettingInfo40> Admin { get; private set; }

			// this is just the setting data - this is a shortcut to
			// the setting data
			public static SiteSettingInfo40 Info { get; private set; }
			public static SiteSettingData40 Data { get; private set; }

			// initialize and create the setting objects
			static SiteSettings()
			{
				Admin = new SettingsMgr<SiteSettingInfo40>(ResetData);
				Info = Admin.Info;
				Data = Info.Data;

				Info.Configure();
			}

			// if we need to reset to the "factory" default
			public static void ResetData()
			{
				if (Admin == null) return;
				// this makes sure the above static class points
				// to the current data structure
				Info = Admin.Info;
				Data = Info.Data;

				Info.Configure();
			}
		}

	#endregion
	}

#endregion

#endregion
}