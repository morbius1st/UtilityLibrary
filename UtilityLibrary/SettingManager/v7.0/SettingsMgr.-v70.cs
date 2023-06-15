#region usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;


using static SettingsManager.SettingMgrStatus;


using UtilityLibrary;

// ReSharper disable IdentifierTypo

#endregion

// requires a reference to:
// System.Runtime.Serialization
// System.Xaml

// item name:	Config
// username:	jeff s
// created:		12/30/2017 4:42:00 PM

//	ver 1.0		initial version
//	ver 2.0		revise to use DataContract
//	ver 2.1		refine use fewer classes / abstract classes
//	ver	2.2		refine
//	ver 2.3		refine move utility methods to library file
//	ver 2.4		move setting file specific info out of base file
//	ver 2.4.1	revise date format
//	ver 2.5		move static setting classes into base file
//				added delegate to properly reset data
//	ver 2.6		incorporate [OnDeserializing] and proper default values
//	ver 3.0		updated to incorporate checking for setting file being
//				a different version and to upgrade to the current version
//	ver 3.1		added call to SyncData() at the end of a normal read();
//	ver 4.0		updated to allow a machine level and site level setting file
//	ver 5.0 & 6.0	refine support classes to use generics and consolidate code
//	ver 6.1		incorporate a data only component
//	ver	7.0		add support for suites
//	ver	7.1		added conditional compilation at primary class definitions
//              adjust site PathAndFile to include company and assembly name
//	ver	7.2		enhance path to have bool for rootpath is valid and settingpath is valid


#region setting file hireacrhy

/*

top
^ +-> site level (per suite)
| |	Site settings:
| |		- applies to all machines & all users
| |		- holds information needed by all machines and, therefore, all users
| v		- maybe placed on the local or remote machine (location specified in app setting file)
|
| +--> Mach settings (per suite)
| |		- applies to a specific machine / all users on that machine
| |		- holds information needed by or shared between all users on the machine
| v		- located in the common app data folder (currently c:\program data)		
|
| per suite
+-----------------
| per user
| 
| +-> Suite settings (per user)
| |		- applies to a specific app suite (multiple programs)
| |		- holds information needed by all programs in the suite, but not all users of the suite
| |		- provides the pointer to the site settings file (why here: allows each user to be associated with different site files)
| v		- located in the user's app data folder
| 
| +-> App settings (per user)
| |		- applies to a specific app in the suite
| |		- holds information specific to the app
| v		- located in the user's app data folder / app name / AppSettings
| 
| +-> User settings (per user) 
| | 	- user's settings for a specific app
v v		- located in the user's app data folder / app name
bottom

*/

#endregion

// ReSharper disable once CheckNamespace

namespace SettingsManager
{

	public enum SettingMgrStatus
	{
		SAVEDISALLOWED = -4,
		INVALIDPATH = -3,
		NOPATH = -2,
		DOESNOTEXIST = -1,

		BEGIN = 0,
		CONSTRUCTED,
		INITIALIZED,
		CREATED,
		READ,
		SAVED,
		EXISTS
	}

	public enum SettingFileType
	{
		OTHER = -1,
		USER  = 0,
		APP   = 1,
		SUITE = 2,
		MACH  = 3,
		SITE  = 4,

		// ReSharper disable once UnusedMember.Global
		LENGTH = 6
	}
#region + Heading

	[DataContract(Namespace =  N_SPACE)]
	public partial class Heading
	{
		public static string ClassVersionName = nameof(DataClassVersion);
		public static string SystemVersionName = nameof(SystemVersion);
		public static string SuiteName = "Andy";

		public const string N_SPACE = "";

		public Heading(string dataClassVersion) => DataClassVersion = dataClassVersion;

		[DataMember(Order = 1)] public string SaveDateTime       = DateTime.Now.ToString("yyyy-MM-dd - HH:mm zzz");
		[DataMember(Order = 2)] public string AssemblyVersion    = CsUtilities.AssemblyVersion;
		[DataMember(Order = 3)] public string SystemVersion      = "7.2";

	}

	// [DataContract(Namespace =  N_SPACE)]
	// public class Heading
	// {
		// public static string ClassVersionName = nameof(DataClassVersion);
		// public static string SystemVersionName = nameof(SystemVersion);
        // public static string SuiteName = "SettingManager";

		// public const string N_SPACE = "";

		// public Heading(string dataClassVersion) => DataClassVersion = dataClassVersion;

		// [DataMember(Order = 1)] public string SaveDateTime       = DateTime.Now.ToString("yyyy-MM-dd - HH:mm zzz");
		// [DataMember(Order = 2)] public string AssemblyVersion    = CsUtilities.AssemblyVersion;
		// [DataMember(Order = 3)] public string SystemVersion      = "7.0";
		// [DataMember(Order = 4)] public string DataClassVersion;
		// [DataMember(Order = 5)] public string Notes              = "created by v7.0";
		// [DataMember(Order = 6)] public string Description;
	// }
#endregion
	public delegate void RstData();

	public class SettingsMgr<Tpath, Tinfo, Tdata>
		where Tpath : PathAndFileBase, new()
		where Tinfo : SettingInfoBase<Tdata>, new()
		where Tdata : new()
	{

	#region + Constructor

		public SettingsMgr(Tpath path, RstData rst)

		{
			Status = BEGIN;

			Path = path;

			if (path == null || String.IsNullOrWhiteSpace(Path.SettingPathAndFile))
			{
				Status = NOPATH;
			}

			_resetData = rst;

			Initialize();

			Status = CONSTRUCTED;
		}

		private void Initialize()
		{
			if (Status != NOPATH)
			{
				Status = INITIALIZED;


// TODO redo the upgrade system
//				UpgradeRequired = !Info.ClassVersionsMatch(Path.ClassVersionFromFile);
//
//				if (FileExists())
//				{
//					if (!Info.ClassVersionsMatch(Path.ClassVersionFromFile) && CanAutoUpgrade)
//					{
//						Upgrade();
//					}
//				}
			}
		}

	#endregion

	#region + Properties

		public Tinfo Info { get; private set; } =
			new Tinfo();

		public Tpath Path { get; set; }

		public SettingMgrStatus Status { get; private set; }

		public bool CanAutoUpgrade { get; set; } = false;

		public bool UpgradeRequired { get; set; } = false;

	#endregion

	#region + Read

		public void Read()
		{
			if (Path.SettingPathAndFile.IsVoid() )
			{
				Status = INVALIDPATH;
				return;
			}

			// does the file already exist?
			if (FileExists())
			{

// TODO re-wirite the upgrade system
//				if (Info.ClassVersionsMatch(Path.ClassVersionFromFile))
//				{
					try
					{
						DataContractSerializer ds = new DataContractSerializer(typeof(Tinfo));

						// file exists - get the current values
						using (FileStream fs = new FileStream(Path.SettingPathAndFile, FileMode.Open))
						{
							Info = (Tinfo) ds.ReadObject(fs);
						}

						Status = READ;
					}
					// catch malformed XML data file and replace with default
					catch (System.Runtime.Serialization.SerializationException)
					{
						Create();
						Save();
					}
					// catch any other errors
					catch (Exception e)
					{


						throw new MessageException(MessageUtilities.nl


							+ "Cannot read setting data for file: "

							+ Path.SettingPathAndFile + MessageUtilities.nl


							+ "("                     + e.Message + ")" + MessageUtilities.nl

							, e.InnerException);
					}
// TODO re-write the upgrade system
//				}
//				else
//				{
//					Upgrade();
//				}
			}
			else
			{
				Create();
				Save();
			}

			// added in v3.1
			SyncData();
		}

		public dynamic Read(Type type)
		{
			dynamic p = null;

			if (FileExists())
			{
				try
				{
					DataContractSerializer ds = new DataContractSerializer(type);

					using (FileStream fs = new FileStream(Path.SettingPathAndFile, FileMode.Open))
					{
						p = ds.ReadObject(fs);
					}
				}
				catch { }
			}

			return p;
		}

	#endregion

	#region + Create

		// create a empty object - initialized with the
		// default data from the app / user setting's class
		public void Create()
		{
			if (Path.SettingPathAndFile.IsVoid() )
			{
				Status = INVALIDPATH;
				return;
			}

			// since the data is "new" and may not match what
			// has been saved, note as not initialized
			Info = new Tinfo();

			Status = CREATED;
		}

	#endregion

	#region + Write

		public void Write()
		{
			Save();
		}

		public void Save()
		{
			if (Path.IsReadOnly || Path.SettingPathAndFile.IsVoid())
			{
				Status = SAVEDISALLOWED;
				return;
			}

			XmlWriterSettings xmlSettings = new XmlWriterSettings() {Indent = true};

			DataContractSerializer ds = new DataContractSerializer(typeof(Tinfo));

			using (XmlWriter w = XmlWriter.Create(Path.SettingPathAndFile, xmlSettings))
			{
				ds.WriteObject(w, Info);
			}

			// since file and memory match
			Status = SAVED;
		}

	#endregion

	#region + Upgrade

// TODO re-write the upgrade system
// upgrade must be re-written to allow for the (5) configuration setting files

//		// upgrade from a prior class version to the current class version
//		// this method is less restricted to allow the user to perform
//		// a manual upgrade - if CanAutoUpgrade is false
//		public void Upgrade()
//		{
//			// is an upgrade needed
//			if (!Info.ClassVersionsMatch(Path.ClassVersionFromFile))
//			{
//				// get a list of prior setting classes from 
//				// which to possible upgrade
//				List<SettingInfoBase<Tdata>> settings = null;
//
//				switch (Info.FileType)
//				{
//				case SettingFileType.APP:
//					{
//						settings =
//							GetMatchingClasses<AppSettingInfoBase<Tdata>>();
//						break;
//					}
//				case SettingFileType.USER:
//					{
//						settings =
//							GetMatchingClasses<UserSettingInfoBase<Tdata>>();
//						break;
//					}
//				}
//
//				// if needed, preform upgrade
//				if (settings != null)
//				{
//					UpgradeList(settings);
//
//					Save();
//
//					UpgradeRequired = false;
//				}
//			}
//		}

	#endregion

	#region + Reset

		private readonly RstData _resetData;

		// reset the data portion to it's default values
		public void Reset()
		{
			Create();

			_resetData?.Invoke();
		}

		public void SyncData()
		{
			if (_resetData != null)
			{
				_resetData?.Invoke();
			}
		}

	#endregion

	#region + Utilities

		// report whether the setting file does exist
		private bool FileExists()
		{
			bool result = Path.Exists;

			if (result)
			{
				Status = EXISTS;
			}
			else
			{
				Status = DOESNOTEXIST;
			}

			return result;
		}

		// use reflection to create a list of classes of matching type
		// that may be used to upgrade the existing class to the
		// current version
		private List<SettingInfoBase<Tdata>> GetMatchingClasses<U>() where U : SettingInfoBase<Tdata>
		{
			Type baseType = typeof(Tinfo).BaseType;

			List<SettingInfoBase<Tdata>> list = new List<SettingInfoBase<Tdata>> {Info};

			Type[] types = Assembly.GetExecutingAssembly().GetTypes();

			// scan the list of all of the types to find the
			// few of the correct type
			foreach (Type type in types)
			{
				if (type == Info.GetType()) continue;

				Type testType = type.BaseType;

				if (testType == baseType)
				{
					list.Add((U) Activator.CreateInstance(type));
				}
			}

			return list;
		}

		// based on a list of matching setting of prior versions
		// upgrade the existing to the current version
		private void UpgradeList(List<SettingInfoBase<Tdata>> settings)
		{
			if (settings == null || settings.Count < 2)
			{
				return;
			}

			if (Info.ClassVersionsMatch(Path.ClassVersionFromFile))
			{
				return;
			}

			settings.Sort(); // must be in the correct order from oldest to newest

			for (int i = 0; i < settings.Count; i++)
			{
				int j = String.Compare(settings[i].DataClassVersion,
					Path.ClassVersionFromFile, StringComparison.Ordinal);

				if (j < 0) continue;

				if (j == 0)
				{
					// found the starting point, read the current setting
					// file into memory
					settings[i] = Read(settings[i].GetType());
				}
				else
				{
					settings[i].UpgradeFromPrior(settings[i - 1]);
				}
			}
		}

	#endregion
	}

#region + Path File and Utilities

	// this class will be associated with the main class
	public abstract class PathAndFileBase
	{
		protected const string SETTINGFILEBASE = @".setting.xml";

		private string   _fileName;
		private string   rootFolderPath;
		private string[] _subFolders = new [] {CsUtilities.AssemblyName};
		private string   _settingPath;
		private string   _settingPathAndFile;

		protected PathAndFileBase()
		{
			Configure();
		}

		public string FileName
		{
			get => _fileName;
			set
			{
				_fileName = value;
				_settingPathAndFile = null;
			}
		}

		/// <summary>
		/// the PATH to the root folder for the referenced setting file
		/// </summary>
		public string RootFolderPath
		{
			get => rootFolderPath;
			set
			{
				rootFolderPath = value;
				_settingPath = null;
				_settingPathAndFile = null;
			}
		}		

		public string[] SubFolders
		{
			get => _subFolders;
			set
			{
				_subFolders = value;
				_settingPath = null;
				_settingPathAndFile = null;
			}
		}

		public bool IsReadOnly { get; set; } = false;
		public bool IsSettingFile { get; set; } = false;
		public bool HasPathAndFile => !_settingPathAndFile.IsVoid();

		/// <summary>
		/// indicates if the referenced setting FILE exists
		/// </summary>
		public bool Exists
		{
			get
			{
				if (SettingPathAndFile.IsVoid()) return false;

				return File.Exists(SettingPathAndFile);
			}
		}
		
		/// <summary>
		/// the PATH string for the referenced setting file
		/// </summary>
		public string SettingFolderPath
		{
			get
			{
				if (_settingPath.IsVoid())
				{
					_settingPath = CreateSettingPath();
				}

				return _settingPath;
			}
		}
		
		/// <summary>
		/// The PATH and FILENAME string for the referenced setting file
		/// </summary>
		public string SettingPathAndFile
		{
			get
			{
				ConfigurePathAndFile();

				return _settingPathAndFile;
			}
		}

		/// <summary>
		/// indicates if the ROOT PATH for the referenced setting file is valid
		/// </summary>
		public bool RootFolderPathIsValid
		{
			get { return Directory.Exists(RootFolderPath); }
		}

		/// <summary>
		/// indicates if the SETTING PATH for the referenced setting file is valid
		/// </summary>
		public bool SettingPathIsValid
		{
			get
			{
				return Directory.Exists(SettingFolderPath);
			}
		}

		public string ClassVersionFromFile => GetVersionFromFile(Heading.ClassVersionName);

		public string SystemVersionFromFile => GetVersionFromFile(Heading.SystemVersionName);

		public string CreateSettingPath()
		{
			if (_subFolders == null) return rootFolderPath;


			return CsUtilities.SubFolder( _subFolders.Length - 1,

				rootFolderPath,  _subFolders);
		}

		protected string CreateSettingPathAndFile()
		{
			if (!rootFolderPath.IsVoid())
			{
				if (!Directory.Exists(SettingFolderPath))
				{

					if (!CsUtilities.CreateSubFolders(rootFolderPath, _subFolders))

					{
						throw new DirectoryNotFoundException(
							"setting file path| " + rootFolderPath ?? "path is null");
					}
				}

				return SettingFolderPath + "\\" + _fileName;
			}

			return null;
		}

		public void ConfigurePathAndFile()
		{
			if (_settingPathAndFile.IsVoid())
			{
				_settingPathAndFile = CreateSettingPathAndFile();
			}
		}

		public abstract void Configure();

		protected string GetVersionFromFile(string elementName) =>

			CsUtilities.ScanXmlForElementValue(SettingPathAndFile, elementName);
	}

#endregion

#region + Support Classes

	[DataContract(Namespace = Heading.N_SPACE)]
	public abstract class SettingInfoBase<Tdata> : IComparable<SettingInfoBase<Tdata>>
		where Tdata : new()
	{
		private bool? classVersionsMatch = null;

		[DataMember(Order = 0)]
		public Heading Header;

		public SettingInfoBase()
		{
			Header = new Heading(DataClassVersion)
			{
				Notes = "Created in Version " + DataClassVersion,
				Description = Description
			};
		}

		[DataMember(Order = 10)]
		public Tdata Data { get; set; } = new Tdata();

		public abstract string Description                 { get; }
		public abstract string DataClassVersion            { get; }
		public abstract SettingFileType  FileType  { get; }

		public int CompareTo(SettingInfoBase<Tdata> other) =>
			String.Compare(DataClassVersion, other.DataClassVersion, StringComparison.Ordinal);

		public abstract void UpgradeFromPrior(SettingInfoBase<Tdata> prior);

		public bool ClassVersionsMatch(string fileClassVersion)
		{
			if (classVersionsMatch == null)
			{
				classVersionsMatch = fileClassVersion?.Equals(DataClassVersion) ?? true;
			}

			return classVersionsMatch.Value;
		}
	}

	[DataContract(Namespace = "")]
	public class BaseSettings<Tpath, Tinfo, Tdata>
		where Tpath : PathAndFileBase, new()
		where Tinfo : SettingInfoBase<Tdata>, new()
		where Tdata : new()
	{
		public static SettingsMgr<Tpath, Tinfo, Tdata> Admin { get; private set; }

		public static Tpath Path { get; private set; } = new Tpath();
		public static Tinfo Info { get; private set; }
		public static Tdata Data { get; private set; }

		static BaseSettings()
		{
			Admin = new SettingsMgr<Tpath, Tinfo, Tdata>(Path, ResetData);
			ResetData();
		}

		public static void ResetData()
		{
			Info = Admin.Info;
			Data = Info.Data;
		}
	}



#region user settings config / conditional

#if USER_SETTINGS
	
	// level 1 setting file
	// user specific data is stored in this file
	// this file is per user
	// this file is stored here:
	// C:\Users\{user}\AppData\Roaming\{company name}\{suite name}\{program name}\
	public class UserSettingPath70 : PathAndFileBase
	{
		public override void Configure()
		{
			FileName = @"user" + SETTINGFILEBASE;
			RootFolderPath =  Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			SubFolders = new []
			{
				CsUtilities.CompanyName,
				Heading.SuiteName,
				CsUtilities.AssemblyName
			};
			IsReadOnly = false;
			IsSettingFile = true;
		}
	}

	[DataContract(Namespace = "")]
	// define file type specific information: User
	public abstract class UserSettingInfoBase<Tdata> : SettingInfoBase<Tdata>
		where Tdata : new()
	{
		public override SettingFileType FileType => SettingFileType.USER;
	}

	public class UserSettings :
		BaseSettings<UserSettingPath70,
		UserSettingInfo70<UserSettingData>,
		UserSettingData> { }

#endif

#endregion

#region app settings config / conditional
#if APP_SETTINGS

	// level 2 setting file
	// app specific data is stored in this file
	// this file will provide the locations for the site setting file
	// this file is per user
	// this file is stored here:
	// C:\Users\{user}\AppData\Roaming\{company name}\{suite name}\{program name}\AppSettings\
	public class AppSettingPath70 : PathAndFileBase
	{
		public override void Configure()
		{
			FileName = CsUtilities.AssemblyName + SETTINGFILEBASE;
			RootFolderPath =  Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			SubFolders = new []
			{
				CsUtilities.CompanyName,
				Heading.SuiteName,
				CsUtilities.AssemblyName,
				"AppSettings"
			};
			IsReadOnly = false;
			IsSettingFile = true;
		}
	}

	[DataContract(Namespace = "")]
	// define file type specific information: app
	public abstract class AppSettingInfoBase<Tdata> : SettingInfoBase<Tdata>
		where Tdata : new()
	{
		public override SettingFileType FileType => SettingFileType.APP;
	}

	public class AppSettings :
		BaseSettings<AppSettingPath70,
		AppSettingInfo70<AppSettingData70>,
		AppSettingData70> { }
//	{
//
//		public static string SiteRootPath => AppSettings.Data.SiteRootPath;
//
//	}
#endif

#endregion

#region suite settings config / conditional

#if SUITE_SETTINGS
	// level 3 setting file
	// suite specific data is stored in this file
	// i.e. information to be used by all programs in a suite
	// this file is per user
	// this file is stored here:
	// C:\Users\{user}\AppData\Roaming\{company name}\{suite name}\

	public class SuiteSettingPath70 : PathAndFileBase
	{
		public override void Configure()
		{

			FileName = Heading.SuiteName + ".suite" + SETTINGFILEBASE;

			RootFolderPath =  Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			SubFolders = new []
			{

				CsUtilities.CompanyName,
				Heading.SuiteName

			};
			IsReadOnly = false;
			IsSettingFile = true;
		}
	}

	[DataContract(Namespace = "")]
	// define file type specific information: app
	public abstract class SuiteSettingInfoBase<Tdata> : SettingInfoBase<Tdata>
		where Tdata : new()
	{
		public override SettingFileType FileType => SettingFileType.SUITE;
	}



	public class SuiteSettings :
		BaseSettings<SuiteSettingPath70,
		SuiteSettingInfo70<SuiteSettingData>,
		SuiteSettingData>
	{
		public static string SiteRootPath => SuiteSettings.Data.SiteRootPath;
	}

#endif

#endregion

#region machine settings config / conditional

#if MACH_SETTINGS
	// level 4a setting file
	// machine specific data is stored in this file
	// i.e. information to be used by all programs by a
	// software developer on the machine (e.g. license info / contact info / web site info)
	// this file is per machine (all users)
	// this file is stored here:
	// C:\ProgramData\{company name}\{program name}\
	public class MachSettingPath70 : PathAndFileBase
	{
		public override void Configure()
		{
			// FileName = CsUtilities.AssemblyName + ".Machine" + SETTINGFILEBASE;
			FileName = Heading.SuiteName + ".machine" + SETTINGFILEBASE;
			RootFolderPath =  Environment. GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			SubFolders = new []
			{
				CsUtilities.CompanyName,
				Heading.SuiteName != null ? Heading.SuiteName : CsUtilities.AssemblyName
			};
			IsReadOnly = false;
			IsSettingFile = true;
		}
	}

	[DataContract(Namespace = "")]
	// define file type specific information: machine
	public abstract class MachSettingInfoBase<Tdata> : SettingInfoBase<Tdata>
		where Tdata : new()
	{
		public override SettingFileType FileType => SettingFileType.MACH;
	}



	public class MachSettings :
		BaseSettings<MachSettingPath70,
		MachSettingInfo70<MachSettingData>,
		MachSettingData> { }

#endif

#endregion

#region site settings config / conditional

#if SITE_SETTINGS
	// level 4b setting file
	// site & suite specific data is stored in this file
	// i.e. information used by multiple programs / suites on
	// multiple machines or multiple sites and machines
	// this file is per "site" (all machines & all users)
	// this file is stored here:
	// {as defined in a suite setting file} - could be on a remote machine
	public class SiteSettingPath70 : PathAndFileBase
	{
		public override void Configure()
		{

			FileName = Heading.SuiteName + ".Site" + SETTINGFILEBASE;

			SubFolders = new []
			{

				CsUtilities.CompanyName,


				Heading.SuiteName

			};
			IsReadOnly = false;
			IsSettingFile = true;
		}
	}

	[DataContract(Namespace = "")]
	// define file type specific information: site
	public abstract class SiteSettingInfoBase<Tdata> : SettingInfoBase<Tdata>
		where Tdata : new()
	{
		public override SettingFileType FileType => SettingFileType.MACH;
	}



	public class SiteSettings :
		BaseSettings<SiteSettingPath70,
		SiteSettingInfo70<SiteSettingData>,
		SiteSettingData> { }

#endif

#endregion

#endregion

#region info class for data

	[DataContract(Name = "DataStorage", Namespace = "")]
	public class StorageMgrInfo<Tdata> : SettingInfoBase<Tdata>
		where Tdata : new ()
	{
		public override string DataClassVersion => "1.0";
		public override string Description => "Data Storage";
		public override SettingFileType FileType => SettingFileType.OTHER;
		public override void UpgradeFromPrior(SettingInfoBase<Tdata> prior) { }
	}

#endregion

#region management class for data

	public class StorageMgrPath : PathAndFileBase
	{
		public override void Configure() { }
	}

#endregion
}