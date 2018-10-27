using System;
using System.IO;
using System.Reflection;


// note GetBitmapImage moved to the file CsUtilitiesMedia
// note GetImage moved to the file CsUtilitiesMedia

namespace UtilityLibrary
{
	public class MessageException : Exception
	{
		public MessageException(string msg, Exception inner) : base(msg, inner) { }
	}

	public static class CsUtilities
	{
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

		internal static string AssemblyName => typeof(CsUtilities).Assembly.GetName().Name;

		internal static string AssemblyVersion =>
			typeof(CsUtilities).Assembly.GetName().Version.ToString();

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

		internal static bool CreateSubFolders(string rootPath, string[] subFolders)
		{
			if (subFolders == null || !Directory.Exists(rootPath)) { return false; }

			for (int i = 0; i < subFolders.Length; i++)
			{
				string path = SubFolder(i, rootPath, subFolders);

				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
			}

			return true;
		}

		internal static string SubFolder(int i, string rootPath, string[] subFolders)
		{
			if (i < 0 ||
				i >= subFolders.Length) return null;

			string path = rootPath;
			for (int j = 0; j < i + 1; j++)
			{
				path += "\\" + subFolders[j];
			}

			return path;
		}

//		// search test string for any characters in the "invalid" string
//		internal static bool ValidateStringChars(string test, string invalid)
//		{
//			bool result = true;
//			// by validate - 
//			// flag any basic non-print characters < \x20
//			// flag any characters in invalid
//			foreach (char c in test)
//			{
//				if (Char.IsControl(c) ||
//					invalid.IndexOf(c) >= 0)
//				{
//					result = false;
//					break;
//				}
//			}
//			return result;
//		}

		// search test string for any characters in the "invalid" char array
		internal static bool ValidateStringChars(string test, char[] invalid)
		{
			bool result = true;
			// by validate - 
			// flag any basic non-print characters < \x20
			// flag any characters in invalid
			foreach (char c in invalid)
			{
				if (test.IndexOf(c) >= 0)
				{
					result = false;
					break;
				}
			}
			return result;
		}

//		// return true if char is OK (not control and not in the invalid string)
//		internal static bool ValidateChar(char c, string invalid)
//		{
//			return !(Char.IsControl(c) || invalid.IndexOf(c) >= 0);
//		}

		// return true if char is OK (not control and not in the invalid string)
		internal static bool ValidateChar(char c, char[] invalid)
		{
			return !(Char.IsControl(c) || invalid.IndexOf(c) >= 0);
		}


	}

	public static class Extensions
	{
		public static int IndexOf<T>(this T[] array, T find)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(find)) return i;
			}

			return -1;
		}
	}
}


