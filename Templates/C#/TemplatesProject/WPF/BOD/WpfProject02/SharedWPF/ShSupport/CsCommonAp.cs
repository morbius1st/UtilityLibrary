#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

#endregion

// user name: jeffs
// created:   6/6/2022 7:30:38 PM

namespace SharedWPF.ShSupport
{
	public class CsCommonAp : DependencyObject
	{
	#region common - mouse over background

		public static readonly DependencyProperty
			BgCommonMouseOverProperty = DependencyProperty.RegisterAttached(
				"BgCommonMouseOver", typeof(SolidColorBrush), typeof(CsCommonAp),
				new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetBgCommonMouseOver(UIElement e, SolidColorBrush value)
		{
			e.SetValue(BgCommonMouseOverProperty, value);
		}

		public static SolidColorBrush GetBgCommonMouseOver(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(BgCommonMouseOverProperty);
		}

	#endregion
		
	#region common - mouse over foreground

		public static readonly DependencyProperty
			FgCommonMouseOverProperty = DependencyProperty.RegisterAttached(
				"FgCommonMouseOver", typeof(SolidColorBrush), typeof(CsCommonAp),
				new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetFgCommonMouseOver(UIElement e, SolidColorBrush value)
		{
			e.SetValue(FgCommonMouseOverProperty, value);
		}

		public static SolidColorBrush GetFgCommonMouseOver(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(FgCommonMouseOverProperty);
		}

	#endregion

	#region common - mouse over borderbrush

		public static readonly DependencyProperty
			BdrCommonMouseOverProperty = DependencyProperty.RegisterAttached(
				"BdrCommonMouseOver", typeof(SolidColorBrush), typeof(CsCommonAp),
				new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetBdrCommonMouseOver(UIElement e, SolidColorBrush value)
		{
			e.SetValue(BdrCommonMouseOverProperty, value);
		}

		public static SolidColorBrush GetBdrCommonMouseOver(UIElement e)
		{
			return (SolidColorBrush) e.GetValue(BdrCommonMouseOverProperty);
		}

	#endregion


	}
}
