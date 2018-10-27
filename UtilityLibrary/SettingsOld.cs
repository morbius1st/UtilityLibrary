#region Using directives
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

#endregion

// itemname:	Settings
// username:	jeffs
// created:		12/30/2017 4:42:00 PM


namespace UtilityLibrary
{
	public class VersionInfo
	{
		public const string SETTINGSYSTEMVERSION = "1.0.2.1";

		[XmlAttribute]
		public string SettingFileVersion = "0.0.0.0";
		[XmlAttribute]
		public string AssemblyVersion = SettingsUtil.AssemblyVersion;
		[XmlAttribute]
		public string SettingSystemVersion = SETTINGSYSTEMVERSION;
	}

	public static class SettingsUser
	{
		public static Settings<UserSettings> USetting { private set; get; }
			= GetInstance();

		public static UserSettings USet;

		private static Settings<UserSettings> userSettings;

		private static Settings<UserSettings> GetInstance()
		{
			if (userSettings == null)
			{
				userSettings = new Settings<UserSettings>();
			}

			USet = userSettings.Setting;

			return userSettings;
		}

		public static void Reset(this Settings<UserSettings> user)
		{
			user.Setting = new UserSettings();
			user.Save();
			GetInstance();
		}
	}

	public static class SettingsApp
	{
		public static Settings<AppSettings> ASetting { private set; get; }
			= GetInstance();

		public static AppSettings ASet;

		private static Settings<AppSettings> appSettings;

		public static Settings<AppSettings> GetInstance()
		{
			if (appSettings == null)
			{
				appSettings = new Settings<AppSettings>();
			}

			ASet = appSettings.Setting;

			return appSettings;
		}

		public static void Reset(this Settings<AppSettings> app)
		{
			app.Setting = new AppSettings();
			app.Save();
			GetInstance();
		}
	}

	public class Settings<T> where T : SettingsPathFileBase, new()
	{
		internal T Setting { get; set; }

		internal string SettingsPathAndFile { get; private set; }

		public Settings()
		{
			SettingsPathAndFile = (new T()).SettingsPathAndFile;
			Read();
		}

		private void Read()
		{
			// does the file already exist?
			if (File.Exists(SettingsPathAndFile))
			{
				try
				{
					// file exists - get the current values
					using (FileStream fs = new FileStream(SettingsPathAndFile, FileMode.Open))
					{
						XmlSerializer xs = new XmlSerializer(typeof(T));
						Setting = (T) xs.Deserialize(fs);
					}
				}
				catch (Exception e)
				{
					throw new Exception("Cannot read setting data for file:\n"
						+ SettingsPathAndFile + "\n"
						+ e.Message);
				}
			}
			else
			{
				// file does not exist - create file and save default values
				using (FileStream fs = new FileStream(SettingsPathAndFile, FileMode.Create, FileAccess.ReadWrite))
				{
					XmlSerializer xs = new XmlSerializer(typeof(T));
					Setting = new T();
					xs.Serialize(fs, Setting);
				}
			}
		}

		public void Save()
		{
			if (!File.Exists(SettingsPathAndFile))
			{
				throw new FileNotFoundException(SettingsPathAndFile);
			}
			// file exists - process
			using (FileStream fs = new FileStream(SettingsPathAndFile, FileMode.Create))
			{
				XmlSerializer xs = new XmlSerializer(typeof(T));

				xs.Serialize(fs, Setting);
			}
		}
	}

	public static class SettingsUtil
	{
		internal static string AssemblyName => typeof(SettingsUtil).Assembly.GetName().Name;

		internal static string CompanyName
		{
			get
			{
				object[] att = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
				if (att.Length > 0)
				{
					return ((AssemblyCompanyAttribute) att[0]).Company;
				}

				throw new MissingFieldException("Company is Missing from Assembly Information");
			}
		}

		internal static string AssemblyDirectory
		{
			get
			{
				string codebase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri = new UriBuilder(codebase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		internal static string AssemblyVersion
		{
			get { return typeof(SettingsUtil).Assembly.GetName().Version.ToString(); }
		}

		internal static bool CreateSubFolders(string RootPath, string[] SubFolders)
		{
			if (!Directory.Exists(RootPath)) { return false; }

			for (int i = 0; i < SubFolders.Length; i++)
			{
				string path = SubFolder(i, RootPath, SubFolders);

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}

			return true;
		}

		internal static string SubFolder(int i, string RootPath, string[] SubFolders)
		{
			if (i < 0 ||
				i >= SubFolders.Length) return null;

			string path = RootPath;
			for (int j = 0; j < i + 1; j++)
			{
				path += "\\" + SubFolders[j];
			}

			return path;
		}
	}

	public abstract class SettingsPathFileBase
	{
		protected string FileName;
		protected string RootPath;
		protected string[] SubFolders;

		public const string SETTINGFILEBASE = @".setting.xml";
		public abstract string SETTINGFILEVERSION { get; }

		public VersionInfo VersionInfo = new VersionInfo();

		// create the folder path if needed
		public virtual bool CreateFolders()
		{
			if (SubFolders == null) return true;

			return SettingsUtil.CreateSubFolders(RootPath, SubFolders);
		}

		// get the count of sub-folders
		private int SubFolderCount => SubFolders.Length;

		// get the path to the setting file
		public string SettingsPath
		{
			get
			{
				if (SubFolders == null)
				{
					return RootPath;
				}
				return SettingsUtil.SubFolder(SubFolders.Length - 1,
					RootPath, SubFolders);
			}
		}

		// get the path and the file name for the setting file
		public string SettingsPathAndFile
		{
			get
			{
				if (!Directory.Exists(SettingsPath))
				{
					if (!CreateFolders())
					{
						throw new DirectoryNotFoundException("setting file path");
					}
				}
				return SettingsPath + "\\" + FileName;
			}
		}
	}

	public class SettingsPathFileUserBase : SettingsPathFileBase
	{
		public override string SETTINGFILEVERSION { get; } = "0.0.0.1";
	
		public SettingsPathFileUserBase()
		{
			FileName = @"user" + SETTINGFILEBASE;
	
			RootPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
	
			SubFolders = new[] {
				SettingsUtil.CompanyName,
				SettingsUtil.AssemblyName };
		}
	}

	public class SettingsPathFileAppBase : SettingsPathFileBase
	{
		public override string SETTINGFILEVERSION { get; } = "0.0.0.1";

		public SettingsPathFileAppBase()
		{
			FileName = SettingsUtil.AssemblyName + SETTINGFILEBASE;
			RootPath = SettingsUtil.AssemblyDirectory;
			SubFolders = null;
		}
	}

	// sample setting clases

	// sample user settings
	//	public class UserSettings : SettingsPathFileUserBase
	//	{
	//		public int UnCategorizedValue = 10;
	//		public generalValues GeneralValues = new generalValues();
	//		public window1 MainWindow { get; set; } = new window1();
	//	}
	//
	//	public class window1
	//	{
	//		[XmlAttribute]
	//		public int height = 50;
	//		[XmlAttribute]
	//		public int width = 100;
	//	}
	//
	//	public class generalValues
	//	{
	//		public int TestI = 0;
	//		public bool TestB = false;
	//		public double TestD = 0.0;
	//		public string TestS = "this is a test";
	//		public int[] TestIs = new[] { 20, 30 };
	//		public string[] TestSs = new[] { "user 1", "user 2", "user 3" };
	//	}

	// sample app settings
	//	public class AppSettings : SettingsPathFileAppBase
	//	{
	//		public int AppI { get; set; } = 0;
	//		public bool AppB { get; set; } = false;
	//		public double AppD { get; set; } = 0.0;
	//		public string AppS { get; set; } = "this is a App";
	//		public int[] AppIs { get; set; } = new[] { 20, 30 };
	//
	//	}
}

