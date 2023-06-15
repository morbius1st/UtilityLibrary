#region + Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using System.Text;
using SysPath = System.IO.Path;

using static UtilityLibrary.CsExtensions;

#endregion

// itemname: FilePath
// username: jeffs
// created:  11/2/2019 5:18:09 PM

/*
1.  ignore Change name to PathEx?
2.  done - Make properties use Get...
3.  done - Add a const for path separator.
4.  Follow Path class for properties
5.  Validate path characters
6.  done fdAdd GetCurrentDirectory
7.  done getpath = all of the parts as an array
8.  done make volume be the Unc volume and change to UncVloume, add Uncpath be the unc volume + its path
9.  done change root to RootVolume
10. done add switch to provide as unc when possible?

version
2.0		redo - addFilePathInfo, add checks for unc
2.1		adjust to properly allow serialize and de-serialize (adjust GetFullFilePath:set to make object and parse)
		rename field to filePathInfo / adjust AssemblyPaths to be consistent with indexer
2.2		move some static routines & constants to a separate class and update references


usage:
New FilePath<file name type>
+-> MakeFilePathInfo()
	+-> new FilePathInfo<T>.parse(path)
		+-> parse(path)
			+-> parse components
				+-> parseFileAndExtension (... filename = new T() : AFileName)
					+-> @FileNameSimple: 
					|	+-> fileName.Extension = ... (no override - uses AFileName)
					|	+-> fileName.Name = ... (no override - uses AFileName)
					|
					+-> @FileNameAsSheetFile: 
					|	+-> fileName.Extension = ... (no override - uses AFileName)
					|	+-> fileName.Name = ... (@overriden - ParseName() -> splits up name per fixed rules)
					|
					+-> @FileNameAsSheetPdf: 
						+-> fileName.Extension = ... (no override - uses AFileName)
						+-> fileName.Name = ... (@overriden - ParseName())
					



 */


namespace UtilityLibrary
{
	[DataContract(Namespace = "")]
	public class FilePath<T> : IEquatable<FilePath<T>>, IComparable<FilePath<T>>, INotifyPropertyChanged
		where T : AFileName, new()
	{
		// internal const string PATH_SEPARATOR = @"\";
		// internal const char PATH_SEPARATOR_C = '\\';
		// internal const string DRV_SUFFIX = ":";
		// internal const char DRV_SUFFIX_C = ':';
		// internal const string UNC_PREFACE = @"\\";
		// internal const char EXT_SEPARATOR_C = '.';

		private const string VERSION = "2.1";

	#region private fields

		private FilePathInfo<T> filePathInfo;

	#endregion

	#region static properties

		/// <summary>
		/// a FilePath that is not valid (IsValid == false)
		/// </summary>
		public static FilePath<T> Invalid => new FilePath<T>();

		/// <summary>
		/// the current directory
		/// </summary>
		public static FilePath<T> CurrentDirectory => new FilePath<T>(Environment.CurrentDirectory);

	#endregion

	#region ctor

		public FilePath(string initialPath)
		{
			ConfigureFilePath(initialPath);
			OnPropertyChange("GetFullFilePath");
		}

		public FilePath()
		{
			ConfigureFilePath(null);
		}

		public FilePath(string[] path)
		{
			StringBuilder sb = new StringBuilder(path[0]);

			for (int i = 1; i < path.Length; i++)
			{
				sb.Append(FilePathUtil.PATH_SEPARATOR).Append(path[i]);
			}

			ConfigureFilePath(sb.ToString());

		}

		private void ConfigureFilePath(string initialPath)
		{
			// FilePathInfo<T>.
			FilePathUtil.getUncNameMap();

			MakeFilePathInfo(initialPath);
		}

	#endregion

	#region  public status properties

		public string Version => VERSION;

		public bool IsValid { get; private set; }
		public bool HasQualifiedPath => filePathInfo.HasQualifiedPath;
		public bool HasUnc => filePathInfo.HasUnc;
		public bool HasDrive => filePathInfo.HasDrive;
		public bool HasFileName => !filePathInfo.Name.IsVoid();
		public bool IsFolderPath => filePathInfo.IsFolderPath;
		public bool IsFilePath => filePathInfo.IsFilePath;
		public bool IsFound => filePathInfo.IsFound;

	#endregion

	#region public properties

		public FilePathInfo<T> FilePathInfo => filePathInfo;

		/// <summary>
		/// The full path and the file name
		/// </summary>
		[DataMember]
		public string GetFullFilePath
		{
			get
			{
				if (UseUnc)
				{
					return GetPathUnc + "\\" + GetFileName;
				}

				return filePathInfo.Path;
			}
			set => MakeFilePathInfo(value);
		}

		/// <summary>
		/// The drive volume letter (i.e. C)
		/// </summary>
		public string GetDriveVolume => filePathInfo.DriveVolume;

		/// <summary>
		/// The drive volume with a drive suffix (i.e. C:)
		/// </summary>
		public string GetDrivePath => filePathInfo.DriveVolume + FilePathUtil.DRV_SUFFIX;

		/// <summary>
		/// The drive identifier with a slash suffix (i.e. C:\)
		/// </summary>
		public string GetDriveRoot => GetDrivePath + FilePathUtil.PATH_SEPARATOR;

		/// <summary>
		/// The UNC Volume identifier
		/// </summary>
		public string GetUncVolume => filePathInfo.UncVolume;

		/// <summary>
		/// The full path using the UNC equivalent when applies
		/// </summary>
		public string GetUncPath => filePathInfo.UncVolume + filePathInfo.UncShare;

		/// <summary>
		/// The UNC path without the UNC volume
		/// </summary>
		public string GetUncShare => filePathInfo.UncShare;

		/// <summary>
		/// The filename object
		/// </summary>
		public T GetFileNameObject => filePathInfo.FileName;

		/// <summary>
		/// The whole file name (name + extension) when applicable<br/>
		/// Null otherwise
		/// </summary>
		public string GetFileName
		{
			get
			{
				if (filePathInfo.Name.IsVoid() &&
					filePathInfo.Extension.IsVoid()) return null;

				if (filePathInfo.Extension.IsVoid()) return filePathInfo.Name;

				return filePathInfo.Name + "." + filePathInfo.Extension;
			}
		}

		/// <summary>
		/// File name sans extension
		/// </summary>
		public string GetFileNameWithoutExtension => filePathInfo.Name;

		/// <summary>
		/// File extension, with separator, sans file name
		/// </summary>
		public string GetFileExtension => FilePathUtil.EXT_SEPARATOR_C + filePathInfo.Extension;

		/// <summary>
		/// File extension, without separator, sans file name
		/// </summary>
		public string GetFileExtensionNoSeparator => filePathInfo.Extension;

		/// <summary>
		/// The path without the filename and extension
		/// </summary>
		public string GetPath
		{
			get
			{
				if (!IsValid) return null;

				return AssemblePath(Depth);
			}
		}

		/// <summary>
		/// The path without the filename and extension and using unc if exists
		/// </summary>
		public string GetPathUnc
		{
			get
			{
				if (!IsValid) return null;

				return AssemblePath(Depth, true);
			}
		}

		[IgnoreDataMember]
		public string[] GetPathNames => PathNames(false);

		[IgnoreDataMember]
		public string[] GetPathNamesAlt => PathNames(true);

		public string GetFolders
		{
			get
			{
				if (!IsValid) return null;

				return AssembleFolders();
			}
		}

		public int GetFolderCount => filePathInfo.Folders.Count;

		/// <summary>
		/// The number of folders deep counting the volume as a depth
		/// </summary>
		public int Depth
		{
			get
			{
				int d = HasDrive ? 1 : 0;

				d += filePathInfo.Folders.Count;

				return d;
			}
		}

		/// <summary>
		/// Number of characters in the Full Path<br/>
		/// Affected by GetUNC
		/// </summary>
		public int Length => GetFullFilePath.Length;

		/// <summary>
		/// cause information returned to provide the UncShare
		/// rather than the GetDrivePath
		/// </summary>
		public bool UseUnc { get; set; } = false;

	#endregion

	#region public methods

		public void ChangeFileName(string name)
		{
			filePathInfo.Name = name;
		}

		public void ChangeExtension(string ext)
		{
			filePathInfo.Extension = ext;
		}

		public string[] PathNames(bool withBackSlash = false, bool useUnc = false)
		{
			if (!IsValid) return null;

			List<string> path = new List<string>();

			path.Add(GetRootPath(useUnc));

			for (int i = 0; i < Depth - 1; i++)
			{
				path.Add(
					(withBackSlash ? FilePathUtil.PATH_SEPARATOR : "") +
					filePathInfo.Folders[i]);
			}

			return path.ToArray();
		}

		public string AssemblePath(int index, bool useUnc = false)
		{
			if (!IsValid) return null;

			int idx = CalcIndex(index);

			if (idx < 0) return null;

			if (idx == 0)
			{
				return GetRootPath(useUnc);
			}

			StringBuilder sb = new StringBuilder(GetRootPath(useUnc));

//			for (int i = 0; i < idx - 1; i++)
			for (int i = 0; i < idx; i++)
			{
				sb.Append(FilePathUtil.PATH_SEPARATOR).Append(filePathInfo.Folders[i]);
			}

			return sb.ToString();
		}

		public string AssembleFolders(int index = 0)
		{
			if (!IsValid) return null;

			int idx = CalcIndex(index);

			if (idx < 0) return null;

			// do not return just the root
			if (idx == 0) idx = Depth;

			StringBuilder sb = new StringBuilder();

			for (int i = 1; i < idx - 1; i++)
			{
				sb.Append(FilePathUtil.PATH_SEPARATOR).Append(filePathInfo.Folders[i - 1]);
			}

			return sb.ToString();
		}

		public FilePath<T> Clone()
		{
			return new FilePath<T>(GetFullFilePath);
		}

	#endregion

	#region indexer

		public string this[double index]
		{
			get
			{
				if (!IsValid) return null;

				bool makePath = !(index % 1).Equals(0);

				int idx = CalcIndex((int) index);

				if (idx < 0) return null;

				if (idx == 0 ||
					filePathInfo.Folders.Count == 0) return GetRootPath();

				return
					(makePath ? FilePathUtil.PATH_SEPARATOR : "") +
					filePathInfo.Folders[idx - 1];
			}
		}

	#endregion

	#region private methods

		private void MakeFilePathInfo(string path)
		{
			filePathInfo = new FilePathInfo<T>();

			IsValid = filePathInfo.parse(path);
		}

		private string GetRootPath(bool useUnc = false)
		{
			if ((useUnc || UseUnc) && HasUnc)
			{
				return GetUncPath;
			}

			return GetDrivePath;
		}

		private int CalcIndex(int index)
		{
			if (index + 1 >= Depth ||
				(-1 * index) > Depth) return Depth - 1;

			if (index < 0) index = Depth + index;

			return index;
		}

	#endregion

	#region event handling

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system methods

		public bool Equals(FilePath<T> other)
		{
			return this.GetFullFilePath.ToUpper().Equals(other.GetFullFilePath.ToUpper());
		}

		public int CompareTo(FilePath<T> other)
		{
			return GetFullFilePath.CompareTo(other.GetFullFilePath);
		}

		public override string ToString()
		{
			return GetFullFilePath;
		}

	#endregion
	}

	public static class DllImports
	{
		[DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int WNetGetConnection(
			[MarshalAs(UnmanagedType.LPTStr)] string localName,
			[MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
			ref int length);
	}

	public class FilePathInfo<T> : INotifyPropertyChanged where T : AFileName, new()
	{
	#region public fields

		// public static Dictionary<string, string> UncNameMap { get; private set; }  =
		// 	new Dictionary<string, string>(10);

	#endregion

	#region private fields

		private string path; // original - un-modified;

		private string uncVolume; // '//' + computer name

		private string uncShare; // all except for the uncVolume

		private string driveVolume; // drive letter + colon

		private List<string> folders; // all of the folders along the path

//		private string fileName;

		private T fileName = new T();

	#endregion

	#region public properties

		public string Path
		{
			get => path;
			set
			{
				parse(value);
				OnPropertyChange();
			}
		}

		public string UncVolume
		{
			get => uncVolume;
			private set => uncVolume = value;
		}

		public string UncShare
		{
			get => uncShare;
			private set => uncShare = value;
		}

		public string DriveVolume
		{
			get => driveVolume;
			private set => driveVolume = value;
		}

		public List<string> Folders
		{
			get => folders;
			private set => folders = value;
		}

		public T FileName => fileName;

		/// <summary>
		/// the name of the file - no extension
		/// </summary>
		public string Name
		{
			get => fileName.FileNameNoExt;
			set
			{
				fileName.FileNameNoExt = value;
				UpdatePath();
				OnPropertyChange();
			}
		}

		/// <summary>
		/// the file's extension
		/// </summary>
		public string Extension
		{
			get => fileName.Extension;
			set
			{
				fileName.Extension = value;
				UpdatePath();
				OnPropertyChange();
			}
		}

		public string FolderPath
		{
			get
			{
				StringBuilder sb =  new StringBuilder();

				if (HasDrive)
				{
					sb.Append(driveVolume).Append(FilePathUtil.DRV_SUFFIX);
				}
				else
				{
					sb.Append(uncVolume);
				}

				foreach (string folder in folders)
				{
					sb.Append(FilePathUtil.PATH_SEPARATOR).Append(folder);
				}

				return sb.ToString();
			}
		}

	#endregion



	#region public status properties

		public bool HasQualifiedPath => HasDrive && (IsFolderPath || IsFilePath);

		public bool IsFolderPath { get; private set; }

		public bool IsFilePath { get; private set; }

		public bool IsFound { get; private set; }

		public bool HasDrive => !driveVolume.IsVoid();

		public bool HasUnc => !uncVolume.IsVoid();

	#endregion

	#region public methods

		private void UpdatePath()
		{
			path = FolderPath;

			if (IsFilePath)
			{
				path += FilePathUtil.PATH_SEPARATOR + FileName.FullName;
			}

			OnPropertyChange("Path");
		}

	#endregion

	#region parse

		public bool parse(string pathWay)
		{
			this.path = pathWay;

			if (pathWay.IsVoid()) return false;

			string remain;

			parseReset();

			string path = FilePathUtil.CleanPath<T>(pathWay);

			IsFound = parseIsFound(pathWay);

			remain = parseVolume(path);

			remain = parseFileAndExtension(remain);

			folders =
				parseFolders(remain);

			return true;
		}

		private void parseReset()
		{
			folders = new List<string>();
			uncVolume = null;
			driveVolume = null;
			uncShare = null;
			fileName = new T();
		}

		private bool parseIsFound(string pathWay)
		{
			bool isFolderPath;
			bool isFilePath;

			bool isFound = FilePathUtil.Exists(pathWay, out isFolderPath, out isFilePath);

			IsFolderPath = isFolderPath;
			IsFilePath = isFilePath;

			return isFound;
		}

		private List<string> parseFolders(string folders)
		{
			if (folders.IsVoid()) return new List<string>();

			return new List<string>(folders.Split(new char[] {FilePathUtil.PATH_SEPARATOR_C},
				StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// extracts the filename and file extension from a partial path string
		/// IsFilePath & IsFolderPath must be determined before calling
		/// </summary>
		/// <param name="foldersAndFile"></param>
		/// <returns>the original string with the fileneme and file extension removed</returns>
		private string parseFileAndExtension(string foldersAndFile)
		{
			if (foldersAndFile.IsVoid()) return null;

			// note, do not do a full check - don't test
			// IsFilePath as the path may refer to a currently
			// non-existent file.  However, if it is
			// for sure a folder path, return
			// this will allow folder paths that looks like 
			// a filename and file extension to not be parsed wrong
			if (IsFolderPath) return foldersAndFile;

			// possible cases
			// A  (>0)
			//    0         1
			//    012345678901234
			// ...\path\file.ext  ('.'=10, '\'=5 -> 10-5 = 5)
			// B  (<0)
			//    0         1
			//    012345678901234
			// ...\path\file      ('.'=0, '\'=5 -> 0-5 = -5)
			// C  (== 1)
			//    0         1
			//    012345678901234
			// ...\path\.ext      ('.'=6, '\'=5 -> 6-5 = 1)
			// D  (<0)
			//    0         1
			//    012345678901234
			// ...\path.x\file    ('.'=5, '\'=7 -> 5-7 = -2)
			// E  (>0)
			//    0         1
			//    012345678901234
			//    file.ext    ('.'=4, '\'=-1 -> 4-(-1) = 5)
			// F  (>0)
			//    0         1
			//    012345678901234
			//    file.    ('.'=5, '\'=-1 -> 4-(-1) = 5)
			// G  (==0)
			//    0         1
			//    012345678901234
			//    fileorpath    ('.'=0, '\'=-1 -> 0-(-1) = 1)

			int posEndSeparator = foldersAndFile.LastIndexOf(FilePathUtil.PATH_SEPARATOR_C);

			// convert cases E, F, G to A, B, or C
			if (posEndSeparator == -1)
			{
				foldersAndFile = FilePathUtil.PATH_SEPARATOR_C + foldersAndFile;
				posEndSeparator = 0;
			}

			int posPeriod = foldersAndFile.LastIndexOf(FilePathUtil.EXT_SEPARATOR_C);
			int result = posPeriod - posEndSeparator;

			// note set extension first
			if (result > 1)
			{
				// case A
				fileName.Extension = foldersAndFile.Substring(posPeriod + 1);
				fileName.FileNameNoExt = foldersAndFile.Substring(posEndSeparator + 1, posPeriod - posEndSeparator - 1);
			}
			else if (result == 1)
			{
				// case C
				fileName.Extension = foldersAndFile.Substring(posPeriod + 1);
				fileName.FileNameNoExt = "";
			}
			else
			{
				// case B
				// case D
				fileName.Extension = "";
				fileName.FileNameNoExt = foldersAndFile.Substring(posEndSeparator + 1);
			}

			return foldersAndFile.Substring(0, posEndSeparator);
		}

		private string parseVolume(string path)
		{
			if (path.IsVoid()) return null;

			uncShare = FilePathUtil.UncVolumeFromPath(path);
			driveVolume = FilePathUtil.DriveVolumeFromPath(path);

			if (!driveVolume.IsVoid())
			{
				return parseRemainderFromVolume(path);
			}


		// #pragma warning disable CS0219 // The variable 'result' is assigned but its value is never used
		// 	string result = null;
		// #pragma warning restore CS0219 // The variable 'result' is assigned but its value is never used

			if (path.StartsWith(FilePathUtil.PATH_SEPARATOR))
			{
				return extractUncVolume(path);
			}

			return parseRemainder(path);
		}

		private string parseRemainder(string path)
		{
			if (path[0] == FilePathUtil.PATH_SEPARATOR_C &&
				path.Length == 1) return null;

			return path;
		}

		private string parseRemainderFromVolume(string path)
		{
			// maybe A or B
			int pos = 2;

			parseUncVolume();

			if (path.StartsWith(FilePathUtil.PATH_SEPARATOR) && !uncShare.IsVoid())
			{
				pos = uncVolume.Length + uncShare.Length;
			}

			if (path.Length == pos) return null;

			return path.Substring(pos);
		}

		private void parseUncVolume()
		{
			if (uncShare.IsVoid()) return;

			int pos = uncShare.IndexOf(FilePathUtil.PATH_SEPARATOR_C, 2);

			if (pos == -1)
			{
				uncVolume = uncShare;
				uncShare = null;
				return;
			}

			if (pos > 2)
			{
				uncVolume = uncShare.Substring(0, pos);
				uncShare = uncShare.Substring(pos);
				return;
			}

			uncVolume = null;
			uncShare = null;
		}

		private string extractUncVolume(string path)
		{
			if (path.Length == 2) return null;

			int pos = path.IndexOf(FilePathUtil.PATH_SEPARATOR_C, 2);

			if (pos == -1)
			{
				uncVolume = path;
				return null;
			}
			else if (pos > 2)
			{
				uncVolume = path.Substring(0, pos);
				return path.Substring(pos);
			}

			return null;
		}

	#endregion

	#region public static methods

	#endregion

	#region event handling

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

	#region system overrites

		public override string ToString()
		{
			return path;
		}

	#endregion
	}

#region filepathutils

	// filepath helper utilities
	public static class FilePathUtil
	{
		public const string PATH_SEPARATOR = @"\";
		public const char PATH_SEPARATOR_C = '\\';
		public const string DRV_SUFFIX = ":";
		public const char DRV_SUFFIX_C = ':';
		public const string UNC_PREFACE = @"\\";
		public const char EXT_SEPARATOR_C = '.';
		public const char EXT_SEPARATOR = EXT_SEPARATOR_C;

	#region public static methods

		/// <summary>
		/// determine if the path points of an
		/// actual file or folder
		/// </summary>
		/// <param name="path">String of the folder or file determine if it exists</param>
		/// <param name="isFolderPath">out bool - true if the path references a folder</param>
		/// <param name="isFilePath">out bool - true if the path references a file</param>
		/// <returns>true if path points to an actual file or folder</returns>
		public static bool Exists(string path, out bool isFolderPath, out bool isFilePath)
		{
			isFolderPath = false;
			isFilePath = false;
			try
			{
				// note: must check for file existing first
				isFilePath = File.Exists(path);
			}
			catch
			{
				/* ignored */
			}

			if (!isFilePath)
			{
				try
				{
					isFolderPath = Directory.Exists(path);
				}
				catch
				{
					isFolderPath = false;
				}
			}

			return isFolderPath || isFilePath;
		}


		/// <summary>
		/// clean the string that represents a path -
		/// replace slashes with back slashes
		/// keep preface and suffix spaces
		/// </summary>
		/// <param name="path">the file path to clean</param>
		/// <returns></returns>
		public static string CleanPath<T>(string path) where T : AFileName, new()
		{
			if (string.IsNullOrWhiteSpace(path))
			{
				return null;
			}

			string result;

			try
			{
				result = path.Replace('/', FilePathUtil.PATH_SEPARATOR_C).Trim();

				if (result[1] == FilePathUtil.DRV_SUFFIX_C)
				{
					result = result.Substring(0, 1).ToUpper() + result.Substring(1);
				}
			}
			catch
			{
				return null;
			}

			return result;
		}

	#region public fields

		public static Dictionary<string, string> UncNameMap { get; private set; }  =
			new Dictionary<string, string>(10);

	#endregion

		public static void getUncNameMap() // where T : AFileName, new()
		{
			DriveInfo[] drives = DriveInfo.GetDrives();

			foreach (DriveInfo di in drives)
			{
				if (di.IsReady)
				{
					string driveletter = di.Name.Substring(0, 2);

					string unc = UncVolumeFromPath(driveletter);

					if (!string.IsNullOrWhiteSpace(unc))
					{
						if (!UncNameMap.ContainsKey(driveletter))
						{
							UncNameMap.Add(driveletter, unc);
						}
					}
				}
			}
		}

		public static string UncVolumeFromPath(string path) //where T : AFileName, new()
		{
			if (string.IsNullOrWhiteSpace(path) ||
				path.Length < 2
				) return null;

			if (!path.StartsWith(FilePathUtil.UNC_PREFACE))
			{
				StringBuilder sb = new StringBuilder(1024);
				int size = sb.Capacity;

				// still may fail but has a better chance;
				int error = DllImports.WNetGetConnection(path.Substring(0, 2), sb, ref size);

				if (error != 0) return null;

				return sb.ToString();
			}

			return findUncFromUncPath(path);
		}

		public static string DriveVolumeFromPath(string path) //where T : AFileName, new()
		{
			if (path.IsVoid()) return null;

			string drive = findDriveFromUncPath(path);

			if (!drive.IsVoid()) return drive;


			if (!path.StartsWith(FilePathUtil.UNC_PREFACE))
			{
				// does not start with "\\" if character 2 is ':' 
				// assume provided with a drive and return that portion
				if (path.Substring(1, 1).Equals(FilePathUtil.DRV_SUFFIX)) return path.Substring(0, 1);
			}

			return null;
		}

		public static string findDriveFromUncPath(string path) // where T : AFileName, new()
		{
			if (string.IsNullOrWhiteSpace(path) ||
				!path.StartsWith(FilePathUtil.UNC_PREFACE) ||
				path.Length < 3) return null;

			if (UncNameMap == null || UncNameMap.Count == 0) getUncNameMap();

			foreach (KeyValuePair<string, string> kvp in UncNameMap)
			{
				int len = kvp.Value.Length;

				if (path.Length < len) continue;

				if (kvp.Value.ToLower().Equals(path.Substring(0, len).ToLower())) return kvp.Key;
			}

			return null;
		}

		public static string findUncFromUncPath(string path) // where T : AFileName, new()
		{
			if (string.IsNullOrWhiteSpace(path)
				|| !path.StartsWith(FilePathUtil.UNC_PREFACE)
				) return null;

			if (UncNameMap == null || UncNameMap.Count == 0) getUncNameMap();

			foreach (KeyValuePair<string, string> kvp in UncNameMap)
			{
				int len = kvp.Value.Length;

				if (path.Length < len) continue;

				if (kvp.Value.ToLower().Equals(path.Substring(0, len).ToLower())) return kvp.Value;
			}

			return null;
		}


	#endregion
	}
	

#endregion

	public abstract class AFileName : IEquatable<AFileName>, IComparable<AFileName>
	{
	#region protected fields

		protected string fileNameNoExt;
		protected string fileextension;

	#endregion

	#region public properties

		public virtual string FullName => fileNameNoExt + FilePathUtil.EXT_SEPARATOR + fileextension;

		/// <summary>
		/// The file name without the extension or separator
		/// </summary>
		public virtual string FileNameNoExt
		{
			get => fileNameNoExt;
			set => fileNameNoExt = value;
		}

		/// <summary>
		/// The file's extension
		/// </summary>
		public virtual string Extension
		{
			get => fileextension;
			set => fileextension = value;
		}

	#endregion

	#region system overrides

		public bool Equals(AFileName other)
		{
			return FileNameNoExt.Equals(other.FileNameNoExt) &&
				Extension.Equals(other.Extension);
		}

		public int CompareTo(AFileName other)
		{
			int result = FileNameNoExt.CompareTo(other.FileNameNoExt);

			if ( result != 0) return result;

			return Extension.CompareTo(other.Extension);
		}

	#endregion
	}

	public class FileNameSimple : AFileName, INotifyPropertyChanged
	{
//		public FileNameSimple() { }

//		public FileNameSimple(string filename)
//		{
//			this.filename = filename;
//		}

	#region event handling

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion
	}


	public class FileNameAsSheetFile : AFileName, INotifyPropertyChanged
	{
		private string sheetnumber;
		private string sheetname;

		public string SheetNumber
		{
			get => sheetnumber;
			set
			{
				sheetnumber = value;
				OnPropertyChange();
			}
		}

		public string SheetName
		{
			get => sheetname;
			set
			{
				sheetname = value;
				OnPropertyChange();
			}
		}

		public override string FileNameNoExt
		{
			get => sheetnumber + " :: " + sheetname;
			set
			{
				if (value.IsVoid())
				{
					SheetNumber = "";
					SheetName = "";

					return;
				}

				ParseName(value);
				OnPropertyChange("");
			}
		}

		private void ParseName(string name)
		{
			sheetnumber = name?.Substring(0, 5) ?? null;
			sheetname = name?.Substring(6) ?? null;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}
	}


	public abstract class FileExtensionClassifier
	{
		public abstract string[] fileTypes { get; set; }

		public bool IsCorrectFileType(string extNoDot)
		{
			string searchText = extNoDot.ToLower();

			foreach (string fileType in fileTypes)
			{
				if (fileType.Equals(searchText)) return true;
			}

			return false;
		}
	}
}

/*
Depth = 4
GetFolderCount = 3
Length = 63 (characters long)

the below is modified by UseUnc

use indexer
note: use index of [#.x] to prepend a slash -----------------------------v
FolderName 3  [+3] or [-1]                                v----------v   [+3.1] or [-1.1] = \FolderName 3
FolderName 2  [+2] or [-2]                   v----------v |          |   [+2.1] or [-2.1] = \FolderName 2
FolderName 1  [+1] or [-3]      v----------v |          | |          |   [+1.1] or [-3.1] = \FolderName 1
P:            [+0] or [-4]   vv |          | |          | |          |   [+0.1] or [-4.1] = P:\
GetDriveRoot                 v-v|          | |          | |          |
GetDrivePath                 vv||          | |          | |          |
GetDriveVolume               v|||          | |          | |          |
GetFullFilePath                  P:\FolderName 1\FolderName 2\FolderName 3\New Text Document.txt
GetPath                      ^---------------------------------------^ |               | | |
GetFolders                    |^-------------------------------------^ |               | | |
equivalent unc:               ||                                     | |               | | |
GetFullFilePath    \\CS-006\P Drive\FolderName 1\FolderName 2\FolderName 3\New Text Document.txt
(with UseUnc)  |      ||      |                                      | |               | | | 
GetUncVolume   ^------^|      |                                      | |               | | |
GetUncShare    |       ^------^                                      | |               | | |
GetUncPath     ^--------------^                                      | |               | | |
GetPathUnc     ^-----------------------------------------------------^ |               | | |
GetFileNameWithoutExtension                                          | ^---------------^ | | (does not include '.')
GetExtension                                                         | |                 ^-^ (does not include '.')
GetFileName                                                          | ^-------------------^ (does include '.')

get array: GetPathNames    -> [0] same as indexer[0],   [1] same as indexer [1], etc.
get array: GetPathNamesAlt -> [0] same as indexer[0.1], [1] same as indexer [1.1], etc.

AssemblePath(1)     | P:\FolderName 1
AssemblePath(-1)    | P:\FolderName 1\FolderName 2

*** need to verify (same as assemblePath without the drive path / unc share
AssembleFolders(1)	| \FolderName 1
AssembleFolders(-1)	| \FolderName 1\FolderName 2

*** Settings ***
UseUnc              | False

*** Status ***
IsValid             | True
HasQualifiedPath    | True
HasUnc              | True
HasDrive            | True
hasFilename         | True
IsFolderPath        | False
IsFilePath          | True
IsFound             | True


*/