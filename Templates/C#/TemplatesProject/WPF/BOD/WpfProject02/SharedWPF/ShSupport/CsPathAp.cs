#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

#endregion

// user name: jeffs
// created:   7/6/2022 6:42:42 AM

namespace SharedWPF.ShSupport
{
	public class CsPathAp
	{

	#region double - icon Disabled scale x

		public static readonly DependencyProperty IconDisabledScaleXProperty = DependencyProperty.RegisterAttached(
			"IconDisabledScaleX", typeof(double), typeof(CsPathAp), 
			new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetIconDisabledScaleX(UIElement e, double value)
		{
			e.SetValue(IconDisabledScaleXProperty, value);
		}

		public static double GetIconDisabledScaleX(UIElement e)
		{
			return (double) e.GetValue(IconDisabledScaleXProperty);
		}

	#endregion

	#region double - icon IsSel scale x

		public static readonly DependencyProperty IconIsSelScaleXProperty = DependencyProperty.RegisterAttached(
			"IconIsSelScaleX", typeof(double), typeof(CsPathAp), 
			new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetIconIsSelScaleX(UIElement e, double value)
		{
			e.SetValue(IconIsSelScaleXProperty, value);
		}

		public static double GetIconIsSelScaleX(UIElement e)
		{
			return (double) e.GetValue(IconIsSelScaleXProperty);
		}

	#endregion

	#region double - icon IsEditing scale x

		public static readonly DependencyProperty IconIsEditingScaleXProperty = DependencyProperty.RegisterAttached(
			"IconIsEditingScaleX", typeof(double), typeof(CsPathAp), 
			new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetIconIsEditingScaleX(UIElement e, double value)
		{
			e.SetValue(IconIsEditingScaleXProperty, value);
		}

		public static double GetIconIsEditingScaleX(UIElement e)
		{
			return (double) e.GetValue(IconIsEditingScaleXProperty);
		}

	#endregion

	#region double - icon IsLocked scale x

		public static readonly DependencyProperty IconIsLockedScaleXProperty = DependencyProperty.RegisterAttached(
			"IconIsLockedScaleX", typeof(double), typeof(CsPathAp), 
			new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.Inherits));

		public static void SetIconIsLockedScaleX(UIElement e, double value)
		{
			e.SetValue(IconIsLockedScaleXProperty, value);
		}

		public static double GetIconIsLockedScaleX(UIElement e)
		{
			return (double) e.GetValue(IconIsLockedScaleXProperty);
		}

	#endregion

	#region thickness - path margin

		public static readonly DependencyProperty IconIsEditingMarginProperty = DependencyProperty.RegisterAttached(
			"IconIsEditingMargin", typeof(Thickness), typeof(CsPathAp), 
			new FrameworkPropertyMetadata(new Thickness(0,0,0,0), FrameworkPropertyMetadataOptions.Inherits));

		public static void SetIconIsEditingMargin(UIElement e, Thickness value)
		{
			e.SetValue(IconIsEditingMarginProperty, value);
		}

		public static Thickness GetIconIsEditingMargin(UIElement e)
		{
			return (Thickness) e.GetValue(IconIsEditingMarginProperty);
		}

	#endregion

	}
}
