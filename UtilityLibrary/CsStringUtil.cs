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

			sb.Append(JustifyString(lines[lines.Count-1], j, maxLength));

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
			if (maxLength == 0 || j== JustifyHoriz.UNSPECIFIED) return s;

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
			string[] e = new [] { "…", "…", "…" };

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


	}
}