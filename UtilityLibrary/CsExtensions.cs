using System;
using System.Drawing;
using System.Text;


namespace UtilityLibrary
{
	internal static class CsExtensions
	{
	#region > string

		internal static string IfNullOrWhiteSpace(this string s, string alternate)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				return alternate;
			}

			return s;
		}

		internal static string Repeat(this string s, int quantity)
		{
			if (quantity <= 0) return "";

			StringBuilder sb = new StringBuilder();

			for (int i = 0; i < quantity; i++)
			{
				sb.Append(s);
			}

			return sb.ToString();
		}

		internal static int CountSubstring(this string s, string substring)
		{
			int count = 0;
			int i = 0;
			while ((i = s.IndexOf(substring, i, StringComparison.Ordinal)) != -1)
			{
				i += substring.Length;
				count++;
			}

			return count;
		}

		internal static int IndexOfToOccurance(this string s, string substring,
			int occurance, int start = 0
			)
		{
			if (s.Trim().Length == 0) { return -1; }

			if (occurance < 0) { return -1; }

			int pos = start;

			for (int count = 0; count < occurance; count++)
			{
				pos = s.IndexOf(substring, pos, StringComparison.Ordinal);

				if (pos == -1) { return pos; }

				pos += substring.Length;
			}

			return pos - substring.Length;
		}

		internal static string GetSubDirectory(this string path, int requestedDepth)
		{
			requestedDepth++;
			if (requestedDepth == 0) { return "\\"; }

			path = path.TrimEnd('\\');

			int depth = CountSubstring(path, "\\");

			if (requestedDepth > depth) { requestedDepth = depth; }

			int pos = IndexOfToOccurance(path, "\\", requestedDepth);

			if (pos < 0) { return ""; }

			pos = IndexOfToOccurance(path, "\\", requestedDepth + 1);

			if (pos < 0) { pos = path.Length; }

			return path.Substring(0, pos);
		}

		public static string GetSubDirectoryName(this string path, int requestedDepth)
		{
			requestedDepth++;

			path = path.TrimEnd('\\');

			if (requestedDepth > CountSubstring(path, "\\")) { return ""; }

			string result = GetSubDirectory(path, requestedDepth - 1);

			if (result.Length == 0) { return ""; }

			int pos = IndexOfToOccurance(path, "\\", requestedDepth) + 1;

			return result.Substring(pos);
		}

		public static string PadCenter(this string str, int totalLength, char padChar = '\u00A0')
		{
			int padAmount = totalLength - str.Length;

			if (padAmount <= 1)
			{
				if (padAmount == 1)
				{
					return str.PadRight(totalLength);
				}

				return str;
			}

			int padLeft = padAmount / 2 + str.Length;

			return str.PadLeft(padLeft).PadRight(totalLength);
		}

	#endregion

	#region > rectangle

		public static Rectangle Scale(this Rectangle rc, double scaleFactor)
		{
			return new Rectangle(
				(int) (rc.Left * scaleFactor),
				(int) (rc.Top * scaleFactor),
				(int) (rc.Width * scaleFactor),
				(int) (rc.Height * scaleFactor));
		}

		public static string ToString(this Rectangle rc)
		{
			return string.Format("top|{0,5:D} left|{1,5:D} height|{2,5:D} width|{3,5:D}",
				rc.Top, rc.Left, rc.Height, rc.Width);
		}

	#endregion

	#region > emum extension

		public static dynamic Value(this Enum e)
		{
			switch (e.GetTypeCode())
			{
			case TypeCode.Byte:
				{
					return (byte) (IConvertible) e;
				}
			case TypeCode.Int16:
				{
					return (short) (IConvertible) e;
				}
			case TypeCode.Int32:
				{
					return (int) (IConvertible) e;
				}
			case TypeCode.Int64:
				{
					return (long) (IConvertible) e;
				}
			case TypeCode.UInt16:
				{
					return (ushort) (IConvertible) e;
				}
			case TypeCode.UInt32:
				{
					return (uint) (IConvertible) e;
				}
			case TypeCode.UInt64:
				{
					return (ulong) (IConvertible) e;
				}
			case TypeCode.SByte:
				{
					return (sbyte) (IConvertible) e;
				}
			}

			return 0;
		}

		public static string Name(this Enum e)
		{
			return e.ToString();
		}

	#endregion

	}
}