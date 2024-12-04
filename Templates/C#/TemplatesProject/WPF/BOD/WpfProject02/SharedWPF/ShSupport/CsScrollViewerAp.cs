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
	public class CsScrollViewerAp : DependencyObject
	{




	#region scroll viewer border thickness

		public static readonly DependencyProperty ScrollViewerBorderThicknessProperty = DependencyProperty.RegisterAttached(
			"ScrollViewerBorderThickness", typeof(Thickness), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new Thickness(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetScrollViewerBorderThickness(UIElement e, Thickness value)
		{
			e.SetValue(ScrollViewerBorderThicknessProperty, value);
		}

		public static Thickness GetScrollViewerBorderThickness(UIElement e)
		{
			return (Thickness) e.GetValue(ScrollViewerBorderThicknessProperty);
		}

	#endregion

	#region scroll viewer border color

		public static readonly DependencyProperty ScrollViewerBorderColorProperty = DependencyProperty.RegisterAttached(
			"ScrollViewerBorderColor", typeof(SolidColorBrush), typeof(CsScrollViewerAp),
			new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetScrollViewerBorderColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(ScrollViewerBorderColorProperty, value);
		}

		public static SolidColorBrush GetScrollViewerBorderColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(ScrollViewerBorderColorProperty);
		}

	#endregion

	#region scroll viewer corner radius

		public static readonly DependencyProperty ScrollViewerCornerRadiusProperty = DependencyProperty.RegisterAttached(
			"ScrollViewerCornerRadius", typeof(CornerRadius), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetScrollViewerCornerRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(ScrollViewerCornerRadiusProperty, value);
		}

		public static CornerRadius GetScrollViewerCornerRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(ScrollViewerCornerRadiusProperty);
		}

	#endregion

		// corner rect bkg brush  (applied to corner rectangle 4 (top of stack)

	#region corner rectangle background brush

		public static readonly DependencyProperty CornerRectBkgBrushProperty = DependencyProperty.RegisterAttached(
			"CornerRectBkgBrush", typeof(SolidColorBrush), typeof(CsScrollViewerAp),
			new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits|FrameworkPropertyMetadataOptions.AffectsRender));

		public static void SetCornerRectBkgBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRectBkgBrushProperty, value);
		}

		public static SolidColorBrush GetCornerRectBkgBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRectBkgBrushProperty);
		}

	#endregion

		// corner rect 1, 2, 3, 4
		// border brush
		// radius
		// thickness

		//corner rect border 1
	#region corner rectangle 1 Border Brush

		public static readonly DependencyProperty CornerRect1BdrBrushProperty = DependencyProperty.RegisterAttached(
			"CornerRect1BdrBrush", typeof(SolidColorBrush), typeof(CsScrollViewerAp),
			new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect1BdrBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRect1BdrBrushProperty, value);
		}

		public static SolidColorBrush GetCornerRect1BdrBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRect1BdrBrushProperty);
		}

	#endregion

	#region corner rectangle 1 border thickness

		public static readonly DependencyProperty CornerRect1BdrThicknessProperty = DependencyProperty.RegisterAttached(
			"CornerRect1BdrThickness", typeof(Thickness), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect1BdrThickness(UIElement e, Thickness value)
		{
			e.SetValue(CornerRect1BdrThicknessProperty, value);
		}

		public static Thickness GetCornerRect1BdrThickness(UIElement e)
		{
			return (Thickness) e.GetValue(CornerRect1BdrThicknessProperty);
		}

	#endregion

	#region corner rectangle 1 border radius

		public static readonly DependencyProperty CornerRect1BdrRadiusProperty = DependencyProperty.RegisterAttached(
			"CornerRect1BdrRadius", typeof(CornerRadius), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect1BdrRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(CornerRect1BdrRadiusProperty, value);
		}

		public static CornerRadius GetCornerRect1BdrRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(CornerRect1BdrRadiusProperty);
		}

	#endregion


		//corner rect border 2
	#region corner rectangle 2 Border Brush

		public static readonly DependencyProperty CornerRect2BdrBrushProperty = DependencyProperty.RegisterAttached(
			"CornerRect2BdrBrush", typeof(SolidColorBrush), typeof(CsScrollViewerAp),
			new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect2BdrBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRect2BdrBrushProperty, value);
		}

		public static SolidColorBrush GetCornerRect2BdrBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRect2BdrBrushProperty);
		}

	#endregion

	#region corner rectangle 2 border thickness

		public static readonly DependencyProperty CornerRect2BdrThicknessProperty = DependencyProperty.RegisterAttached(
			"CornerRect2BdrThickness", typeof(Thickness), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect2BdrThickness(UIElement e, Thickness value)
		{
			e.SetValue(CornerRect2BdrThicknessProperty, value);
		}

		public static Thickness GetCornerRect2BdrThickness(UIElement e)
		{
			return (Thickness) e.GetValue(CornerRect2BdrThicknessProperty);
		}

	#endregion

	#region corner rectangle 2 border radius

		public static readonly DependencyProperty CornerRect2BdrRadiusProperty = DependencyProperty.RegisterAttached(
			"CornerRect2BdrRadius", typeof(CornerRadius), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect2BdrRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(CornerRect2BdrRadiusProperty, value);
		}

		public static CornerRadius GetCornerRect2BdrRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(CornerRect2BdrRadiusProperty);
		}

	#endregion

		//corner rect border 3
	#region corner rectangle 3 Border Brush

		public static readonly DependencyProperty CornerRect3BdrBrushProperty = DependencyProperty.RegisterAttached(
			"CornerRect3BdrBrush", typeof(SolidColorBrush), typeof(CsScrollViewerAp),
			new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect3BdrBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRect3BdrBrushProperty, value);
		}

		public static SolidColorBrush GetCornerRect3BdrBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRect3BdrBrushProperty);
		}

	#endregion

	#region corner rectangle 3 border thickness

		public static readonly DependencyProperty CornerRect3BdrThicknessProperty = DependencyProperty.RegisterAttached(
			"CornerRect3BdrThickness", typeof(Thickness), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect3BdrThickness(UIElement e, Thickness value)
		{
			e.SetValue(CornerRect3BdrThicknessProperty, value);
		}

		public static Thickness GetCornerRect3BdrThickness(UIElement e)
		{
			return (Thickness) e.GetValue(CornerRect3BdrThicknessProperty);
		}

	#endregion

	#region corner rectangle 3 border radius

		public static readonly DependencyProperty CornerRect3BdrRadiusProperty = DependencyProperty.RegisterAttached(
			"CornerRect3BdrRadius", typeof(CornerRadius), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect3BdrRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(CornerRect3BdrRadiusProperty, value);
		}

		public static CornerRadius GetCornerRect3BdrRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(CornerRect3BdrRadiusProperty);
		}

	#endregion

		//corner rect border 4
	#region corner rectangle 4 Border Brush

		public static readonly DependencyProperty CornerRect4BdrBrushProperty = DependencyProperty.RegisterAttached(
			"CornerRect4BdrBrush", typeof(SolidColorBrush), typeof(CsScrollViewerAp),
			new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect4BdrBrush(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRect4BdrBrushProperty, value);
		}

		public static SolidColorBrush GetCornerRect4BdrBrush(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRect4BdrBrushProperty);
		}

	#endregion

	#region corner rectangle 4 border thickness

		public static readonly DependencyProperty CornerRect4BdrThicknessProperty = DependencyProperty.RegisterAttached(
			"CornerRect4BdrThickness", typeof(Thickness), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new Thickness(0, 0, 0, 0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect4BdrThickness(UIElement e, Thickness value)
		{
			e.SetValue(CornerRect4BdrThicknessProperty, value);
		}

		public static Thickness GetCornerRect4BdrThickness(UIElement e)
		{
			return (Thickness) e.GetValue(CornerRect4BdrThicknessProperty);
		}

	#endregion

	#region corner rectangle 4 border radius

		public static readonly DependencyProperty CornerRect4BdrRadiusProperty = DependencyProperty.RegisterAttached(
			"CornerRect4BdrRadius", typeof(CornerRadius), typeof(CsScrollViewerAp), new FrameworkPropertyMetadata(new CornerRadius(0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetCornerRect4BdrRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(CornerRect4BdrRadiusProperty, value);
		}

		public static CornerRadius GetCornerRect4BdrRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(CornerRect4BdrRadiusProperty);
		}

	#endregion


	}
}