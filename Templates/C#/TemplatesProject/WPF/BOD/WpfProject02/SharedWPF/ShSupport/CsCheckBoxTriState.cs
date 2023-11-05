#region + Using Directives
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

// user name: jeffs
// created:   10/8/2023 6:43:16 AM

namespace SharedWPF.ShSupport
{
	public class CsCheckBoxTriState: CheckBox
	{
		
		public bool InvertCheckStateOrder
		{
			get { return (bool)GetValue(InvertCheckStateOrderProperty); }
			set { SetValue(InvertCheckStateOrderProperty, value); }
		}

		// Using a DependencyProperty as the backing store for InvertCheckStateOrder.  This enables animation, styling, binding, etc...
		public static readonly DependencyProperty InvertCheckStateOrderProperty =
			DependencyProperty.Register("InvertCheckStateOrder", typeof(bool), typeof(CsCheckBoxTriState), new UIPropertyMetadata(false));


		protected override void OnToggle()
		{
			if (this.InvertCheckStateOrder)
			{
				// getting the current value of ischecked
				if (this.IsChecked == false)
				{
					// false -> (null or true)
					this.IsChecked = this.IsThreeState ? null : (bool?) true;
				}
				else if (this.IsChecked == true)
				{
					// true -> (false)
					this.IsChecked = false;
				}
				else
				{
					// only when three state
					// null -> (true)
					this.IsChecked = true;
				}
			}
			else
			{
				base.OnToggle();
			}
		}



		// protected override void OnToggle()
		// {
		// 	if (this.InvertCheckStateOrder)
		// 	{
		// 		if (this.IsChecked == true)
		// 		{
		// 			this.IsChecked = false;
		// 		}
		// 		else if (this.IsChecked == false)
		// 		{
		// 			this.IsChecked = this.IsThreeState ? null : (bool?)true;
		// 		}
		// 		else
		// 		{
		// 			this.IsChecked = true;
		// 		}
		// 	}
		// 	else
		// 	{
		// 		base.OnToggle();
		// 	}
		// }

	}
}
