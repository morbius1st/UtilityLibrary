#region + Using Directives

using System.Windows;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace SharedApp.Windows.ShSupport
{
	public class VisualStatesAttached : DependencyObject
	{
	#region corner radius

		public static readonly DependencyProperty NormalCornerRadiusProperty = DependencyProperty.RegisterAttached(
			"NormalCornerRadius", typeof(CornerRadius), typeof(VisualStatesAttached), new PropertyMetadata(new CornerRadius(3.0)));

		public static void SetNormalCornerRadius(UIElement element, CornerRadius value)
		{
			element.SetValue(NormalCornerRadiusProperty, value);
		}

		public static CornerRadius GetNormalCornerRadius(UIElement element)
		{
			return (CornerRadius) element.GetValue(NormalCornerRadiusProperty);
		}

	#endregion

	}
}