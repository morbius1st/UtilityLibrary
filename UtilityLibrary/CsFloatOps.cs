using System;
using System.Diagnostics;


namespace UtilityLibrary
{
	public class FloatOps
	{
		private const double TOL = 1e-7;
		// private static string t = TOL.ToString("F12");

		public static float pi180 = (float) Math.PI;
		public static float pi90 = pi180 / 2;
		public static float pi270 = pi90 * 3;

		// convert 90 / 180 / 270 / 360 to radians
		public static float NewsToRad(float news)
		{
			return news == 360 ? 0 : news / 90 * pi90;
		}

		public static float ToRad(float angle)
		{
			return (float) ( angle / 180.0f * Math.PI);
		}

		public static float ToDec(float angle)
		{
			return (float) (angle / Math.PI * 180);
		}

		public static bool GreaterThan(float a, float b)
		{
			// float f = a - b;
			// string s = f.ToString("F12");
			//
			// show("GT", s);

			return (a - b) > TOL;
		}

		public static bool LessThan(float a, float b)
		{
			// float f = (b - a);
			// string s = f.ToString("F12");
			//
			// show("LT", s);

			return (b - a) > TOL;
		}

		public static bool Equals(float a, float b)
		{
			// float f = Math.Abs(a - b);
			//
			// string s = f.ToString("F12");
			//
			// show("EQ", s);

			return Math.Abs(a - b) <= TOL;
		}

		public static bool GreaterThanOrEqual(float a, float b)
		{
			return GreaterThan(a, b) || Equals(a, b);
		}

		public static bool LessThanOrEqual(float a, float b)
		{
			return LessThan(a, b) || Equals(a, b);
		}

		public static bool EqualWithInTolerance(float a, float b, float tol, bool inclusive = true)
		{
			if (inclusive) return Math.Abs(a - b) <= tol;

			return Math.Abs(a - b) < tol;
		}

		// private static void show(string which, string s) 
		// {
		// 	Debug.WriteLine($"\n{which,-6}| tol {t}");
		// 	Debug.WriteLine($"{which,-6}| val {s}");
		// }


		public static void TestFloatMath()
		{
			float a = 1.0f - 0.9f;
			float b = 0.1f;

			testFloatMath(a, b);

			a = 0.1f;
			b = 0.2f;

			testFloatMath(a, b);

			a = 0.2f;
			b = 0.1f;

			testFloatMath(a, b);

		}

		private static void testFloatMath(float a, float b)
		{
			bool result1;
			bool result2;

			Debug.WriteLine($"\n\na {a} | b {b}");

			result1 = FloatOps.Equals(a, b);
			result2 = a.Equals(b); 

			Debug.WriteLine($"a.eq(b)| lib| {result1,6} | c#| {result2}");

			result2 = a == b; 
			
			Debug.WriteLine($"a == b | lib| {result1,6} | c#| {result2}");

			
			result1 = FloatOps.GreaterThan(a, b);
			result2 = a > b;
			
			Debug.WriteLine($"a > b  | lib| {result1,6} | c#| {result2}");
			

			result1 = FloatOps.GreaterThanOrEqual(a, b);
			result2 = a >= b;

			Debug.WriteLine($"a >= b | lib| {result1,6} | c#| {result2}");

			
			result1 = FloatOps.LessThan(a, b);
			result2 = a < b;

			Debug.WriteLine($"a < b  | lib| {result1,6} | c#| {result2}");
			

			result1 = FloatOps.LessThanOrEqual(a, b);
			result2 = a <= b;

			Debug.WriteLine($"a <= b | lib| {result1,6} | c#| {result2}");
		}




	}

}
