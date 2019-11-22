using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;


// note GetBitmapImage moved to the file CsUtilitiesMedia
// note GetImage moved to the file CsUtilitiesMedia

namespace UtilityLibrary
{
	public class MessageException : Exception
	{
		public MessageException(string msg,
			Exception inner) : base(msg, inner)
		{
		}
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
					return CsExtensions.IfNullOrWhiteSpace(((AssemblyCompanyAttribute) att[0]).Company, "CyberStudio");
				}

				throw new MissingFieldException("Company is Missing from Assembly Information");
			}
		}

		internal static string AssemblyName => CsExtensions.IfNullOrWhiteSpace(typeof(CsUtilities).Assembly.GetName().Name, "DefaultAssembly");

		internal static string AssemblyVersion => typeof(CsUtilities).Assembly.GetName().Version.ToString();

		internal static string AssemblyDirectory
		{
			get
			{
				string     codebase = Assembly.GetExecutingAssembly().CodeBase;
				UriBuilder uri      = new UriBuilder(codebase);
				string     path     = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		internal static bool CreateSubFolders(string rootPath,
			string[] subFolders)
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

		internal static string SubFolder(int i,
			string rootPath,
			string[] subFolders)
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

		// search test string for any characters in the "invalid" char array
		internal static bool ValidateStringChars(string test,
			char[] invalid)
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

		// return true if char is OK (not control and not in the invalid string)
		internal static bool ValidateChar(char c,
			char[] invalid)
		{
			return !(Char.IsControl(c) || Extensions.IndexOf(invalid, c) >= 0);
		}

		// scan an xml file for a specific element and return its value
		internal static string ScanXmlForElementValue(string PathAndFile, string elementName)
		{
			if (!File.Exists(PathAndFile))
			{
				return null;
			}

			using (XmlReader reader = XmlReader.Create(PathAndFile))
			{
				while (reader.Read())
				{
					if (reader.IsStartElement(elementName))
					{
						return reader.ReadString();
					}
				}
			}
			return "";
		}
	}

	// extension to get the index of an element within an array of type T
	// this is here because one of the utilities above depends on this
	// and I did not want to depend on CsExtensions
	public static class Extensions
	{
		internal static int IndexOf<T>(this T[] array, T find)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Equals(find)) return i;
			}

			return -1;
		}
	}
}


