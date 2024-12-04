using System;
using System.Collections.Generic;
using System.IO;
// using System.Drawing;
// using System.Drawing.Text;
using Microsoft.Win32;
// using System.DirectoryServices.ActiveDirectory;
// using System.Windows.Media;
// using FontFamily = System.Windows.Media.FontFamily;


namespace UtilityLibrary
{
	public static class CsWindowHelpers
	{
		public static bool FontListReady { get; private set; } = false;

		public static SortedList<string, string> RegisteredFontList { get; set; }

		public static string FontsFolderPath { get; private set; } = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);

		static CsWindowHelpers()
		{
			InitFontLists();
		}

		/*	public static void TestApxMatch()
		{
			bool result;
			bool match;

			string results;
			string matchs;

			// sample / test / max / spaces? / result
			List<Tuple<string, string, int, bool, bool >> samples = new List<Tuple<string, string, int, bool, bool >>()
			{
				new ("Swiss 721 BT", "Swiss 721 BT", 1, true, true),
				new ("Swiss 721 BT", "Swis721BT"   , 1, true, true),
				new ("Swiss 721 BT", "Swis72BT"    , 2, true, true),

				new ("Swiss 721 BT", "Swis721BT"   , 1, false, false),
				new ("Swiss 721 BT", "Swis 721BT"  , 1, false, false),
				new ("Swiss 721 BT", "Swiss 721BT" , 1, false, true),
				new ("Swiss 721 BT", "Swis 721 BT" , 1, false, true),
				new ("Swiss 721 BT", "Swis 721BT"  , 2, false, true),
				new ("Swiss 721 BT", "Xwiss 721BT" , 2, false, false),
				new ("Swiss 721 BT", "Swis72BT"    , 2, false, false),
			};

			foreach (Tuple<string, string, int, bool, bool> s in samples)
			{
				result =CsStringUtil.ApproximateMatch(s.Item1, s.Item2, s.Item3, s.Item4);
				match = result == s.Item5;

				results = result ? "did match" : "not match";
				matchs = match ? "correct" : "wrong  ";

				Debug.WriteLine($"{results} : {matchs} | {s.Item1,-15} vs {s.Item2,-15}");
			}
		}
		*/

		/*  public static void TestGetFontFile()
		{
			string result;
			string filePath;
			bool match;

			Dictionary<string, string> testNamesx = new Dictionary<string, string>()
			{
				{ "adf", "asdf" }, { "asddf", "asdf" }
			};

			Dictionary<string, string> testNames = new Dictionary<string, string>()
			{
				{ "Adobe Arabic", "AdobeArabic-Regular.otf" },
				{ "Adobe Gothic Std", "AdobeGothicStd-Bold.otf" },
				{ "Adobe Ming Std", "AdobeMingStd-Light.otf" },
				{ "Adobe Pi Std", "AdobePiStd.otf" },
				{ "Arial", "arial.ttf" },
				{ "Arial Narrow", "ARIALN.TTF" },
				{ "Arial Rounded MT", "ARLRDBD.TTF" },
				{ "Comic Sans MS", "comic.ttf" },
				{ "Courier Std", "CourierStd.otf" },
				{ "DecoType Naskh Swashes", "DTNASKH4.TTF" },
				{ "HYGothic", "H2GTRE.TTF" },
				{ "Swiss 721 BT", "swiss.ttf" },
				{ "Swis721 BT", "swiss.ttf" },
			};

			foreach (KeyValuePair<string, string> kvp in testNames)
			{
				result = GetFontFile(kvp.Key);
				filePath = GetFontFilePath(kvp.Key);
				match = result?.Equals(kvp.Value) ?? false;

				Debug.Write($"{match,-6} | {kvp.Key,-28} = {$">{(result ?? "is null")}<",-30} " );
				// Debug.Write($"vs >{kvp.Value}<" );
				Debug.Write($"{filePath}");
				Debug.Write("\n");
			}
		}
*/

		public static void InitFontLists()
		{
			GetRegisteredFonts();

			// GetFontFamiliesFromFolder();
			// populate font lists
			// GetFontNames();

			FontListReady = true;
		}

		public static string GetFontFilePath(string fontName)
		{
			string result = GetFontFile(fontName);

			if (result == null) return null;

			return $"{FontsFolderPath}\\{result}";
		}

		public static string GetFontFile(string fontName)
		{
			int pos1;

			string testFontName = fontName.ToUpper();
			string altFontName  = testFontName.Replace(" ", String.Empty);

			string testFontName2 = testFontName.Substring(0, 4);
			string regFontName;
			string firstFontFile = null;
			string firstAltFontName = null;

			// FontFamily ff;
			// Typeface t;


			bool repeat = false;
			// first - simple search
			foreach (KeyValuePair<string, string> kvp in RegisteredFontList)
			{
				regFontName = kvp.Key.ToUpper();

				if (repeat)
				{
					// processing same font set?
					if (regFontName.StartsWith(firstAltFontName))
					{
						pos1 = kvp.Key.IndexOf('-');

						if (kvp.Key.Substring(pos1 + 1).ToUpper().Equals("REGULAR")) return kvp.Value;
					}
					else
					{
						return firstFontFile;
					}

					continue;
				}

				if (regFontName.StartsWith(testFontName)) return kvp.Value;

				// matching altName
				if (regFontName.StartsWith(altFontName))
				{
					pos1 = kvp.Key.IndexOf('-');

					// case 1 - no dash - only one variant - done
					if (pos1 == -1) return kvp.Value;

					// case 2 - this variant is the "regular" version - done
					if (kvp.Key.Substring(pos1 + 1).ToUpper().Equals("REGULAR")) return kvp.Value;

					// case 3 - multiple variants - search for "regular" 
					// or use first if not found
					repeat = true;
					firstAltFontName = altFontName;
					firstFontFile = kvp.Value;

					continue;
				}

				// worst case scenario 
				// check for an approximate match - that is, a match with a max of
				// one missing character (except for spaces
				if (CsStringUtil.ApproximateMatch(regFontName, testFontName, 1, true)) return kvp.Value;

			}

			return null;
		}

		// get the list of registered font names from the registry
		// has the font's file name but the name 
		// includes qualifiers
		// ignores old bitmap fonts (i.e. ".FON" files
		public static void GetRegisteredFonts()
		{
			RegistryKey fonts = null;
			string[] altNames;
			string fontName;
			string fileName;

			try
			{
				fonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts", false);
				if (fonts == null)
				{
					fonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Fonts", false);
					if (fonts == null)
					{
						return;
					}
				}

				RegisteredFontList = new SortedList<string, string>();

				foreach (string fName in fonts.GetValueNames())
				{
					fontName = CsStringUtil.RemoveBetween(fName, "(", ")").TrimEnd();

					fileName = fonts.GetValue(fName) as string;

					if (Path.GetExtension(fileName).ToUpper().Equals(".FON")) continue;

					altNames = fontName.Split(new [] { " & " }, StringSplitOptions.None);

					foreach (string altName in altNames)
					{
						RegisteredFontList.Add(altName, fileName);
					}
				}
			}
			finally
			{
				if (fonts != null)
				{
					fonts.Dispose();
				}
			}
		}


		/* moved to CsStringUtil
		// does s1 approximately match s2?
		// approximate match - all characters are the same and
		// in the same order with maxDiffs differences
		// if spacesCount, then differences based on spaces count
		// towards maxDifs
		// this requires that the first character in each string match
		// s1 is the whole value and s2 is the partial therefore
		// s2 must be the same size or smaller than s1
		private static bool ApproximateMatch2(string s1, string s2, int maxDifs, bool ignoreSpaces)
		{
			if (!s1[0].Equals(s2[0])) return false;

			int difs = 0;
			int iS1 = 1;
			int iS2 = 1;

			bool repeat;

			int count = s1.Length >= s2.Length ? s1.Length : s2.Length;

			Debug.WriteLine($"\nmatching {s1} vs {s2} | count {count} | ignore spaces? {ignoreSpaces}");

			if (ignoreSpaces)
			{
				difs = (s1.Replace(" ", String.Empty)).Length - (s2.Replace(" ", String.Empty)).Length;
				if (difs < 0 || difs > maxDifs)
				{
					Debug.WriteLine($"\tfail A | total difs {difs} vs ({maxDifs})");
					return false;
				}
			}
			else
			{
				difs = s1.Length - s2.Length;
				if (difs < 0 || difs > maxDifs)
				{
					Debug.WriteLine($"\tfail B | total difs {difs} vs ({maxDifs})");
					return false;
				}
			}

			difs = 0;

			for (int i = 1; i < count; i++)
			{
				Debug.Write($"{i,-3}| >{s1[iS1]}< vs >{s2[iS2]}< | {iS1,3:##} & {iS2,-3:##}  ");

				if (!s1[iS1].Equals(s2[iS2]))
				{
					Debug.WriteLine($" NOT MATCH");

					// not totally match condition
					// first - space mis-match ok?
					if (ignoreSpaces)
					{
						Debug.Write("\tignoring spaces -");
						if (s1[iS1].Equals(' '))
						{
							Debug.WriteLine(" s1 is space - skipped");
							iS1++;
							continue;
						}

						if (s1[iS1].Equals(' '))
						{
							Debug.WriteLine(" s2 is space - skipped");
							iS2++;
							continue;
						}

						Debug.WriteLine(" neither is space - continue");
					}


					// not match
					// ignore the current s1[] and test the next s1
					// log the difference

					iS1++;
					difs++;

					Debug.WriteLine($"\tdif count {difs} (vs {maxDifs})");

					if (difs > maxDifs) return false;
					if (iS1 == s1.Length) return false;

					continue;
				}
				else
				{
					Debug.WriteLine(" MATCHED");
				}

				iS1++;
				iS2++;


				Debug.WriteLine($"\tdif count {difs} (vs {maxDifs})");

				if (iS1 == s1.Length && iS2 == s2.Length) return true;

				if (iS1 == s1.Length || iS2 == s2.Length) return false;
			}


			return true;
		}
*/

		// gets the complete list of font names
		// but a reduced set
		// does not include the file name
		// provide "human" readable names
		/*public static void GetFontNames2()
		{
			FontList = new SortedDictionary<string, string>();
			// FontListStripped = new SortedDictionary<string, string>();

			ICollection<FontFamily> ffs = GetAllFontFamilies();

			int count = 0;

			foreach (FontFamily ff in ffs)
			{
				string a = ff.ToString();
				FontList.Add(a, "");

				string b = a.Replace(" ", String.Empty);

				// if (b.StartsWith("Swis"))
				// {
				// 	Typeface t = new Typeface(ff.ToString());
				//
				// 	int x = 1;
				// }
			}
		}*/




		/*public static ICollection<FontFamily> GetAllFontFamilies2()
		{
			return Fonts.SystemFontFamilies;
		}

		
		public static string GetSystemFontFileName(Font font)
		{
			RegistryKey fonts = null;

			try
			{
				fonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Fonts", false);
				if (fonts == null)
				{
					fonts = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Fonts", false);
					if (fonts == null)
					{
						throw new Exception("Can't find font registry database.");
					}
				}

				string suffix = "";
				if (font.Bold)
					suffix += "(?: Bold)?";
				if (font.Italic)
					suffix += "(?: Italic)?";

				var regex = new Regex(@"^(?:.+ & )?" + Regex.Escape(font.Name) + @"(?: & .+)?(?<suffix>" + suffix + @") \(TrueType\)$");

				string[] names = fonts.GetValueNames();

				string name = names.Select(n => regex.Match(n)).Where(m => m.Success).OrderByDescending(m => m.Groups["suffix"].Length).Select(m => m.Value).FirstOrDefault();

				if (name != null)
				{
					return fonts.GetValue(name).ToString();
				}
				else
				{
					return null;
				}
			}
			finally
			{
				if (fonts != null)
				{
					fonts.Dispose();
				}
			}
		}*/


		// public static SortedDictionary<string, string> FontList { get; private set; }

		// public static SortedList<string, string> FontFileList { get; private set; }

		// public static SortedDictionary<string, string> FontListStripped { get; private set; }

		// public static SortedList<string,string> registeredFonts;

		// public static void GetFontFamiliesFromFolder()
		// {
		// 	string[] fontInfo;
		//
		// 	FontFileList = new SortedList<string, string>();
		//
		// 	ICollection<FontFamily> fontFamilies = Fonts.GetFontFamilies(FontsFolderPath);
		//
		// 	foreach (FontFamily ff in fontFamilies)
		// 	{
		// 		fontInfo = ff.Source.Split("#");
		//
		// 		FontFileList.Add(fontInfo[1], $"{fontInfo[0]}\\{fontInfo[1]}");
		// 	}
		//
		// 	foreach (KeyValuePair<string, string> kvp in FontFileList)
		// 	{
		// 		Debug.WriteLine($"{kvp.Key,-40}{kvp.Value}");
		// 	}
		//
		//
		// }

	}
}