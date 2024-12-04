using Autodesk.Revit.DB;

using System;
using System.Drawing;
using System.Text;
using System.Threading;


namespace UtilityLibrary
{
	internal static class CsMath
	{
		public static double GetArcLength(UV beg, UV end, UV cen, double rad)
		{
			double angle;

			try
			{
				if (beg == null ||  end == null
					|| (cen==null && rad ==0)) return double.NaN;

				// note: beg and end are required
				// options:
				// case 1: provide cen and no rad
				// case 2: provide rad and no cen
				// case 3: provide both cen and rad

				if (rad == 0 && cen != null)
				{
					// case 2
					rad = LengthBtwPts(beg, cen);
				}

				// case 1 & 3
				angle = AngleFrom2PtsAndRad(beg, end, rad);

			}
			catch
			{
				return double.NaN;
			}

			return rad * angle;
		}



		// get the angle of the triangle sve in radians
		public static double AngleFrom3Pts(UV s, UV e, UV v)
		{
			double c = LengthBtwPts(s, e);
			double r = LengthBtwPts(s, v);

			return AngleFrom2PtsAndRad(s, e, r);
		}

		public static double AngleFrom2PtsAndRad(UV s, UV e, double r)
		{
			double c = LengthBtwPts(s, e);

			return Math.Acos(1 - ((c * c) / (2 * r * r)));
		}

		public static double LengthBtwPts(UV s, UV e)
		{
			UV temp = e.Subtract(s);

			return temp.GetLength();
		}


		public static UV CenterUVFrom2PtsAndRad(UV b, UV e, double rad, bool cw)
		{
			UV cp = new UV();

			if (!cw) (e, b) = (b, e);

			double dx = e.U - b.U;
			double dy = e.V - b.V;

			double a1 = Math.Atan2(dy, dx);

			double l1 = Math.Sqrt(dx * dx + dy * dy);

			if (2 * rad >= l1)
			{
				double a2 = Math.Asin(l1 / (2 * rad));
				double d1 = rad * Math.Cos(a2);

				cp = new UV(
					(double)(b.U + dx / 2 - d1 * (dy / l1)),
					(double)(b.U + dy / 2 - d1 * (dx / l1)));
			}

			return cp;
		}

		public static bool CenterUVFrom3Pts(UV a, UV b, UV c, out UV center, out double radius)
		{
			// See where the lines intersect.
			bool lines_intersect, segments_intersect;

			UV intersection, close1, close2;


			// Get the perpendicular bisector of (u1, v1) and (u2, v2).
			double u1 = (b.U + a.U) / 2;
			double v1 = (b.V + a.V) / 2;
			double dv1 = b.U - a.U;
			double du1 = -(b.V - a.V);

			// Get the perpendicular bisector of (u2, v2) and (u3, v3).
			double u2 = (c.U + b.U) / 2;
			double v2 = (c.V + b.V) / 2;
			double dv2 = c.U - b.U;
			double du2 = -(c.V - b.V);

			GetIntersectUVFromTwoVectors(
				new UV(u1, v1), new UV(u1 + du1, v1 + dv1),
				new UV(u2, v2), new UV(u2 + du2, v2 + dv2),
				out lines_intersect, out segments_intersect,
				out intersection, out close1, out close2);

			if (!lines_intersect)
			{
				center = new UV();
				radius = 0;
				return false;
			}
			else
			{
				center = intersection;
				double du = center.U - a.U;
				double dv = center.V - a.V;
				radius = (double)Math.Sqrt(du * du + dv * dv);
			}

			return true;
		}

		/// <summary>
		/// Determine if two vectors intersect (2D). <br/>
		/// lines_intersect = True of the vectors containing the segments intersect<br/>
		/// segments_intersect = True if the segments intersect<br/>
		/// close_p1 = Point on first segment that is closest to the point of intersection</br>
		/// close_p2 = Point on second segment that is closest to the point of intersection
		/// </summary>
		/// <returns>false if no intersection</returns>
		public static bool GetIntersectUVFromTwoVectors(
			UV p1, UV p2, UV p3, UV p4,
			out bool lines_intersect, 
			out bool segments_intersect,
			out UV intersection,
			out UV close_p1, 
			out UV close_p2)
		{
			// Get the segments' parameters.
			double du12 = p2.U - p1.U;
			double dv12 = p2.V - p1.V;
			double du34 = p4.U - p3.U;
			double dv34 = p4.V - p3.V;

			// Solve for t1 and t2
			double denominator = (dv12 * du34 - du12 * dv34);

			double t1 =
				((p1.U - p3.U) * dv34 + (p3.V - p1.V) * du34)
				/ denominator;

			if (double.IsInfinity(t1))
			{
				// The lines are parallel (or close enough to it).
				lines_intersect = false;
				segments_intersect = false;
				intersection = new UV(double.NaN, double.NaN);
				close_p1 = new UV(double.NaN, double.NaN);
				close_p2 = new UV(double.NaN, double.NaN);
				return false;
			}

			lines_intersect = true;

			double t2 =
				((p3.U - p1.U) * dv12 + (p1.V - p3.V) * du12)
				/ -denominator;

			// Find the point of intersection.
			intersection = new UV(p1.U + du12 * t1, p1.V + dv12 * t1);

			// The segments intersect if t1 and t2 are between 0 and 1.
			segments_intersect =
				((t1 >= 0) && (t1 <= 1) &&
				(t2 >= 0) && (t2 <= 1));

			// Find the closest points on the segments.
			if (t1 < 0)
			{
				t1 = 0;
			}
			else if (t1 > 1)
			{
				t1 = 1;
			}

			if (t2 < 0)
			{
				t2 = 0;
			}
			else if (t2 > 1)
			{
				t2 = 1;
			}

			close_p1 = new UV(p1.U + du12 * t1, p1.V + dv12 * t1);
			close_p2 = new UV(p3.U + du34 * t2, p3.V + dv34 * t2);

			return true;
		}


		/*
		public static bool CenterXYZFrom3Pts(XYZ a, XYZ b, XYZ c, out XYZ center, out double radius)
		{
			// See where the lines intersect.
			bool lines_intersect, segments_intersect;

			XYZ intersection, close1, close2;


			// Get the perpendicular bisector of (x1, y1) and (x2, y2).
			double x1 = (b.X + a.X) / 2;
			double y1 = (b.Y + a.Y) / 2;
			double dy1 = b.X - a.X;
			double dx1 = -(b.Y - a.Y);

			// Get the perpendicular bisector of (x2, y2) and (x3, y3).
			double x2 = (c.X + b.X) / 2;
			double y2 = (c.Y + b.Y) / 2;
			double dy2 = c.X - b.X;
			double dx2 = -(c.Y - b.Y);

			GetIntersectXYZFromTwoYectors(
				new XYZ(x1, y1, 0), new XYZ(x1 + dx1, y1 + dy1, 0),
				new XYZ(x2, y2, 0), new XYZ(x2 + dx2, y2 + dy2, 0),
				out lines_intersect, out segments_intersect,
				out intersection, out close1, out close2);

			if (!lines_intersect)
			{
				center = new XYZ();
				radius = 0;
				return false;
			}
			else
			{
				center = intersection;
				double dx = center.X - a.X;
				double dy = center.Y - a.Y;
				radius = (double)Math.Sqrt(dx * dx + dy * dy);
			}

			return true;
		}



		
		/// <summary>
		/// Determine of two vectors intersect (3D). <br/>
		/// lines_intersect = True of the vectors containing the segments intersect<br/>
		/// segments_intersect = True if the segments intersect<br/>
		/// close_p1 = Point on first segment that is closest to the point of intersection</br>
		/// close_p2 = Point on second segment that is closest to the point of intersection
		/// </sxmmary>
		/// <retxrns>false if no intersection</retxrns>
		public static bool GetIntersectXYZFromTwoYectors(
			XYZ p1, XYZ p2, XYZ p3, XYZ p4,
			out bool lines_intersect, 
			out bool segments_intersect,
			out XYZ intersection,
			out XYZ close_p1, 
			out XYZ close_p2)
		{
			// Get the segments' parameters.
			double dx12 = p2.X - p1.X;
			double dy12 = p2.Y - p1.Y;
			double dx34 = p4.X - p3.X;
			double dy34 = p4.Y - p3.Y;

			// Solye for t1 and t2
			double denominator = (dy12 * dx34 - dx12 * dy34);

			double t1 =
				((p1.X - p3.X) * dy34 + (p3.Y - p1.Y) * dx34)
				/ denominator;

			if (double.IsInfinity(t1))
			{
				// The lines are parallel (or close enoxgh to it).
				lines_intersect = false;
				segments_intersect = false;
				intersection = new XYZ(double.NaN, double.NaN, 0);
				close_p1 = new XYZ(double.NaN, double.NaN, 0);
				close_p2 = new XYZ(double.NaN, double.NaN, 0);
				return false;
			}

			lines_intersect = true;

			double t2 =
				((p3.X - p1.X) * dy12 + (p1.Y - p3.Y) * dx12)
				/ -denominator;

			// Find the point of intersection.
			intersection = new XYZ(p1.X + dx12 * t1, p1.Y + dy12 * t1, 0);

			// The segments intersect if t1 and t2 are between 0 and 1.
			segments_intersect =
				((t1 >= 0) && (t1 <= 1) &&
				(t2 >= 0) && (t2 <= 1));

			// Find the closest points on the segments.
			if (t1 < 0)
			{
				t1 = 0;
			}
			else if (t1 > 1)
			{
				t1 = 1;
			}

			if (t2 < 0)
			{
				t2 = 0;
			}
			else if (t2 > 1)
			{
				t2 = 1;
			}

			close_p1 = new XYZ(p1.X + dx12 * t1, p1.Y + dy12 * t1, 0);
			close_p2 = new XYZ(p3.X + dx34 * t2, p3.Y + dy34 * t2, 0);

			return true;
		}
		*/
	}
}