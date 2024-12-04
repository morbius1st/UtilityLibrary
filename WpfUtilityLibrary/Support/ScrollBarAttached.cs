#region + Using Directives

using System.Windows;
using System.Windows.Media;
using System.Drawing;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace SharedApp.Windows.ShSupport
{
	public class ScrollBarAttached : DependencyObject
	{
	#region ScrollBarWidth

		public static readonly DependencyProperty ScrollBarWidthProperty = DependencyProperty.RegisterAttached(
			"ScrollBarWidth", typeof(double), typeof(ScrollBarAttached), new PropertyMetadata(8.0));

		public static void SetScrollBarWidth(UIElement element, double value)
		{
			element.SetValue(ScrollBarWidthProperty, value);
		}

		public static double GetScrollBarWidth(UIElement element)
		{
			return (double) element.GetValue(ScrollBarWidthProperty);
		}

	#endregion

	#region scrollbar corner radius

		public static readonly DependencyProperty ScrollBarCornerRadiusProperty = DependencyProperty.RegisterAttached(
			"ScrollBarCornerRadius", typeof(CornerRadius), typeof(ScrollBarAttached), new PropertyMetadata(new CornerRadius(0)));

		public static void SetScrollBarCornerRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(ScrollBarCornerRadiusProperty, value);
		}

		public static CornerRadius GetScrollBarCornerRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(ScrollBarCornerRadiusProperty);
		}

	#endregion

	#region thumb corner radius

		public static readonly DependencyProperty ThumbCornerRadiusProperty = DependencyProperty.RegisterAttached(
			"ThumbCornerRadius", typeof(CornerRadius), typeof(ScrollBarAttached), new PropertyMetadata(new CornerRadius(0)));

		public static void SetThumbCornerRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(ThumbCornerRadiusProperty, value);
		}

		public static CornerRadius GetThumbCornerRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(ThumbCornerRadiusProperty);
		}

	#endregion

	#region vertical border thickness

		public static readonly DependencyProperty VertBorderThicknessProperty = DependencyProperty.RegisterAttached(
			"VertBorderThickness", typeof(Thickness), typeof(ScrollBarAttached), new PropertyMetadata(new Thickness(1,1,1,1)));

		public static void SetVertBorderThickness(UIElement e, Thickness value)
		{
			e.SetValue(VertBorderThicknessProperty, value);
		}

		public static Thickness GetVertBorderThickness(UIElement e)
		{
			return (Thickness) e.GetValue(VertBorderThicknessProperty);
		}

	#endregion

	#region vertical border color

		public static readonly DependencyProperty VertBorderColorProperty = DependencyProperty.RegisterAttached(
			"VertBorderColor", typeof(SolidColorBrush), typeof(ScrollBarAttached), 
			new PropertyMetadata(Brushes.Black));

		public static void SetVertBorderColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(VertBorderColorProperty, value);
		}

		public static SolidColorBrush GetVertBorderColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(VertBorderColorProperty);
		}

	#endregion

	#region horizontal border thickness

		public static readonly DependencyProperty HorizBorderThicknessProperty = DependencyProperty.RegisterAttached(
			"HorizBorderThickness", typeof(Thickness), typeof(ScrollBarAttached), new PropertyMetadata(new Thickness(1,1,1,1)));

		public static void SetHorizBorderThickness(UIElement e, Thickness value)
		{
			e.SetValue(HorizBorderThicknessProperty, value);
		}

		public static Thickness GetHorizBorderThickness(UIElement e)
		{
			return (Thickness) e.GetValue(HorizBorderThicknessProperty);
		}

	#endregion

	#region horizontal border color

		public static readonly DependencyProperty HorizBorderColorProperty = DependencyProperty.RegisterAttached(
			"HorizBorderColor", typeof(SolidColorBrush), typeof(ScrollBarAttached), 
			new PropertyMetadata(Brushes.Black));

		public static void SetHorizBorderColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(HorizBorderColorProperty, value);
		}

		public static SolidColorBrush GetHorizBorderColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(HorizBorderColorProperty);
		}

	#endregion

	#region TrackWidth

		public static readonly DependencyProperty TrackWidthProperty = DependencyProperty.RegisterAttached(
			"TrackWidth", typeof(double), typeof(ScrollBarAttached), new FrameworkPropertyMetadata(4.0,
			FrameworkPropertyMetadataOptions.Inherits | FrameworkPropertyMetadataOptions.AffectsRender));

		public static void SetTrackWidth(UIElement e, double value)
		{
			e.SetValue(TrackWidthProperty, value);
		}

		public static double GetTrackWidth(UIElement e)
		{
			return (double) e.GetValue(TrackWidthProperty);
		}

	#endregion

	#region RepeatButtonLength

		public static readonly DependencyProperty RepeatButtonLengthProperty = DependencyProperty.RegisterAttached(
			"RepeatButtonLength", typeof(double), typeof(ScrollBarAttached), new PropertyMetadata(8.0));

		public static void SetRepeatButtonLength(UIElement e, double value)
		{
			e.SetValue(RepeatButtonLengthProperty, value);
		}

		public static double GetRepeatButtonLength(UIElement e)
		{
			return (double) e.GetValue(RepeatButtonLengthProperty);
		}

	#endregion

	#region RepeatButtonIconLength

		public static readonly DependencyProperty RepeatButtonIconLengthProperty = DependencyProperty.RegisterAttached(
			"RepeatButtonIconLength", typeof(double), typeof(ScrollBarAttached), new PropertyMetadata(6.0));

		public static void SetRepeatButtonIconLength(UIElement e, double value)
		{
			e.SetValue(RepeatButtonIconLengthProperty, value);
		}

		public static double GetRepeatButtonIconLength(UIElement e)
		{
			return (double) e.GetValue(RepeatButtonIconLengthProperty);
		}

	#endregion

	#region RepeatButtonIconGirth

		public static readonly DependencyProperty RepeatButtonIconGirthProperty = DependencyProperty.RegisterAttached(
			"RepeatButtonIconGirth", typeof(double), typeof(ScrollBarAttached), new PropertyMetadata(6.0));

		public static void SetRepeatButtonIconGirth(UIElement e, double value)
		{
			e.SetValue(RepeatButtonIconGirthProperty, value);
		}

		public static double GetRepeatButtonIconGirth(UIElement e)
		{
			return (double) e.GetValue(RepeatButtonIconGirthProperty);
		}

	#endregion

	#region IconMarginTop

		public static readonly DependencyProperty IconMarginTopEndProperty = DependencyProperty.RegisterAttached(
			"IconMarginTopEnd", typeof(Thickness), typeof(ScrollBarAttached), new PropertyMetadata(new Thickness(0,0,0,0)));

		public static void SetIconMarginTopEnd(UIElement e, Thickness value)
		{
			e.SetValue(IconMarginTopEndProperty, value);
		}

		public static Thickness GetIconMarginTopEnd(UIElement e)
		{
			return (Thickness) e.GetValue(IconMarginTopEndProperty);
		}

	#endregion

	#region IconMarginBottom

		public static readonly DependencyProperty IconMarginBottomEndProperty = DependencyProperty.RegisterAttached(
			"IconMarginBottomEnd", typeof(Thickness), typeof(ScrollBarAttached), new PropertyMetadata(new Thickness(0,0,0,0)));

		public static void SetIconMarginBottomEnd(UIElement e, Thickness value)
		{
			e.SetValue(IconMarginBottomEndProperty, value);
		}

		public static Thickness GetIconMarginBottomEnd(UIElement e)
		{
			return (Thickness) e.GetValue(IconMarginBottomEndProperty);
		}

	#endregion

	#region IconMarginLeft

		public static readonly DependencyProperty IconMarginLeftEndProperty = DependencyProperty.RegisterAttached(
			"IconMarginLeftEnd", typeof(Thickness), typeof(ScrollBarAttached), new PropertyMetadata(new Thickness(0,0,0,0)));

		public static void SetIconMarginLeftEnd(UIElement e, Thickness value)
		{
			e.SetValue(IconMarginLeftEndProperty, value);
		}

		public static Thickness GetIconMarginLeftEnd(UIElement e)
		{
			return (Thickness) e.GetValue(IconMarginLeftEndProperty);
		}

	#endregion

	#region IconMarginRight

		public static readonly DependencyProperty IconMarginRightEndProperty = DependencyProperty.RegisterAttached(
			"IconMarginRightEnd", typeof(Thickness), typeof(ScrollBarAttached), new PropertyMetadata(new Thickness(0,0,0,0)));

		public static void SetIconMarginRightEnd(UIElement e, Thickness value)
		{
			e.SetValue(IconMarginRightEndProperty, value);
		}

		public static Thickness GetIconMarginRightEnd(UIElement e)
		{
			return (Thickness) e.GetValue(IconMarginRightEndProperty);
		}

	#endregion

	}
}