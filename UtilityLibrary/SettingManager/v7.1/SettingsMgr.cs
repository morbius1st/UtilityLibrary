#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Xml;
using UtilityLibrary;
using static SettingsManager.SettingMgrStatus;

// ReSharper disable CommentTypo
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedMember.Global
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
//	ver	7.0.1	added conditional compilation at primary class definitions
//              adjust site PathAndFile to include company and assembly name

//	ver	7.0.2	enhance path to have bool for rootpath is valid and settingpath is valid

//	ver 7.1		* adjust data file system to allow more than one data file, of a type, 
//				accessible at the same time
//				* remove the version number from class names
//				* adjust names to clarify path to a file "FilePath" versus
//				a path to a folder "FolderPath"
//				* add some summary descriptions
//	ver 7.1.1	


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
	// ReSharper disable once UnusedType.Global
	internal class Aav71 { }

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
		SETTING_MGR_DATA  = -1,
		SETTING_MGR_USER  = 0,
		SETTING_MGR_APP   = 1,
		SETTING_MGR_SUITE = 2,
		SETTING_MGR_MACH  = 3,
		SETTING_MGR_SITE  = 4,

		// ReSharper disable once UnusedMember.Global
		LENGTH = 6
	}

#region + Heading

	[DataContract(Namespace =  N_SPACE)]
	public partial class Heading
	{
		public static string ClassVersionName = nameof(DataClassVersion);
		public static string SystemVersionName = nameof(SystemVersion);


		public const string N_SPACE = "";

		public Heading(string dataClassVersion) => DataClassVersion = dataClassVersion;

		[DataMember(Order = 1)] public string SaveDateTime       = DateTime.Now.ToString("yyyy-MM-dd - HH:mm zzz");
		[DataMember(Order = 2)] public string AssemblyVersion    = CsUtilities.AssemblyVersion;
		[DataMember(Order = 3)] public string SystemVersion      = "7.2";
	}

#endregion

	public delegate void RstData();

	public class SettingsMgr<TPath, TInfo, TData>
		where TPath : PathAndFileBase, new()
		where TInfo : SettingInfoBase<TData>, new()
		where TData : new()
	{
	#region + Constructor

		public SettingsMgr(TPath path, RstData rst)
		{
			Status = BEGIN;

			Path = path;

			if (path == null || String.IsNullOrWhiteSpace(Path.SettingFilePath))
			{
				Status = NOPATH;
			}

			_resetData = rst;

			Initialize();

			Status = CONSTRUCTED;
		}

		// ReSharper disable once InconsistentNaming
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

		public TInfo Info { get; private set; } =
			new TInfo();

		public TPath Path { get; set; }

		public SettingMgrStatus Status { get; private set; }

		public bool CanAutoUpgrade { get; set; } = false;

		public bool UpgradeRequired { get; set; } = false;

	#endregion

	#region + Read

		public void Read()
		{
			if (Path.SettingFilePath.IsVoid() )
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
					DataContractSerializer ds = new DataContractSerializer(typeof(TInfo));

					// file exists - get the current values
					using (FileStream fs = new FileStream(Path.SettingFilePath, FileMode.Open))
					{
						Info = (TInfo) ds.ReadObject(fs);
					}

					Status = READ;
				}
				// catch malformed XML data file and replace with default
				catch (SerializationException)
				{
					Create();
					Save();
				}
				// catch any other errors
				catch (Exception e)
				{
					throw new MessageException(MessageUtilities.nl
						+ "Cannot read setting data for file: "
						+ Path.SettingFilePath + MessageUtilities.nl
						+ "(" + e.Message + ")" + MessageUtilities.nl
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

					using (FileStream fs = new FileStream(Path.SettingFilePath, FileMode.Open))
					{
						p = ds.ReadObject(fs);
					}
				}
				catch
				{
					// ignored
				}
			}

			return p;
		}

	#endregion

	#region + Create

		// create a empty object - initialized with the
		// default data from the app / user setting's class
		public void Create()
		{
			if (Path.SettingFilePath.IsVoid() )
			{
				Status = INVALIDPATH;
				return;
			}

			// since the data is "new" and may not match what
			// has been saved, note as not initialized
			Info = new TInfo();

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
			if (Path.IsReadOnly || Path.SettingFilePath.IsVoid())
			{
				Status = SAVEDISALLOWED;
				return;
			}

			XmlWriterSettings xmlSettings = new XmlWriterSettings() {Indent = true};

			DataContractSerializer ds = new DataContractSerializer(typeof(TInfo));

			using (XmlWriter w = XmlWriter.Create(Path.SettingFilePath, xmlSettings))
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
			_resetData?.Invoke();
		}

	#endregion

	#region + Utilities

		// report whether the setting file does exist
		// ReSharper disable once InconsistentNaming
		private bool FileExists()
		{
			bool result = Path.Exists;

			Status = result ? EXISTS : DOESNOTEXIST;

			return result;
		}

		// use reflection to create a list of classes of matching type
		// that may be used to upgrade the existing class to the
		// current version
		// ReSharper disable once InconsistentNaming
		// ReSharper disable once UnusedMember.Local
		private List<SettingInfoBase<TData>> GetMatchingClasses<TU>() where TU : SettingInfoBase<TData>
		{
			Type baseType = typeof(TInfo).BaseType;

			List<SettingInfoBase<TData>> list = new List<SettingInfoBase<TData>> {Info};

			Type[] types = Assembly.GetExecutingAssembly().GetTypes();

			// scan the list of all of the types to find the
			// few of the correct type
			foreach (Type type in types)
			{
				if (type == Info.GetType()) continue;

				Type testType = type.BaseType;

				if (testType == baseType)
				{
					list.Add((TU) Activator.CreateInstance(type));
				}
			}

			return list;
		}

		// based on a list of matching setting of prior versions
		// upgrade the existing to the current version
		// ReSharper disable once UnusedMember.Local
		// ReSharper disable once InconsistentNaming
		private void UpgradeList(List<SettingInfoBase<TData>> settings)
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

		private string   fileName;
		private string   rootFolderPath;
		private string[] subFolders = new [] {CsUtilities.AssemblyName};
		private string   settingFolderPath;
		private string   settingFilePath;

		protected PathAndFileBase()
		{
			// ReSharper disable once VirtualMemberCallInConstructor
			Configure();
		}

		public string FileName
		{
			get => fileName;
			set
			{
				fileName = value;
				settingFilePath = null;
			}
		}

		/// <summary>
		/// the FOLDER PATH to the root folder for the referenced setting file
		/// </summary>
		public string RootFolderPath
		{
			get => rootFolderPath;
			set
			{
				rootFolderPath = value;
				settingFolderPath = null;
				settingFilePath = null;
			}
		}

		/// <summary>
		/// The sub-folders, below the root folder, used to create the<br/>
		/// complete folder path
		/// </summary>
		public string[] SubFolders
		{
			get => subFolders;
			set
			{
				subFolders = value;
				settingFolderPath = null;
				settingFilePath = null;
			}
		}

		public bool IsReadOnly { get; protected set; }

		// ReSharper disable once MemberCanBeProtected.Global
		public bool IsSettingFile { get; set; }
		public bool HasFilePath => !settingFilePath.IsVoid();

		/// <summary>
		/// Indicates if the referenced setting FILE exists
		/// </summary>
		public bool Exists
		{
			get
			{
				if (SettingFilePath.IsVoid()) return false;

				return File.Exists(SettingFilePath);
			}
		}

		/// <summary>
		/// the FOLDER PATH string for the referenced setting file
		/// </summary>
		public string SettingFolderPath
		{
			get
			{
				if (settingFolderPath.IsVoid())
				{
					settingFolderPath = CreateSettingFolderPath();
				}

				return settingFolderPath;
			}
		}

		/// <summary>
		/// The FILE PATH string for the referenced setting file
		/// </summary>
		public string SettingFilePath
		{
			get
			{
				ConfigureFilePath();

				return settingFilePath;
			}
		}

		/// <summary>
		/// indicates if the ROOT PATH for the referenced setting file is valid
		/// </summary>
		public bool RootFolderPathIsValid => Directory.Exists(RootFolderPath);

		/// <summary>
		/// indicates if the SETTING PATH for the referenced setting file is valid
		/// </summary>
		public bool SettingFolderPathIsValid => Directory.Exists(SettingFolderPath);

		public string ClassVersionFromFile => GetVersionFromFile(Heading.ClassVersionName);

		public string SystemVersionFromFile => GetVersionFromFile(Heading.SystemVersionName);

		protected string CreateSettingFolderPath()
		{
			if (subFolders == null) return rootFolderPath;

			return CsUtilities.SubFolder( subFolders.Length - 1,
				rootFolderPath,  subFolders);
		}

		protected string CreateSettingFilePath()
		{
			if (!rootFolderPath.IsVoid())
			{
				if (!Directory.Exists(SettingFolderPath))
				{
					if (!CsUtilities.CreateSubFolders(rootFolderPath, subFolders))
					{
						throw new DirectoryNotFoundException(
							// ReSharper disable once ConstantNullCoalescingCondition
							"setting file path| " + rootFolderPath ?? "path is null");
					}
				}

				return SettingFolderPath + "\\" + fileName;
			}

			return null;
		}

		public void ConfigureFilePath()
		{
			if (settingFilePath.IsVoid())
			{
				settingFilePath = CreateSettingFilePath();
			}

		}

		protected abstract void Configure();

		protected string GetVersionFromFile(string elementName) => 
			CsXmlUtilities.ScanXmlForElementValue(SettingFilePath, elementName);
	}

#endregion

#region + Support Classes

	[DataContract(Namespace = Heading.N_SPACE)]
	public abstract class SettingInfoBase<TData> : IComparable<SettingInfoBase<TData>>
		where TData : new()
	{
		private bool? classVersionsMatch;

		[DataMember(Order = 0)]
		public abstract SettingFileType  FileType  { get; set; }

		[DataMember(Order = 5)]
		public Heading Header;

		[DataMember(Order = 10)]
		public TData Data { get; set; } = new TData();

		public abstract string Description                 { get; }
		public abstract string DataClassVersion            { get; }

		protected SettingInfoBase()
		{
			// ReSharper disable VirtualMemberCallInConstructor
			Header = new Heading(DataClassVersion)
			{
				Notes = "Created in Version " + DataClassVersion,
				Description = Description
			};
			// ReSharper restore VirtualMemberCallInConstructor
		}

		public int CompareTo(SettingInfoBase<TData> other) =>
			String.Compare(DataClassVersion, other.DataClassVersion, StringComparison.Ordinal);

		// ReSharper disable once UnusedParameter.Global
		public abstract void UpgradeFromPrior(SettingInfoBase<TData> prior);

		public bool ClassVersionsMatch(string fileClassVersion)
		{
			if (classVersionsMatch == null)
			{
				classVersionsMatch = fileClassVersion?.Equals(DataClassVersion) ?? true;
			}

			return classVersionsMatch.Value;
		}
	}

	public class BaseSettings<TPath, TInfo, TData>
		where TPath : PathAndFileBase, new()
		where TInfo : SettingInfoBase<TData>, new()
		where TData : new()
	{
		public static SettingsMgr<TPath, TInfo, TData> Admin { get;  }

		public static TPath Path { get;  } = new TPath();
		public static TInfo Info { get; private set; }
		public static TData Data { get; private set; }

		static BaseSettings()
		{
			Admin = new SettingsMgr<TPath, TInfo, TData>(Path, ResetData);
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
	// this file is stored here:
	// C:\Users\{user}\AppData\Roaming\{company name}\{suite name}\{program name}\
	public class UserSettingPath : PathAndFileBase
	{
		protected override void Configure()
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

	// define file type specific information: User
	[DataContract]
	public abstract class UserSettingInfoBase<TData> : SettingInfoBase<TData>
		where TData : new()
	{
		// [DataMember(Order = 0)]
		public override SettingFileType FileType
		{
			get => SettingFileType.SETTING_MGR_USER;
			set { }
		}
	}

	public class UserSettings :
		BaseSettings<UserSettingPath,
		UserSettingInfo<UserSettingData>,
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
	public class AppSettingPath : PathAndFileBase
	{
		protected override void Configure()
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
	public abstract class AppSettingInfoBase<TData> : SettingInfoBase<TData>
		where TData : new()
	{
		[DataMember(Order = 0)]
		public override SettingFileType FileType
		{
			get => SettingFileType.SETTING_MGR_APP;
			set { }
		}
	}

	internal class AppSettings :
		BaseSettings<AppSettingPath,
		AppSettingInfo<AppSettingData>,
		AppSettingData> { }

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
	public class SuiteSettingPath : PathAndFileBase
	{
		protected override void Configure()
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

	// define file type specific information: app
	[DataContract(Namespace = "")]
	public abstract class SuiteSettingInfoBase<TData> : SettingInfoBase<TData>
		where TData : new()
	{
		[DataMember(Order = 0)]
		public override SettingFileType FileType
		{
			get => SettingFileType.SETTING_MGR_SUITE;
			set { }
		}
	}

	public class SuiteSettings :
		BaseSettings<SuiteSettingPath,
		SuiteSettingInfo<SuiteSettingData>,
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
	public class MachSettingPath : PathAndFileBase
	{
		protected override void Configure()
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
	public abstract class MachSettingInfoBase<TData> : SettingInfoBase<TData>
		where TData : new()
	{
		[DataMember(Order = 0)]
		public override SettingFileType FileType
		{
			get => SettingFileType.SETTING_MGR_MACH;
			set { }
		}
	}

	public class MachSettings :
		BaseSettings<MachSettingPath,
		MachSettingInfo<MachSettingData>,
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
	public class SiteSettingPath : PathAndFileBase
	{
		protected override void Configure()
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
	public abstract class SiteSettingInfoBase<TData> : SettingInfoBase<TData>
		where TData : new()
	{
		[DataMember(Order = 0)]
		public override SettingFileType FileType
		{
			get => SettingFileType.SETTING_MGR_SITE;
			set { }
		}
	}

	public class SiteSettings :
		BaseSettings<SiteSettingPath,
		SiteSettingInfo<SiteSettingData>,
		SiteSettingData> { }

#endif

#endregion

#endregion

#region info class for data

	[DataContract(Name = "DataStorage", Namespace = "")]
	public class StorageMgrInfo<TData> : SettingInfoBase<TData>
		where TData : new ()
	{
		[DataMember(Order = 0)]
		public override SettingFileType FileType
		{
			get => SettingFileType.SETTING_MGR_DATA;
			set { }
		}

		public override string DataClassVersion => "1.0";
		public override string Description => "Data Storage";
		public override void UpgradeFromPrior(SettingInfoBase<TData> prior) { }
	}

#endregion

#region management classes for data

	public class StorageMgrPath : PathAndFileBase
	{
		protected override void Configure() { }
	}

	public class BaseDataFile<TData> : INotifyPropertyChanged
		where TData : class, new()
	{
		public bool Initialized => Path.HasFilePath;

		public SettingsMgr<StorageMgrPath, StorageMgrInfo<TData>, TData> Admin { get;  }
		public StorageMgrPath Path { get;  } = new StorageMgrPath();
		public StorageMgrInfo<TData> Info { get; private set; }
		public TData Data { get; private set; }

		public BaseDataFile()
		{
			Admin = new SettingsMgr<StorageMgrPath, StorageMgrInfo<TData>, TData>(Path, ResetData);
			ResetData();
		}

		public void ResetData()
		{
			Info = Admin.Info;
			Data = Info.Data;
		}

		public void Configure(string rootPath, string fileName)
		{
			Path.SubFolders = null;
			Path.RootFolderPath = rootPath;
			Path.FileName = fileName;

			Path.ConfigureFilePath();

			// ReSharper disable once ExplicitCallerInfoArgument
			OnPropertyChange("Initialized");
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
	}

#endregion
}