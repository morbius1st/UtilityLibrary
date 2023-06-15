#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using SettingsManager;
using SettingsManagerV40;
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

// standard settings manager

// ReSharper disable once CheckNamespace
namespace SettingsManager
{
#region + Header

	[DataContract(Namespace = N_SPACE)]
	//	[DataContract]
	public class Heading
	{
		public static string ClassVersionName = nameof(ClassVersion);
		public static string SystemVersionName = nameof(SystemVersion);

		public enum SettingFileType
		{
			APP = 0,
			USER = 1,
			MACH = 2,
			SITE = 3,

			// ReSharper disable once UnusedMember.Global
			LENGTH = 4
		}

		public const string N_SPACE = "";

		public Heading(string classVersion) => ClassVersion = classVersion;

		[DataMember(Order = 1)] public string SaveDateTime         = DateTime.Now.ToString("yyyy-MM-dd - HH:mm zzz");
		[DataMember(Order = 2)] public string AssemblyVersion      = CsUtilities.AssemblyVersion;
		[DataMember(Order = 3)] public string SystemVersion = "4.0";
		[DataMember(Order = 4)] public string ClassVersion;
		[DataMember(Order = 5)] public string Notes = "created by v4.0";
		[DataMember(Order = 6)] public string Description;
	}

#endregion

	public enum SettingMgrStatus
	{
		SAVEDISALLOWED = -4,
		INVALIDPATH = -3,
		NOPATH = -2,
		DOESNOTEXIST = -1,

		CONSTRUCTED = 0,
		INITIALIZED,
		CREATED,
		READ,
		SAVED,
		EXISTS
	}

	public delegate void RstData();

	public class SettingsMgr<T>
		where T : SettingBase, new()
	{
	#region + Constructor

		public SettingsMgr(RstData rst)
		{
			Status = CONSTRUCTED;

			if (String.IsNullOrWhiteSpace(Info.SettingPathAndFile))
			{
				Status = NOPATH;
			}

			_resetData = rst;
		}

		public void Initialize()
		{
			if (Status != NOPATH)
			{
				Status = INITIALIZED;

				UpgradeRequired = !Info.ClassVersionsMatch;

				if (FileExists())
				{
					if (!Info.ClassVersionsMatch && CanAutoUpgrade)
					{
						Upgrade();
					}
				}
			}
		}

	#endregion

	#region + Properties

		public T Info { get; private set; } =
			new T();

		public SettingMgrStatus Status { get; private set; }

		public bool CanAutoUpgrade { get; set; } = false;

		public bool UpgradeRequired { get; set; } = false;

	#endregion

	#region + Read

		public void Read()
		{
			if (Info.SettingPathAndFile.IsVoid() )
			{
				Status = INVALIDPATH;
				return;
			}

			// does the file already exist?
			if (FileExists())
			{
				if (Info.ClassVersionsMatch)
				{
					try
					{
						DataContractSerializer ds = new DataContractSerializer(typeof(T));

						// file exists - get the current values
						using (FileStream fs = new FileStream(Info.SettingPathAndFile, FileMode.Open))
						{
							Info = (T) ds.ReadObject(fs);
							
							Info.Configure();
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
							+ Info.SettingPathAndFile + MessageUtilities.nl
							+ "("                     + e.Message + ")" + MessageUtilities.nl
							, e.InnerException);
					}
				}
				else
				{
					Upgrade();
				}
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

					using (FileStream fs = new FileStream(Info.SettingPathAndFile, FileMode.Open))
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
			if (Info.SettingPathAndFile.IsVoid() )
			{
				Status = INVALIDPATH;
				return;
			}

			// since the data is "new" and may not match what
			// has been saved, note as not initialized
			Info = new T();

			Info.Configure();

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
			if (Info.IsReadOnly || Info.SettingPathAndFile.IsVoid())
			{
				Status = SAVEDISALLOWED;
				return;
			}

			XmlWriterSettings xmlSettings = new XmlWriterSettings() {Indent = true};

			DataContractSerializer ds = new DataContractSerializer(typeof(T));

			using (XmlWriter w = XmlWriter.Create(Info.SettingPathAndFile, xmlSettings))
			{
				ds.WriteObject(w, Info);
			}

			// since file and memory match
			Status = SAVED;
		}

	#endregion

	#region + Upgrade

		// upgrade from a prior class version to the current class version
		// this method is less restricted to allow the user to perform
		// a manual upgrade - if CanAutoUpgrade is false
		public void Upgrade()
		{
			// is an upgrade needed
			if (!Info.ClassVersionsMatch)
			{
				// get a list of prior setting classes from 
				// which to possible upgrade
				List<SettingBase> settings = null;

				switch (Info.FileType)
				{
				case Heading.SettingFileType.APP:
					{
						settings =
							GetMatchingClasses<AppSettingBase>();
						break;
					}
				case Heading.SettingFileType.USER:
					{
						settings =
							GetMatchingClasses<UserSettingBase>();
						break;
					}
				}

				// if needed, preform upgrade
				if (settings != null)
				{
					UpgradeList(settings);

					Save();
				}
			}

			// flag whether an upgrade is required
			// keep current
			UpgradeRequired = !Info.ClassVersionsMatch;
		}

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
			bool result = Info.Exists;

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
		private List<SettingBase> GetMatchingClasses<U>() where U : SettingBase
		{
			Type baseType = typeof(T).BaseType;

			List<SettingBase> list = new List<SettingBase> {Info};

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
		private void UpgradeList(List<SettingBase> settings)
		{
			if (settings == null || settings.Count < 2)
			{
				return;
			}

			if (Info.ClassVersionsMatch)
			{
				return;
			}

			settings.Sort(); // must be in the correct order from oldest to newest

			for (int i = 0; i < settings.Count; i++)
			{
				int j = String.Compare(settings[i].ClassVersion,
					Info.ClassVersionFromFile, StringComparison.Ordinal);

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

#region + Support Classes

	[DataContract(Namespace = Heading.N_SPACE)]
	//	[DataContract]
	public abstract class SettingBase : IComparable<SettingBase>
	{
		[DataMember] public Heading Header;

		public abstract Heading.SettingFileType  FileType { get; }
		public abstract bool ClassVersionsMatch           { get; }
		public abstract string ClassVersion               { get; }
		public abstract string ClassVersionFromFile       { get; }
		public abstract string SystemVersionFromFile      { get; }
		public abstract string FileName                   { get; }
		public abstract string RootPath                   { get; }
		public abstract string[] SubFolders               { get; }
		public abstract string SettingPath                { get; }
		public abstract string SettingPathAndFile         { get; }
		public abstract bool Exists                       { get; }
		public abstract string Description                { get; }
		public bool IsReadOnly                            { get; } = false;
		public bool PathGetsAssigned                      { get; } = false;
		public SettingBase() => Header = new Heading(ClassVersion)
		{
			Notes = "Created in Version " + ClassVersion,
			Description = Description
		};

		public int CompareTo(SettingBase other) =>
			String.Compare(ClassVersion, other.ClassVersion, StringComparison.Ordinal);

		public abstract void UpgradeFromPrior(SettingBase prior);

		public abstract void Configure();
	}

	[DataContract]
	// define file type specific information: User
	public abstract class UserSettingBase : SettingBase
	{
		public override bool Exists                      => PathAndFile.User.Exists;
		public override string SettingPath               => PathAndFile.User.SettingPath;

		/// <summary>
		/// User: et the path and file name for the setting file
		/// </summary>
		public override string SettingPathAndFile        => PathAndFile.User.SettingPathAndFile;
		public override string FileName                  => PathAndFile.User.FileName;
		public override string RootPath                  => PathAndFile.User.RootPath;
		public override string[] SubFolders              => PathAndFile.User.SubFolders;
		public override string ClassVersionFromFile      => PathAndFile.User.ClassVersionFromFile;
		public override string SystemVersionFromFile     => PathAndFile.User.SystemVersionFromFile;
		public override bool ClassVersionsMatch          => Header.ClassVersion.Equals(ClassVersionFromFile);
		public override Heading.SettingFileType FileType => Heading.SettingFileType.USER;

		public override void Configure() { }
	}

	[DataContract]
	// define file type specific information: App
	public abstract class AppSettingBase : SettingBase
	{
		public void SetSiteRootPath(string rootPath)
		{
			PathAndFile.Site.RootPath = rootPath;
		}

		public abstract string SiteRootPath { get; set; }


		public override bool Exists                      => PathAndFile.App.Exists;
		public override string SettingPath               => PathAndFile.App.SettingPath;
		/// <summary>
		/// App: et the path and file name for the setting file
		/// </summary>
		public override string SettingPathAndFile        => PathAndFile.App.SettingPathAndFile;
		public override string FileName                  => PathAndFile.App.FileName;
		public override string RootPath                  => PathAndFile.App.RootPath;
		public override string[] SubFolders              => PathAndFile.App.SubFolders;
		public override string ClassVersionFromFile      => PathAndFile.App.ClassVersionFromFile;
		public override string SystemVersionFromFile     => PathAndFile.App.SystemVersionFromFile;
		public override bool ClassVersionsMatch          => Header.ClassVersion.Equals(ClassVersionFromFile);
		public override Heading.SettingFileType FileType => Heading.SettingFileType.APP;
		public override void Configure() { }
	}

	[DataContract]
	// define file type specific information: Common
	public abstract class MachSettingBase : SettingBase
	{
		public new bool IsReadOnly => true;
		public new bool PathGetsAssigned => false;
		public override bool Exists                      => PathAndFile.Mach.Exists;
		public override string SettingPath               => PathAndFile.Mach.SettingPath;

		/// <summary>
		/// Common: et the path and file name for the setting file
		/// </summary>
		public override string SettingPathAndFile        => PathAndFile.Mach.SettingPathAndFile;

		public override string FileName                  => PathAndFile.Mach.FileName;
		public override string RootPath                  => PathAndFile.Mach.RootPath;
		public override string[] SubFolders              => PathAndFile.Mach.SubFolders;
		public override string ClassVersionFromFile      => PathAndFile.Mach.ClassVersionFromFile;
		public override string SystemVersionFromFile     => PathAndFile.Mach.SystemVersionFromFile;
		public override bool ClassVersionsMatch          => Header.ClassVersion.Equals(ClassVersionFromFile);
		public override Heading.SettingFileType FileType => Heading.SettingFileType.MACH;

		public override void Configure() { }
	}

	[DataContract]
	// define file type specific information: Common
	public abstract class SiteSettingBase : SettingBase
	{
		public new bool IsReadOnly => true;
		public new bool PathGetsAssigned => true;

		public override bool Exists                      => PathAndFile.Site.Exists;
		public override string SettingPath               => PathAndFile.Site.SettingPath;

		/// <summary>
		/// Site: et the path and file name for the setting file
		/// </summary>
		public override string SettingPathAndFile        => PathAndFile.Site.SettingPathAndFile;

//		public override string SettingPathAndFile        => PathAndFile.Site.SettingPathAndFile;
		public override string FileName                  => PathAndFile.Site.FileName;
		public override string RootPath                  => PathAndFile.Site.RootPath;
		public override string[] SubFolders              => PathAndFile.Site.SubFolders;
		public override string ClassVersionFromFile      => PathAndFile.Site.ClassVersionFromFile;
		public override string SystemVersionFromFile     => PathAndFile.Site.SystemVersionFromFile;
		public override bool ClassVersionsMatch          => Header.ClassVersion.Equals(ClassVersionFromFile);

		public override Heading.SettingFileType FileType => Heading.SettingFileType.SITE;
		public override void Configure() {}
	}

#endregion

#region + Path File and Utilities

	public static class PathAndFile
	{
		internal static SitePathAndFile Site = new SitePathAndFile();
		internal static MachPathAndFile Mach = new MachPathAndFile();
		internal static AppPathAndFile  App  = new AppPathAndFile();
		internal static UserPathAndFile User = new UserPathAndFile();

		private const string SETTINGFILEBASE = @".setting.xml";

		internal abstract class PathAndFileBase
		{
			private string savedPath = null;

			public abstract string FileName                { get; }
			public abstract string RootPath                { get; set; }
			public abstract string[]SubFolders             { get; }
			public abstract string ClassVersionFromFile    { get; }
			public abstract string SystemVersionFromFile   { get; }
			public bool IsReadOnly                         { get; } = false;
			public bool PathGetsAssigned                   { get; } = false;
			public bool Exists => File.Exists(SettingPathAndFile);

			public string SettingPath
			{
				get
				{
					if (SubFolders == null)
					{
						return RootPath;
					}

					return CsUtilities.SubFolder(SubFolders.Length - 1,
						RootPath, SubFolders);
				}
			}

			// get the path and the file name for the setting file
			public string SettingPathAndFile
			{
				get
				{
					if (!savedPath.IsVoid()) return savedPath;

					if (!RootPath.IsVoid())
					{
						if (!Directory.Exists(SettingPath))
						{
							if (!CsUtilities.CreateSubFolders(RootPath, SubFolders))
							{
								throw new DirectoryNotFoundException("setting file path");
							}
						}

						savedPath = SettingPath + "\\" + FileName;

						return savedPath;
					}

					return null;
				}
			}

			protected string GetVersionFromFile(string elementName) =>
				CsUtilities.ScanXmlForElementValue(SettingPathAndFile, elementName);
		}

		internal class SitePathAndFile : PathAndFileBase
		{
			public new bool IsReadOnly { get; } = true;
			public new bool PathGetsAssigned { get; } = true;
			public override string FileName => CsUtilities.AssemblyName + ".Site" +  SETTINGFILEBASE;

			public override string RootPath { get; set; } = null;

			public override string[] SubFolders => new []
			{
				CsUtilities.CompanyName,
				CsUtilities.AssemblyName
			};

			public override string ClassVersionFromFile => GetVersionFromFile(Heading.ClassVersionName);

			public override string SystemVersionFromFile => GetVersionFromFile(Heading.SystemVersionName);
		}

		internal class MachPathAndFile : PathAndFileBase
		{
			public override string FileName => CsUtilities.AssemblyName + ".Machine" + SETTINGFILEBASE;

			public override string RootPath
			{
				get => Environment. GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
				set { }
			}

			public override string[] SubFolders => new []
			{
				CsUtilities.CompanyName,
				CsUtilities.AssemblyName
			};

			public override string ClassVersionFromFile => GetVersionFromFile(Heading.ClassVersionName);

			public override string SystemVersionFromFile => GetVersionFromFile(Heading.SystemVersionName);
		}

		internal class AppPathAndFile : PathAndFileBase
		{
			public override string FileName => CsUtilities.AssemblyName + SETTINGFILEBASE;

			public override string RootPath {
				get => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				set { }
			}

			public override string[] SubFolders => new []
			{
				CsUtilities.CompanyName,
				CsUtilities.AssemblyName,
				"AppSettings"
			};

			public override string ClassVersionFromFile => GetVersionFromFile(Heading.ClassVersionName);

			public override string SystemVersionFromFile => GetVersionFromFile(Heading.SystemVersionName);
		}

		internal class UserPathAndFile : PathAndFileBase
		{
			public override string FileName => @"user" + SETTINGFILEBASE;

			public override string RootPath {
				get => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
				set { }

			}

			public override string[] SubFolders => new string []
			{
				CsUtilities.CompanyName,
				CsUtilities.AssemblyName
			};

			public override string ClassVersionFromFile => GetVersionFromFile(Heading.ClassVersionName);

			public override string SystemVersionFromFile => GetVersionFromFile(Heading.SystemVersionName);
		}
	}

#endregion

#region + management classes

#region user management root class

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

			Info.Configure();
		}

		public static void ResetData()
		{
			// this makes sure the above static class points
			// to the current data structure
			Info  = Admin.Info;
			Data  = Info.Data;

			Info.Configure();
		}
	}

#endregion

#endregion


}