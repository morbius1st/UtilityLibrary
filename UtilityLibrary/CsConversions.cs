using System;

namespace UtilityLibrary
{
	public static class CsConversions
	{
		public static class FromDoubleFeet
		{
			public static string ToFeetAndDecimalInches( double feet,
				double accuracy,
				bool keepLeadingZero,
				bool keepTrailingZero
				)
			{
				if (feet.Equals(0.0)) return "0\"";

				string resultFt = null;
				string resultIn = null;

				double ft = (int)feet;
				double inches = Math.Abs((feet - ft) * 12);

				if (ft.Equals(0.0))
				{
					if (keepLeadingZero)
					{
						resultFt = "0\'";
					}
				}
				else
				{
					resultFt = ft.ToString("F0") + "\'";
				}

				resultIn = FromDoubleInches.ToDecimalInches(inches, accuracy, keepTrailingZero);

				if (string.IsNullOrWhiteSpace(resultIn))
				{
					return resultFt;
				}

				if (string.IsNullOrWhiteSpace(resultFt))
				{
					return resultIn;
				}

				return resultFt + "-" + resultIn;
			}
		}

	}

	public static class FromDoubleInches
	{
		public static string ToDecimalInches(double inches,
			double accuracy,
			bool keepTrailingZero)
		{
			string resultIn = "";
			string inchFmt = "";

			if (inches.Equals(0.0))
			{
				if (keepTrailingZero)
				{
					return "0\""; 
				}

				return null;
			}

			inchFmt = Formatting.FormatStringPerAccuracyAndZeros(accuracy, keepTrailingZero);

			resultIn = inches.ToString(inchFmt);

			if (resultIn.EndsWith("."))
			{
				resultIn = resultIn.Substring(0, resultIn.Length - 1);
			}

			return resultIn + "\"";
		}
	}

	public static class Formatting
	{

		internal static string FormatStringPerAccuracyAndZeros(double accuracy,
			bool keepTrailingZero)
		{
			string fmt = "";
			string fmtCode = "#";

			if (keepTrailingZero)
			{
				fmtCode = "0";
			}

			fmt = CsExtensions.Repeat(fmtCode, ((int)(1 / accuracy)).ToString().Length - 1);

			if (string.IsNullOrEmpty(fmt))
			{
				fmt = "0";
			}
			else
			{
				fmt = "0." + fmt;
			}

			return fmt;
		}
	}
}


