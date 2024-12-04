#region + Using Directives

using System.Windows;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace SharedApp.Windows.ShSupport
{
	public class ScrollViewerAttached : DependencyObject
	{

	#region corner rectangle color

		public static readonly DependencyProperty CornerRectColorProperty = DependencyProperty.RegisterAttached(
			"CornerRectColor", typeof(SolidColorBrush), typeof(ScrollViewerAttached), 
			new PropertyMetadata(Brushes.Black));

		public static void SetCornerRectColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRectColorProperty, value);
		}

		public static SolidColorBrush GetCornerRectColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRectColorProperty);
		}

	#endregion

	#region corner rectangle left border color

		public static readonly DependencyProperty CornerRectLeftBdrColorProperty = DependencyProperty.RegisterAttached(
			"CornerRectLeftBdrColor", typeof(SolidColorBrush), typeof(ScrollViewerAttached), 
			new PropertyMetadata(Brushes.Black));

		public static void SetCornerRectLeftBdrColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRectLeftBdrColorProperty, value);
		}

		public static SolidColorBrush GetCornerRectLeftBdrColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRectLeftBdrColorProperty);
		}

	#endregion

	#region corner rectangle top border color

		public static readonly DependencyProperty CornerRectTopBdrColorProperty = DependencyProperty.RegisterAttached(
			"CornerRectTopBdrColor", typeof(SolidColorBrush), typeof(ScrollViewerAttached), 
			new PropertyMetadata(Brushes.Black));

		public static void SetCornerRectTopBdrColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRectTopBdrColorProperty, value);
		}

		public static SolidColorBrush GetCornerRectTopBdrColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRectTopBdrColorProperty);
		}

	#endregion
		
	#region corner rectangle right border color

		public static readonly DependencyProperty CornerRectRightBdrColorProperty = DependencyProperty.RegisterAttached(
			"CornerRectRightBdrColor", typeof(SolidColorBrush), typeof(ScrollViewerAttached), 
			new PropertyMetadata(Brushes.Black));

		public static void SetCornerRectRightBdrColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRectRightBdrColorProperty, value);
		}

		public static SolidColorBrush GetCornerRectRightBdrColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRectRightBdrColorProperty);
		}

	#endregion
				
	#region corner rectangle bottom border color

		public static readonly DependencyProperty CornerRectBottBdrColorProperty = DependencyProperty.RegisterAttached(
			"CornerRectBottBdrColor", typeof(SolidColorBrush), typeof(ScrollViewerAttached), 
			new PropertyMetadata(Brushes.Black));

		public static void SetCornerRectBottBdrColor(UIElement e, SolidColorBrush value)
		{
			e.SetValue(CornerRectBottBdrColorProperty, value);
		}

		public static SolidColorBrush GetCornerRectBottBdrColor(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(CornerRectBottBdrColorProperty);
		}

	#endregion

	#region corner rectangle left border height

		public static readonly DependencyProperty CornerRectLeftBdrHeightProperty = DependencyProperty.RegisterAttached(
			"CornerRectLeftBdrHeight", typeof(double), typeof(ScrollViewerAttached), new PropertyMetadata(0.0));

		public static void SetCornerRectLeftBdrHeight(UIElement e, double value)
		{
			e.SetValue(CornerRectLeftBdrHeightProperty, value);
		}

		public static double GetCornerRectLeftBdrHeight(UIElement e)
		{
			return (double) e.GetValue(CornerRectLeftBdrHeightProperty);
		}

	#endregion

	#region corner rectangle top border height

		public static readonly DependencyProperty CornerRectTopBdrHeightProperty = DependencyProperty.RegisterAttached(
			"CornerRectTopBdrHeight", typeof(double), typeof(ScrollViewerAttached), new PropertyMetadata(0.0));

		public static void SetCornerRectTopBdrHeight(UIElement e, double value)
		{
			e.SetValue(CornerRectTopBdrHeightProperty, value);
		}

		public static double GetCornerRectTopBdrHeight(UIElement e)
		{
			return (double) e.GetValue(CornerRectTopBdrHeightProperty);
		}

	#endregion
		
	#region corner rectangle right border height

		public static readonly DependencyProperty CornerRectRightBdrHeightProperty = DependencyProperty.RegisterAttached(
			"CornerRectRightBdrHeight", typeof(double), typeof(ScrollViewerAttached), 
			new PropertyMetadata(0.0));

		public static void SetCornerRectRightBdrHeight(UIElement e, double value)
		{
			e.SetValue(CornerRectRightBdrHeightProperty, value);
		}

		public static double GetCornerRectRightBdrHeight(UIElement e)
		{
			return (double) e.GetValue(CornerRectRightBdrHeightProperty);
		}

	#endregion

	#region corner rectangle bottom border height

		public static readonly DependencyProperty CornerRectBottBdrHeightProperty = DependencyProperty.RegisterAttached(
			"CornerRectBottBdrHeight", typeof(double), typeof(ScrollViewerAttached), 
			new PropertyMetadata(0.0));

		public static void SetCornerRectBottBdrHeight(UIElement e, double value)
		{
			e.SetValue(CornerRectBottBdrHeightProperty, value);
		}

		public static double GetCornerRectBottBdrHeight(UIElement e)
		{
			return (double) e.GetValue(CornerRectBottBdrHeightProperty);
		}

	#endregion

	}
}