#region  
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;
using static SettingManager.SettingMgrStatus;

using UtilityLibrary;
using static UtilityLibrary.MessageUtilities2;
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

// standard settings manager

// ReSharper disable once CheckNamespace
namespace SettingManager
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
			// ReSharper disable once UnusedMember.Global
			LENGTH = 2
		}

		public const string N_SPACE = "";

		public Heading(string classVersion) => ClassVersion = classVersion;

		[DataMember(Order = 1)] public string SaveDateTime         = DateTime.Now.ToString("yyyy-MM-dd - HH:mm zzz");
		[DataMember(Order = 2)] public string AssemblyVersion      = CsUtilities.AssemblyVersion;
		[DataMember(Order = 3)] public string SystemVersion = "3.0";
		[DataMember(Order = 4)] public string ClassVersion;
		[DataMember(Order = 5)] public string Notes = "created by v3.0";
	}

	#endregion

	public enum SettingMgrStatus
	{
		DOESNOTEXIST = -2,
		NOPATH = -1,
		
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

		public T Info { get; private set; } = new T();

		public SettingMgrStatus Status { get; private set; }

		public bool CanAutoUpgrade { get; set; } = false;

		public bool UpgradeRequired { get; set; } = false;

		#endregion

		#region + Read

		public void Read()
		{
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
							SyncData();
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
			// since the data is "new" and may not match what
			// has been saved, note as not initialized
			Info = new T();

			Status = CREATED;
		}

		#endregion

		#region + Save

		public void Save()
		{
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

		#region +Upgrade

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

		#region +Reset

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

	#region Support Classes

	[DataContract(Namespace = Heading.N_SPACE)]
	//	[DataContract]
	public abstract class SettingBase : IComparable<SettingBase>
	{
		[DataMember] public Heading Header;

		public abstract Heading.SettingFileType  FileType { get; }
		public abstract bool ClassVersionsMatch { get; }

		public abstract string ClassVersion { get; }
		public abstract string ClassVersionFromFile { get; }
		public abstract string SystemVersionFromFile { get; }
		public abstract string FileName { get; }
		public abstract string RootPath { get; }
		public abstract string[] SubFolders { get; }

		public abstract string SettingPath { get; }
		public abstract string SettingPathAndFile { get; }

		public abstract bool Exists { get; }

		public SettingBase() => Header = new Heading(ClassVersion) { Notes = "Created in Version " + ClassVersion };

		public int CompareTo(SettingBase other) => String.Compare(ClassVersion, other.ClassVersion, StringComparison.Ordinal);

		public abstract void UpgradeFromPrior(SettingBase prior);
	}

	[DataContract]
	// define file type specific information: User
	public abstract class UserSettingBase : SettingBase
	{
		public override bool Exists => PathAndFile.User.Exists;

		public override string SettingPath => PathAndFile.User.SettingPath;
		public override string SettingPathAndFile => PathAndFile.User.SettingPathAndFile;

		public override string FileName => PathAndFile.User.FileName;
		public override string RootPath => PathAndFile.User.RootPath;
		public override string[] SubFolders => PathAndFile.User.SubFolders;

		public override string ClassVersionFromFile => PathAndFile.User.ClassVersionFromFile;

		public override string SystemVersionFromFile => PathAndFile.User.SystemVersionFromFile;

		public override bool ClassVersionsMatch => Header.ClassVersion.Equals(ClassVersionFromFile);

		public override Heading.SettingFileType FileType => Heading.SettingFileType.USER;
	}

	[DataContract]
	// define file type specific information: App
	public abstract class AppSettingBase : SettingBase
	{
		public override bool Exists => PathAndFile.App.Exists;

		public override string SettingPath        => PathAndFile.App.SettingPath;
		public override string SettingPathAndFile => PathAndFile.App.SettingPathAndFile;

		public override string   FileName   => PathAndFile.App.FileName;
		public override string   RootPath   => PathAndFile.App.RootPath;
		public override string[] SubFolders => PathAndFile.App.SubFolders;

		public override string ClassVersionFromFile => PathAndFile.App.ClassVersionFromFile;

		public override string SystemVersionFromFile => PathAndFile.App.SystemVersionFromFile;

		public override bool ClassVersionsMatch => Header.ClassVersion.Equals(ClassVersionFromFile);

		public override Heading.SettingFileType FileType => Heading.SettingFileType.APP;
	}

	#endregion

	#region + Path File and Utilities

	public static class PathAndFile
	{
		internal static AppPathAndFile  App  = new AppPathAndFile();
		internal static UserPathAndFile User = new UserPathAndFile();

		private const string SETTINGFILEBASE = @".setting.xml";

		internal abstract class PathAndFileBase
		{
			public abstract string   FileName              { get; }
			public abstract string   RootPath              { get; }
			public abstract string[] SubFolders            { get; }
			public abstract string   ClassVersionFromFile  { get; }
			public abstract string   SystemVersionFromFile { get; }

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
					if (!Directory.Exists(SettingPath))
					{
						if (!CsUtilities.CreateSubFolders(RootPath, SubFolders))
						{
							throw new DirectoryNotFoundException("setting file path");
						}
					}
					return SettingPath + "\\" + FileName;
				}
			}

			protected string GetVersionFromFile(string elementName) => CsUtilities.ScanXmlForElementValue(SettingPathAndFile, elementName);
		}

		internal class AppPathAndFile : PathAndFileBase
		{
			public override string FileName => CsUtilities.AssemblyName + SETTINGFILEBASE;

			public override string RootPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

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

			public override string RootPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

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
}
