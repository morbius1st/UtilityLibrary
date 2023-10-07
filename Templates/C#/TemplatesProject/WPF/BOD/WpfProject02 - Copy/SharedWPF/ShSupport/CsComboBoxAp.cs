#region + Using Directives
using System; 
using System.Windows;
using System.Windows.Media;
#endregion

// user name: jeffs
// created:   5/9/2020 10:23:38 PM


namespace SharedWPF.ShSupport
{
	public class CsComboBoxAp : DependencyObject
	{

	#region combobox border radius

		public static readonly DependencyProperty ComboBoxBdrRadiusProperty = DependencyProperty.RegisterAttached(
			"ComboBoxBdrRadius", typeof(CornerRadius), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetComboBoxBdrRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(ComboBoxBdrRadiusProperty, value);
		}

		public static CornerRadius GetComboBoxBdrRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(ComboBoxBdrRadiusProperty);
		}

	#endregion

	#region drop down Border Brush

		public static readonly DependencyProperty DropDownBdrBrushProperty = DependencyProperty.RegisterAttached(
			"DropDownBdrBrush", typeof(SolidColorBrush), typeof(CsComboBoxAp),
			new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetDropDownBdrBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(DropDownBdrBrushProperty, value);
		}

		public static SolidColorBrush GetDropDownBdrBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(DropDownBdrBrushProperty);
		}

	#endregion

	#region drop down border thickness

		public static readonly DependencyProperty DropDownBdrThicknessProperty = DependencyProperty.RegisterAttached(
			"DropDownBdrThickness", typeof(Thickness), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetDropDownBdrThickness(UIElement e, Thickness value)
		{
			e.SetValue(DropDownBdrThicknessProperty, value);
		}

		public static Thickness GetDropDownBdrThickness(UIElement e)
		{
			return (Thickness) e.GetValue(DropDownBdrThicknessProperty);
		}

	#endregion

	#region drop down border radius

		public static readonly DependencyProperty DropDownBdrRadiusProperty = DependencyProperty.RegisterAttached(
			"DropDownBdrRadius", typeof(CornerRadius), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetDropDownBdrRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(DropDownBdrRadiusProperty, value);
		}

		public static CornerRadius GetDropDownBdrRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(DropDownBdrRadiusProperty);
		}

	#endregion

	#region DropDownMinWidth

		public static readonly DependencyProperty DropDownMinWidthProperty = DependencyProperty.RegisterAttached(
			"DropDownMinWidth", typeof(double), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(Double.PositiveInfinity, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetDropDownMinWidth(UIElement e, double value)
		{
			e.SetValue(DropDownMinWidthProperty, value);
		}

		public static double GetDropDownMinWidth(UIElement e)
		{
			return (double) e.GetValue(DropDownMinWidthProperty);
		}

	#endregion

	#region DropDownMaxWidth

		public static readonly DependencyProperty DropDownMaxWidthProperty = DependencyProperty.RegisterAttached(
			"DropDownMaxWidth", typeof(double), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(Double.PositiveInfinity, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetDropDownMaxWidth(UIElement e, double value)
		{
			e.SetValue(DropDownMaxWidthProperty, value);
		}

		public static double GetDropDownMaxWidth(UIElement e)
		{
			return (double) e.GetValue(DropDownMaxWidthProperty);
		}

	#endregion

	#region DropDownBrush

		public static readonly DependencyProperty  DropDownBrushProperty = DependencyProperty.RegisterAttached(
			"DropDownBrush", typeof(SolidColorBrush), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(default(SolidColorBrush), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetDropDownBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(DropDownBrushProperty, value);
		}

		public static SolidColorBrush GetDropDownBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(DropDownBrushProperty);
		}

	#endregion


	// #region MouseOverBgBrush
	//
	// 	public static readonly DependencyProperty MouseOverBgBrushProperty = DependencyProperty.RegisterAttached(
	// 		"MouseOverBgBrush", typeof(SolidColorBrush), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(default(SolidColorBrush), FrameworkPropertyMetadataOptions.Inherits));
	//
	// 	public static void SetMouseOverBgBrush(UIElement e, SolidColorBrush value)
	// 	{
	// 		e.SetValue(MouseOverBgBrushProperty, value);
	// 	}
	//
	// 	public static SolidColorBrush GetMouseOverBgBrush(UIElement e)
	// 	{
	// 		return (SolidColorBrush) e.GetValue(MouseOverBgBrushProperty);
	// 	}
	//
	// #endregion

	// #region MouseOverFgBrush
	//
	// 	public static readonly DependencyProperty MouseOverFgBrushProperty = DependencyProperty.RegisterAttached(
	// 		"MouseOverFgBrush", typeof(SolidColorBrush), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(default(SolidColorBrush), FrameworkPropertyMetadataOptions.Inherits));
	//
	// 	public static void SetMouseOverFgBrush(UIElement e, SolidColorBrush value)
	// 	{
	// 		e.SetValue(MouseOverFgBrushProperty, value);
	// 	}
	//
	// 	public static SolidColorBrush GetMouseOverFgBrush(UIElement e)
	// 	{
	// 		return (SolidColorBrush) e.GetValue(MouseOverFgBrushProperty);
	// 	}
	//
	// #endregion
		


	// #region NotEnabledBrush
	//
	// 	public static readonly DependencyProperty NotEnabledBrushProperty = DependencyProperty.RegisterAttached(
	// 		"NotEnabledBrush", typeof(SolidColorBrush), typeof(CsComboBoxAp), new FrameworkPropertyMetadata(default(SolidColorBrush), FrameworkPropertyMetadataOptions.Inherits));
	//
	// 	public static void SetNotEnabledBrush(UIElement e, SolidColorBrush value)
	// 	{
	// 		e.SetValue(NotEnabledBrushProperty, value);
	// 	}
	//
	// 	public static SolidColorBrush GetNotEnabledBrush(UIElement e)
	// 	{
	// 		return (SolidColorBrush) e.GetValue(NotEnabledBrushProperty);
	// 	}
	//
	// #endregion

	}
}
