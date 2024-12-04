using System;
using System.Collections.Generic;
using System.Text;


namespace UtilityLibrary
{
	/// <summary>
	/// StringDivide - divide a string into sub-strings of maxLength size and a maximum of maxLines<br/>
	/// JustifyString - justify (L/R/C) within a provided column width<br/>
	/// EllipsisifyString  - ellipsisify (L/R/C) within a provided column width<br/>
	/// </summary>
	internal static class CsStringUtil
	{
		public enum JustifyHoriz
		{
			UNSPECIFIED,
			LEFT,
			CENTER,
			RIGHT
		}

		public enum JustifyVertical
		{
			UNSPECIFIED,
			TOP,
			MIDDLE,
			BOTTOM
		}

		// does s1 approximately match s2?
		// approximate match - all characters are the same and
		// in the same order with maxDiffs differences
		// if spacesCount, then differences based on spaces count
		// towards maxDifs
		// this requires that the first character in each string match
		// s1 is the whole value and s2 is the partial therefore
		// s2 must be the same size or smaller than s1
		public static bool ApproximateMatch(string s1, string s2, int maxDifs, bool ignoreSpaces)
		{
			if (!s1[0].Equals(s2[0])) return false;

			int difs = 0;
			int iS1 = 1;
			int iS2 = 1;

			int count = s1.Length >= s2.Length ? s1.Length : s2.Length;

			if (ignoreSpaces)
			{
				difs = (s1.Replace(" ", String.Empty)).Length - (s2.Replace(" ", String.Empty)).Length;
				if (difs < 0 || difs > maxDifs) return false;
			}
			else
			{
				difs = s1.Length - s2.Length;
				if (difs < 0 || difs > maxDifs) return false;
			}

			difs = 0;

			for (int i = 1; i < count; i++)
			{
				if (!s1[iS1].Equals(s2[iS2]))
				{
					// not totally match condition
					// first - space mis-match ok?
					if (ignoreSpaces)
					{
						if (s1[iS1].Equals(' '))
						{
							iS1++;
							continue;
						}

						if (s1[iS1].Equals(' '))
						{
							iS2++;
							continue;
						}
					}

					// not match
					// ignore the current s1[] and test the next s1
					// log the difference
					iS1++;
					difs++;

					if (difs > maxDifs) return false;
					if (iS1 == s1.Length) return false;

					continue;
				}

				iS1++;
				iS2++;

				if (iS1 == s1.Length && iS2 == s2.Length) return true;
				if (iS1 == s1.Length || iS2 == s2.Length) return false;
			}

			return true;
		}


		public static string RemoveBetween(string phase, string start, string end)
		{
			int pos1 = phase.IndexOf(start);
			int pos2 = phase.IndexOf(end);
			if (pos1 == -1 ||  pos2 == -1 || pos1 >= pos2) return phase;

			return phase.Remove(pos1, pos2 - pos1 + 1);
		}

		public static string RemoveWords(string phrase, string[] words)
		{
			string result = phrase;

			foreach (string word in words)
			{
				result = RemoveWord(result, word);
			}

			return result;
		}

		public static string RemoveWord(string phrase, string word)
		{
			int pos1 = phrase.IndexOf(word);

			if (pos1 == -1) return phrase;

			return phrase.Remove(pos1,  word.Length);
		}

		public static string MakeMultiLineString(List<string> lines, int maxLength, JustifyHoriz j = JustifyHoriz.UNSPECIFIED)
		{
			StringBuilder sb = new StringBuilder(2);

			if (lines.Count > 1)
			{
				for (var i = 0; i < lines.Count - 1; i++)
				{
					sb.Append(JustifyString(lines[i], j, maxLength));
					sb.Append("\n");
				}
			}

			sb.Append(JustifyString(lines[lines.Count - 1], j, maxLength));

			return sb.ToString();
		}

		// divide a string into sub-strings of maxLength size and a maximum
		// of maxLines.  Last line has the overflow if any.
		// maxLines == 0 for no maximum
		// maxLength > 0 means split on Word boundaries
		// < 0 means split on character boundaries (exact maxLength)
		// when maxLength > 0 the returned line may exceed maxLength
		// example result = StringDivide(s, new [] { ' ' }, titleWidth, maxLines);
		public static List<string> StringDivide(string text,
			char[] splitanyOf,
			int maxLength,
			int maxLines)
		{
			text = text ?? "";

			bool splitMidWord = false;

			if ( maxLength < 0)
			{
				splitMidWord = true;
				maxLength *= -1;
			}

			List<string> result = new List<string>();

			string final;

			// result.Add("");

			int index = 0;
			int loop = 0;

			while (text.Length > 0)
			{
				int splitIdx;

				if (maxLength + 1 <= text.Length)
				{
					splitIdx = text.Substring(0, maxLength - 1).LastIndexOfAny(splitanyOf) + 1;

					if (!splitMidWord)
					{
						if ((splitIdx == 0 || splitIdx == -1))
						{
							splitIdx = text.IndexOfAny(splitanyOf, maxLength);
						}
					}
				}
				else
				{
					splitIdx = text.Length - index;
				}

				if (splitIdx == -1 || splitIdx == 0)
				{
					splitIdx = maxLength;
				}

				if (loop + 1 == maxLines)
				{
					final = text;
					splitIdx = text.Length;
				}
				else
				{
					final = text.Substring(0, splitIdx);
				}

				result.Add(final);

				if (text.Length > splitIdx)
				{
					text = text.Substring(splitIdx);
				}
				else
				{
					text = string.Empty;
				}

				loop++;
			}

			return result;
		}

		// justify the string within the provided colWidth
		public static string JustifyString(string s, JustifyHoriz j, int maxLength)
		{
			if (maxLength == 0 || j == JustifyHoriz.UNSPECIFIED) return s;

			string msg = s.IsVoid() ? "" : s;

			switch (j)
			{
			case JustifyHoriz.RIGHT:
				{
					msg = msg.PadLeft(maxLength);
					break;
				}
			case JustifyHoriz.CENTER:
				{
					msg = msg.PadCenter(maxLength);
					break;
				}
			default:
				{
					msg = msg.PadRight(maxLength);
					break;
				}
			}

			return msg;
		}

		// ellipsisify - do not trim
		public static string EllipsisifyString(string s, JustifyHoriz j, int maxLength)
		{
			string msg = s ?? "";
			int beg = 0;
			int end = 0;
			int len = msg.Length;

			if (maxLength >= len || maxLength < 2) return s;

			//                     L    C    R
			string[] e = new [] { " …", " … ", "… " };

			switch (j)
			{
			case JustifyHoriz.RIGHT:
				{
					// beg = 0;
					// ellipsis in left
					end = len - (maxLength - e[2].Length);
					msg = e[2] + s.Substring(end);
					break;
				}
			case JustifyHoriz.LEFT:
				{
					// ellipsis on right
					beg = maxLength - e[0].Length;
					msg = s.Substring(0, beg) + e[0];
					break;
				}
			default:
				{
					// ellipsis in center
					beg = (int) (((maxLength - e[1].Length) / 2) + 0.5);
					end = len - (maxLength - (beg + e[1].Length));

					msg = s.Substring(0, beg) + e[1] + s.Substring(end);
					break;
				}
			}


			return msg;
		}

		/// <summary>
		/// provide a word depending on count (max array length).
		/// return the word form depending on the count number.
		/// use to pluralize a word for example
		/// this is 1 based (1 = the first word form)
		/// </summary>
		/// <param name="s">string array with the various word options</param>
		/// <param name="index">the index for the word form to return (max array length - 1)</param>
		/// <returns></returns>
		public static string ChoseWord(string[] s, int index)
		{
			return s[(index > s.Length ? s.Length - 1 : index - 1)];
		}
	}
}