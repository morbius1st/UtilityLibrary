#region + Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

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
6.  done Add GetCurrentDirectory
7.  done getpath = all of the parts as an array
8.  done make volume be the Unc volume and change to UncVloume, add Uncpath be the unc volume + its path
9.  done change root to RootVolume
10. done add switch to provide as unc when possible?

version
2.0		*redo - addFilePathInfo, add checks for unc
2.1		*adjust to properly allow serialize and de-serialize (adjust GetFullFilePath:set to make object and parse)
		*rename field to filePathInfo / adjust AssemblyPaths to be consistent with indexer
2.2		*move some static routines & constants to a separate class and update references
3.0		* add static routine to split a file path into folderpath, filenameNoExt, and extension
		* provide Up and Down methods to allow adjusting the folderpath Up one folder level or
			Down a folder level based on the supplied folder name
		* adjust indexer to provide filename + extension for index of 0 and
			with a prefaced '\' when "near +0" e.g. 0.1
		* adjust assembly folders to provide the whole path on a bad index or index of zero and
			don't provide or provide a preface '\' when positive or negative index (respectively)
		* adjusted Unc methods to better match their non-Unc method names
		* adjust AssemblyFolders to have a switch to provide the preface folder separator
		* adjust AssemblyFolders to use negative values to provide the folders in reverse order
		* misc fixes

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
#region filepath

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

		private const string VERSION = "3.0";

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
		public bool HasFileName => !filePathInfo.FileNameNoExt.IsVoid();
		public bool IsFolderPath => filePathInfo.IsFolderPath;
		public bool IsFilePath => filePathInfo.IsFilePath;
		public bool IsFound => filePathInfo.IsFound;
		public bool Exists => filePathInfo.IsFound;

	#endregion

	#region public properties

		public FilePathInfo<T> FilePathInfo => filePathInfo;

		/// <summary>
		/// The full path and the file name
		/// </summary>
		[DataMember]
		public string FullFilePath
		{
			get
			{
				if (UseUnc)
				{
					return UncFolderPath + FilePathUtil.PATH_SEPARATOR + FileName;
				}

				return filePathInfo.Path;
			}

			set => MakeFilePathInfo(value);
		}

		/// <summary>
		/// The path without the filename and extension
		/// </summary>
		public string FolderPath
		{
			get
			{
				if (!IsValid) return null;

				return AssemblePath(Depth);
			}
		}

		/// <summary>
		/// The whole file name (name + extension) when applicable<br/>
		/// Null otherwise
		/// </summary>
		public string FileName
		{
			get
			{
				return filePathInfo.FileName;


				// if (filePathInfo.FileNameNoExt.IsVoid() &&
				// 	filePathInfo.Extension.IsVoid()) return null;
				//
				// if (filePathInfo.Extension.IsVoid()) return filePathInfo.FileNameNoExt;
				//
				// return filePathInfo.FileNameNoExt + "." + filePathInfo.Extension;
			}
		}

		/// <summary>
		/// File name sans extension
		/// </summary>
		public string FileNameNoExt => filePathInfo.FileNameNoExt;

		/// <summary>
		/// File extension, with separator, sans file name
		/// </summary>
		public string Extension => filePathInfo.Extension;

		/// <summary>
		/// File extension, without separator, sans file name
		/// </summary>
		public string FileExtensionNoSep => filePathInfo.ExtensionNoSep;

		/// <summary>
		/// The drive volume letter (i.e. C)
		/// </summary>
		public string DriveVolume => filePathInfo.DriveVolume;

		/// <summary>
		/// The drive volume with a drive suffix (i.e. C:)
		/// </summary>
		public string DrivePath => filePathInfo.DriveVolume + FilePathUtil.DRV_SUFFIX;

		/// <summary>
		/// The drive identifier with a slash suffix (i.e. C:\)
		/// </summary>
		public string DriveRoot => DrivePath + FilePathUtil.PATH_SEPARATOR;

		/// <summary>
		/// The path without the filename and extension and using unc if exists
		/// </summary>
		public string UncFullFilePath
		{
			get
			{
				if (!IsValid) return null;

				return UncVolume == null ? null : AssemblePath(Depth, true) + FilePathUtil.PATH_SEPARATOR + FileName;
			}
		}

		/// <summary>
		/// The path without the filename and extension and using unc if exists
		/// </summary>
		public string UncFolderPath
		{
			get
			{
				if (!IsValid) return null;

				return AssemblePath(Depth, true);
			}
		}

		/// <summary>
		/// The full path using the UNC equivalent when applies
		/// </summary>
		public string UncRoot => UncVolume == null ? null : filePathInfo.UncVolume + filePathInfo.UncShare;

		/// <summary>
		/// The UNC Volume identifier
		/// </summary>
		public string UncVolume => filePathInfo.UncVolume;

		/// <summary>
		/// The UNC path without the UNC volume
		/// </summary>
		public string UncShare => filePathInfo.UncShare;

		/// <summary>
		/// The filename object
		/// </summary>
		public T FileNameObject => filePathInfo.FileNameObject;

		[IgnoreDataMember]
		public string[] GetPathNames => PathNames(false);

		[IgnoreDataMember]
		public string[] GetPathNamesAlt => PathNames(true);

		public string Folders
		{
			get
			{
				if (!IsValid) return null;

				return AssembleFolders();
			}
		}

		public int FolderCount => filePathInfo.Folders.Count;

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
		public int Length => FullFilePath.Length;

		/// <summary>
		/// cause information returned to provide the UncShare
		/// rather than the GetDrivePath
		/// </summary>
		public bool UseUnc { get; set; } = false;

	#endregion

	#region public methods

		public void Up()
		{
			// string adjFilePath = AssemblePath(-2) + FilePathUtil.PATH_SEPARATOR +
			// 	( FileName != null ? FilePathUtil.PATH_SEPARATOR + FileName : null);

			string adjFilePath = FilePathUtil.AssembleFilePathS(FileNameNoExt, FileExtensionNoSep, AssemblePath(-2));

			MakeFilePathInfo(adjFilePath);
		}

		public void Down(string folder)
		{
			// string adjFilePath = FolderPath  + FilePathUtil.PATH_SEPARATOR + folder + 
			// 	( FileName != null ? FilePathUtil.PATH_SEPARATOR + FileName : null);
			//
			string adjFilePath = FilePathUtil.AssembleFilePathS(FileNameNoExt, FileExtensionNoSep,
				FolderPath, folder);

			MakeFilePathInfo(adjFilePath);
		}

		// public void ChangeFileName(string name)
		// {
		// 	filePathInfo.Name = name;
		// }
		//
		// public void ChangeExtension(string ext)
		// {
		// 	filePathInfo.Extension = ext;
		// }

		/// <summary>
		/// Change the name and extension of the file<br/>
		/// if empty string is provided, keep the existing component, if both are empty string, do nothing.<br/>
		/// if null is provided, remove that component (make it null).
		/// </summary>
		/// <param name="fileNameNoExt"></param>
		/// <param name="ext"></param>
		public void ChangeFileName(string fileNameNoExt, string ext = "")
		{
			fileNameNoExt = fileNameNoExt?.TrimStart(null) ?? null; // PreserveSigAttribute any trailing spaces
			ext = ext?.Trim() ?? null;

			if ((fileNameNoExt?.Length ?? 1) == 0 && (ext?.Length ?? 1) == 0) return;

			if (fileNameNoExt == null || fileNameNoExt.Length > 0)
			{
				filePathInfo.FileNameNoExt = fileNameNoExt;
			}

			if (ext == null || ext.Length > 0)
			{
				filePathInfo.Extension = ext;
			}

			MakeFilePathInfo(FilePathUtil.AssembleFilePathS(
				filePathInfo.FileNameNoExt,
				filePathInfo.ExtensionNoSep,
				FolderPath));
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

			if (idx < 0 || idx == int.MaxValue) return null;

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

		public string AssembleFolders(int index = 0, bool separatorPreface = false)
		{
			if (!IsValid) return null;

			bool reverse = index < 0;

			index = index >= 0 ? index : -1 * index;

			int idx = (index == 0 || index >= Depth - 1) ? Depth - 1 : index;

			// Debug.WriteLine("index| " + index + "  init idx| " + idx  + "  depth| " + Depth);

			// StringBuilder sb = new StringBuilder();
			//
			// for (int i = 0; i < idx; i++)
			// {
			// 	sb.Append((separatorPreface || i > 0) ? FilePathUtil.PATH_SEPARATOR : "")
			// 	.Append(filePathInfo.Folders[i]);
			// }
			//
			// return sb.ToString();

			if (reverse) return AssemblyFoldersReverse(idx, separatorPreface);

			return AssemblyFoldersForward(idx, separatorPreface);
		}

		public void MakeFilePathInfo(string path)
		{
			filePathInfo = new FilePathInfo<T>();

			IsValid = filePathInfo.parse(path);
		}

		public FilePath<T> Clone()
		{
			return new FilePath<T>(FullFilePath);
		}

	#endregion

	#region indexer

		public string this[double index]
		{
			get
			{
				if (!IsValid) return null;

				int idx = CalcIndex((int) index);

				// add path separator prefix?
				bool makePath = !(index % 1).Equals(0);
				//
				// if (idx <= 0 &&
				// 	filePathInfo.Folders.Count == 0) return GetRootPath();

				if (idx <= 0) return null;

				if (idx == int.MaxValue)
					return (makePath ? FilePathUtil.PATH_SEPARATOR : "") + FileName;

				return
					(makePath ? FilePathUtil.PATH_SEPARATOR : "") +
					filePathInfo.Folders[idx - 1];
			}
		}

	#endregion

	#region private methods

		private string AssemblyFoldersForward(int idx, bool separatorPreface)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < idx; i++)
			{
				sb.Append((separatorPreface || i > 0) ? FilePathUtil.PATH_SEPARATOR : "")
				.Append(filePathInfo.Folders[i]);
			}

			return sb.ToString();
		}

		private string AssemblyFoldersReverse2(int idx, bool separatorPreface)
		{
			StringBuilder sb = new StringBuilder();

			for (int i = FolderCount - 1; i > idx - 1; i--)
			{
				sb.Append((separatorPreface || i > 0) ? FilePathUtil.PATH_SEPARATOR : "")
				.Append(filePathInfo.Folders[i]);
			}

			return sb.ToString();
		}

		private string AssemblyFoldersReverse(int idx, bool separatorPreface)
		{
			StringBuilder sb = new StringBuilder();

			// string A0 = filePathInfo.Folders[0];
			// string A1 = filePathInfo.Folders[1];
			// string A2 = filePathInfo.Folders[2];

			idx = idx > FolderCount ? FolderCount : idx;


			for (int i = FolderCount - 1; i >= FolderCount - idx; i--)
			{
				sb.Insert(0, filePathInfo.Folders[i]);
				sb.Insert(0, ((separatorPreface || i > FolderCount - idx) ? FilePathUtil.PATH_SEPARATOR : ""));
			}

			return sb.ToString();
		}

		private string GetRootPath(bool useUnc = false)
		{
			if ((useUnc || UseUnc) && HasUnc)
			{
				return UncRoot;
			}

			return DrivePath;
		}

		// private int CalcIndex(int index)
		// {
		// 	if (index + 1 >= Depth ||
		// 		(-1 * index) > Depth) return Depth - 1;
		//
		// 	if (index < 0) index = Depth + index;
		//
		// 	return index;
		// }

		private int CalcIndex(int index)
		{
			if (index > Depth ||
				(-1 * index) > Depth) return - 1;

			if (index == 0) { index = int.MaxValue; }
			else if (index > 0) { index -= 1; }
			else if (index < 0) { index = Depth + index; }

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
			return this.FullFilePath.ToUpper().Equals(other.FullFilePath.ToUpper());
		}

		public int CompareTo(FilePath<T> other)
		{
			return FullFilePath.CompareTo(other.FullFilePath);
		}

		public override string ToString()
		{
			return FullFilePath;
		}

	#endregion
	}

#endregion

	public static class DllImports
	{
		[DllImport("mpr.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		public static extern int WNetGetConnection(
			[MarshalAs(UnmanagedType.LPTStr)] string localName,
			[MarshalAs(UnmanagedType.LPTStr)] StringBuilder remoteName,
			ref int length);
	}

#region file path info

	public class FilePathInfo<T> : INotifyPropertyChanged where T : AFileName, new()
	{
	#region public fields

		// public static Dictionary<string, string> UncNameMap { get; private set; }  =
		// 	new Dictionary<string, string>(10);

	#endregion

	#region private fields

		private string originalFullPath; // original - un-modified;

		private string uncVolume; // '//' + computer name

		private string uncShare; // all except for the uncVolume

		private string driveVolume; // drive letter + colon

		private List<string> folders; // all of the folders along the path

//		private string fileName;

		private T fileNameObject = new T();

	#endregion

	#region public properties

		public string Path
		{
			get => originalFullPath;
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

		public T FileNameObject => fileNameObject;


		/// <summary>
		/// the whole name of the file - File Name + extension
		/// </summary>
		public string FileName => fileNameObject.FileName;


		/// <summary>
		/// the name of the file - no extension
		/// </summary>
		public string FileNameNoExt
		{
			get => fileNameObject.FileNameNoExt;
			set
			{
				fileNameObject.FileNameNoExt = value;
				UpdateFilePath();
				OnPropertyChange();
			}
		}

		/// <summary>
		/// the file's extension with the file separator
		/// </summary>
		public string Extension
		{
			get
			{
				return fileNameObject.ExtensionNoSep.IsVoid()
					? null
					: FilePathUtil.EXT_SEPARATOR +
					fileNameObject.ExtensionNoSep;
			}
			set
			{
				fileNameObject.ExtensionNoSep = value;
				UpdateFilePath();
				OnPropertyChange();
			}
		}

		/// <summary>
		/// the file's extension with the file separator
		/// </summary>
		public string ExtensionNoSep
		{
			get => fileNameObject.ExtensionNoSep;
			set
			{
				fileNameObject.ExtensionNoSep = value;
				UpdateFilePath();
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

		/// <summary>
		/// extracts the filename and file extension from a partial path string
		/// do not use this on just a folder path - returned information will
		/// not be correct
		/// </summary>
		/// <param name="foldersAndFile"></param>
		/// <returns>the original string with the fileneme and file extension removed</returns>
		public static string splitFolderAndFile(string foldersAndFile,
			out string fileNameNoExt, out string extension)
		{
			fileNameNoExt = null;
			extension = null;

			if (foldersAndFile.IsVoid()) return null;

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
				extension = foldersAndFile.Substring(posPeriod + 1);
				fileNameNoExt = foldersAndFile.Substring(posEndSeparator + 1, posPeriod - posEndSeparator - 1);
			}
			else if (result == 1)
			{
				// case C
				extension = foldersAndFile.Substring(posPeriod + 1);
				fileNameNoExt = "";
			}
			else
			{
				// case B
				// case D
				extension = "";
				fileNameNoExt = foldersAndFile.Substring(posEndSeparator + 1);
			}

			return foldersAndFile.Substring(0, posEndSeparator);
		}

	#endregion

	#region private methods

		private void UpdateFilePath()
		{
			originalFullPath = FolderPath;

			if (IsFilePath)
			{
				originalFullPath += FilePathUtil.PATH_SEPARATOR + FileNameObject.FileName;
			}

			OnPropertyChange("Path");
		}

	#endregion


	#region parse

		internal bool parse(string pathWay)
		{
			this.originalFullPath = pathWay;

			if (pathWay.IsVoid()) return false;

			string remain;

			parseReset();

			string path = FilePathUtil.CleanPath<T>(pathWay);

			IsFound = parseIsFound(pathWay);

			remain = parseVolume(path);

			remain = parseFileAndExtension(remain, IsFolderPath);

			folders =
				parseFolders(remain);

			if (IsFilePath)
			{
				if (FileNameNoExt == null && Extension == null || !FileNameObject.IsValid)
				{
					return false;
				}
			}

			return true;
		}

		private void parseReset()
		{
			folders = new List<string>();
			uncVolume = null;
			driveVolume = null;
			uncShare = null;
			fileNameObject = new T();
		}

		private bool parseIsFound(string pathWay)
		{
			bool isFolderPath;
			bool isFilePath;

			bool isFound = FilePathUtil.Exists(pathWay, out isFolderPath, out isFilePath);

			IsFilePath = isFilePath;

			IsFolderPath = isFolderPath;

			// if both are false
			// in this case, the path does not point to a real location
			// determine if this is a file path or a folder path
			if (!isFilePath && !isFolderPath)
			{
				IsFilePath = !(IsFolderPath = ParseIsFolderPath(pathWay));
			}

			return isFound;
		}

		// both IsFilePath && IsFolderPath are false
		// in this case, the path does not point to a real location
		// determine if this is a folder path
		private bool ParseIsFolderPath(string pathWay)
		{
			int pathDivider = pathWay.LastIndexOf('\\');
			int fileNameDivider = pathWay.LastIndexOf('.');

			return !((fileNameDivider - pathDivider) > 0);
		}

		private List<string> parseFolders(string folders)
		{
			bool i  = IsFolderPath;

			if (folders.IsVoid()) return new List<string>();

			return new List<string>(folders.Split(new char[] { FilePathUtil.PATH_SEPARATOR_C },
				StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// extracts the filename and file extension from a partial path string
		/// IsFilePath & IsFolderPath must be determined before calling
		/// </summary>
		/// <param name="foldersAndFile"></param>
		/// <returns>the original string with the fileneme and file extension removed</returns>
		private string parseFileAndExtension(string foldersAndFile, bool isAFolderPath = false)
		{
			if (isAFolderPath) return foldersAndFile;

			if (foldersAndFile.IsVoid()) return null;

			string filenameNoExt;
			string extension;

			string folderPath = splitFolderAndFile(foldersAndFile, out filenameNoExt, out extension);

			fileNameObject.ExtensionNoSep = extension;
			fileNameObject.FileNameNoExt = filenameNoExt;

			return folderPath;

			//
			// // note, do not do a full check - don't test
			// // IsFilePath as the path may refer to a currently
			// // non-existent file.  However, if it is
			// // for sure a folder path, return
			// // this will allow folder paths that looks like 
			// // a filename and file extension to not be parsed wrong
			// if (IsFolderPath) return foldersAndFile;
			//
			// // possible cases
			// // A  (>0)
			// //    0         1
			// //    012345678901234
			// // ...\path\file.ext  ('.'=10, '\'=5 -> 10-5 = 5)
			// // B  (<0)
			// //    0         1
			// //    012345678901234
			// // ...\path\file      ('.'=0, '\'=5 -> 0-5 = -5)
			// // C  (== 1)
			// //    0         1
			// //    012345678901234
			// // ...\path\.ext      ('.'=6, '\'=5 -> 6-5 = 1)
			// // D  (<0)
			// //    0         1
			// //    012345678901234
			// // ...\path.x\file    ('.'=5, '\'=7 -> 5-7 = -2)
			// // E  (>0)
			// //    0         1
			// //    012345678901234
			// //    file.ext    ('.'=4, '\'=-1 -> 4-(-1) = 5)
			// // F  (>0)
			// //    0         1
			// //    012345678901234
			// //    file.    ('.'=5, '\'=-1 -> 4-(-1) = 5)
			// // G  (==0)
			// //    0         1
			// //    012345678901234
			// //    fileorpath    ('.'=0, '\'=-1 -> 0-(-1) = 1)
			//
			// int posEndSeparator = foldersAndFile.LastIndexOf(FilePathUtil.PATH_SEPARATOR_C);
			//
			// // convert cases E, F, G to A, B, or C
			// if (posEndSeparator == -1)
			// {
			// 	foldersAndFile = FilePathUtil.PATH_SEPARATOR_C + foldersAndFile;
			// 	posEndSeparator = 0;
			// }
			//
			// int posPeriod = foldersAndFile.LastIndexOf(FilePathUtil.EXT_SEPARATOR_C);
			// int result = posPeriod - posEndSeparator;
			//
			// // note set extension first
			// if (result > 1)
			// {
			// 	// case A
			// 	fileName.Extension = foldersAndFile.Substring(posPeriod + 1);
			// 	fileName.FileNameNoExt = foldersAndFile.Substring(posEndSeparator + 1, posPeriod - posEndSeparator - 1);
			// }
			// else if (result == 1)
			// {
			// 	// case C
			// 	fileName.Extension = foldersAndFile.Substring(posPeriod + 1);
			// 	fileName.FileNameNoExt = "";
			// }
			// else
			// {
			// 	// case B
			// 	// case D
			// 	fileName.Extension = "";
			// 	fileName.FileNameNoExt = foldersAndFile.Substring(posEndSeparator + 1);
			// }
			//
			// return foldersAndFile.Substring(0, posEndSeparator);
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
			return originalFullPath;
		}

	#endregion
	}

#endregion

#region FILE PATH UTILS

	// filepath helper utilities
	public static class FilePathUtil
	{
		public const string PATH_SEPARATOR = @"\";
		public const char PATH_SEPARATOR_C = '\\';
		public const string DRV_SUFFIX = ":";
		public const char DRV_SUFFIX_C = ':';
		public const string UNC_PREFACE = @"\\";
		public const char EXT_SEPARATOR_C = '.';
		public const string EXT_SEPARATOR = ".";
		public const string BACKUP_EXTNOSEP = "bak";
		public const string XML_EXTNOSEP = "xml";

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
		/// assemble a list of folder names into a folderpath with separators <br/>
		/// between each folder name (of course)<br/>
		/// the preface separator is optional based on 'addPreface'
		/// </summary>
		/// <param name="folderList"></param>
		/// <returns></returns>
		public static string AssembleFolderPath(bool addPreface, params string[] folderList)
		{
			string folderPath = null;

			if ((folderList?.Length ?? 0) > 0)
			{
				for (int i = 0; i < folderList.Length; i++)
				{
					folderPath += (i > 0 || addPreface ? PATH_SEPARATOR : "") + folderList[i];
				}
			}

			return folderPath;
		}


		public static FilePath<FileNameSimple> AssembleFilePath(string fileNameNoExt, string extensionNoSep,
			params string[] folderList)
		{
			return new FilePath<FileNameSimple>(AssembleFilePathS(fileNameNoExt, extensionNoSep, folderList));
		}


		/// <summary>
		/// Assembly the path parts into a single path string
		/// </summary>
		/// <param name="fileNameNoExt"></param>
		/// <param name="extensionNoSep"></param>
		/// <param name="folderList"></param>
		/// <returns></returns>
		public static string AssembleFilePathS(string fileNameNoExt, string extensionNoSep, params string[] folderList)
		{
			if (fileNameNoExt.IsVoid() && extensionNoSep.IsVoid() && folderList == null) return null;

			string folderPath = AssembleFolderPath(false, folderList);

			// if ((folderList?.Length ?? 0) > 0)
			// {
			// 	for (int i = 0; i < folderList.Length; i++)
			// 	{
			// 		folderPath += (i > 0 ? PATH_SEPARATOR : "") + folderList[i];
			// 	}
			//
			//
			// 	// foreach (string f in folderList)
			//
			// 	// {
			// 	// 	folderPath += PATH_SEPARATOR + f;
			// 	// }
			// }

			string result = fileNameNoExt + (extensionNoSep.IsVoid() ? null : EXT_SEPARATOR + extensionNoSep);

			return folderPath + (folderPath.IsVoid() || result.IsVoid() ? null : PATH_SEPARATOR) + result;

			// return folderPath
			// 	+ (!folderPath.IsVoid() && !result.IsVoid() ? PATH_SEPARATOR : null)
			// 	+ (result.IsVoid() ? null : result);
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

#region FILE NAME CLASSES

	public interface IFileName
	{
		/// <summary>
		/// The complete file name including the<br/>
		/// separation character
		/// </summary>
		string FileName { get; }

		/// <summary>
		/// The file's name without the file extension<br/>
		/// and without the separation character
		/// </summary>
		string FileNameNoExt { get; set; }

		/// <summary>
		/// The file's extension wihtout the separation character
		/// </summary>
		string ExtensionNoSep { get; set; }

		/// <summary>
		/// indicates that either or both FileNameNoExt<br/>
		/// and ExtensionNoSep are not null / has a value
		/// </summary>
		bool IsValid { get; }
	}


	public abstract class AFileName : IEquatable<AFileName>, IComparable<AFileName>, IFileName
	{
	#region protected fields

		protected string fileNameNoExt;

		// needs to be just the extension and no separator character
		protected string extensionNoSep;
		// protected bool isValid;

	#endregion

	#region public properties

		// public virtual string FileName => fileNameNoExt + FilePathUtil.EXT_SEPARATOR + extension;
		public virtual string FileName => FilePathUtil.AssembleFilePathS(fileNameNoExt, ExtensionNoSep, null);

		/// <summary>
		/// The file name without the extension or separator
		/// </summary>
		public virtual string FileNameNoExt
		{
			get => fileNameNoExt;
			set => fileNameNoExt = value;
		}

		/// <summary>
		/// The file's extension with no separator
		/// </summary>
		public virtual string ExtensionNoSep
		{
			get => extensionNoSep;
			set
			{
				string result = value.Trim();

				result = result.IsVoid() ? result.Substring(result[0] == FilePathUtil.EXT_SEPARATOR_C ? 1 : 0) : null;

				extensionNoSep = value;
			}
		}

		public virtual bool IsValid
		{
			get
			{
				if (fileNameNoExt == null && extensionNoSep == null) return false;

				return true;
			}
		}

	#endregion

	#region system overrides

		public bool Equals(AFileName other)
		{
			return FileNameNoExt.Equals(other.FileNameNoExt) &&
				ExtensionNoSep.Equals(other.ExtensionNoSep);
		}

		public int CompareTo(AFileName other)
		{
			int result = FileNameNoExt.CompareTo(other.FileNameNoExt);

			if ( result != 0) return result;

			return ExtensionNoSep.CompareTo(other.ExtensionNoSep);
		}

	#endregion
	}

	public class FileNameSimple : AFileName, INotifyPropertyChanged
	{
	#region event handling

		public event PropertyChangedEventHandler PropertyChanged;

		private void OnPropertyChange([CallerMemberName] string memberName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
		}

	#endregion

		public override bool IsValid
		{
			get
			{
				if (fileNameNoExt == null && extensionNoSep == null) return false;

				return true;
			}
		}
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

		public override bool IsValid
		{
			get
			{
				if (SheetName == null || SheetNumber == null && ExtensionNoSep == null) return false;

				return true;
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

#endregion

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
note: use index of [#.x] toprepend a slash ---------------------------------v
[+4} or [-1] FolderName 3                                    v----------v   [+3.1] or [-1.1] = \FolderName 3
[+3] or [-2] FolderName 2                       v----------v |          |   [+2.1] or [-2.1] = \FolderName 2
[+2] or [-3] FolderName 1          v----------v |          | |          |   [+1.1] or [-3.1] = \FolderName 1
[+1] or [-4] P:                 vv |          | |          | |          |   [+0.1] or [-4.1] = P:\
[+0] New Text Document.txt      || |          | |          | |          | v-------------------v
DriveRoot                       v-v|          | |          | |          | |                   |
DrivePath                       vv||          | |          | |          | |				   |
DriveVolume                     v|||          | |          | |          | |				   |
FullFilePath                    P:\FolderName 1\FolderName 2\FolderName 3\New Text Document.txt
Path                            ^---------------------------------------^ |               | | |
Folders                          |^-------------------------------------^ |               | | |
FileNameWithoutExtension         ||                                     | ^---------------^ | | (does not include '.')
Extension                        ||                                     | |                 ^-^ (does not include '.')
FileName                         ||                                     | ^-------------------^ (does include '.')
equivalent unc:                  ||                                     | 
UncFullFilePath   \\CS-006\P Drive\FolderName 1\FolderName 2\FolderName 3\New Text Document.txt
(with UseUnc)     |      ||       |                                     | 
UncVolume         ^------^|       |                                     |
UncShare          |       ^-------^                                     |
UncRoot           ^---------------^                                     |
UncFolderPath     ^-----------------------------------------------------^


get array: GetPathNames    -> [0] same as indexer[0],   [1] same as indexer [1], etc.
get array: GetPathNamesAlt -> [0] same as indexer[0.1], [1] same as indexer [1.1], etc.

AssemblePath(1)     | P:\FolderName 1
AssemblePath(-1)    | P:\FolderName 1\FolderName 2\FolderName 3

*** need to verify (same as assemblePath without the drive path / unc share
AssembleFolders(1)	| \FolderName 1
AssembleFolders(-1)	| \FolderName 1\FolderName 2\FolderName 3

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