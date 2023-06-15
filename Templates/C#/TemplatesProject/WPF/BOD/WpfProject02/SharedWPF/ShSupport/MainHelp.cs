#region + Using Directives

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Brushes = System.Windows.Media.Brushes;

#endregion

// user name: jeffs
// created:   12/30/2020 11:11:08 PM

namespace SharedWPF.ShSupport
{
	/// <summary>
	/// structure the provides the help information
	/// Help string is placed in 1 of 7 fields.
	/// the int determines which field the help string will be placed.
	/// this allows text to stack across a single lines (basically acts as a tab)
	/// the higher the number the further to the right the text is placed
	/// </summary>
	public struct HelpInfo
	{
		public Thickness Marg { get; private set; }
		public string[] HelpDesc { get; private set; }

		public HelpInfo(Tuple<int, string>[] info, Thickness marg = default(Thickness))
		{
			Marg = marg;

			HelpDesc = new [] { "", "", "", "", "" , "", "" };

			foreach (var tuple in info)
			{
				HelpDesc[tuple.Item1] = tuple.Item2;
			}
		}
	}

	public class MainHelp : DependencyObject
	{
	#region general - main title

		public static readonly DependencyProperty
			MainTitleProperty = DependencyProperty.RegisterAttached(
				"MainTitle", typeof(string), typeof(MainHelp),
				new FrameworkPropertyMetadata("",
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetMainTitle(UIElement e, string value)
		{
			e.SetValue(MainTitleProperty, value);
		}

		public static string GetMainTitle(UIElement e)
		{
			return (string) e.GetValue(MainTitleProperty);
		}

	#endregion

	#region general - main description

		public static readonly DependencyProperty
			MainDescriptionProperty = DependencyProperty.RegisterAttached(
				"MainDescription", typeof(string), typeof(MainHelp),
				new FrameworkPropertyMetadata("",
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetMainDescription(UIElement e, string value)
		{
			e.SetValue(MainDescriptionProperty, value);
		}

		public static string GetMainDescription(UIElement e)
		{
			return (string) e.GetValue(MainDescriptionProperty);
		}

	#endregion

	#region general - main content

		public static readonly DependencyProperty
			MainContentProperty = DependencyProperty.RegisterAttached(
				"MainContent", typeof(string), typeof(MainHelp),
				new FrameworkPropertyMetadata("",
					FrameworkPropertyMetadataOptions.Inherits));

		public static void SetMainContent(UIElement e, string value)
		{
			e.SetValue(MainContentProperty, value);
		}

		public static string GetMainContent(UIElement e)
		{
			return (string) e.GetValue(MainContentProperty);
		}

	#endregion

		// special

	#region special - main info - popup help

		public static readonly DependencyProperty MainInfoProperty = DependencyProperty.RegisterAttached(
			"MainInfo", typeof(List<HelpInfo>), typeof(MainHelp),
			new FrameworkPropertyMetadata(new List<HelpInfo>(),
				FrameworkPropertyMetadataOptions.Inherits));

		public static void SetMainInfo(UIElement e, List<HelpInfo> value)
		{
			e.SetValue(MainInfoProperty, value);
		}

		public static List<HelpInfo> GetMainInfo(UIElement e)
		{
			return (List<HelpInfo>) e.GetValue(MainInfoProperty);
		}

	#endregion


		// #region general - does mouse over
		//
		// 	public static readonly DependencyProperty
		// 		DoesMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"DoesMouseOver", typeof(bool), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(false,
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetDoesMouseOver(UIElement e, bool value)
		// 	{
		// 		e.SetValue(DoesMouseOverProperty, value);
		// 	}
		//
		// 	public static bool GetDoesMouseOver(UIElement e)
		// 	{
		// 		return (bool) e.GetValue(DoesMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// #region general - is_activated
		//
		// 	public static readonly DependencyProperty
		// 		IsActivatedProperty = DependencyProperty.RegisterAttached(
		// 			"IsActivated", typeof(bool), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(false,
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIsActivated(UIElement e, bool value)
		// 	{
		// 		e.SetValue(IsActivatedProperty, value);
		// 	}
		//
		// 	public static bool GetIsActivated(UIElement e)
		// 	{
		// 		return (bool) e.GetValue(IsActivatedProperty);
		// 	}
		//
		// #endregion
		//
		// #region general - is_selected
		//
		// 	public static readonly DependencyProperty
		// 		IsSelectedProperty = DependencyProperty.RegisterAttached(
		// 			"IsSelected", typeof(bool), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(false,
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIsSelected(UIElement e, bool value)
		// 	{
		// 		e.SetValue(IsSelectedProperty, value);
		// 	}
		//
		// 	public static bool GetIsSelected(UIElement e)
		// 	{
		// 		return (bool) e.GetValue(IsSelectedProperty);
		// 	}
		//
		// #endregion
		//
		// #region general - is_editing
		//
		// 	public static readonly DependencyProperty
		// 		IsEditingProperty = DependencyProperty.RegisterAttached(
		// 			"IsEditing", typeof(bool), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(false,
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIsEditing(UIElement e, bool value)
		// 	{
		// 		e.SetValue(IsEditingProperty, value);
		// 	}
		//
		// 	public static bool GetIsEditing(UIElement e)
		// 	{
		// 		return (bool) e.GetValue(IsEditingProperty);
		// 	}
		//
		// #endregion
		//
		// #region general - is_modified
		//
		// 	public static readonly DependencyProperty
		// 		IsModifiedProperty = DependencyProperty.RegisterAttached(
		// 			"IsModified", typeof(bool), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(false,
		// 				FrameworkPropertyMetadataOptions.None));
		//
		// 	public static void SetIsModified(UIElement e, bool value)
		// 	{
		// 		e.SetValue(IsModifiedProperty, value);
		// 	}
		//
		// 	public static bool GetIsModified(UIElement e)
		// 	{
		// 		return (bool) e.GetValue(IsModifiedProperty);
		// 	}
		//
		// #endregion
		//
		// #region general - is_readonly
		//
		// 	public static readonly DependencyProperty
		// 		IsReadOnlyProperty = DependencyProperty.RegisterAttached(
		// 			"IsReadOnly", typeof(bool), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(false,
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIsReadOnly(UIElement e, bool value)
		// 	{
		// 		e.SetValue(IsReadOnlyProperty, value);
		// 	}
		//
		// 	public static bool GetIsReadOnly(UIElement e)
		// 	{
		// 		return (bool) e.GetValue(IsReadOnlyProperty);
		// 	}
		//
		// #endregion
		//
		// #region general - is_selected_locked
		//
		// 	public static readonly DependencyProperty
		// 		IsLockedProperty = DependencyProperty.RegisterAttached(
		// 			"IsLocked", typeof(bool), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(false,
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIsLocked(UIElement e, bool value)
		// 	{
		// 		e.SetValue(IsLockedProperty, value);
		// 	}
		//
		// 	public static bool GetIsLocked(UIElement e)
		// 	{
		// 		return (bool) e.GetValue(IsLockedProperty);
		// 	}
		//
		// #endregion
		//
		// #region general - is_goodbad (& null)
		//
		// 	public static readonly DependencyProperty
		// 		IsGoodBadProperty = DependencyProperty.RegisterAttached(
		// 			"IsGoodBad", typeof(bool?), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(null,
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIsGoodBad(UIElement e, bool? value)
		// 	{
		// 		e.SetValue(IsGoodBadProperty, value);
		// 	}
		//
		// 	public static bool? GetIsGoodBad(UIElement e)
		// 	{
		// 		return (bool?) e.GetValue(IsGoodBadProperty);
		// 	}
		//
		// #endregion
		//
		//
		// 	// special
		//
		// #region special - main info - popup help
		//
		// 		public static readonly DependencyProperty MainInfoProperty = DependencyProperty.RegisterAttached(
		// 			"MainInfo", typeof(List<HelpInfo>), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new List<HelpInfo>(),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		// 	
		// 		public static void SetMainInfo(UIElement e, List<HelpInfo> value)
		// 		{
		// 			e.SetValue(MainInfoProperty, value);
		// 		}
		// 	
		// 		public static List<HelpInfo> GetMainInfo(UIElement e)
		// 		{
		// 			return (List<HelpInfo>) e.GetValue(MainInfoProperty);
		// 		}
		//
		// #endregion
		//
		//
		// 	// color properties
		//
		// 	// disabled
		//
		// #region disabled - background (any)
		//
		// 	public static readonly DependencyProperty
		// 		BgDisabledProperty = DependencyProperty.RegisterAttached(
		// 			"BgDisabled", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgDisabled(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgDisabledProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgDisabled(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgDisabledProperty);
		// 	}
		//
		// #endregion
		//
		// #region disabled - foreground (any)
		//
		// 	public static readonly DependencyProperty
		// 		FgDisabledProperty = DependencyProperty.RegisterAttached(
		// 			"FgDisabled", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgDisabled(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgDisabledProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgDisabled(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgDisabledProperty);
		// 	}
		//
		// #endregion
		//
		// #region disabled - borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrDisabledProperty = DependencyProperty.RegisterAttached(
		// 			"BdrDisabled", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrDisabled(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrDisabledProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrDisabled(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrDisabledProperty);
		// 	}
		//
		// #endregion
		//
		// #region disabled - icon geometry
		//
		// 	public static readonly DependencyProperty
		// 		IconGeomDisabledProperty = DependencyProperty.RegisterAttached(
		// 			"IconGeomDisabled", typeof(Geometry), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new PathGeometry(),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIconGeomDisabled(UIElement e, Geometry value)
		// 	{
		// 		e.SetValue(IconGeomDisabledProperty, value);
		// 	}
		//
		// 	public static Geometry GetIconGeomDisabled(UIElement e)
		// 	{
		// 		return (Geometry) e.GetValue(IconGeomDisabledProperty);
		// 	}
		//
		// #endregion
		//
		// 	// not selected (enabled / not selected)
		//
		// #region is_enabled - background (any)
		//
		// 	public static readonly DependencyProperty
		// 		BgIsEnabledProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsEnabled", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsEnabled(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsEnabledProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsEnabled(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsEnabledProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_enabled - foreground (any)
		//
		// 	public static readonly DependencyProperty
		// 		FgIsEnabledProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsEnabled", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsEnabled(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsEnabledProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsEnabled(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsEnabledProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_enabled - borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsEnabledProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsEnabled", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsEnabled(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsEnabledProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsEnabled(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsEnabledProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_enabled - icon geometry
		//
		// 	public static readonly DependencyProperty
		// 		IconGeometryIsEnabledProperty = DependencyProperty.RegisterAttached(
		// 			"IconGeometryIsEnabled", typeof(Geometry), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new PathGeometry(),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIconGeometryIsEnabled(UIElement e, Geometry value)
		// 	{
		// 		e.SetValue(IconGeometryIsEnabledProperty, value);
		// 	}
		//
		// 	public static Geometry GetIconGeometryIsEnabled(UIElement e)
		// 	{
		// 		return (Geometry) e.GetValue(IconGeometryIsEnabledProperty);
		// 	}
		//
		// #endregion
		//
		//
		// #region is_enabled - mouse over background
		//
		// 	public static readonly DependencyProperty
		// 		BgIsEnabledMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsEnabledMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsEnabledMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsEnabledMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsEnabledMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsEnabledMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_enabled - mouse over foreground
		//
		// 	public static readonly DependencyProperty
		// 		FgIsEnabledMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsEnabledMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsEnabledMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsEnabledMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsEnabledMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsEnabledMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_enabled - mouse over borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsEnabledMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsEnabledMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsEnabledMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsEnabledMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsEnabledMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsEnabledMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		//
		// 	// enabled & selected
		//
		// #region is_sel- background (any)
		//
		// 	public static readonly DependencyProperty
		// 		BgIsSelProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsSel", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsSel(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsSelProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsSel(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsSelProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_sel - foreground (any)
		//
		// 	public static readonly DependencyProperty
		// 		FgIsSelProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsSel", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsSel(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsSelProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsSel(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsSelProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_sel - borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsSelProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsSel", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsSel(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsSelProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsSel(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsSelProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_sel - icon geometry
		//
		// 	public static readonly DependencyProperty
		// 		IconGeometryIsSelProperty = DependencyProperty.RegisterAttached(
		// 			"IconGeometryIsSel", typeof(Geometry), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new PathGeometry(),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIconGeometryIsSel(UIElement e, Geometry value)
		// 	{
		// 		e.SetValue(IconGeometryIsSelProperty, value);
		// 	}
		//
		// 	public static Geometry GetIconGeometryIsSel(UIElement e)
		// 	{
		// 		return (Geometry) e.GetValue(IconGeometryIsSelProperty);
		// 	}
		//
		// #endregion
		//
		//
		// #region is_sel - mouse over background
		//
		// 	public static readonly DependencyProperty
		// 		BgIsSelMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsSelMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsSelMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsSelMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsSelMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsSelMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_sel - mouse over foreground
		//
		// 	public static readonly DependencyProperty
		// 		FgIsSelMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsSelMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsSelMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsSelMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsSelMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsSelMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_sel - mouse over borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsSelMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsSelMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsSelMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsSelMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsSelMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsSelMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		//
		// 	// enabled & selected & focused
		//
		// #region is_editing - background (any)
		//
		// 	public static readonly DependencyProperty
		// 		BgIsEditingProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsEditing", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsEditing(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsEditingProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsEditing(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsEditingProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_editing - foreground (any)
		//
		// 	public static readonly DependencyProperty
		// 		FgIsEditingProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsEditing", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsEditing(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsEditingProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsEditing(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsEditingProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_editing - borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsEditingProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsEditing", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsEditing(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsEditingProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsEditing(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsEditingProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_editing - icon geometry
		//
		// 	public static readonly DependencyProperty
		// 		IconGeometryIsEditingProperty = DependencyProperty.RegisterAttached(
		// 			"IconGeometryIsEditing", typeof(Geometry), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new PathGeometry(),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIconGeometryIsEditing(UIElement e, Geometry value)
		// 	{
		// 		e.SetValue(IconGeometryIsEditingProperty, value);
		// 	}
		//
		// 	public static Geometry GetIconGeometryIsEditing(UIElement e)
		// 	{
		// 		return (Geometry) e.GetValue(IconGeometryIsEditingProperty);
		// 	}
		//
		// #endregion
		//
		//
		// #region is_editing - mouse over background
		//
		// 	public static readonly DependencyProperty
		// 		BgIsEditingMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsEditingMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsEditingMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsEditingMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsEditingMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsEditingMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_editing - mouse over foreground
		//
		// 	public static readonly DependencyProperty
		// 		FgIsEditingMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsEditingMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsEditingMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsEditingMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsEditingMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsEditingMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_editing - mouse over borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsEditingMouseOverProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsEditingMouseOver", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsEditingMouseOver(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsEditingMouseOverProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsEditingMouseOver(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsEditingMouseOverProperty);
		// 	}
		//
		// #endregion
		//
		// 	// is read only
		//
		// #region is_readonly - background (any)
		//
		// 	public static readonly DependencyProperty
		// 		BgIsReadOnlyProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsReadOnly", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsReadOnly(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsReadOnlyProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsReadOnly(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsReadOnlyProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_readonly - foreground (any)
		//
		// 	public static readonly DependencyProperty
		// 		FgIsReadOnlyProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsReadOnly", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsReadOnly(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsReadOnlyProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsReadOnly(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsReadOnlyProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_readonly - borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsReadOnlyProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsReadOnly", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsReadOnly(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsReadOnlyProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsReadOnly(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsReadOnlyProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_readonly - icon geometry
		//
		// 	public static readonly DependencyProperty
		// 		IconGeometryIsReadOnlyProperty = DependencyProperty.RegisterAttached(
		// 			"IconGeometryIsReadOnly", typeof(Geometry), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new PathGeometry(),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIconGeometryIsReadOnly(UIElement e, Geometry value)
		// 	{
		// 		e.SetValue(IconGeometryIsReadOnlyProperty, value);
		// 	}
		//
		// 	public static Geometry GetIconGeometryIsReadOnly(UIElement e)
		// 	{
		// 		return (Geometry) e.GetValue(IconGeometryIsReadOnlyProperty);
		// 	}
		//
		// #endregion
		//
		// 	// is locked
		//
		// #region is_selected_locked - background (any)
		//
		// 	public static readonly DependencyProperty
		// 		BgIsLockedProperty = DependencyProperty.RegisterAttached(
		// 			"BgIsLocked", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBgIsLocked(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BgIsLockedProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBgIsLocked(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BgIsLockedProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_selected_locked - foreground (any)
		//
		// 	public static readonly DependencyProperty
		// 		FgIsLockedProperty = DependencyProperty.RegisterAttached(
		// 			"FgIsLocked", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetFgIsLocked(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(FgIsLockedProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetFgIsLocked(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(FgIsLockedProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_selected_locked - borderbrush
		//
		// 	public static readonly DependencyProperty
		// 		BdrIsLockedProperty = DependencyProperty.RegisterAttached(
		// 			"BdrIsLocked", typeof(SolidColorBrush), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new SolidColorBrush(Colors.IndianRed),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetBdrIsLocked(UIElement e, SolidColorBrush value)
		// 	{
		// 		e.SetValue(BdrIsLockedProperty, value);
		// 	}
		//
		// 	public static SolidColorBrush GetBdrIsLocked(UIElement e)
		// 	{
		// 		return (SolidColorBrush) e.GetValue(BdrIsLockedProperty);
		// 	}
		//
		// #endregion
		//
		// #region is_selected_locked - icon geometry
		//
		// 	public static readonly DependencyProperty
		// 		IconGeometryIsLockedProperty = DependencyProperty.RegisterAttached(
		// 			"IconGeometryIsLocked", typeof(Geometry), typeof(VisualStates),
		// 			new FrameworkPropertyMetadata(new PathGeometry(),
		// 				FrameworkPropertyMetadataOptions.Inherits));
		//
		// 	public static void SetIconGeometryIsLocked(UIElement e, Geometry value)
		// 	{
		// 		e.SetValue(IconGeometryIsLockedProperty, value);
		// 	}
		//
		// 	public static Geometry GetIconGeometryIsLocked(UIElement e)
		// 	{
		// 		return (Geometry) e.GetValue(IconGeometryIsLockedProperty);
		// 	}
		//
		// #endregion
	}
}