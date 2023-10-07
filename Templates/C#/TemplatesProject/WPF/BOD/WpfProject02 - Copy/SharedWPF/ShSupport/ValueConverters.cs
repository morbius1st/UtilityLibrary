#region using

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

#endregion

// username: jeffs
// created:  12/31/2021 6:23:40 AM

namespace SharedWPF.ShSupport
{
	[MarkupExtensionReturnType (typeof(GridLen))]
	public class GridLen : UIElement
	{
		// Register an attached dependency property with the specified
		// property name, property type, owner type, and property metadata.

		public static readonly DependencyProperty GridLenProperty =
			DependencyProperty.RegisterAttached(
				"GridLen",
				typeof(GridLength),
				typeof(GridLen),
				new FrameworkPropertyMetadata(defaultValue: new GridLength(0.0),
					flags: FrameworkPropertyMetadataOptions.AffectsMeasure)
				);

		// Declare a get accessor method.

		public static GridLength GetGridLen(UIElement target) => (GridLength)target.GetValue(GridLenProperty);

		// Declare a set accessor method.

		public static void SetGridLen(UIElement target, GridLength value) => target.SetValue(GridLenProperty, value);

		public static string Formula
		{
			set { }
		}
	}

#region double divider converter

	[ValueConversion(typeof(double), typeof(double))]
	public class DivideConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double valu = (double) (value ?? 0.0);

			bool result = double.TryParse((string) (parameter ?? "1.0"), out double divisor);

			if (!result || divisor == 0) return valu;

			return valu / divisor;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion

#region double multi add converter

	[ValueConversion(typeof(double), typeof(double))]
	public class MultiAddConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double sum = 0.0;

			foreach (Double value in values)
			{
				sum += value;
			}

			return sum;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { 0.0 };
		}
	}

#endregion

#region double add converter

	[ValueConversion(typeof(double), typeof(double))]
	public class AddConverter : IMultiValueConverter
	{
		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
		{
			double sum = 0.0;

			foreach (Double value in values)
			{
				sum += value;
			}

			return sum;
		}

		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
		{
			return new object[] { 0.0 };
		}
	}

#endregion

#region double subtract converter

	[ValueConversion(typeof(double), typeof(double))]
	public class SubtractConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			double valu = (double) (value ?? 0.0);
			double operand = 0.0;

			bool result = false;

			if (parameter is string)
			{
				result = double.TryParse((string) (parameter ?? "1.0"), out operand);
			}
			else
			{
				if (parameter is double)
				{
					operand = (double) parameter;
					result = true;
				}
			}

			if (!result) return valu;

			return (valu - operand) < 0 ? 0 : valu - operand;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion

#region pass-through converter

	// for debugging only
	// allows breakpoint here
	[ValueConversion(typeof(object), typeof(object))]
	public class PassThroughConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value;
		}
	}

#endregion

#region thickness add parameter converter

	[ValueConversion(typeof(Thickness), typeof(Thickness))]
	public class AddThicknessConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			Thickness valu = (Thickness) (value ?? default(Thickness));
			double operand = 0.0;

			bool result = false;

			if (parameter is string)
			{
				result = double.TryParse((string) (parameter ?? "1.0"), out operand);
			}
			else
			{
				if (parameter is double)
				{
					operand = (double) parameter;
					result = true;
				}
			}

			if (!result) return valu;

			return new Thickness(valu.Left + operand, valu.Top + operand, valu.Right + operand, valu.Bottom + operand) ;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion

#region invert bool value converter

	[ValueConversion(typeof(bool), typeof(bool))]
	public class InvertBool : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return !(bool) (value ?? false);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

#endregion

// #region bool to visibility value converter
//
// 	// yes = visible
// 	// no = hidden
// 	[ValueConversion(typeof(bool), typeof(Visibility))]
// 	public class BoolToVisibilityConverter : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			// if (targetType != typeof(object))
// 			// 	throw new InvalidOperationException("The target must be an object");
// 			return ((bool) (value ?? false)) ? Visibility.Visible : Visibility.Hidden;
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter,
// 			System.Globalization.CultureInfo culture)
// 		{
// 			throw new NotSupportedException();
// 		}
// 	}
//
// #endregion
//
// #region bool to "On" / "Off" string value converter
//
// 	[ValueConversion(typeof(bool), typeof(string))]
// 	public class BoolToOnOffConverter : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			// if (targetType != typeof(object))
// 			// 	throw new InvalidOperationException("The target must be an object");
// 			return ((bool) (value ?? false)) ? "On" : "Off";
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter,
// 			System.Globalization.CultureInfo culture)
// 		{
// 			throw new NotSupportedException();
// 		}
// 	}
//
// #endregion
//
// #region bool to "Yes" / "No" string value converter
//
// 	[ValueConversion(typeof(bool), typeof(string))]
// 	public class BoolToYesNoConverter : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			// if (targetType != typeof(object))
// 			// 	throw new InvalidOperationException("The target must be an object");
//
// 			return ((bool) (value ?? false)) ? "Yes" : "No";
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter,
// 			System.Globalization.CultureInfo culture)
// 		{
// 			throw new NotSupportedException();
// 		}
// 	}
//
// #endregion
//
// #region not bool value converter
//
// 	[ValueConversion(typeof(bool), typeof(bool))]
// 	public class InvertBool : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return !(bool) (value ?? false);
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
//
// #region null to bool value converter
//
// 	[ValueConversion(typeof(object), typeof(bool))]
// 	public class NullObjToBool : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return value == null;
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
//
// #region null to string value converter
//
// 	[ValueConversion(typeof(string), typeof(string))]
// 	public class NullStringToMessage : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			if (parameter == null) parameter = "is null";
//
// 			return (string) value ?? (string) parameter;
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
// 	
// #region null object to string value converter
//
// 	[ValueConversion(typeof(object), typeof(string))]
// 	public class NullObjToMessage : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			if (parameter == null) parameter = "is null";
//
// 			return value ?? (string) parameter;
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
//
// #region multi bool "or" value converter
//
// 	[ValueConversion(typeof(bool), typeof(bool))]
// 	public class BoolOr : IMultiValueConverter
// 	{
// 		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			if (values.Length == 0 ||
// 				!(values[0] is bool) ||
// 				!(values[1] is bool) ) return false;
//
// 			bool result = (bool) values[0];
//
// 			if (values.Length == 1) return result;
//
// 			for (int i = 1; i < values.Length; i++)
// 			{
// 				if (!(values[i] is bool)) continue;
//
// 				result = result || (bool) values[i];
// 			}
//
// 			return result;
// 		}
//
// 		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
//
// #region multi "equals to" value converter
//
// 	[ValueConversion(typeof(object), typeof(bool))]
// 	public class EqualsToBool : IMultiValueConverter
// 	{
// 		public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			if (values.Length < 2 ||
// 				!(values[0].GetType() == values[1].GetType())) return false;
//
// 			return values[0].Equals(values[1]);
// 		}
//
// 		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
//
// #region string value converter
//
// 	/// <summary>
// 	/// based on the string provided return<br/>
// 	/// if null: the first string<br/>
// 	/// if not null: the second string<br/>
// 	/// this uses a specialized collection which must be referenced<br/>
// 	/// here as well as in the XAML file ('xmlns:cs="clr-namespace:System.Collections.Specialized;assembly=System" ')
// 	/// </summary>
// 	[ValueConversion(typeof(string), typeof(string))]
// 	public class StringToMessage : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			if (parameter == null) return "";
//
// 			string[] p = new string[((StringCollection) parameter).Count];
// 			((StringCollection) parameter).CopyTo(p, 0);
//
// 			if (p[1].IsVoid()) p[1] = (string) value;
//
// 			return ((string) value).IsVoid() ? p[0] : p[1];
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
//
// #region int values comparison
//
// 	/// <summary>
// 	/// based on the string provided return<br/>
// 	/// if null: the first string<br/>
// 	/// if not null: the second string<br/>
// 	/// this uses a specialized collection which must be referenced<br/>
// 	/// here as well as in the XAML file ('xmlns:cs="clr-namespace:System.Collections.Specialized;assembly=System" ')
// 	/// </summary>
// 	[ValueConversion(typeof(Int32), typeof(bool))]
// 	public class Int32Comparison : IValueConverter
// 	{
// 		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			if (parameter == null) return false;
//
// 			string[] p = ((string) parameter).Split(' ');
//
// 			if (p.Length != 2 || p[0].IsVoid() || p[1].IsVoid()) return false;
//
// 			int valu = (int) (value ?? 0); // use (Int32) rather than (int) ???
// 			int test;
//
// 			bool result = Int32.TryParse(p[1], out test);
// 			if (!result) return false;
//
// 			result = false;
//
// 			switch (p[0])
// 			{
// 			case ">":
// 				{
// 					result = valu > test;
// 					break;
// 				}
// 			case "<":
// 				{
// 					result = valu < test;
// 					break;
// 				}
// 			case "!=":
// 				{
// 					result = valu != test;
// 					break;
// 				}
// 			case ">=":
// 				{
// 					result = valu >= test;
// 					break;
// 				}
// 			case "<=":
// 				{
// 					result = valu >= test;
// 					break;
// 				}
// 			case "==":
// 				{
// 					result = valu == test;
// 					break;
// 				}
// 			}
//
// 			return result;
// 		}
//
// 		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
// 		{
// 			return null;
// 		}
// 	}
//
// #endregion
}