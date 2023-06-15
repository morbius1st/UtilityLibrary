#region + Using Directives

using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace SharedWPF.ShSupport
{

	public class CsButtons : DependencyObject
	{

	#region corner radius - button corner radius

		public static readonly DependencyProperty ButtonCornerRadiusProperty = DependencyProperty.RegisterAttached(
			"ButtonCornerRadius", typeof(CornerRadius), typeof(CsButtons), 
			new FrameworkPropertyMetadata(new CornerRadius(3,3,3,3), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetButtonCornerRadius(UIElement e, CornerRadius value)
		{
			e.SetValue(ButtonCornerRadiusProperty, value);
		}

		public static CornerRadius GetButtonCornerRadius(UIElement e)
		{
			return (CornerRadius) e.GetValue(ButtonCornerRadiusProperty);
		}

	#endregion

	#region thickness - button border thickness

		public static readonly DependencyProperty ButtonBorderThicknessProperty = DependencyProperty.RegisterAttached(
			"ButtonBorderThickness", typeof(Thickness), typeof(CsButtons), 
			new FrameworkPropertyMetadata(new Thickness(0,0,0,0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetButtonBorderThickness(UIElement e, Thickness value)
		{
			e.SetValue(ButtonBorderThicknessProperty, value);
		}

		public static Thickness GetButtonBorderThickness(UIElement e)
		{
			return (Thickness) e.GetValue(ButtonBorderThicknessProperty);
		}

	#endregion

	}
}