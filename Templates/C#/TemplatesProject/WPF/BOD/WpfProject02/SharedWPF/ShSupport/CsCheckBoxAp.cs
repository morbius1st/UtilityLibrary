#region + Using Directives

using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace SharedWPF.ShSupport
{
	public class CsCheckBoxAp : DependencyObject
	{

	#region double - checkbox box size

		public static readonly DependencyProperty CheckBoxBoxSizeProperty = DependencyProperty.RegisterAttached(
			"CheckBoxBoxSize", typeof(double), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(8.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxBoxSize(UIElement e, double value)
		{
			e.SetValue(CheckBoxBoxSizeProperty, value);
		}

		public static double GetCheckBoxBoxSize(UIElement e)
		{
			return (double) e.GetValue(CheckBoxBoxSizeProperty);
		}

	#endregion

	#region thickness = checkbox box margin

		public static readonly DependencyProperty
			CheckBoxBoxMarginProperty = DependencyProperty.RegisterAttached(
				"CheckBoxBoxMargin", typeof(Thickness),
				typeof(CsCheckBoxAp), new FrameworkPropertyMetadata(new Thickness(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxBoxMargin(UIElement e, Thickness value)
		{
			e.SetValue(CheckBoxBoxMarginProperty, value);
		}

		public static Thickness GetCheckBoxBoxMargin(UIElement e)
		{
			return (Thickness) e.GetValue(CheckBoxBoxMarginProperty);
		}

	#endregion

	#region thickness - checkbox check margin

		public static readonly DependencyProperty
			CheckBoxCheckMarginProperty = DependencyProperty.RegisterAttached(
				"CheckBoxCheckMargin", typeof(Thickness),
				typeof(CsCheckBoxAp), new FrameworkPropertyMetadata(new Thickness(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxCheckMargin(UIElement e, Thickness value)
		{
			e.SetValue(CheckBoxCheckMarginProperty, value);
		}

		public static Thickness GetCheckBoxCheckMargin(UIElement e)
		{
			return (Thickness) e.GetValue(CheckBoxCheckMarginProperty);
		}

	#endregion

	#region thickness - content margin

		public static readonly DependencyProperty
			CheckBoxContentMarginProperty = DependencyProperty.RegisterAttached(
				"CheckBoxContentMargin", typeof(Thickness),
				typeof(CsCheckBoxAp), new FrameworkPropertyMetadata(new Thickness(0),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCheckBoxContentMargin(UIElement e, Thickness value)
		{
			e.SetValue(CheckBoxContentMarginProperty, value);
		}

		public static Thickness GetCheckBoxContentMargin(UIElement e)
		{
			return (Thickness) e.GetValue(CheckBoxContentMarginProperty);
		}

	#endregion

	#region brush - option mark fill color

		public static readonly DependencyProperty OptionMarkFillBrushProperty = DependencyProperty.RegisterAttached(
			"OptionMarkFillBrush", typeof(SolidColorBrush), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetOptionMarkFillBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(OptionMarkFillBrushProperty, value);
		}

		public static SolidColorBrush GetOptionMarkFillBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(OptionMarkFillBrushProperty);
		}

	#endregion

	#region brush - Indeterminant mark fill color

		public static readonly DependencyProperty IndeterminantMarkFillBrushProperty = DependencyProperty.RegisterAttached(
			"IndeterminantMarkFillBrush", typeof(SolidColorBrush), typeof(CsCheckBoxAp),
			new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetIndeterminantMarkFillBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(IndeterminantMarkFillBrushProperty, value);
		}

		public static SolidColorBrush GetIndeterminantMarkFillBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(IndeterminantMarkFillBrushProperty);
		}

	#endregion


	}
}