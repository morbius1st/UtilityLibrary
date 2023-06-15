using System.Runtime.Serialization;
using SettingManager;

namespace HelpDesk.Settings
{
	public static class AppSettings
	{
		// this is the primary data structure - it holds the settings
		// configuration information as well as the setting data
		public static SettingsMgr<AppSettingInfo30> Admin { get; private set; }

		// this is just the setting data - this is a shortcut to
		// the setting data
		public static AppSettingInfo30 Info { get; private set; }
		public static AppSettingData30 Data { get; private set; }

		// initialize and create the setting objects
		static AppSettings()
		{
			Admin = new SettingsMgr<AppSettingInfo30>(ResetClass);
			Info = Admin.Info;
			Data = Info.Data;

//			logMsgLn2();
//			logMsgLn2("at ctor AppSettings", "status| " + Admin.Status
//				+ "  CanAutoUpgrade?| " + Admin.CanAutoUpgrade
//					);
//			logMsgLn2();
		}

		// if we need to reset to the "factory" default
		public static void ResetClass()
		{
			Info = Admin.Info;
			Data = Info.Data;

//			logMsgLn2();
//			logMsgLn2("at AppSettings reset", "status| " + Admin.Status);
		}
	}

	// this is the actual data set saved to the user's configuration file
	// this is unique for each program
	[DataContract(Name = "AppSettingData30")]
	public partial class AppSettingData30
	{
		[DataMember(Order = 1)]
		public string HelpDeskEmailAddress { get; set; } = "production@aoarchitects.com";

//		[DataMember(Order = 2)]
//		public bool DebugMode { get; set; } = true;
//
//		[DataMember(Order = 2)]
//		public bool AppB { get; set; } = false;
//
//		[DataMember(Order = 3)]
//		public double AppD { get; set; } = 0.0;
//
//		[DataMember(Order = 4)]
//		public string AppS { get; set; } = "this is an App";
//
//		[DataMember(Order = 5)]
//		public int[] AppIs { get; set; } = new[] {20, 30};
//
//		[DataMember(Order = 20)]
//		public int AppI20 { get; set; } = 0;
//
//		[DataMember(Order = 21)]
//		public bool AppB21 { get; set; } = false;
//
//		[DataMember(Order = 22)]
//		public double AppD22 { get; set; } = 0.0;
	}
	

	// this is a management class
	[DataContract(Name = "AppSettingInfo30")]
	public class AppSettingInfo30 : AppSettingBase
	{
		[DataMember]
		public AppSettingData30 Data = new AppSettingData30();

		public override string ClassVersion => "1.0";

		public override void UpgradeFromPrior(SettingBase prior)
		{
			// no implementation - first invocation

//			AppSettingInfo21 p = (AppSettingInfo21) prior;
//
//			Header.Notes =
//				p.Header.Notes + " :: updated to v" + ClassVersion;
//
//			Data.AppI   = p.Data.AppI;
//			Data.AppB   = p.Data.AppB;
//			Data.AppD   = p.Data.AppD;
//			Data.AppS   = p.Data.AppS;
//			Data.AppI20 = p.Data.AppI20;
//			Data.AppB21 = p.Data.AppB21;
//
//			for (int i = 0;
//				i < (Data.AppIs.Length < p.Data.AppIs.Length ? Data.AppIs.Length : p.Data.AppIs.Length);
//				i++)
//			{
//				Data.AppIs[i] = p.Data.AppIs[i];
//			}

		}


	}
}