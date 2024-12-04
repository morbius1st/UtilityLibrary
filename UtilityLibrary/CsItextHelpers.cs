// user name: jeffs
// created:   5/12/2024 6:52:04 AM


using iText.Kernel.Geom;
using System.Text;
using iText.Kernel.Colors;

namespace UtilityLibrary
{
	public class CsItextHelpers
	{


	}


	public class FormatItextData
	{

		public static string FormatDashArray(float[] dashes)
		{
			float cvtFactor = 1;
			int numWidth = 5;
			string numFmt = "F2";
			int descWidth = 2;

			return FormatFloatArray(
				dashes,
				numWidth, numFmt,
				new [] { "d", "s","d", "s","d", "s","d", "s","d", "s" },
				descWidth);
		}

		public static string FormatColor(Color c, bool showLables=true)
		{
			if (c == null) return "transparent";

			float cvtFactor = 255;
			int numWidth = showLables ? 3:1;
			string numFmt = "F0";
			int descWidth = 1;
			string [] divider = new string[] { "", ", " };

			string[] colorTags = null;

			if (c.GetNumberOfComponents() == 3)
			{
				colorTags = new [] { "r", "g", "b" };
			}
			else if (c.GetNumberOfComponents() == 4)
			{
				colorTags = new [] { "c", "m", "y", "k" };
			}
			else
			{
				colorTags = new [] { "g" };
			}

			return FormatFloatArray(
				CsMath.ArrayMultiply(c.GetColorValue(), cvtFactor),
				numWidth, numFmt, colorTags, descWidth, divider, showLables);
		}

		public static string FormatRectangle(Rectangle r, bool showLables = true)
		{
			float cvtFactor = 1;
			int numWidth = showLables ? 7 : 1;
			string numFmt = "F2";
			int descWidth = 1;
			string [] divider = new string[] { "", ", " };

			return FormatFloatArray(
				new [] { r?.GetX() * cvtFactor ?? -1, 
					r?.GetY() * cvtFactor ?? -1, 
					r?.GetWidth() * cvtFactor  ?? -1, 
					r?.GetHeight() * cvtFactor  ?? -1, },
				numWidth, numFmt,
				new [] { "x", "y", "w", "h" },
				descWidth, divider, showLables);
		}

		public static string FormatRectangle(Rectangle r, int numWidth, string numFmt, int descWidth, float cvtFactor = 1.0f)
		{
			return FormatFloatArray(
				new [] { r?.GetX() * cvtFactor ?? -1, 
					r?.GetY() * cvtFactor ?? -1, 
					r?.GetWidth() * cvtFactor  ?? -1, 
					r?.GetHeight() * cvtFactor  ?? -1, },
				numWidth, numFmt,
				new [] { "x", "y", "w", "h" },
				descWidth);


			// return $"X| {r.GetX(),9:F2}| Y| {r.GetY(),9:F2}| W| {r.GetWidth(),9:F2}| H| {r.GetHeight(),9:F2}";
		}

		public static string FormatFloatArray(float[] array, 
			int quadWidth, string quadNumFmt, string[] desc, 
			int descWidth, string[] divider = null, bool showLables=true)
		{
			if (array == null || array.Length == 0) return "empty";

			if (divider == null) divider = new string[] { "| ", "| " };

			int i = 0;
			// int j = 0;

			StringBuilder sb = new StringBuilder();

			string fmt1 = $"{{0,{descWidth}}}{{2}} {{1,{quadWidth}:{quadNumFmt}}}{{3}}";
			string fmt2 = $"{{1,{quadWidth}:{quadNumFmt}}}{{3}}";

			string fmt;

			fmt = showLables ? fmt1 : fmt2;

			for (i = 0; i < array.Length-1; i++)
			{
				// j = j + 1 == desc.Length ? 0 : j++;

				sb.Append(string.Format(fmt, desc[i], array[i], divider[0], divider[1]));

			}

			sb.Append(string.Format(fmt, desc[i], array[i], divider[0], ""));

			return sb.ToString();
		}

		public static string FormatFontStyle(int style)
		{
			if (style == -1) return "UNDEFINED";
			if (style == 0) return "NORMAL";

			string result = null;

			if ((style & 1) == 1) result = "BOLD";
			if ((style & 2) == 2) result += "ITALIC";

			return result;
		}


	}
}