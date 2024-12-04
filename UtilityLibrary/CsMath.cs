


namespace UtilityLibrary
{
	public class CsMath
	{
		public static float[] ArrayMultiply(float[] a, float scalar)
		{
			float[] b = new float[a.Length];

			for (int i = 0; i < a.Length; i++)
			{
				b[i] = a[i] * scalar;
			}

			return b;
		}

	}
}